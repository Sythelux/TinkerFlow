using Godot;
using System;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for a 0-1 float slider.
    /// </summary>
    internal class NormalizedFloatFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var hSlider = new HSlider();
            if (currentValue is float floatValue)
                hSlider.Value = floatValue;
            hSlider.MaxValue = 1;
            hSlider.MinValue = 0;
            hSlider.Changed += () => changeValueCallback?.Invoke(hSlider.Value);
            return hSlider;
        }
    }
}
