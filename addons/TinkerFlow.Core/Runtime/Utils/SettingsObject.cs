// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Godot;
using Godot.Collections;

namespace VRBuilder.Core.Runtime.Utils;

/// <summary>
/// ScriptableObject with additional load and save mechanic to make it a singleton.
/// </summary>
/// <typeparam name="T">The class itself</typeparam>
public partial class SettingsObject<T> : Resource where T : Resource, new()
{
    private static T? instance;

    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
                if (EditorUtility.IsDirty(instance))
                {
                    instance = null;
                }
#endif
            return instance ??= Load();
        }
    }

    private static T Load()
    {
        var settings = new T();
        foreach (Dictionary property in settings.GetPropertyList())
        {
            if (!property["usage"].Equals(Variant.From(4102))) continue;
            StringName propertyName = property["name"].AsStringName();
            var key = $"TinkerFlow/{typeof(T).Name}/{propertyName}";
            if (ProjectSettings.Singleton.HasSetting(key))
                settings.Set(propertyName, ProjectSettings.Singleton.GetSetting(key));
            else
                ProjectSettings.Singleton.SetSetting(key, default);
        }

        return settings;
    }

    public override bool _Set(StringName property, Variant value)
    {
        ProjectSettings.Singleton.SetSetting(property, default);
        return base._Set(property, value);
    }

    /// <summary>
    /// Saves the VR Builder settings, only works in editor.
    /// </summary>
    public void Save()
    {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#elif GODOT4
        // ProjectSettings.Singleton.SetSetting($"TinkerFlow/{typeof(T).Name}", this);
        // EditorInterface.Singleton.Set($"TinkerFlow/{typeof(T).Name}", this);
#endif
    }

    ~SettingsObject()
    {
#if UNITY_EDITOR
            if (EditorUtility.IsDirty(this))
            {
                Save();
            }
#endif
    }
}