#if UNITY_5_3_OR_NEWER
using UnityEditor
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Settings;
using VRBuilder.Editor.UndoRedo;

namespace VRBuilder.Editor.UI.Drawers
{
    /// <summary>
    /// Drawer for <see cref="SceneObjectTagBase"/>.
    /// </summary>
    [DefaultProcessDrawer(typeof(SceneObjectTagBase))]
    [Obsolete("This drawer is obsolete and will be removed in the next major version.")]
    public class SceneObjectTagDrawer : AbstractDrawer
    {
        private const string noComponentSelected = "<none>";
        protected bool isUndoOperation;

        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            SceneObjectTagBase sceneObjectTag = (SceneObjectTagBase)currentValue;
            Guid oldGuid = sceneObjectTag.Guid;
            SceneObjectGroups.SceneObjectGroup[] tags = SceneObjectGroups.Instance.Groups.ToArray();
            List<string> labels = tags.Select(tag => tag.Label).ToList();
            SceneObjectGroups.SceneObjectGroup currentTag = tags.FirstOrDefault(tag => tag.Guid == oldGuid);
            Rect guiLineRect = rect;

            if (currentTag != null)
            {
                foreach (ISceneObject sceneObject in RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(currentTag.Guid))
                {
                    CheckForMisconfigurationIssues(sceneObject.GameObject, sceneObjectTag.GetReferenceType(), ref rect, ref guiLineRect);
                }
            }

            EditorGUI.BeginDisabledGroup(tags.Length == 0);

            int selectedTagIndex = Array.IndexOf(tags, currentTag);
            bool isTagInvalid = false;

            if (selectedTagIndex == -1)
            {
                selectedTagIndex = 0;
                labels.Insert(0, noComponentSelected);
                isTagInvalid = true;
            }

            selectedTagIndex = EditorGUI.Popup(guiLineRect, label.text, selectedTagIndex, labels.ToArray());
            EditorGUI.EndDisabledGroup();

            if (isTagInvalid && selectedTagIndex == 0)
            {
                return rect;
            }
            else if (isTagInvalid)
            {
                selectedTagIndex--;
            }

            Guid newGuid = tags[selectedTagIndex].Guid;

            if (oldGuid != newGuid)
            {
                ChangeValue(
                () =>
                {
                    sceneObjectTag.Guid = newGuid;
                    return sceneObjectTag;
                },
                () =>
                {
                    sceneObjectTag.Guid = oldGuid;
                    return sceneObjectTag;
                },
                changeValueCallback);
            }

            return rect;
        }

        protected void CheckForMisconfigurationIssues(GameObject selectedSceneObject, Type valueType, ref Rect originalRect, ref Rect guiLineRect)
        {
            if (selectedSceneObject != null && selectedSceneObject.GetComponent(valueType) == null)
            {
                string warning = $"{selectedSceneObject.name} is not configured as {valueType.Name}";
                const string button = "Fix it";
                EditorGUI.HelpBox(guiLineRect, warning, MessageType.Warning);
                guiLineRect = AddNewRectLine(ref originalRect);

                if (GUI.Button(guiLineRect, button))
                {
                    // Only relevant for Undoing a Process Property.
                    bool isAlreadySceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() != null && typeof(ISceneObjectProperty).IsAssignableFrom(valueType);
                    Component[] alreadyAttachedProperties = selectedSceneObject.GetComponents(typeof(Component));

                    RevertableChangesHandler.Do(
                        new ProcessCommand(
                            () => SceneObjectAutomaticSetup(selectedSceneObject, valueType),
                            () => UndoSceneObjectAutomaticSetup(selectedSceneObject, valueType, isAlreadySceneObject, alreadyAttachedProperties)));
                }

                guiLineRect = AddNewRectLine(ref originalRect);
            }
        }

        protected Rect AddNewRectLine(ref Rect currentRect)
        {
            Rect newRectLine = currentRect;
            newRectLine.height = EditorDrawingHelper.SingleLineHeight;
            newRectLine.y += currentRect.height + EditorDrawingHelper.VerticalSpacing;

            currentRect.height += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
            return newRectLine;
        }

        protected void SceneObjectAutomaticSetup(GameObject selectedSceneObject, Type valueType)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() ?? selectedSceneObject.AddComponent<ProcessSceneObject>();

            if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
            {
                sceneObject.AddProcessProperty(valueType);
            }

            isUndoOperation = true;
        }

        private void UndoSceneObjectAutomaticSetup(GameObject selectedSceneObject, Type valueType, bool hadProcessComponent, Component[] alreadyAttachedProperties)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();

            if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
            {
                sceneObject.RemoveProcessProperty(valueType, true, alreadyAttachedProperties);
            }

            if (hadProcessComponent == false)
            {
                UnityEngine.Object.DestroyImmediate((ProcessSceneObject)sceneObject);
            }

            isUndoOperation = true;
        }
    }
}

#elif GODOT
using Godot;
//TODO
#endif
