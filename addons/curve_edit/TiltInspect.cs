using Godot;
using System;

[Tool]
public partial class TiltInspect : Control
{
    // var title = "Vector";
    private string title = "Vector";

    // var vector = Vector3(0,0,0);
    private Vector3 vector = new Vector3();

    // var tilt: float = float(0);
    private float tilt = 0f;

    [Signal]
    public delegate void VectorChangedEventHandler(Vector3 newVector);

    [Signal]
    public delegate void TiltChangedEventHandler(float newTilt);

    public override void _Ready()
    {
        Setup(title);
    }

    public void Setup(string newTitle)
    {
        title = newTitle;
        GetNode<Label>("Label").Text = title;
    }

    public void OnXBoxValueChanged(float value)
    {
        vector.X = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    public void OnYBoxValueChanged(float value)
    {
        vector.Y = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    public void OnZBoxValueChanged(float value)
    {
        vector.Z = value;
        EmitSignal(SignalName.VectorChanged, vector);
    }

    public void OnTiltBoxValueChanged(float value)
    {
        tilt = value;
        EmitSignal(SignalName.TiltChanged, tilt);
    }

    public float GetTilt()
    {
        return tilt;
    }
    
    public void SetTilt(float newTilt)
    {
        tilt = newTilt;
        GetNode<HSlider>("$valueContain/tiltContain/tiltBox").Value = tilt;
    }
}