// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    [DefaultProcessDrawer(typeof(LockableObjectsCollection))]
    internal class LockableObjectsFactory : DataOwnerFactory
    {
        private LockableObjectsCollection lockableCollection;
        private Dictionary<Guid, bool> foldoutStatus = new Dictionary<Guid, bool>();

        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not LockableObjectsCollection lockableCol)
                return new Control{ Name = GetType().Name + "." + text };

            lockableCollection = lockableCol;

            /*Rect currentPosition = new Rect(rect.x, rect.y, rect.width, EditorDrawingHelper.HeaderLineHeight);
            currentPosition.y += 10;

            GUI.Label(currentPosition, "Automatically unlocked objects in this step");

            for (int i = 0; i < lockableCollection.SceneObjects.Count; i++)
            {
                ISceneObject objectInScene = lockableCollection.SceneObjects[i];
                currentPosition = DrawSceneObject(currentPosition, objectInScene);
                currentPosition.y += EditorDrawingHelper.SingleLineHeight;
            }

            currentPosition.y += EditorDrawingHelper.SingleLineHeight;
            EditorGUI.LabelField(currentPosition, "To add new ProcessSceneObject, drag it in here:");
            currentPosition.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;

            ProcessSceneObject newSceneObject = (ProcessSceneObject)EditorGUI.ObjectField(currentPosition, null, typeof(ProcessSceneObject), true);
            if (newSceneObject != null)
            {
                lockableCollection.AddSceneObject(newSceneObject);
            }

            currentPosition.y += EditorDrawingHelper.VerticalSpacing;

            Rect guiRect = currentPosition;
            guiRect.y += currentPosition.height;
            guiRect.height = EditorDrawingHelper.SingleLineHeight;
            if (GUI.Button(guiRect, "Add group to unlock list"))
            {
                Action<SceneObjectGroups.SceneObjectGroup> onItemSelected = (SceneObjectGroups.SceneObjectGroup selectedGroup) =>
                {
                    lockableCollection.AddGroup(selectedGroup.Guid);

                    if (foldoutStatus.ContainsKey(selectedGroup.Guid) == false)
                    {
                        foldoutStatus.Add(selectedGroup.Guid, true);
                    }
                };

                DrawSearchableGroupListPopup(guiRect, onItemSelected, lockableCollection.TagsToUnlock);
            }

            guiRect.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;

            EditorGUI.LabelField(guiRect, "Select the properties to attempt to unlock for each group:");
            guiRect.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;

            foreach (Guid guid in new List<Guid>(lockableCollection.TagsToUnlock))
            {
                GUILayout.BeginArea(guiRect);
                GUILayout.BeginHorizontal();

                if (foldoutStatus.ContainsKey(guid) == false)
                {
                    foldoutStatus.Add(guid, false);
                }

                foldoutStatus[guid] = EditorGUILayout.Foldout(foldoutStatus[guid], SceneObjectGroups.Instance.GetLabel(guid));

                if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                {
                    lockableCollection.RemoveGroup(guid);
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                    break;
                }

                guiRect.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                if (foldoutStatus[guid])
                {
                    foreach (Type type in PropertyReflectionHelper.ExtractFittingPropertyType<LockableProperty>(typeof(LockableProperty)))
                    {
                        Rect objectPosition = guiRect;
                        objectPosition.x += EditorDrawingHelper.IndentationWidth * 2f;
                        objectPosition.width -= EditorDrawingHelper.IndentationWidth * 2f;

                        bool isFlagged = lockableCollection.IsPropertyEnabledForGroup(guid, type);

                        if (EditorGUI.Toggle(guiRect, isFlagged) != isFlagged)
                        {
                            if (isFlagged)
                            {
                                lockableCollection.RemovePropertyFromGroup(guid, type);
                                break;
                            }
                            else
                            {
                                lockableCollection.AddPropertyToGroup(guid, type);
                                break;
                            }
                        }

                        EditorGUI.LabelField(objectPosition, type.Name);

                        guiRect.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
                    }
                }
            }

            currentPosition = guiRect;*/

            // EditorDrawingHelper.HeaderLineHeight - 24f is just the magic number to make it properly fit...
            return new Control{ Name = GetType().Name + "." + text };
        }

        /*private Control DrawSceneObject(ISceneObject sceneObject)
        {
            currentPosition.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;

            Rect objectFieldPosition = currentPosition;
            objectFieldPosition.width -= 24;
            GUI.enabled = false;
            EditorGUI.ObjectField(objectFieldPosition, (ProcessSceneObject)sceneObject, typeof(ProcessSceneObject), true);
            // If scene object is used by a property, dont allow removing it.
            GUI.enabled = lockableCollection.IsUsedInAutoUnlock(sceneObject) == false;
            objectFieldPosition.x = currentPosition.width - 24 + 6f;
            objectFieldPosition.width = 20;
            if (GUI.Button(objectFieldPosition, "x", new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold }))
            {
                lockableCollection.RemoveSceneObject(sceneObject);
            }
            GUI.enabled = true;

            try
            {
                foreach (LockableProperty property in sceneObject.Properties.Where(property => property is LockableProperty))
                {
                    currentPosition = DrawProperty(currentPosition, property);
                }
            }
            catch (MissingReferenceException)
            {
                // Swallow this exception, will be thrown in frames between exiting playmode and having setup the object reference library.
            }

            return currentPosition;
        }*/

        /*private Control DrawProperty(LockableProperty property)
        {
            currentPosition.y += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
            Rect objectPosition = currentPosition;
            objectPosition.x += EditorDrawingHelper.IndentationWidth * 2f;
            objectPosition.width -= EditorDrawingHelper.IndentationWidth * 2f;

            GUI.enabled = lockableCollection.IsInAutoUnlockList(property) == false;
            bool isFlagged = GUI.enabled == false || lockableCollection.IsInManualUnlockList(property);
            if (EditorGUI.Toggle(currentPosition, isFlagged) != isFlagged)
            {
                // Inverted due to not updated with the toggle
                if (isFlagged)
                {
                    lockableCollection.Remove(property);
                }
                else
                {
                    lockableCollection.Add(property);
                }
            }
            GUI.enabled = true;
            EditorGUI.LabelField(objectPosition, property.GetType().Name);
            return currentPosition;
        }*/

        /*private void DrawSearchableGroupListPopup(Action<SceneObjectGroups.SceneObjectGroup> onItemSelected, IEnumerable<Guid> groupsToExclude)
        {
            VisualTreeAsset searchableList = ViewDictionary.LoadAsset(ViewDictionary.EnumType.SearchableList);
            VisualTreeAsset groupListItem = ViewDictionary.LoadAsset(ViewDictionary.EnumType.SearchableListItem);

            SearchableGroupListPopup content = new SearchableGroupListPopup(onItemSelected, searchableList, groupListItem);

            var groups = new List<SceneObjectGroups.SceneObjectGroup>(SceneObjectGroups.Instance.Groups);
            groups = groups.Where(group => !groupsToExclude.Contains(group.Guid)).OrderBy(t => t.Label).ToList();
            content.SetAvailableGroups(groups);
            content.SetWindowSize(windowWith: rect.width);

            UnityEditor.PopupWindow.Show(rect, content);
        }*/
    }
}
