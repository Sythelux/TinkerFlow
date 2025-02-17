// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.Godot;
using VRBuilder.Core.Editor.Util;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Draws a dropdown button with all <see cref="InstantiationOption{IBehavior}"/> in the project, and creates a new instance of choosen behavior on click.
    /// </summary>
    [InstantiatorProcessDrawer(typeof(IBehavior))]
    internal partial class BehaviorInstantiatorFactory : AbstractInstantiatorFactory<IBehavior>
    {
        bool? drawButtonAllowed;
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var container = new VBoxContainer { Name = GetType().Name + "." + text };

            var row = new HBoxContainer();
            row.Alignment = BoxContainer.AlignmentMode.Center;
            Button drawAddButton = EditorDrawingHelper.DrawAddButton("Add Behavior");
            //TODO: this needs to be watched and dynamically refreshed
            drawAddButton.Disabled = !DrawButtonAllowed;
            drawAddButton.Pressed += () =>
            {
                IList<TestableEditorElements.MenuOption> options = ConvertFromConfigurationOptionsToGenericMenuOptions(EditorConfigurator.Instance.BehaviorsMenuContent.ToList(), currentValue, changeValueCallback);
                PopupMenu displayContextMenu = TestableEditorElements.DisplayContextMenu(options);
                drawAddButton.AddChild(displayContextMenu);
                displayContextMenu.PopupOnParent(new Rect2I((int)drawAddButton.GlobalPosition.X, (int)drawAddButton.GlobalPosition.Y, displayContextMenu.Size.X, 256));
                if (currentValue != null)
                {
                    GD.Print("Current value is not null");
                }
            };
            row.AddChild(drawAddButton);

            var pasteButton = EditorDrawingHelper.DrawPasteButton();
            //TODO: this needs to be watched and dynamically refreshed
            bool tryParseJson = TryParseJson(DisplayServer.ClipboardGet(), out IBehavior result);
            pasteButton.Disabled = !DisplayServer.ClipboardHas() || tryParseJson;
            pasteButton.Pressed += () =>
            {
                IEntity entity = result;
                changeValueCallback(entity);
            };

            Button drawHelpButton = EditorDrawingHelper.DrawHelpButton();
            drawHelpButton.Pressed += () => Application.OpenURL("https://www.mindport.co/vr-builder/manual/default-behaviors");
            row.AddChild(drawHelpButton);

            container.AddChild(row);

            if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetBehaviorMenuOptions().Any() == false)
            {
                container.AddChild(EditorGUI.HelpBox("Your project does not contain any Behaviors. Either create one or import a VR Builder Component.", EditorGUI.MessageType.Error));
            }

            return container;
        }

        public bool DrawButtonAllowed
        {
            get
            {
                drawButtonAllowed ??= EditorConfigurator.Instance.AllowedMenuItemsSettings.GetBehaviorMenuOptions().Any();
                return (bool)drawButtonAllowed;
            }
        }

        public static bool TryParseJson<T>(string @this, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    success = false;
                    args.ErrorContext.Handled = true;
                },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
    }
}
