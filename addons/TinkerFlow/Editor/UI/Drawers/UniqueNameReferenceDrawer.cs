//  // Copyright (c) 2013-2019 Innoactive GmbH
//  // Licensed under the Apache License, Version 2.0
//  // Modifications copyright (c) 2021-2023 MindPort GmbH
//
//  using System;
//  using System.Collections.Generic;
//  using System.ComponentModel;
//  using System.Diagnostics;
//  using System.Reflection;
//  using Godot;
//  using VRBuilder.Core.Configuration;
//  using VRBuilder.Core.Properties;
//  using VRBuilder.Core.SceneObjects;
//  using VRBuilder.Core.Utils;
//
//  namespace VRBuilder.Editor.UI.Drawers
//  {
//      /// <summary>
// /// TODO: implement drop: https://docs.godotengine.org/en/latest/classes/class_control.html#class-control-private-method-get-drag-data
//      /// Process drawer for <see cref="UniqueNameReference"/> members.
//      /// </summary>
//      [DefaultProcessDrawer(typeof(UniqueNameReference))]
//      public partial class UniqueNameReferenceDrawer : AbstractProcessFactory
//      {
//          protected bool isUndoOperation;
//          protected const string undoGroupName = "brotcat";
//
//          protected readonly HashSet<string> missingUniqueNames = new HashSet<string>();
//
//          /// <inheritdoc />
//          public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
//          {
//              var control = new Control();
//              if (RuntimeConfigurator.Exists == false)
//              {
//                  return control;
//              }
//
//              isUndoOperation = false;
//              UniqueNameReference? uniqueNameReference = currentValue as UniqueNameReference;
//              PropertyInfo? valueProperty = currentValue?.GetType().GetProperty("Value");
//              Type valueType = ReflectionUtils.GetDeclaredTypeOfPropertyOrField(valueProperty);
//
//              if (valueProperty == null)
//              {
//                  throw new ArgumentException("Only ObjectReference<> implementations should inherit from the UniqueNameReference type.");
//              }
//
//              string oldUniqueName = uniqueNameReference.UniqueName;
//              Node? selectedSceneObject = GetGameObjectFromID(oldUniqueName);
//
//              if (selectedSceneObject == null && string.IsNullOrEmpty(oldUniqueName) == false && missingUniqueNames.Contains(oldUniqueName) == false)
//              {
//                  missingUniqueNames.Add(oldUniqueName);
//                  GD.PushError($"The process object with the unique name '{oldUniqueName}' cannot be found!");
//              }
//
//              CheckForMisconfigurationIssues(selectedSceneObject, valueType);
//              selectedSceneObject = ObjectField(label, selectedSceneObject, typeof(Node), true) as Node;
//
//              string newUniqueName = GetIDFromSelectedObject(selectedSceneObject, valueType, oldUniqueName);
//
//              if (oldUniqueName != newUniqueName)
//              {
//                  RevertableChangesHandler.Do(
//                      new ProcessCommand(
//                          () =>
//                          {
//                              uniqueNameReference.UniqueName = newUniqueName;
//                              changeValueCallback(uniqueNameReference);
//                          },
//                          () =>
//                          {
//                              uniqueNameReference.UniqueName = oldUniqueName;
//                              changeValueCallback(uniqueNameReference);
//                          }),
//                      isUndoOperation ? undoGroupName : string.Empty);
//
//                  if (isUndoOperation)
//                  {
//                      RevertableChangesHandler.CollapseUndoOperations(undoGroupName);
//                  }
//              }
//
//              return control;
//          }
//
//          private Node ObjectField(Control label, Node selectedSceneObject, Type type, bool b)
//          {
//              // https://docs.godotengine.org/en/latest/classes/class_control.html#class-control-private-method-get-drag-data
//          }
//
//          protected Node? GetGameObjectFromID(string objectUniqueName)
//          {
//              if (string.IsNullOrEmpty(objectUniqueName))
//              {
//                  return null;
//              }
//
//              // If the Runtime Configurator exists, we try to retrieve the process object
//              try
//              {
//                  if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsName(objectUniqueName) == false)
//                  {
//                      // If the saved unique name is not registered in the scene, perhaps is actually a GameObject's InstanceID
//                      return GetGameObjectFromInstanceID(objectUniqueName);
//                  }
//
//                  ISceneObject sceneObject = RuntimeConfigurator.Configuration.SceneObjectRegistry.GetByName(objectUniqueName);
//                  return sceneObject.GameObject;
//              }
//              catch
//              {
//                  return null;
//              }
//          }
//
//          protected string GetIDFromSelectedObject(GameObject selectedSceneObject, Type valueType, string oldUniqueName)
//          {
//              string newUniqueName = string.Empty;
//
//              if (selectedSceneObject != null)
//              {
//                  if (selectedSceneObject.GetComponent(valueType) != null)
//                  {
//                      if (typeof(ISceneObject).IsAssignableFrom(valueType))
//                      {
//                          newUniqueName = GetUniqueNameFromSceneObject(selectedSceneObject);
//                      }
//                      else if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
//                      {
//                          newUniqueName = GetUniqueNameFromProcessProperty(selectedSceneObject, valueType, oldUniqueName);
//                      }
//                  }
//                  else
//                  {
//                      newUniqueName = selectedSceneObject.GetInstanceID().ToString();
//                  }
//              }
//
//              return newUniqueName;
//          }
//
//          private GameObject GetGameObjectFromInstanceID(string objectUniqueName)
//          {
//              GameObject gameObject = null;
//
//              if (int.TryParse(objectUniqueName, out int instanceId))
//              {
//                  gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
//              }
//
//              return gameObject;
//          }
//
//          private string GetUniqueNameFromSceneObject(GameObject selectedSceneObject)
//          {
//              ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();
//
//              if (sceneObject != null)
//              {
//                  return sceneObject.UniqueName;
//              }
//
//              Debug.LogWarning($"Game Object \"{selectedSceneObject.name}\" does not have a Process Object component.");
//              return string.Empty;
//          }
//
//          private string GetUniqueNameFromProcessProperty(GameObject selectedProcessPropertyObject, Type valueType, string oldUniqueName)
//          {
//              if (selectedProcessPropertyObject.GetComponent(valueType) is ISceneObjectProperty processProperty)
//              {
//                  return processProperty.SceneObject.UniqueName;
//              }
//
//              Debug.LogWarning($"Scene Object \"{selectedProcessPropertyObject.name}\" with Unique Name \"{oldUniqueName}\" does not have a {valueType.Name} component.");
//              return string.Empty;
//          }
//
//          protected void CheckForMisconfigurationIssues(GameObject selectedSceneObject, Type valueType, ref Rect originalRect, ref Rect guiLineRect)
//          {
//              if (selectedSceneObject != null && selectedSceneObject.GetComponent(valueType) == null)
//              {
//                  string warning = $"{selectedSceneObject.name} is not configured as {valueType.Name}";
//                  const string button = "Fix it";
//
//                  EditorGUI.HelpBox(guiLineRect, warning, MessageType.Error);
//                  guiLineRect = AddNewRectLine(ref originalRect);
//
//                  if (GUI.Button(guiLineRect, button))
//                  {
//                      // Only relevant for Undoing a Process Property.
//                      bool isAlreadySceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() != null && typeof(ISceneObjectProperty).IsAssignableFrom(valueType);
//                      Component[] alreadyAttachedProperties = selectedSceneObject.GetComponents(typeof(Component));
//
//                      RevertableChangesHandler.Do(
//                          new ProcessCommand(
//                              () => SceneObjectAutomaticSetup(selectedSceneObject, valueType),
//                              () => UndoSceneObjectAutomaticSetup(selectedSceneObject, valueType, isAlreadySceneObject, alreadyAttachedProperties)),
//                          undoGroupName);
//                  }
//
//                  guiLineRect = AddNewRectLine(ref originalRect);
//              }
//          }
//
//          protected void SceneObjectAutomaticSetup(GameObject selectedSceneObject, Type valueType)
//          {
//              ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>() ?? selectedSceneObject.AddComponent<ProcessSceneObject>();
//
//              if (RuntimeConfigurator.Configuration.SceneObjectRegistry.ContainsGuid(sceneObject.Guid) == false)
//              {
//                  // Sets a UniqueName and then registers it.
//                  sceneObject.SetSuitableName();
//              }
//
//              if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
//              {
//                  sceneObject.AddProcessProperty(valueType);
//              }
//
//              isUndoOperation = true;
//          }
//
//          private void UndoSceneObjectAutomaticSetup(GameObject selectedSceneObject, Type valueType, bool hadProcessComponent, Component[] alreadyAttachedProperties)
//          {
//              ISceneObject sceneObject = selectedSceneObject.GetComponent<ProcessSceneObject>();
//
//              if (typeof(ISceneObjectProperty).IsAssignableFrom(valueType))
//              {
//                  sceneObject.RemoveProcessProperty(valueType, true, alreadyAttachedProperties);
//              }
//
//              if (hadProcessComponent == false)
//              {
//                  Object.DestroyImmediate((ProcessSceneObject)sceneObject);
//              }
//
//              isUndoOperation = true;
//          }
//
//          protected Rect AddNewRectLine(ref Rect currentRect)
//          {
//              Rect newRectLine = currentRect;
//              newRectLine.height = EditorDrawingHelper.SingleLineHeight;
//              newRectLine.y += currentRect.height + EditorDrawingHelper.VerticalSpacing;
//
//              currentRect.height += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
//              return newRectLine;
//          }
//      }
//  }

