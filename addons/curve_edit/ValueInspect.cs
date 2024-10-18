using Godot;
using System;

[Tool]
public partial class ValueInspect : Control
{
    private string title = "Value";
    private int value = 0;
    private SpinBox? vBox;
    public SpinBox VBox => vBox ??= GetNode<SpinBox>("VBox");

    [Signal]
    public delegate void ValueChangedEventHandler(int newValue);

    public override void _Ready()
    {
        Setup(title);
    }

    public void Setup(string newTitle)
    {
        title = newTitle;
        GetNode<Label>("Label").Text = title;
    }

    private void _OnVBoxValueChanged(int newValue)
    {
        value = newValue;
        EmitSignal(SignalName.ValueChanged, value);
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        VBox.Value = newValue;
    }

    public void SetMinMax(int newMin, int newMax)
    {
        VBox.MinValue = newMin;
        VBox.MaxValue = newMax;
        VBox.Value = Mathf.Clamp(value, newMin, newMax);
    }
}