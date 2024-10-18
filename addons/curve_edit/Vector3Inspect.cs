using Godot;
using System;

[Tool]
public partial class Vector3Inspect : Control
{
    private string title = "Vector";
    private Vector3 vector = new Vector3();

    [Signal]
    public delegate void VectorChangedEventHandler();

    public override void _Ready()
    {
        Setup(title);
    }

    private void Setup(string newTitle)
    {
        title = newTitle;
        GetNode<Label>("Label").Text = title;
    }

    private void OnXBoxValueChanged(float value)
    {
        vector.X = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    private void OnYBoxValueChanged(float value)
    {
        vector.Y = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    private void OnZBoxValueChanged(float value)
    {
        vector.Z = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    public Vector3 GetVector()
    {
        return vector;
    }

    public void SetVector(Vector3 newVector)
    {
        vector = newVector;
        GetNode<HSlider>("$valueContain/xContain/xBox").Value = vector.X;
        GetNode<HSlider>("$valueContain/yContain/yBox").Value = vector.Y;
        GetNode<HSlider>("$valueContain/zContain/zBox").Value = vector.Z;
    }
}