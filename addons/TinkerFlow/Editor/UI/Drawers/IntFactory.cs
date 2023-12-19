using System;
using System.Reflection;
using Godot;

namespace VRBuilder.Editor.UI.Drawers;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[DefaultProcessDrawer(typeof(int))]
public partial class IntFactory : AbstractProcessFactory
{
    /// <inheritdoc />
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string label)
    {
        var value = Convert.ToInt32(currentValue);
        var spinBox = new SpinBox();
        spinBox.CustomArrowStep = 1;
        spinBox.ValueChanged += SpinBoxOnChanged;

        void SpinBoxOnChanged(double d)
        {
            ChangeValue(() => (int)d, () => value, changeValueCallback);
        }

        return spinBox;
    }
}