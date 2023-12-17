// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Exceptions;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils.Logging;

namespace VRBuilder.Core.SceneObjects;

/// <inheritdoc cref="ISceneObject"/>
[Tool]
public partial class ProcessSceneObject : Node3D, ISceneObject, ITagContainer
{
    public event EventHandler<LockStateChangedEventArgs> Locked;
    public event EventHandler<LockStateChangedEventArgs> Unlocked;
    public event EventHandler<SceneObjectNameChanged> UniqueNameChanged;
    public Node GameObject => this; //was GameObject

    [Export]
    // [Tooltip("Unique name which identifies an object in scene, can be null or empty, but has to be unique in the scene.")]
    protected string uniqueName = null;

    /// <inheritdoc />
    public string UniqueName
    {
        get
        {
            if (string.IsNullOrEmpty(uniqueName)) return "REF-" + Guid;

            return uniqueName;
        }
    }

    private Guid guid = Guid.NewGuid();
    private List<IStepData> unlockers = new();

    /// <inheritdoc />
    public Guid Guid => guid;

    public IEnumerable<ISceneObjectProperty> Properties => GetChildren().OfType<ISceneObjectProperty>();

    public bool IsLocked { get; private set; }

    private bool IsRegistered => RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsGuid(Guid);

    [SerializeField]
    protected List<string> tags = new();

    /// <inheritdoc />
    public IEnumerable<Guid> Tags => tags.Select(tag => Guid.Parse(tag));

    /// <inheritdoc />
    [Signal]
    public delegate void TagAddedEventHandler(string tag);

    /// <inheritdoc />
    [Signal]
    public delegate void TagRemovedEventHandler(string tag);

    protected void Awake()
    {
        Init();

        IEnumerable<ProcessSceneObject> processSceneObjects = FindChildren("*", recursive: true).OfType<ProcessSceneObject>();
        foreach (ProcessSceneObject pso in processSceneObjects)
            if (!pso.Visible) //was if (!pso.isActiveAndEnabled) 
                pso.Init();
    }

    protected void Init()
    {
#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewScene(gameObject.scene))
            {
                return;
            }
#endif
        if (RuntimeConfigurator.Exists == false) return;

        if (IsRegistered) return;

        this.SetSuitableName(uniqueName);

        if (IsRegistered == false)
        {
            RuntimeConfigurator.Configuration.SceneObjectRegistry.Register(this);

            if (UniqueNameChanged != null) UniqueNameChanged.Invoke(this, new SceneObjectNameChanged(UniqueName, UniqueName));
        }
    }

    private void OnDestroy()
    {
        if (RuntimeConfigurator.Exists) RuntimeConfigurator.Configuration.SceneObjectRegistry.Unregister(this);
    }

    public bool CheckHasProperty<T>() where T : ISceneObjectProperty
    {
        return CheckHasProperty(typeof(T));
    }

    public bool CheckHasProperty(Type type)
    {
        return FindProperty(type) != null;
    }

    public T GetProperty<T>() where T : ISceneObjectProperty
    {
        ISceneObjectProperty property = FindProperty(typeof(T));
        if (property == null) throw new PropertyNotFoundException(this, typeof(T));

        return (T)property;
    }

    public void ValidateProperties(IEnumerable<Type> properties)
    {
        var hasFailed = false;
        foreach (Type propertyType in properties)
            // ReSharper disable once InvertIf
            if (CheckHasProperty(propertyType) == false)
            {
                GD.PrintErr($"Property of type '{propertyType.Name}' is not attached to SceneObject '{UniqueName}'");
                hasFailed = true;
            }

        if (hasFailed) throw new PropertyNotFoundException("One or more SceneObjectProperties could not be found, check your log entries for more information.");
    }

    public void SetLocked(bool lockState)
    {
        if (IsLocked == lockState) return;

        IsLocked = lockState;

        if (IsLocked)
        {
            if (Locked != null) Locked.Invoke(this, new LockStateChangedEventArgs(IsLocked));
        }
        else
        {
            if (Unlocked != null) Unlocked.Invoke(this, new LockStateChangedEventArgs(IsLocked));
        }
    }

    /// <inheritdoc/>
    public virtual void RequestLocked(bool lockState, IStepData stepData)
    {
        if (lockState == false && unlockers.Contains(stepData) == false) unlockers.Add(stepData);

        if (lockState && unlockers.Contains(stepData)) unlockers.Remove(stepData);

        bool canLock = unlockers.Count == 0;

        if (LifeCycleLoggingConfig.Instance.LogLockState)
        {
            string lockType = lockState ? "lock" : "unlock";
            string requester = stepData == null ? "NULL" : stepData.Name;
            var unlockerList = new StringBuilder();

            foreach (IStepData unlocker in unlockers) unlockerList.Append($"\n<i>{unlocker.Name}</i>");

            string listUnlockers = unlockers.Count == 0 ? "" : $"\nSteps keeping this object unlocked:{unlockerList}";

            GD.Print($"<i>{GetType().Name}</i> on <i>{Name}</i> received a <b>{lockType}</b> request from <i>{requester}</i>." +
                     $"\nCurrent lock state: <b>{IsLocked}</b>. Future lock state: <b>{lockState && canLock}</b>{listUnlockers}");
        }

        SetLocked(lockState && canLock);
    }

    /// <inheritdoc/>
    public bool RemoveUnlocker(IStepData data)
    {
        return unlockers.Remove(data);
    }

    /// <summary>
    /// Tries to find property which is assignable to given type, this method
    /// will return null if none is found.
    /// </summary>
    private ISceneObjectProperty FindProperty(Type type)
    {
        return GetChildren().FirstOrDefault(c => c.GetType() == type) as ISceneObjectProperty;
    }

    public void ChangeUniqueName(string newName)
    {
        if (newName == UniqueName) return;

        if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsName(newName))
        {
            GD.PrintErr($"An object with a name '{newName}' is already registered. The new name is ignored. The name is still '{UniqueName}'.");
            return;
        }

        string previousName = UniqueName;

        if (IsRegistered) RuntimeConfigurator.Configuration.SceneObjectRegistry.Unregister(this);

        uniqueName = newName;

        RuntimeConfigurator.Configuration.SceneObjectRegistry.Register(this);

        if (UniqueNameChanged != null) UniqueNameChanged.Invoke(this, new SceneObjectNameChanged(UniqueName, previousName));
    }

    /// <inheritdoc />
    public void AddTag(Guid tag)
    {
        if (Tags.Contains(tag) == false)
        {
            tags.Add(tag.ToString());
            EmitSignal(SignalName.TagAdded, tag.ToString()); // TagAdded?.Invoke(this, new TaggableObjectEventArgs(tag.ToString()));
        }
    }

    /// <inheritdoc />
    public bool HasTag(Guid tag)
    {
        return Tags.Contains(tag);
    }

    /// <inheritdoc />
    public bool RemoveTag(Guid tag)
    {
        if (tags.Remove(tag.ToString()))
        {
            EmitSignal(SignalName.TagRemoved, tag.ToString()); // was TagRemoved?.Invoke(this, new TaggableObjectEventArgs(tag.ToString()));
            return true;
        }

        return false;
    }
}