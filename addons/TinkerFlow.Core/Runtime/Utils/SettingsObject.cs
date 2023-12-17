// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
#if UNITY_EDITOR
using UnityEditor;
#elif GODOT4
using Godot;
#endif

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
        var settings = ResourceLoader.Load<T>(typeof(T).Name);

        if (settings == null)
        {
            // Create an instance
            //TODO: settings = CreateInstance<T>();
#if UNITY_EDITOR
                if (!Directory.Exists("Assets/MindPort/VR Builder/Resources"))
                {
                    Directory.CreateDirectory("Assets/MindPort/VR Builder/Resources");
                }
                AssetDatabase.CreateAsset(settings, $"Assets/MindPort/VR Builder/Resources/{typeof(T).Name}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#elif GODOT4
            settings = EditorInterface.Singleton.Get($"TinkerFlow/{typeof(T).Name}").Obj as T ?? new T();
#endif
        }

        return settings;
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
        EditorInterface.Singleton.Set($"TinkerFlow/{typeof(T).Name}", this);
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