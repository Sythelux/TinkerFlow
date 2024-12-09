using Godot;
using TinkerFlow.addons.curve_edit;

[Tool]
public partial class CurveEdit : EditorPlugin
{
    private CurveInspect plugin;

    public override void _EnterTree()
    {
        // Curve2DInspect is a resource, so use new
        plugin = new CurveInspect();
        AddInspectorPlugin(plugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(plugin);
    }
}
