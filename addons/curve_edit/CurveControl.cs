// using Godot;
// using System;
//
// [Tool]
// public class YourClassName : VBoxContainer
// {
//     private int numPoints = 0;
//
//     private PackedScene CurvePointInspector = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurvePointInspector.tscn");
//     private SpinBox numPointsInspector;
//     private Array curveInspectorBoxes = new Array();
//     private Array curveInspectors = new Array();
//
//     private Curve curve;
//     private bool is2D = true;
//
//     // Called when the position of a control point is changed
//     public void ChangePos(Vector2 vector, int id)
//     {
//         curve.SetPointPosition(id, vector);
//     }
//
//     // Called when the in-tangent of a control point is changed
//     public void ChangeIn(Vector2 vector, int id)
//     {
//         curve.SetPointIn(id, vector);
//     }
//
//     // Called when the out-tangent of a control point is changed
//     public void ChangeOut(Vector2 vector, int id)
//     {
//         curve.SetPointOut(id, vector);
//     }
//
//     public void ChangeTilt(float tilt, int id)
//     {
//         curve.SetPointTilt(id, tilt);
//     }
//
//     // Set the curve that this controller manipulates, and generate the Inspector controls
//     public void SetCurve(object newCurve)
//     {
//         curve = newCurve;
//         is2D = curve is Curve2D; // if this is a Curve2D, then we're working with 2D curves, otherwise, treat it as 3D
//
//         ClearChildren(); // Clear out the old UI elements (or they'll stack)
//
//         CreateRefresh();
//
//         CreateNumPointInspector();
//
//         // add inspector controls for all points the curve has
//         for (int n = 0; n < numPoints; n++)
//         {
//             AddPointInspector(n);
//         }
//     }
//
//     // Reapplies the curve so changes made in the editor window will be reflected in the inspector
//     public void CreateRefresh()
//     {
//         Button b = new Button();
//         b.Text = "Refresh";
//         b.Connect("pressed", this, nameof(SetCurve));
//         AddChild(b);
//     }
//
//     // Creates and attaches the Inspector element tracking the number of points
//     public void CreateNumPointInspector()
//     {
//         Label l = new Label();
//         l.Text = "Number of Points";
//         l.Align = Label.AlignEnum.Center;
//         AddChild(l);
//
//         // Determine the number of points, then create a spinbox that will manipulate that value
//         numPoints = curve.GetPointCount();
//
//         numPointsInspector = new SpinBox();
//         numPointsInspector.Value = numPoints;
//         numPointsInspector.MaxValue = 4096; // Max number of control points allowed by box
//         numPointsInspector.Connect("value_changed", this, nameof(NumPointsChanged));
//         AddChild(numPointsInspector);
//     }
//
//     // Clears out the CurveControl's children.
//     public void ClearChildren()
//     {
//         foreach (Node child in GetChildren())
//         {
//             RemoveChild(child);
//             child.QueueFree();
//         }
//
//         curveInspectorBoxes.Clear();
//         curveInspectors.Clear();
//     }
//
//     // Create a new point on the curve, then add a new inspector for it
//     public void AddNewPoint(int index)
//     {
//         if (is2D)
//         {
//             curve.AddPoint(new Vector2(0, 0)); // Add a new 2D point
//         }
//         else
//         {
//             curve.AddPoint(new Vector3(0, 0, 0)); // Add a new 3D point
//         }
//         numPoints = curve.GetPointCount();
//         numPointsInspector.Value = numPoints;
//         AddPointInspector(index);
//     }
//
//     // Add a new curve point inspector (label and a CurvePointInspector), also hook them up
//     public void AddPointInspector(int index)
//     {
//         HBoxContainer box = new HBoxContainer{Name="CurveControl"};
//
//         Button button = new Button();
//         button.Text = "Remove";
//         button.Connect("pressed", this, nameof(RemovePoint));
//         box.AddChild(button);
//
//         box.AddConstantOverride("separation", 50);
//
//         Label l = new Label();
//         l.Text = "Point " + index.ToString();
//         l.Align = Label.TextAlign.Center;
//         box.AddChild(l);
//
//         curveInspectorBoxes.Add(box);
//         AddChild(box);
//
//         var i = (CurvePointInspector.Instance() as CurvePointInspector);
//         i.SetupInspector(is2D);
//         if (is2D)
//         {
//             i.Set2DPointVals(curve.GetPointPosition(index), curve.GetPointIn(index), curve.GetPointOut(index));
//         }
//         else
//         {
//             i.Set3DPointVals(curve.GetPointPosition(index), curve.GetPointIn(index), curve.GetPointOut(index), curve.GetPointTilt