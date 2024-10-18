using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class CurveValueControl : VBoxContainer
{
    private int numPoints = 0;

    private PackedScene CurvePointInspector = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurveValueInspector.tscn");
    private object numPointsInspector;
    private readonly Array<Label> curveInspectorLabels = new Array<Label>();
    private readonly Array<CurveValueInspector> curveInspectors = new Array<CurveValueInspector>();

    private Curve curve;

    public void ChangeOffset(float offset, int id)
    {
        curve.SetPointOffset(id, offset);
    }

    public void ChangeValue(float value, int id)
    {
        curve.SetPointValue(id, value);
    }

    public void ChangeLeftAngle(float angle, int id)
    {
        curve.SetPointLeftTangent(id, angle);
    }

    public void ChangeRightAngle(float angle, int id)
    {
        curve.SetPointRightTangent(id, angle);
    }

    public void ChangeTilt(float tilt, int id)
    {
        // curve.SetPointTilt(id, tilt);
    }

    public void SetCurve(Curve newCurve)
    {
        curve = newCurve;

        ClearChildren();

        curve.RangeChanged -= ChangeRange; //in case it is already connected
        curve.RangeChanged += ChangeRange;

        CreateRefresh();

        numPoints = curve.GetPointCount();

        for (int n = 0; n < numPoints; n++)
        {
            AddPointInspector(n);
        }
    }

    public void CreateRefresh()
    {
        Button b = new Button();
        b.Text = "Refresh";
        b.Pressed += () => SetCurve(curve);
        AddChild(b);
    }

    public void ClearChildren()
    {
        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        curveInspectorLabels.Clear();
        curveInspectors.Clear();
    }

    public void AddPointInspector(int index)
    {
        Label l = new Label();
        l.Text = "Point " + index;
        l.HorizontalAlignment = HorizontalAlignment.Center;
        l.VerticalAlignment = VerticalAlignment.Center;
        AddChild(l);

        curveInspectorLabels.Add(l);

        var i = CurvePointInspector.Instantiate<CurveValueInspector>();
        if (curve is Curve3D)
        {
            // Assuming 'Curve3D' is a class with the necessary methods
            // i.Call("SetPointVals", curve.GetPointPosition(index).x, curve.GetPointPosition(index).y, curve.GetPointLeftTangent(index), curve.GetPointRightTangent(index), curve.GetPointTilt(index));
            // i.Connect("tilt_changed", this, "ChangeTilt", new object[] { index });
        }
        else
        {
            i.Call("SetPointVals", curve.GetPointPosition(index).X, curve.GetPointPosition(index).Y, curve.GetPointLeftTangent(index), curve.GetPointRightTangent(index));
        }

        i.PointOffsetChanged += offset => ChangeOffset(offset, index);
        i.PointValueChanged += offset => ChangeValue(offset, index);
        i.AngleLeftChanged += offset => ChangeLeftAngle(offset, index);
        i.AngleRightChanged += offset => ChangeRightAngle(offset, index);
        curveInspectors.Add(i);
        AddChild(i);
    }

    public void ChangeRange()
    {
        for (int n = 0; n < curveInspectors.Count; n++)
        {
            var i = curveInspectors[n];
            i.SetValue(curve.GetPointPosition(n).Y);
            i.SetMinMax(curve.MinValue, curve.MaxValue);
            ChangeValue(i.GetValue(), n);
        }

        SetCurve(curve);
    }
}