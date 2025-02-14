// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Editor.ProcessValidation;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Drawer for values implementing INameable interface.
    /// Instead of drawing a plain text as a label, it draws a TextField with the name.
    /// </summary>
    [DefaultProcessDrawer(typeof(INamedData))]
    public class NameableFactory : ObjectFactory
    {
        protected virtual Control CreateLabel<T>(T currentValue, Action<object> changeValueCallback, Label label)
        {
            if (currentValue is not INamedData nameable)
                return label;

            //TODO:
            // List<EditorReportEntry> reports = GetValidationReportsFor(nameable);
            // if (reports.Count > 0)
            // {
            //     return AddValidationInformation(new Label(), reports);
            // }

            if (nameable.GetType().GetProperties().Any(propertyInfo => propertyInfo.Name == nameof(nameable.Name)
                                                                       && propertyInfo.GetAttributes<IgnoreInStepInspectorAttribute>(true).FirstOrDefault() != null))
            {
                return CreateLabel(currentValue, changeValueCallback, label);
            }

            if (nameable is IRenameableData renameable)
            {
                DrawRenameable(renameable, changeValueCallback);
            }
            else
            {
                DrawName(nameable);
            }

            return label;
        }

        private void DrawRenameable(IRenameableData renameable, Action<object> changeValueCallback)
        {
            /*Rect nameRect = rect;
            nameRect.width = EditorGUIUtility.labelWidth;
            Rect typeRect = rect;
            typeRect.x += EditorGUIUtility.labelWidth;
            typeRect.width -= EditorGUIUtility.labelWidth;

            GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            string newName = EditorGUI.DelayedTextField(nameRect, renameable.Name, textFieldStyle);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                padding = new RectOffset(4, 0, 0, 0)
            };
            EditorGUI.LabelField(typeRect, GetTypeNameLabel(renameable, renameable.GetType()), labelStyle);

            if (newName != renameable.Name)
            {
                string oldName = renameable.Name;
                renameable.SetName(newName);
                ChangeValue(() =>
                    {
                        renameable.SetName(newName);
                        return renameable;
                    },
                    () =>
                    {
                        renameable.SetName(oldName);
                        return renameable;
                    }, changeValueCallback);
            }*/
        }

        private void DrawName(INamedData nameable)
        {
            /*GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                padding = new RectOffset(4, 0, 0, 0)
            };

            string label = nameable.Name;
            if (string.IsNullOrEmpty(label))
            {
                EditorGUI.LabelField(rect, GetTypeNameLabel(nameable, nameable.GetType()), labelStyle);
            }
            else
            {
                EditorGUI.LabelField(rect, new GUIContent(label), labelStyle);
            }*/
        }
    }
}
