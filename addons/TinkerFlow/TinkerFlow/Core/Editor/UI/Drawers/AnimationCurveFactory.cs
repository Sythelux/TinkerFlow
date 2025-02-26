#if UNITY_5_3_OR_NEWER
using UnityEditor
using UnityEngine;
#elif GODOT
using Godot;
#endif
using System;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Drawer for <see cref="AnimationCurve"/>
    /// </summary>
    [DefaultProcessDrawer(typeof(Curve))]
    internal class AnimationCurveFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print(
                $"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is Curve curve)
            {
                void OnDraw()
                {
                    ChangeValue(() => curve, () => curve, changeValueCallback);
                }

                curve.Changed += OnDraw;

                var button = new Button
                {
                    Name = GetType().Name + "." + text,
                    Text = "Edit in Inspector"
                };

                button.Pressed += () => EditorInterface.Singleton.EditResource(curve);
                return button;
            }

            return new Control
            {
                Name = GetType().Name + "." + text
            };
        }
    }
}