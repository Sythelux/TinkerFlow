#if UNITY_6000_0_OR_NEWER
// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Reflection;
using Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for boolean members.
    /// </summary>
    [DefaultProcessDrawer(typeof(bool))]
    internal class BoolFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var value = Convert.ToBoolean(currentValue);

            var container = new HBoxContainer();
            Node checkBox = CreateCheckBox<T>(changeValueCallback, value);
            checkBox.Name = GetType().Name + "." + text;
            container.AddChild(checkBox);
            var label = new Label { Text = text };
            container.AddChild(label);

            return container;
        }

        private Node CreateCheckBox<T>(Action<object> changeValueCallback, bool value)
        {
            var checkBox = new CheckBox();
            checkBox.ButtonPressed = value;
            checkBox.Toggled += OnValueChanged;

            return checkBox;

            void OnValueChanged(bool toggledOn)
            {
                ChangeValue(() => toggledOn, () => value, changeValueCallback);
            }
        }
    }
}
#endif
