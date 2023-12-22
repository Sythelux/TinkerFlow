// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using Godot;

namespace VRBuilder.Editor.UI.Drawers;

/// <summary>
/// Process drawer for string members.
/// </summary>
[DefaultProcessDrawer(typeof(string))]
internal class StringDrawer : AbstractProcessFactory
{
    /// <inheritdoc />
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
    {
        var value = Convert.ToString(currentValue);
        var textEdit = new TextEdit();
        textEdit.Text = value;
        textEdit.TextChanged += OnValueChanged;

        return textEdit;

        void OnValueChanged()
        {
            ChangeValue(() => textEdit.Text ?? string.Empty, () => value ?? string.Empty, changeValueCallback);
        }
    }
}