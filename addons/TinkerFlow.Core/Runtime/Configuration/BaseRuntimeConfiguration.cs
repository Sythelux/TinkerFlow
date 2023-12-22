// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.IO;
using VRBuilder.Core.Properties;
using VRBuilder.Core.RestrictiveEnvironment;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Serialization;
using VRBuilder.Core.Serialization.JSON;
using VRBuilder.Core.Utils;
using FileAccess = Godot.FileAccess;

namespace VRBuilder.Core.Configuration;

/// <summary>
/// Base class for your runtime process configuration. Extend it to create your own.
/// </summary>
#pragma warning disable 0618
[Tool]
public abstract partial class BaseRuntimeConfiguration : Resource, IRuntimeConfiguration
{
#pragma warning restore 0618
    /// <summary>
    /// Name of the manifest file that could be used to save process asset information.
    /// </summary>
    public static string ManifestFileName => "ProcessManifest";

    protected ISceneObjectRegistry? sceneObjectRegistry;
    protected ISceneConfiguration? sceneConfiguration;

    /// <inheritdoc />
    public virtual ISceneObjectRegistry SceneObjectRegistry => sceneObjectRegistry ??= new BaseSceneObjectRegistry();

    /// <inheritdoc />
    public IProcessSerializer Serializer { get; set; } = new SystemJsonProcessSerializerV1();

    /// <summary>
    /// Default input action asset which is used when no customization of key bindings are done.
    /// Should be stored inside the VR Builder package.
    /// </summary>
    public virtual string DefaultInputActionAssetPath { get; } = "KeyBindings/BuilderDefaultKeyBindings";

    /// <summary>
    /// Custom InputActionAsset path which is used when key bindings are modified.
    /// Should be stored in project path.
    /// </summary>
    public virtual string CustomInputActionAssetPath { get; } = "KeyBindings/BuilderCustomKeyBindings";

#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM_PACKAGE
        private UnityEngine.InputSystem.InputActionAsset inputActionAsset;

        /// <summary>
        /// Current active InputActionAsset.
        /// </summary>
        public virtual UnityEngine.InputSystem.InputActionAsset CurrentInputActionAsset
        {
            get
            {
                if (inputActionAsset == null)
                {
                    inputActionAsset = Resources.Load<UnityEngine.InputSystem.InputActionAsset>(CustomInputActionAssetPath);
                    if (inputActionAsset == null)
                    {
                        inputActionAsset = Resources.Load<UnityEngine.InputSystem.InputActionAsset>(DefaultInputActionAssetPath);
                    }
                }

                return inputActionAsset;
            }

            set => inputActionAsset = value;
        }
#endif

    /// <inheritdoc />
    public IModeHandler Modes { get; protected set; }

    /// <inheritdoc />
    public abstract ProcessSceneObject User { get; }

    /// <inheritdoc />
    public abstract UserSceneObject LocalUser { get; }

    /// <inheritdoc />
    public abstract AudioStreamPlayer InstructionPlayer { get; }

    /// <summary>
    /// Determines the property locking strategy used for this runtime configuration.
    /// </summary>
    public StepLockHandlingStrategy StepLockHandling { get; set; }

    /// <inheritdoc />
    public abstract IEnumerable<UserSceneObject> Users { get; }

    /// <inheritdoc />
    public abstract IProcessAudioPlayer ProcessAudioPlayer { get; }

    /// <inheritdoc />
    public abstract ISceneObjectManager SceneObjectManager { get; }

    /// <inheritdoc />
    public virtual ISceneConfiguration SceneConfiguration
    {
        get
        {
            if (sceneConfiguration == null)
            {
                var configuration = RuntimeConfigurator.Instance.GetComponent<ISceneConfiguration>();

                if (configuration == null) configuration = RuntimeConfigurator.Instance.AddComponent<SceneConfiguration>();

                sceneConfiguration = configuration;
            }

            return sceneConfiguration;
        }
    }

    protected BaseRuntimeConfiguration() : this(new EmptyStepLockHandling())
    {
    }

    protected BaseRuntimeConfiguration(StepLockHandlingStrategy lockHandling)
    {
        StepLockHandling = lockHandling;
    }

    /// <inheritdoc />
    public virtual async Task<IProcess> LoadProcess(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Given path is null or empty!");

            int index = path.LastIndexOf("/");
            string processFolder = path.Substring(0, index);
            string processName = GetProcessNameFromPath(path);
            var manifestPath = $"{processFolder}/{ManifestFileName}.{Serializer.FileFormat}";

            IProcessAssetManifest manifest = await FetchManifest(processName, manifestPath);
            var assetStrategy = ReflectionUtils.CreateInstanceOfType(ReflectionUtils.GetConcreteImplementationsOf<IProcessAssetStrategy>().FirstOrDefault(type => type.FullName == manifest.AssetStrategyTypeName)) as IProcessAssetStrategy;

            var processAssetPath = $"{processFolder}/{manifest.ProcessFileName}.{Serializer.FileFormat}";

            List<byte[]> additionalData = await GetAdditionalProcessData(processFolder, manifest);

            return assetStrategy.GetProcessFromSerializedData(FileAccess.GetFileAsBytes(processAssetPath), additionalData, Serializer);
        }
        catch (Exception exception)
        {
            GD.PrintErr($"Error when loading process. {exception.GetType().Name}, {exception.Message}\n{exception.StackTrace}", RuntimeConfigurator.Instance);
        }

        return null;
    }

    private Task<List<byte[]>> GetAdditionalProcessData(string processFolder, IProcessAssetManifest manifest)
    {
        List<byte[]> additionalData = new();
        foreach (string fileName in manifest.AdditionalFileNames)
        {
            var filePath = $"{processFolder}/{fileName}.{Serializer.FileFormat}";

            if (FileAccess.FileExists(filePath))
                additionalData.Add(FileAccess.GetFileAsBytes(filePath));
            else
                GD.PrintErr($"Error loading process. File not found: {filePath}");
        }

        return Task.FromResult(additionalData);
    }

    private Task<IProcessAssetManifest> FetchManifest(string processName, string manifestPath)
    {
        IProcessAssetManifest manifest;

        if (FileAccess.FileExists(manifestPath))
        {
            byte[] manifestData = FileAccess.GetFileAsBytes(manifestPath);
            manifest = Serializer.ManifestFromByteArray(manifestData);
        }
        else
        {
            manifest = new ProcessAssetManifest()
            {
                AssetStrategyTypeName = typeof(SingleFileProcessAssetStrategy).FullName,
                ProcessFileName = processName,
                AdditionalFileNames = Array.Empty<string>()
            };
        }

        return Task.FromResult(manifest);
    }

    private static string GetProcessNameFromPath(string path)
    {
        int slashIndex = path.LastIndexOf('/');
        string fileName = path.Substring(slashIndex + 1);
        int pointIndex = fileName.LastIndexOf('.');
        fileName = fileName.Substring(0, pointIndex);

        return fileName;
    }
}

public class EmptyStepLockHandling : StepLockHandlingStrategy
{
    public override void Unlock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked)
    {
    }

    public override void Lock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked)
    {
    }
}