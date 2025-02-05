using Godot;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Core.Editor.UI.GraphView.Nodes;

[Tool]
public abstract partial class ProcessGraphNode : GraphNode
{
    protected const string emptyOutputPortText = "Go to next Chapter";
    protected PackedScene RowPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/StepNodeRow.tscn"));
    protected const string deleteIconFileName = "Eraser";
    protected const string editIconFileName = "Edit";

    /// <summary>
    /// Steps this node leads to.
    /// </summary>
    public abstract IStep[] Outputs { get; }

    /// <summary>
    /// Step other nodes connect to.
    /// </summary>
    public abstract IStep? EntryPoint { get; }

    public bool Dirty { get; set; }

    public void OnGuiInput(InputEvent @event)
    {
        if (Selected)
            if (@event is InputEventKey { Pressed: true, Keycode: Key.Delete })
                GetParent<ProcessGraph>().RemoveStep(this);
    }

    /// <summary>
    /// Refreshes the node's graphics.
    /// </summary>
    public virtual void Refresh()
    {
        Dirty = false;
    }

    public abstract void UpdateTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0);

    public abstract void RemoveTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0);

}
