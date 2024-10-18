using Godot;
using System;

[Tool]
public partial class CurveValueInspector : VBoxContainer
{
    private PackedScene ValueInspector = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/ValueInspect.tscn");
    
    private ValueInspect pointOffset;
    private ValueInspect pointVal;
    private ValueInspect angleLeft;
    private ValueInspect angleRight;
    private ValueInspect tilt;

    [Signal]
    public delegate void PointOffsetChangedEventHandler(float value);
    [Signal]
    public delegate void PointValueChangedEventHandler(float value);
    [Signal]
    public delegate void AngleLeftChangedEventHandler(float value);
    [Signal]
    public delegate void AngleRightChangedEventHandler(float value);
    [Signal]
    public delegate void TiltChangedEventHandler(float value);

    public override void _Ready()
    {
        SetupInspector();
    }

    public void SetupInspector()
    {
        foreach (Node n in GetChildren())
        {
            RemoveChild(n);
            n.QueueFree();
        }

        pointOffset = ValueInspector.Instantiate<ValueInspect>();
        pointOffset.Setup("Offset");
        pointOffset.ValueChanged += OnPointOffsetChanged;

        pointVal = ValueInspector.Instantiate<ValueInspect>();
        pointVal.Call("setup", "Value");
        pointVal.ValueChanged += OnPointValueChanged;


        angleLeft = ValueInspector.Instantiate<ValueInspect>();
        angleLeft.Call("setup", "Left (y/x) Ratio");
        angleLeft.ValueChanged += OnAngleLeftChanged;

        angleRight = ValueInspector.Instantiate<ValueInspect>();
        angleRight.Call("setup", "Right (y/x) Ratio");
        angleRight.ValueChanged += OnAngleRightChanged;

        tilt = ValueInspector.Instantiate<ValueInspect>();
        tilt.Call("setup", "Tilt");
        tilt.ValueChanged += OnTiltChanged;

        AddChild(pointOffset);
        AddChild(pointVal);
        AddChild(angleLeft);
        AddChild(angleRight);
        AddChild(tilt);
    }

    public void OnPointOffsetChanged(int value)
    {
        EmitSignal(SignalName.PointOffsetChanged, value);
    }

    public void OnPointValueChanged(int value)
    {
        EmitSignal(SignalName.PointValueChanged, value);
    }

    public void OnAngleLeftChanged(int value)
    {
        EmitSignal(SignalName.AngleLeftChanged, value);
    }

    public void OnAngleRightChanged(int value)
    {
        EmitSignal(SignalName.AngleRightChanged, value);
    }

    public void OnTiltChanged(int value)
    {
        EmitSignal(SignalName.TiltChanged, value);
    }

    public void SetPointVals(float offset, float value, float left, float right, float tiltvalue)
    {
        pointOffset.Call("set_value", offset);
        pointVal.Call("set_value", value);
        angleLeft.Call("set_value", left);
        angleRight.Call("set_value", right);
        tilt.Call("set_value", tiltvalue);
    }

    public void SetMinMax(float minVal, float maxVal)
    {
        pointVal.Call("set_min_max", minVal, maxVal);
    }

    public float GetValue()
    {
        return (float)pointVal.Call("get_value");
    }

    public void SetValue(float value)
    {
        pointVal.Call("set_value", value);
    }
}