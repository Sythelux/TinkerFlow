// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Draws a dropdown button with all <see cref="InstantiationOption{ICondition}"/> in the project, and creates a new instance of choosen condition on click.
    /// </summary>
    [InstantiatorProcessDrawer(typeof(ICondition))]
    internal partial class ConditionInstantiatorFactory : AbstractInstantiatorFactory<ICondition>
    {
        bool? drawButtonAllowed;

        public bool DrawButtonAllowed
        {
            get
            {
                drawButtonAllowed ??= EditorConfigurator.Instance.AllowedMenuItemsSettings.GetConditionMenuOptions().Any();
                return (bool)drawButtonAllowed;
            }
        }

        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var container = new VBoxContainer { Name = GetType().Name + "." + text };

            var row = new HBoxContainer
            {
                Name = GetType().Name + ".row",
                Alignment = BoxContainer.AlignmentMode.Center
            };

            Button drawAddButton = EditorDrawingHelper.DrawAddButton("Add Condition");

            drawAddButton.Disabled = !DrawButtonAllowed;
            drawAddButton.Pressed += OnDrawAddButtonPressed;

            row.AddChild(drawAddButton);


            var pasteButton = EditorDrawingHelper.DrawPasteButton();
            //TODO: this needs to be watched and dynamically refreshed
            bool tryParseJson = TryParseJson(DisplayServer.ClipboardGet(), out ICondition result);
            pasteButton.Disabled = !DisplayServer.ClipboardHas() || tryParseJson;

            pasteButton.Pressed += OnPasteButtonPressed;

            Button drawHelpButton = EditorDrawingHelper.DrawHelpButton();
            drawHelpButton.Pressed += OnDrawHelpButtonPressed;
            row.AddChild(drawHelpButton);

            container.AddChild(row);

            if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetConditionMenuOptions().Any() == false)
            {
                container.AddChild(new VSeparator());
                container.AddChild(EditorGUI.HelpBox("Your project does not contain any Conditions. Either create one or import a VR Builder Component.", EditorGUI.MessageType.Error));
                container.AddChild(new VSeparator());
            }

            return container;

            void OnDrawAddButtonPressed()
            {
                IList<TestableEditorElements.MenuOption> options = ConvertFromConfigurationOptionsToGenericMenuOptions(EditorConfigurator.Instance.ConditionsMenuContent.ToList(), currentValue, changeValueCallback);
                PopupMenu displayContextMenu = TestableEditorElements.DisplayContextMenu(options);
                drawAddButton.AddChild(displayContextMenu);
                displayContextMenu.PopupOnParent(new Rect2I((int)drawAddButton.GlobalPosition.X, (int)drawAddButton.GlobalPosition.Y, displayContextMenu.Size.X, 256));
                if (currentValue != null)
                {
                    GD.Print("Current value is not null");
                }
            }

            void OnDrawHelpButtonPressed()
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo("https://www.mindport.co/vr-builder/manual/default-conditions") { UseShellExecute = true });
            }

            void OnPasteButtonPressed()
            {
                IEntity entity = result;
                changeValueCallback(entity);
            }
        }
    }
}