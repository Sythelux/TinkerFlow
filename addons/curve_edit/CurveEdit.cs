// using Godot;
// using System;
//
// [Tool]
// public class YourPluginName : EditorPlugin
// {
//     private CurveInspect plugin;
//
//     public override void _EnterTree()
//     {
//         // Curve2DInspect is a resource, so use new
//         plugin = (CurveInspect)ResourceLoader.Load("res://addons/curve_edit/CurveInspect.gd").New();
//         AddInspectorPlugin(plugin);
//     }
//
//     public override void _ExitTree()
//     {
//         RemoveInspectorPlugin(plugin);
//     }
// }