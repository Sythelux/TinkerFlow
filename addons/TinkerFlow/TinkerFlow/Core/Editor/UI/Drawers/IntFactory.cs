using System;
using System.Reflection;
using Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [DefaultProcessDrawer(typeof(int))]
    public partial class IntFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var value = Convert.ToInt32(currentValue);
            // var label = new Label { Text = text };
            SpinBox spinBox = CreateSpinBox<T>(changeValueCallback, value);
            spinBox.Name = GetType().Name + "." + text;
            return spinBox;
        }

        private SpinBox CreateSpinBox<T>(Action<object> changeValueCallback, int value)
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
