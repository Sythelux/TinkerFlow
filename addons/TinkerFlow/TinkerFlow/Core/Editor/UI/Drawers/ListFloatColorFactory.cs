// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Custom color drawer for the case when color is stored as a list of four floats.
    /// </summary>
    internal class ListFloatColorFactory : AbstractProcessFactory
    {
        private static List<float> ColorToList(Color color)
        {
            return new List<float>
            {
                color.R,
                color.G,
                color.B,
                color.A
            };
        }

        /// <inheritdoc />
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            // rect.height = EditorDrawingHelper.SingleLineHeight;
            //
            // List<float> list = (List<float>)currentValue;
            // if (list == null)
            // {
            //     list = ColorToList(Color.white);
            // }
            //
            // Color oldColor = new Color(list[0], list[1], list[2], list[3]);
            // Color newColor = EditorGUI.ColorField(rect, label, oldColor);
            //
            // if (newColor == oldColor)
            // {
            //     return rect;
            // }
            //
            // ChangeValue(() => ColorToList(newColor), () => ColorToList(oldColor), changeValueCallback);

            return new Control();
        }
    }
}
