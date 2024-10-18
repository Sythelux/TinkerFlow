// namespace TinkerFlow.addons.curve_edit;
//
// using Godot;
// using System;
//
// public class CurveInspect : EditorInspectorPlugin
// {
//     private PackedScene ControlVector = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurveControl.tscn");
//     private PackedScene ControlValue = ResourceLoader.Load<PackedScene>("res://addons/curve_edit/CurveValueControl.tscn");
//
//     private object curveEdit = null;
//
//     public override bool CanHandle(object obj)
//     {
//         return (obj is Curve2D) || (obj is Curve3D) || (obj is Curve); // handle Curve2D and Curve3D
//     }
//
//     public override void ParseBegin(object obj)
//     {
//         if (obj is Curve2D || obj is Curve3D)
//         {
//             var curveVectorControl = (ControlVector.Instance() as Control);
//             curveVectorControl.SetCurve(obj);
//             AddCustomControl(curveVectorControl);
//         }
//
//         if (obj is Curve)
//         {
//             curveEdit = obj;
//         }
//     }
//
//     public override void ParseEnd()
//     {
//         if (curveEdit is Curve)
//         {
//             var curveValueControl = (ControlValue.Instance() as Control);
//             curveValueControl.SetCurve((Curve)curveEdit);
//             AddCustomControl(curveValueControl);
//             curveEdit = null;
//         }
//     }
// }