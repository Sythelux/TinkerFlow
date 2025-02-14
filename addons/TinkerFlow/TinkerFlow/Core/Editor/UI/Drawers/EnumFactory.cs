// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Linq;
using VRBuilder.Core.Editor.Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for `System.Enum` members.
    /// </summary>
    [DefaultProcessDrawer(typeof(Enum))]
    internal class EnumFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            Control control = new Control();

            // Enum oldValue = currentValue as Enum;
            //
            // Enum newValue;
            //
            // if (currentValue.GetType().GetAttributes(true).Any(attribute => attribute is FlagsAttribute))
            // {
            //     newValue = EditorGUI.EnumFlagsField(text, oldValue);
            // }
            // else
            // {
            //     newValue = EditorGUI.EnumPopup(text, oldValue);
            // }
            //
            // if (newValue.Equals(oldValue))
            // {
            //     return control;
            // }
            //
            // ChangeValue(() => newValue, () => oldValue, changeValueCallback);

            return control;
        }
    }
}
