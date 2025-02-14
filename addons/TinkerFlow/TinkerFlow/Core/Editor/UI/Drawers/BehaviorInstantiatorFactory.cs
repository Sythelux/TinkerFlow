// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Draws a dropdown button with all <see cref="InstantiationOption{IBehavior}"/> in the project, and creates a new instance of choosen behavior on click.
    /// </summary>
    [InstantiatorProcessDrawer(typeof(IBehavior))]
    internal partial class BehaviorInstantiatorFactory : AbstractInstantiatorFactory<IBehavior>
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            Control control = new Control();
            if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetBehaviorMenuOptions().Any())
            {
                control.AddChild(new Button { Text = "Add Behavior" });

                IList<TestableEditorElements.MenuOption> options = ConvertFromConfigurationOptionsToGenericMenuOptions(EditorConfigurator.Instance.BehaviorsMenuContent.ToList(), currentValue, changeValueCallback);
                control.AddChild(TestableEditorElements.DisplayContextMenu(options));

                // TODO:
                // if (EditorDrawingHelper.DrawHelpButton(ref rect))
                // {
                //     Application.OpenURL("https://www.mindport.co/vr-builder/manual/default-behaviors");
                // }
            }
            else
            {
                control.AddChild(EditorGUI.HelpBox("Your project does not contain any Behaviors. Either create one or import a VR Builder Component.", EditorGUI.MessageType.Error));
            }

            return control;
        }
    }
}
