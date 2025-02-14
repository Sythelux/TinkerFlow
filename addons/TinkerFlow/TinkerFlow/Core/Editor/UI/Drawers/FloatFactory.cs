// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for float members.
    /// </summary>
    [DefaultProcessDrawer(typeof(float))]
    internal class FloatFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var value = Convert.ToDouble(currentValue);
            // var label = new Label { Text = text };
            SpinBox spinBox = CreateSpinBox<T>(changeValueCallback, value);
            spinBox.Name = GetType().Name + "." + text;
            return spinBox;
        }

        private SpinBox CreateSpinBox<T>(Action<object> changeValueCallback, double value)
        {
            var spinBox = new SpinBox();
            spinBox.Value = value;
            spinBox.CustomArrowStep = 1;
            spinBox.ValueChanged += OnValueChanged;
            return spinBox;

            void OnValueChanged(double d)
            {
                ChangeValue(() => (int)d, () => value, changeValueCallback);
            }
        }
    }
}
