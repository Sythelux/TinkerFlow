// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Reflection;
using VRBuilder.Core.Editor.Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for `System.Color` members.
    /// </summary>
    [DefaultProcessDrawer(typeof(Color))]
    internal class SystemColorFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not Color color)
                return new Control();

            var colorPickerButton = new ColorPickerButton();
            colorPickerButton.Text = text;
            colorPickerButton.Color = color;
            colorPickerButton.ColorChanged += newColor =>
            {
                if (color != newColor)
                    ChangeValue(() => newColor, () => color, changeValueCallback);
            };

            return colorPickerButton;
        }
    }
}
