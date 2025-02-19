// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VRBuilder.Core.Conditions;
using VRBuilder.Editor.Configuration;
using Godot;
using Godot.Collections;
using VRBuilder.Editor.Godot;

namespace VRBuilder.Editor.UI.Drawers
{
    /// <summary>
    /// Draws a dropdown button with all <see cref="InstantiationOption{ICondition}"/> in the project, and creates a new instance of choosen condition on click.
    /// </summary>
    [InstantiatorProcessDrawer(typeof(ICondition))]
    internal partial class ConditionInstantiatorDrawer : AbstractInstantiatorFactory<ICondition>
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            var control = new VBoxContainer();

            if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetConditionMenuOptions().Any() == true)
            {
                var tree = new Tree();

                var drawAddButton = EditorDrawingHelper.DrawAddButton("Add Condition");
                var popupMenu = new PopupMenu();
                drawAddButton.AddChild(popupMenu);
                drawAddButton.Pressed += OnButtonPressed;
                tree.AddChild(drawAddButton);

                void OnButtonPressed()
                {
                    var options = ConvertFromConfigurationOptionsToGenericMenuOptions(EditorConfigurator.Instance.ConditionsMenuContent, currentValue, changeValueCallback);
                    foreach (TestableEditorElements.MenuOption control in options)
                    {
                        if (control is TestableEditorElements.MenuSeparator separator)
                            popupMenu.AddSeparator(separator.Label.Text);
                        else
                            popupMenu.AddItem(control.Label.Text);
                    }

                    popupMenu.Show();
                }

                control.AddChild(tree);
            }


            var helpButton = EditorDrawingHelper.DrawHelpButton();
            helpButton.Pressed += OpenHelp;

            void OpenHelp()
            {
                Process.Start(new ProcessStartInfo("https://www.mindport.co/vr-builder/manual/default-conditions") { UseShellExecute = true });
            }

            if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetConditionMenuOptions().Any() == false)
            {
                control.AddChild(new VSeparator());
                control.AddChild(EditorGUI.HelpBox("Your project does not contain any Conditions. Either create one or import a VR Builder Component.", EditorGUI.MessageType.Error));
                control.AddChild(new VSeparator());
            }

            return control;
        }
    }
}