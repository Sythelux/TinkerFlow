#if UNITY_5_3_OR_NEWER
using UnityEditor
using UnityEngine;
#elif GODOT
using Godot;
#endif
using System;
using System.Reflection;
using VRBuilder.Editor.Godot;

namespace VRBuilder.Editor.UI.Drawers
{
    /// <summary>
    /// Drawer for <see cref="AnimationCurve"/>
    /// </summary>
    [DefaultProcessDrawer(typeof(Curve))]
    internal class AnimationCurveDrawer : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var container = new HBoxContainer();
            // can't use the editor one because of: https://github.com/godotengine/godot-proposals/issues/3244
            // so make this good eventually.
            if (currentValue is Curve curve)
            {
                Line2D line = new Line2D();
                line.BeginCapMode = Line2D.LineCapMode.Round;
                line.JointMode = Line2D.LineJointMode.Round;
                line.Width = 10;
                line.SetCurve(curve);
                line.Draw += OnDraw;
                
                void OnDraw()
                {
                    ChangeValue(() => line.GetCurve(), () => curve, changeValueCallback);
                }
                container.AddChild(line);
            }

            return container;
        }
    }
}