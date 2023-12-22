// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using Godot;

namespace VRBuilder.Editor.UI.Drawers;

/// <summary>
/// Process drawer for boolean members.
/// </summary>
[DefaultProcessDrawer(typeof(bool))]
internal class BoolFactory : AbstractProcessFactory
{
    /// <inheritdoc />
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
    {
        var value = Convert.ToBoolean(currentValue);

        var container = new HBoxContainer();
        container.AddChild(CreateCheckBox<T>(changeValueCallback, value));
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