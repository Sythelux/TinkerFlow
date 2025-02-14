using Godot;

namespace VRBuilder.Core.Editor.UI.GraphView.Nodes;

[Tool]
public abstract partial class ProcessGraphNode : GraphNode
{
    protected const string emptyOutputPortText = "Go to next Chapter";

    /// <summary>
    /// Steps this node leads to.
    /// </summary>
    public abstract IStep[] Outputs { get; }

    /// <summary>
    /// Step other nodes connect to.
    /// </summary>
    public abstract IStep? EntryPoint { get; internal set; }

    public bool Dirty { get; set; }

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
