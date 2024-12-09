using Godot;

namespace TinkerFlow.addons.curve_edit;

public partial class CurveInspect : EditorInspectorPlugin
{
    private PackedScene ControlVector = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurveControl.tscn");
    private PackedScene ControlValue = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurveValueControl.tscn");

    private object curveEdit = null;

    public override bool _CanHandle(GodotObject obj)
    {
        return (obj is Curve2D) || (obj is Curve3D) || (obj is Curve); // handle Curve2D and Curve3D
    }

    public override void _ParseBegin(GodotObject obj)
    {
        if (obj is Curve2D || obj is Curve3D)
        {
            // var curveVectorControl = (ControlVector.Instantiate() as CurveControl); //TODO
            // curveVectorControl.SetCurve(obj);
            // AddCustomControl(curveVectorControl);
        }

        if (obj is Curve)
        {
            curveEdit = obj;
        }
    }

    public override void _ParseEnd(GodotObject obj)
    {
        if (curveEdit is Curve)
        {
            // var curveValueControl = (ControlValue.Instantiate() as CurveControl); //TODO
            // curveValueControl.SetCurve((Curve)curveEdit);
            // AddCustomControl(curveValueControl);
            curveEdit = null;
        }
    }
}
