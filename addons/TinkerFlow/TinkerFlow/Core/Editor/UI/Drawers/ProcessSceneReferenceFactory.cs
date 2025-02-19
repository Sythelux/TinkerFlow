using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Editor.Godot;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Settings;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Drawer for <see cref="ProcessSceneReferenceBase"/>.
    /// </summary>
    [DefaultProcessDrawer(typeof(ProcessSceneReferenceBase))]
    public class ProcessSceneReferenceFactory : AbstractProcessFactory
    {
        protected bool isUndoOperation;
        protected bool isExpanded;

        private static readonly Texture2D deleteIcon = EditorDrawingHelper.GetIcon("icon_delete");
        private static readonly Texture2D editIcon = EditorDrawingHelper.GetIcon("icon_edit");
        private static readonly Texture2D showIcon = EditorDrawingHelper.GetIcon("icon_info"); //HelpSearch
        private static int buttonWidth = 24;

        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not ProcessSceneReferenceBase reference)
                return new Control { Name = GetType().Name + "." + text };

            Type valueType = reference.GetReferenceType();
            List<Guid> oldGuids = reference.Guids.ToList();

            var control = new VBoxContainer { Name = GetType().Name + "." + text };
            control.AddChild(new Label { Text = $"[b]{text}[/b]" });

            control.AddChild(DrawLimitationWarnings(reference.Guids, reference.AllowMultipleValues));

            /*control.AddChild(DrawDragAndDropArea(changeValueCallback, reference, oldGuids, text));

            control.AddChild(DrawMisconfigurationOnSelectedGameObjects(reference, valueType));*/

            return control;
        }

        private string GetReferenceValue(ProcessSceneReferenceBase reference)
        {
            if (reference.IsEmpty())
            {
                return "";
            }
            else
            {
                return reference.ToString();
            }
        }

        private IEnumerable<Guid> GetAllGuids(ISceneObject obj)
        {
            return new List<Guid>() { obj.Guid }.Concat(obj.Guids);
        }

        private Control DrawLimitationWarnings(IEnumerable<Guid> currentObjectGroups, bool allowMultipleValues)
        {
            if (!RuntimeConfigurator.Exists)
            {
                return new Control { Name = GetType().Name };
            }

            int groupedObjectsCount = currentObjectGroups.SelectMany(group => RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(group)).Distinct().Count();

            string message = string.Empty;
            EditorGUI.MessageType messageType = EditorGUI.MessageType.None;

            if (!allowMultipleValues && groupedObjectsCount > 1)
            {
                message = "This only supports a single scene object at a time.";
                messageType = EditorGUI.MessageType.Warning;
            }
            else if (groupedObjectsCount == 0)
            {
                if (SceneObjectGroups.Instance.Groups.Any(group => currentObjectGroups.Contains(group.Guid)))
                {
                    message = "No objects found. A valid object must be spawned before this step.";
                    messageType = EditorGUI.MessageType.Warning;
                }
                else
                {
                    message = "No objects found in scene. This will result in a null reference.";
                    messageType = EditorGUI.MessageType.Error;
                }
            }

            return string.IsNullOrEmpty(message)
                ? new Control { Name = GetType().Name }
                : EditorGUI.HelpBox(message, messageType);

        }

        /*private Node DrawDragAndDropArea(Action<object> changeValueCallback, ProcessSceneReferenceBase reference, List<Guid> oldGuids, string text)
        {
            Action<Node> droppedGameObject = (Node selectedSceneObject) => HandleDroppedGameObject(changeValueCallback, reference, oldGuids, selectedSceneObject);
            return DropAreaGUI(reference, droppedGameObject, changeValueCallback, text);
        }*/

        /*private void DrawMisconfigurationOnSelectedGameObjects(ProcessSceneReferenceBase reference, Type valueType)
        {

            // Find all GameObjects that are missing the the component "valueType" needed
            IEnumerable<Node> gameObjectsWithMissingConfiguration = reference.Guids
                .SelectMany(guidToDisplay => RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(guidToDisplay))
                .Select(sceneObject => sceneObject.GameObject)
                .Where(sceneObject => sceneObject?.GetComponent(valueType) == null)
                .Distinct();


            // Add FixIt all if more than one game object exist
            if (gameObjectsWithMissingConfiguration.Count() > 1)
            {
                // guiLineRect = AddNewRectLine(ref originalRect, EditorDrawingHelper.SingleLineHeight);
                AddFixItAllButton(gameObjectsWithMissingConfiguration, valueType);
            }

            // Add FixIt on each component
            foreach (Node selectedGameObject in gameObjectsWithMissingConfiguration)
            {
                AddFixItButton(selectedGameObject, valueType);
            }
        }

        private void HandleDroppedGameObject(Action<object> changeValueCallback, ProcessSceneReferenceBase reference, List<Guid> oldGuids, Node selectedSceneObject)
        {
            if (selectedSceneObject != null)
            {
                ProcessSceneObject processSceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();

                if (processSceneObject == null)
                {
                    Guid newGuid = OpenMissingProcessSceneObjectDialog(selectedSceneObject);

                    if (newGuid != Guid.Empty)
                    {
                        SetNewGroups(reference, oldGuids, new List<Guid> { newGuid }, changeValueCallback);
                    }
                }
                else if (GetAllGuids(processSceneObject).Count() == 1)
                {
                    SetNewGroups(reference, oldGuids, GetAllGuids(processSceneObject), changeValueCallback);
                }
                else
                {
                    // if the PSO has multiple groups we let the user decide which ones he wants to take
                    Action<SceneObjectGroups.SceneObjectGroup> onItemSelected = (SceneObjectGroups.SceneObjectGroup selectedGroup) => { SetNewGroup(reference, oldGuids, selectedGroup.Guid, changeValueCallback); };

                    // availableGroups the processSceneObject.Guid and all groups of the PSO
                    IEnumerable<SceneObjectGroups.SceneObjectGroup> availableGroups = new List<SceneObjectGroups.SceneObjectGroup>() { new SceneObjectGroups.SceneObjectGroup(SceneObjectGroups.UniqueGuidName, processSceneObject.Guid) };
                    availableGroups = availableGroups.Concat(SceneObjectGroups.Instance.Groups.Where(group => processSceneObject.Guids.Contains(group.Guid) == true));
                    DrawSearchableGroupListPopup(dropDownRect, onItemSelected, availableGroups);
                }
            }
        }*/

        /// <summary>
        /// Renders a drop area GUI for assigning groups to the behavior or condition.
        /// </summary>
        /// <param name="originalRect">The rect of the whole behavior or condition.</param>
        /// <param name="guiLineRect">The rect of the last drawn line.</param>
        /// <param name="dropAction">The action to perform when a game object is dropped.</param>
        /*protected Control DropAreaGUI(ProcessSceneReferenceBase reference, Action<Node> dropAction, Action<object> changeValueCallback, string text)
        {
            Event evt = Event.current;

            // Measure the content size and determine how many lines the content will occupy
            string referenceValue = GetReferenceValue(reference);
            string tooltip = GetTooltip(reference);
            string boxContent = string.IsNullOrEmpty(referenceValue) ? "Drop a game object here to assign it or any of its groups" : $"Selected {referenceValue}";
            GUIContent content = new GUIContent(boxContent, tooltip);
            GUIStyle style = GUI.skin.box;

            int lines = CalculateContentLines(content, originalRect, style, 3 * buttonWidth + 16); // Adding 16 pixels for padding between buttons
            float dropdownHeight = EditorDrawingHelper.ButtonHeight + (lines - 1) * EditorDrawingHelper.SingleLineHeight;
            guiLineRect = AddNewRectLine(ref originalRect, dropdownHeight);
            Rect flyoutRect = guiLineRect;

            GUILayout.BeginArea(guiLineRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(content, GUILayout.Height(dropdownHeight), GUILayout.ExpandWidth(true));

            if (GUILayout.Button(showIcon.Texture, GUILayout.Height(EditorDrawingHelper.ButtonHeight), GUILayout.MaxWidth(buttonWidth)))
            {
                OnShowReferencesClick(reference, changeValueCallback, dropdownHeight, flyoutRect);
            }
            if (GUILayout.Button(editIcon.Texture, GUILayout.Height(EditorDrawingHelper.ButtonHeight), GUILayout.MaxWidth(buttonWidth)))
            {
                OnEditReferencesClick(reference, changeValueCallback, dropdownHeight, flyoutRect);
            }

            if (GUILayout.Button(deleteIcon.Texture, GUILayout.Height(EditorDrawingHelper.ButtonHeight), GUILayout.MaxWidth(buttonWidth)))
            {
                reference.ResetGuids();
                changeValueCallback(reference);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            switch (evt.type)
            {
                case ENetConnection.EventType.DragUpdated:
                case ENetConnection.EventType.DragPerform:
                    if (!guiLineRect.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (evt.type == ENetConnection.EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (GameObject dragged_object in DragAndDrop.objectReferences)
                        {
                            dropAction(dragged_object, flyoutRect);
                        }
                    }
                    break;
            }
        }*/

        /*private Rect OnEditReferencesClick(ProcessSceneReferenceBase reference, Action<object> changeValueCallback, float dropdownHeight, Rect flyoutRect)
        {
            Action<SceneObjectGroups.SceneObjectGroup> onItemSelected = (SceneObjectGroups.SceneObjectGroup selectedGroup) => { AddGroup(reference, reference.Guids, selectedGroup.Guid, changeValueCallback); };

            flyoutRect = SetupLocalFlyoutRect(GUILayoutUtility.GetLastRect(), dropdownHeight, flyoutRect.width);
            var availableGroups = SceneObjectGroups.Instance.Groups.Where(group => !reference.Guids.Contains(group.Guid));
            DrawSearchableGroupListPopup(flyoutRect, onItemSelected, availableGroups);
            return flyoutRect;
        }*/

        /*private void OnShowReferencesClick(ProcessSceneReferenceBase reference, Action<object> changeValueCallback, float dropdownHeight, Rect flyoutRect)
        {
            if (!reference.HasValue())
            {
                if (reference.Guids.Count() > 0)
                {
                    // we have deleted groups and want to to show them in the popup
                    flyoutRect = SetupLocalFlyoutRect(GUILayoutUtility.GetLastRect(), dropdownHeight, flyoutRect.width);
                    DrawSceneReferencesEditorPopup(reference, changeValueCallback, flyoutRect);
                }
            }
            else if (reference.Guids.Count() == 1 && !SceneObjectGroups.Instance.GroupExists(reference.Guids.First()))
            {
                // we have only one guid and it is a PSO so we want to ping the object
                IEnumerable<ISceneObject> processSceneObjectsWithGroup = RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(reference.Guids.First());
                EditorGUIUtility.PingObject(processSceneObjectsWithGroup.First().GameObject);
            }
            else
            {
                flyoutRect = SetupLocalFlyoutRect(GUILayoutUtility.GetLastRect(), dropdownHeight, flyoutRect.width);
                DrawSceneReferencesEditorPopup(reference, changeValueCallback, flyoutRect);
            }
        }*/

        /*private Rect SetupLocalFlyoutRect(Rect lastRect, float dropdownHeight, float flyoutRectWidth)
        {
            Rect editGroupDropdownRect = lastRect;
            editGroupDropdownRect.width = flyoutRectWidth;
            editGroupDropdownRect.y += dropdownHeight;
            return editGroupDropdownRect;
        }

        private void DrawSceneReferencesEditorPopup(ProcessSceneReferenceBase reference, Action<object> changeValueCallback, Rect flyoutRect)
        {
            SceneReferencesEditorPopup sceneReferencesEditorPopup = new SceneReferencesEditorPopup(reference, changeValueCallback);
            sceneReferencesEditorPopup.SetWindowSize(windowWith: flyoutRect.width);

            UnityEditor.PopupWindow.Show(flyoutRect, sceneReferencesEditorPopup);
        }

        private string GetTooltip(ProcessSceneReferenceBase reference)
        {
            if (reference.IsEmpty())
            {
                return "No objects referenced";
            }

            StringBuilder tooltip = new StringBuilder("Objects in scene:");

            foreach (Guid guid in reference.Guids)
            {
                if (SceneObjectGroups.Instance.GroupExists(guid))
                {
                    string label = SceneObjectGroups.Instance.GetLabel(guid);
                    int objectsInScene = RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(guid).Count();
                    tooltip.Append($"\n- Group '{SceneObjectGroups.Instance.GetLabel(guid)}': {objectsInScene} objects");
                }
                else
                {
                    foreach (ISceneObject sceneObject in RuntimeConfigurator.Configuration.SceneObjectRegistry.GetObjects(guid))
                    {
                        tooltip.Append($"\n- {sceneObject.GameObject.name}");
                    }
                }
            }

            return tooltip.ToString();
        }

        private int CalculateContentLines(GUIContent content, Rect originalRect, GUIStyle style, int totalButtonsWidth)
        {
            Vector2 size = style.CalcSize(content);
            int lines = Mathf.CeilToInt(size.x / (originalRect.width - totalButtonsWidth));
            return lines;
        }


        protected Guid OpenMissingProcessSceneObjectDialog(GameObject selectedSceneObject)
        {
            Guid guid = Guid.Empty;

            if (selectedSceneObject != null)
            {
                if (EditorUtility.DisplayDialog("No Process Scene Object component", "This object does not have a Process Scene Object component.\n" +
                                                                                     "A Process Scene Object component is required for the object to work with the VR Builder process.\n" +
                                                                                     "Do you want to add one now?", "Yes", "No"))
                {
                    guid = selectedSceneObject.AddComponent<ProcessSceneObject>().Guid;
                    EditorUtility.SetDirty(selectedSceneObject);
                }
            }
            return guid;
        }

        // TODO Has duplicated code with AddFixItButton. Should be refactored if we keep FixItButton
        // TODO Undo does not work properly here and on AddFixItButton e.g.: a GrabCondition its only removing The GrabbableProperty but not TouchableProperty, IntractableProperty and Rigidbody
        protected void AddFixItAllButton(IEnumerable<GameObject> selectedSceneObject, Type valueType, ref Rect originalRect, ref Rect guiLineRect)
        {
            string warning = $"Some Scene Objects are not configured as {valueType.Name}";
            const string button = "Fix all";
            EditorGUI.HelpBox(guiLineRect, warning, EditorGUI.MessageType.Warning);
            guiLineRect = AddNewRectLine(ref originalRect);

            if (GUI.Button(guiLineRect, button))
            {
                foreach (GameObject sceneObject in selectedSceneObject)
                {
                    // Only relevant for Undoing a Process Property.
                    bool isAlreadySceneObject = sceneObject.GetComponent<ProcessSceneObject>() != null && typeof(ISceneObjectProperty).IsAssignableFrom(valueType);
                    Component[] alreadyAttachedProperties = sceneObject.GetComponents(typeof(Component));

                    RevertableChangesHandler (Godot: TinkerFlowPlugin.Instance.GetUndoRedo()).Do(
                        new ProcessCommand(
                            () => SceneObjectAutomaticSetup(sceneObject, valueType),
                            () => UndoSceneObjectAutomaticSetup(sceneObject, valueType, isAlreadySceneObject, alreadyAttachedProperties)));
                }
            }
            guiLineRect = AddNewRectLine(ref originalRect);
        }

        protected void AddFixItButton(Node selectedSceneObject, Type valueType)
        {
            guiLineRect = AddNewRectLine(ref originalRect);

            string warning = $"{selectedSceneObject.name} is not configured as {valueType.Name}";
            const string button = "Fix it";
            EditorGUI.HelpBox(guiLineRect, warning, EditorGUI.MessageType.Warning);
            guiLineRect = AddNewRectLine(ref originalRect);

            if (GUI.Button(guiLineRect, button))
            {
                // Only relevant for Undoing a Process Property.
                bool isAlreadySceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() != null && typeof(ISceneObjectProperty).IsAssignableFrom(valueType);
                Component[] alreadyAttachedProperties = selectedSceneObject.GetComponents(typeof(Component));

                RevertableChangesHandler (Godot: TinkerFlowPlugin.Instance.GetUndoRedo()).Do(
                    new ProcessCommand(
                        () => SceneObjectAutomaticSetup(selectedSceneObject, valueType),
                        () => UndoSceneObjectAutomaticSetup(selectedSceneObject, valueType, isAlreadySceneObject, alreadyAttachedProperties)));
            }
        }

        // ToDo suggesting to move this in to a helper class
        protected Rect AddNewRectLine(ref Rect currentRect, float height = float.MinValue)
        {
            Rect newRectLine = currentRect;
            newRectLine.height = height == float.MinValue ? EditorDrawingHelper.SingleLineHeight : height;
            newRectLine.y += currentRect.height + EditorDrawingHelper.VerticalSpacing;

            currentRect.height += height == float.MinValue ? EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing : height + EditorDrawingHelper.VerticalSpacing;
            return newRectLine;
        }

        // ToDo suggesting to move this in to a helper class
        protected void SceneObjectAutomaticSetup(GameObject selectedSceneObject, Type valueType)
        {
            ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() ?? selectedSceneObject.AddComponent<ProcessSceneObject>();

            if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
            {
                sceneObject.AddProcessProperty(valueType);
            }

            isUndoOperation = true;
        }

        private void SetNewGroups(ProcessSceneReferenceBase reference, IEnumerable<Guid> oldGuids, IEnumerable<Guid> newGuids, Action<object> changeValueCallback)
        {
            if (new HashSet<Guid>(oldGuids).SetEquals(newGuids))
            {
                return;
            }
            ChangeValue(
                () =>
                {
                    reference.ResetGuids(newGuids);
                    return reference;
                },
                () =>
                {
                    reference.ResetGuids(oldGuids);
                    return reference;
                },
                changeValueCallback);
        }

        private void SetNewGroup(ProcessSceneReferenceBase reference, IEnumerable<Guid> oldGuids, Guid newGuid, Action<object> changeValueCallback)
        {
            if (oldGuids.Count() == 1 && oldGuids.Contains(newGuid))
            {
                return;
            }
            ChangeValue(
                () =>
                {
                    reference.ResetGuids(new List<Guid> { newGuid });
                    return reference;
                },
                () =>
                {
                    reference.ResetGuids(oldGuids);
                    return reference;
                },
                changeValueCallback);
        }

        private void AddGroup(ProcessSceneReferenceBase reference, IEnumerable<Guid> oldGuids, Guid newGuid, Action<object> changeValueCallback)
        {
            if (oldGuids.Contains(newGuid))
            {
                return;
            }

            ChangeValue(
                () =>
                {
                    reference.ResetGuids(oldGuids.Concat(new List<Guid> { newGuid }));
                    return reference;
                },
                () =>
                {
                    reference.ResetGuids(oldGuids);
                    return reference;
                },
                changeValueCallback);
        }

        private void DrawSearchableGroupListPopup(Rect rect, Action<SceneObjectGroups.SceneObjectGroup> onItemSelected, IEnumerable<SceneObjectGroups.SceneObjectGroup> availableGroups = null)
        {
            VisualTreeAsset searchableList = ViewDictionary.LoadAsset(ViewDictionary.EnumType.SearchableList);
            VisualTreeAsset groupListItem = ViewDictionary.LoadAsset(ViewDictionary.EnumType.SearchableListItem);

            SearchableGroupListPopup content = new SearchableGroupListPopup(onItemSelected, searchableList, groupListItem);

            if (availableGroups == null)
            {
                availableGroups = SceneObjectGroups.Instance.Groups;
            }

            content.SetAvailableGroups(availableGroups);
            content.SetWindowSize(windowWith: rect.width);

            UnityEditor.PopupWindow.Show(rect, content);
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

        /// <summary>
        /// Initializes the rich text label style.
        /// </summary>
        /// <remarks>
        /// GUIStyle can only be used within OnGUI() and not in a constructor.
        /// </remarks>
        private void InitializeRichTextLabelStyle()
        {
            if (richTextLabelStyle == null)
            {
                // Note:
                richTextLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    richText = true
                };
            }
        }*/
    }
}
