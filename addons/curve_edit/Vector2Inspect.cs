using Godot;
using System;

[Tool]
public partial class Vector2Inspect : Control
{
    private string title = "Vector";
    private Vector2 vector = new Vector2();

    [Signal]
    public delegate void VectorChangedEventHandler(Vector2 vector);

    public override void _Ready()
    {
        Setup(title);
    }

    private void Setup(string newTitle)
    {
        title = newTitle;
        GetNode<Label>("Label").Text = title;
    }

    private void _OnXBoxValueChanged(float value)
    {
        vector.X = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    private void _OnYBoxValueChanged(float value)
    {
        vector.Y = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    public Vector2 GetVector()
    {
        return vector;
    }

    public void SetVector(Vector2 newVector)
    {
        vector = newVector;
        GetNode<HSlider>("valueContain/xContain/xBox").Value = vector.X;
        GetNode<HSlider>("valueContain/yContain/yBox").Value = vector.Y;
    }
}