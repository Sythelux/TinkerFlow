using System;
using Godot;

namespace VRBuilder.Editor.UI.Drawers;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[DefaultProcessDrawer(typeof(int))]
public partial class IntFactory : AbstractProcessFactory
{
    /// <inheritdoc />
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
    {
        var value = Convert.ToInt32(currentValue);
        var container = new HBoxContainer();
        container.AddChild(label);
        container.AddChild(CreateSpinBox<T>(changeValueCallback, value));
        return container;
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