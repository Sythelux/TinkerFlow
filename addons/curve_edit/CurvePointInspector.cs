using Godot;
using System;

[Tool]
public partial class CurvePointInspector : VBoxContainer
{
    private PackedScene Vector2Inspector = (PackedScene)ResourceLoader.Load("res://addons/curve_edit/Vector2Inspect.tscn");
    private PackedScene Vector3Inspector = (PackedScene)ResourceLoader.Load("res://addons/curve_edit/Vector3Inspect.tscn");
    private PackedScene TiltInspector = (PackedScene)ResourceLoader.Load("res://addons/curve_edit/TiltInspect.tscn");
    private PackedScene CurrentVectorInspector;

    private Node pointPos;
    private Node pointIn;
    private Node pointOut;
    private Node pointTilt;

    [Signal]
    public delegate void PointPosChangedEventHandler(Vector2 vector);

    [Signal]
    public delegate void PointInChangedEventHandler(Vector2 vector);

    [Signal]
    public delegate void PointOutChangedEventHandler(Vector2 vector);

    [Signal]
    public delegate void PointTiltChangedEventHandler(float tilt);

    public override void _Ready()
    {
        SetupInspector(false);
    }

    private void SetupInspector(bool is2D)
    {
        if (is2D)
        {
            CurrentVectorInspector = Vector2Inspector;
        }
        // else
        // {
        //     CurrentVectorInspector = Vector3Inspector;
        // }

        foreach (Node n in GetChildren())
        {
            RemoveChild(n);
            n.QueueFree();
        }

        pointPos = (Node)CurrentVectorInspector.Instantiate();
        pointPos.Call("setup", "Position");
        // pointPos.Connect("vector_changed", this, nameof(PointPositionChanged));

        pointIn = (Node)CurrentVectorInspector.Instantiate();
        pointIn.Call("setup", "Tangent In");
        // pointIn.Connect("vector_changed", this, nameof(PointIncomingChanged));

        pointOut = (Node)CurrentVectorInspector.Instantiate();
        pointOut.Call("setup", "Tangent Out");
        // pointOut.Connect("vector_changed", this, nameof(PointOutgoingChanged));

        AddChild(pointPos);
        AddChild(pointIn);
        AddChild(pointOut);
        if (!is2D)
        {
            pointTilt = (Node)TiltInspector.Instantiate();
            // pointTilt.Connect("tilt_changed", this, nameof(PointTiltingChanged));
            AddChild(pointTilt);
        }
    }

    private void PointPositionChanged(Vector2 vector)
    {
        EmitSignal(nameof(PointPosChanged), vector);
    }

    private void PointIncomingChanged(Vector2 vector)
    {
        EmitSignal(nameof(PointInChanged), vector);
    }

    private void PointOutgoingChanged(Vector2 vector)
    {
        EmitSignal(nameof(PointOutChanged), vector);
    }

    private void PointTiltingChanged(float tilt)
    {
        EmitSignal(nameof(PointTiltChanged), tilt);
    }

    public void Set3DPointValues(Vector3 posVec, Vector3 inVec, Vector3 outVec, float tilt)
    {
        pointPos.Call("set_vector", posVec);
        pointIn.Call("set_vector", inVec);
        pointOut.Call("set_vector", outVec);
        pointTilt.Call("set_tilt", tilt);
    }

    public void Set2DPointValues(Vector2 posVec, Vector2 inVec, Vector2 outVec)
    {
        pointPos.Call("set_vector", posVec);
        pointIn.Call("set_vector", inVec);
        pointOut.Call("set_vector", outVec);
        // pointTilt.Call("set_tilt", tilt);
    }
}