// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Utils;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.Configuration;

/// <summary>
/// Configurator to set the process editor configuration which is used by the process creation editor tools (like Step Inspector).
/// </summary>
public static class EditorConfigurator
{
    private static DefaultEditorConfiguration editorConfiguration;

    public static DefaultEditorConfiguration Instance => editorConfiguration;

    static EditorConfigurator()
    {
        Init();
    }

    public static void Init()
    {
        Type[] lowestPriorityTypes = { typeof(DefaultEditorConfiguration) };
        Type[] definitions = ReflectionUtils.GetFinalImplementationsOf<IEditorConfiguration>(lowestPriorityTypes).ToArray();

        if (definitions.Except(lowestPriorityTypes).Count() > 1)
        {
            string listOfDefinitions = string.Join("', '", definitions.Select(definition => definition.FullName).ToArray());
            GD.PushError($"There is more than one final implementation of process editor configurations in this Unity project: '{listOfDefinitions}'."
                         + $" Remove all editor configurations except for '{typeof(DefaultEditorConfiguration).FullName}' and the one you want to use."
                         + $" '{definitions.First().FullName}' was selected as current editor configuration.");
        }

        var config = (IEditorConfiguration)ReflectionUtils.CreateInstanceOfType(definitions.First());
        if (config is DefaultEditorConfiguration configuration)
            editorConfiguration = configuration;
        else
            editorConfiguration = new EditorConfigWrapper(config);

        LoadAllowedMenuItems();
    }

    private static void LoadAllowedMenuItems()
    {
        if (string.IsNullOrEmpty(Instance.AllowedMenuItemsSettingsAssetPath))
            Instance.AllowedMenuItemsSettings = new AllowedMenuItemsSettings();
        else
            Instance.AllowedMenuItemsSettings = AllowedMenuItemsSettings.Load();

        ApplyConfigurationExtensions();

        Instance.AllowedMenuItemsSettings.RefreshMenuOptions();
    }

    private static void ApplyConfigurationExtensions()
    {
        IEnumerable<Type> extensions = ReflectionUtils.GetFinalImplementationsOf<IEditorConfigurationExtension>();

        List<Type> disabledMenuItems = new();
        List<Type> requiredMenuItems = new();

        foreach (Type type in extensions)
        {
            var extension = (IEditorConfigurationExtension)ReflectionUtils.CreateInstanceOfType(type);
            requiredMenuItems.AddRange(extension.RequiredMenuItems.Where(menuItem => requiredMenuItems.Contains(menuItem) == false));
            disabledMenuItems.AddRange(extension.DisabledMenuItems.Where(menuItem => disabledMenuItems.Contains(menuItem) == false));
        }

        int conflicts = disabledMenuItems.RemoveAll(menuItem => requiredMenuItems.Contains(menuItem));

        if (conflicts > 0) GD.PushWarning($"Conflicts in editor configuration extensions: {conflicts} items were both required and disabled by different extensions. They have been enabled.");

        foreach (Type menuItem in disabledMenuItems)
        {
            if (menuItem.IsSubclassOf(typeof(MenuItem<IBehavior>))) Instance.AllowedMenuItemsSettings.SerializedBehaviorSelections[menuItem.AssemblyQualifiedName] = false;

            if (menuItem.IsSubclassOf(typeof(MenuItem<ICondition>))) Instance.AllowedMenuItemsSettings.SerializedConditionSelections[menuItem.AssemblyQualifiedName] = false;
        }

        foreach (Type menuItem in requiredMenuItems)
        {
            if (menuItem.IsSubclassOf(typeof(MenuItem<IBehavior>))) Instance.AllowedMenuItemsSettings.SerializedBehaviorSelections[menuItem.AssemblyQualifiedName] = true;

            if (menuItem.IsSubclassOf(typeof(MenuItem<ICondition>))) Instance.AllowedMenuItemsSettings.SerializedConditionSelections[menuItem.AssemblyQualifiedName] = true;
        }
    }
}