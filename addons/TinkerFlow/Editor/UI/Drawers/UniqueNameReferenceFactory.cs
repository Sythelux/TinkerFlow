// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Godot;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Editor.Godot;

namespace VRBuilder.Editor.UI.Drawers
{
    /// <summary>
    /// TODO: implement drop: https://docs.godotengine.org/en/latest/classes/class_control.html#class-control-private-method-get-drag-data
    /// Process drawer for <see cref="UniqueNameReference"/> members.
    /// </summary>
    [DefaultProcessDrawer(typeof(UniqueNameReference))]
    public partial class UniqueNameReferenceFactory : AbstractProcessFactory
    {
        protected const string undoGroupName = "brotcat";

        protected readonly HashSet<string> missingUniqueNames = new HashSet<string>();
        protected bool isUndoOperation;

        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var control = new VBoxContainer();
            control.Name = GetType().Name + "." + text;
            if (RuntimeConfigurator.Exists == false)
            {
                return control;
            }

            isUndoOperation = false;
            UniqueNameReference? uniqueNameReference = currentValue as UniqueNameReference;
            PropertyInfo? valueProperty = currentValue?.GetType().GetProperty("Value");
            Type valueType = ReflectionUtils.GetDeclaredTypeOfPropertyOrField(valueProperty);

            if (valueProperty == null)
            {
                throw new ArgumentException("Only ObjectReference<> implementations should inherit from the UniqueNameReference type.");
            }

            string oldUniqueName = uniqueNameReference?.UniqueName;
            Node? selectedSceneObject = GetGameObjectFromID(oldUniqueName);

            if (selectedSceneObject == null && string.IsNullOrEmpty(oldUniqueName) == false && missingUniqueNames.Add(oldUniqueName))
            {
                GD.PushError($"The process object with the unique name '{oldUniqueName}' cannot be found!");
            }

            Control checkForMisconfigurationIssues = CheckForMisconfigurationIssues(selectedSceneObject, valueType);
            control.AddChild(checkForMisconfigurationIssues);

            var hBoxContainer = new HBoxContainer();
            var label = new Label { Text = text };
            hBoxContainer.AddChild(label);

            ObjectDrawer od = EditorGUI.ObjectField(label as Label, selectedSceneObject, typeof(Node), true);
            od.SelectedObjectChanged += OnSelectedObjectChanged;

            void OnSelectedObjectChanged(Variant selectedobject)
            {
                // string newUniqueName = GetIDFromSelectedObject(selectedobject as Node, valueType, oldUniqueName);
                // TODO: if (oldUniqueName != newUniqueName)
                // {
                //     RevertableChangesHandler.Do(
                //         new ProcessCommand(
                //             () =>
                //             {
                //                 uniqueNameReference.UniqueName = newUniqueName;
                //                 changeValueCallback(uniqueNameReference);
                //             },
                //             () =>
                //             {
                //                 uniqueNameReference.UniqueName = oldUniqueName;
                //                 changeValueCallback(uniqueNameReference);
                //             }),
                //         isUndoOperation ? undoGroupName : string.Empty);
                //
                //     if (isUndoOperation)
                //     {
                //         RevertableChangesHandler.CollapseUndoOperations(undoGroupName);
                //     }
                // }
            }

            hBoxContainer.AddChild(od);
            // maybe? od._DropData(new Vector2(1,1), obj);

            // https://docs.godotengine.org/en/latest/classes/class_control.html#class-control-private-method-get-drag-data
            control.AddChild(hBoxContainer);
            return control;
        }

        protected Node? GetGameObjectFromID(string objectUniqueName)
        {
            if (string.IsNullOrEmpty(objectUniqueName))
            {
                return null;
            }

            // If the Runtime Configurator exists, we try to retrieve the process object
            try
            {
                if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsName(objectUniqueName) == false)
                {
                    // If the saved unique name is not registered in the scene, perhaps is actually a GameObject's InstanceID
                    return GetGameObjectFromInstanceID(objectUniqueName);
                }

                ISceneObject sceneObject = RuntimeConfigurator.Configuration.SceneObjectRegistry.GetByName(objectUniqueName);
                return sceneObject.GameObject;
            }
            catch
            {
                return null;
            }
        }

        protected string GetIDFromSelectedObject(Node selectedSceneObject, Type valueType, string oldUniqueName)
        {
            string newUniqueName = string.Empty;

            if (selectedSceneObject != null)
            {
                if (selectedSceneObject.GetComponent(valueType) != null)
                {
                    if (typeof(ISceneObject).IsAssignableFrom(valueType))
                    {
                        newUniqueName = GetUniqueNameFromSceneObject(selectedSceneObject);
                    }
                    else if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
                    {
                        newUniqueName = GetUniqueNameFromProcessProperty(selectedSceneObject, valueType, oldUniqueName);
                    }
                }
                else
                {
                    newUniqueName = selectedSceneObject.GetInstanceId().ToString();
                }
            }

            return newUniqueName;
        }

        private Node GetGameObjectFromInstanceID(string objectUniqueName)
        {
            Node gameObject = null;

            if (ulong.TryParse(objectUniqueName, out ulong instanceId))
            {
                gameObject = GodotObject.InstanceFromId(instanceId) as Node;
            }

            return gameObject;
        }

        private string GetUniqueNameFromSceneObject(Node selectedSceneObject)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();

            if (sceneObject != null)
            {
                return sceneObject.UniqueName;
            }

            GD.PushWarning($"Game Object \"{selectedSceneObject.Name}\" does not have a Process Object component.");
            return string.Empty;
        }

        private string GetUniqueNameFromProcessProperty(Node selectedProcessPropertyObject, Type valueType, string oldUniqueName)
        {
            if (selectedProcessPropertyObject.GetComponent(valueType) is ISceneObjectProperty processProperty)
            {
                return processProperty.SceneObject.UniqueName;
            }

            GD.PushWarning($"Scene Object \"{selectedProcessPropertyObject.Name}\" with Unique Name \"{oldUniqueName}\" does not have a {valueType.Name} component.");
            return string.Empty;
        }

        protected Control CheckForMisconfigurationIssues(Node selectedSceneObject, Type valueType)
        {
            if (selectedSceneObject != null && selectedSceneObject.GetComponent(valueType) == null)
            {
                string warning = $"{selectedSceneObject.Name} is not configured as {valueType.Name}";
                const string button = "Fix it";

                //TODO: add fixit button
                // EditorGUI.HelpBox(warning, EditorGUI.MessageType.Error);
                // guiLineRect = AddNewRectLine(ref originalRect);

                // if (GUI.Button(guiLineRect, button))
                // {
                // Only relevant for Undoing a Process Property.
                // bool isAlreadySceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() != null && typeof(ISceneObjectProperty).IsAssignableFrom(valueType);
                // Component[] alreadyAttachedProperties = selectedSceneObject.GetComponents(typeof(Component));

                // RevertableChangesHandler.Do(
                //     new ProcessCommand(
                //         () => SceneObjectAutomaticSetup(selectedSceneObject, valueType),
                //         () => UndoSceneObjectAutomaticSetup(selectedSceneObject, valueType, isAlreadySceneObject, alreadyAttachedProperties)),
                //     undoGroupName);
                // }

                // guiLineRect = AddNewRectLine(ref originalRect);
            }

            return new Control();
        }

        protected void SceneObjectAutomaticSetup(Node selectedSceneObject, Type valueType)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() ?? selectedSceneObject.AddComponent<ProcessSceneObject>();

            if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsGuid(sceneObject.Guid) == false)
            {
                // Sets a UniqueName and then registers it.
                sceneObject.SetSuitableName();
            }

            if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
            {
                sceneObject.AddProcessProperty(valueType);
            }

            isUndoOperation = true;
        }

        private void UndoSceneObjectAutomaticSetup(Node selectedSceneObject, Type valueType, bool hadProcessComponent, Component[] alreadyAttachedProperties)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();

            if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
            {
                sceneObject.RemoveProcessProperty(valueType, true, alreadyAttachedProperties);
            }

            if (hadProcessComponent == false)
            {
                selectedSceneObject.QueueFree();
                // Object.DestroyImmediate((ProcessSceneObject)sceneObject);
            }

            isUndoOperation = true;
        }

        // protected Rect AddNewRectLine(ref Rect currentRect)
        // {
        //     Rect newRectLine = currentRect;
        //     newRectLine.height = EditorDrawingHelper.SingleLineHeight;
        //     newRectLine.y += currentRect.height + EditorDrawingHelper.VerticalSpacing;
        //
        //     currentRect.height += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
        //     return newRectLine;
        // }
    }
}