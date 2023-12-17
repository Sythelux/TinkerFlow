using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;

[Tool]
public partial class ProcessGraphNode : GraphNode
{
    private const string emptyOutputPortText = "Go to next Chapter";
    protected PackedScene RowPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/StepNodeRow.tscn"));

    /// <summary>
    /// Steps this node leads to.
    /// </summary>
    public virtual IStep[] Outputs { get; }

    /// <summary>
    /// Step other nodes connect to.
    /// </summary>
    public virtual IStep? EntryPoint { get; }

    /// <summary>
    /// True if this is the "Start" node.
    /// </summary>
    public bool IsEntryPoint { get; set; }

    public bool Dirty { get; set; }

    public override void _EnterTree()
    {
        ChildEnteredTree += OnChildAdded;
        ChildExitingTree += OnChildRemoved;
    }

    private void OnChildRemoved(Node node)
    {
        if (node is StepNodeRow stepNodeRow)
        {
            stepNodeRow.AddTransition -= AddRow;
            stepNodeRow.RemoveTransition -= RemoveRow;
        }
    }

    private void OnChildAdded(Node node)
    {
        if (node is StepNodeRow stepNodeRow)
        {
            stepNodeRow.AddTransition += AddRow;
            stepNodeRow.RemoveTransition += RemoveRow;
            SetSlotEnabledRight(GetChildCount() - 1, true);
        }
    }

    public void AddRow(bool isDeletablePort = true, bool supportsAddingNewRows = true)
    {
        AddRow(null, isDeletablePort, supportsAddingNewRows);
    }

    public void AddRow(StepNodeRow? sourceRow)
    {
        AddRow(sourceRow, true, true);
    }

    public void AddRow(StepNodeRow? sourceRow, bool isDeletablePort, bool supportsAddingNewRows)
    {
        var newRow = RowPrefab.Instantiate<StepNodeRow>();
        newRow.Removable = isDeletablePort;
        newRow.Addable = supportsAddingNewRows;
        AddChild(newRow);
    }

    public void RemoveRow(StepNodeRow row)
    {
        RemoveChild(row);
    }

    public void OnGuiInput(InputEvent @event)
    {
        if (Selected)
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Keycode == Key.Delete)
                    GetParent<ProcessGraph>().RemoveStep(this);
    }

    public void UpdateTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
    {
        GetChild<StepNodeRow>(fromPort).Title = "Transition to: " + toNode.Title;
    }

    public void RemoveTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
    {
        GetChild<StepNodeRow>(fromPort).Title = emptyOutputPortText;
    }

    /// <summary>
    /// Refreshes the node's graphics.
    /// </summary>
    public virtual void Refresh()
    {
        Dirty = false;
    }


    /// <summary>
    /// Sets an output to the specified step.
    /// </summary>        
    public virtual void SetOutput(int index, IStep output)
    {
    }

    /// <summary>
    /// Adds node to specified chapter.
    /// </summary>        
    public virtual void AddToChapter(IChapter chapter)
    {
    }

    /// <summary>
    /// Removes node from specified chapter.
    /// </summary>        
    public virtual void RemoveFromChapter(IChapter chapter)
    {
    }

    /// <summary>
    /// Remove port with undo.
    /// </summary>        
    protected virtual void RemovePortWithUndo(int port)
    {
    }
}