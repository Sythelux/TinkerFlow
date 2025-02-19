// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for string members.
    /// </summary>
    [DefaultProcessDrawer(typeof(string))]
    public partial class StringFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            // var label = new Label { Text = text };
            // label.Owner = container;
            var value = Convert.ToString(currentValue);
            var textEdit = new TextEdit { Name = GetType().Name };
            var size = textEdit.CustomMinimumSize;
            size.Y = 48;
            textEdit.CustomMinimumSize = size;
            textEdit.Name = GetType().Name + "." + text;
            textEdit.Text = value;
            textEdit.PlaceholderText = text;
            textEdit.TextChanged += OnValueChanged;

            return textEdit;

            void OnValueChanged()
            {
                ChangeValue(() => textEdit.Text ?? string.Empty, () => value ?? string.Empty, changeValueCallback);
            }
        }
    }
}