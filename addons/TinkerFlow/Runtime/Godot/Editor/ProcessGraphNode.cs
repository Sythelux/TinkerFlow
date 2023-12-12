using Godot;
using System;
using Godot.Collections;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;
using Array = System.Array;

[Tool]
public abstract partial class ProcessGraphNode : GraphNode
{
    private readonly Array<StepNodeRow> rows = new();
    private const string emptyOutputPortText = "Go to next Chapter";

    /// <summary>
    /// Steps this node leads to.
    /// </summary>
    public abstract IStep[] Outputs { get; }

    /// <summary>
    /// Step other nodes connect to.
    /// </summary>
    public abstract IStep EntryPoint { get; }

    public override void _EnterTree()
    {
        ChildEnteredTree += OnChildAdded;
        ChildExitingTree += OnChildRemoved;
    }

    public override void _Ready()
    {
        // for (int i = 0; i < GetChildCount(); i++)
        // {
        //     OnChildAdded(GetChild(i));
        // }
        // var firstRow = GetNode<StepNodeRow>("FirstRow");
        // OnChildAdded(firstRow);
    }

    private void OnChildRemoved(Node node)
    {
        if (node is StepNodeRow stepNodeRow)
        {
            stepNodeRow.AddTransition -= AddRow;
            stepNodeRow.RemoveTransition -= RemoveRow;
            rows.Remove(stepNodeRow);
        }
    }

    private void OnChildAdded(Node node)
    {
        if (node is StepNodeRow stepNodeRow)
        {
            stepNodeRow.AddTransition += AddRow;
            stepNodeRow.RemoveTransition += RemoveRow;
            rows.Add(stepNodeRow);
            SetSlotEnabledRight(GetChildCount() - 1, true);
        }
    }

    public void AddRow(StepNodeRow row)
    {
        if (rows[0].Duplicate() is StepNodeRow nextRow)
        {
            nextRow.Removable = true;
            AddChild(nextRow);
        }
    }

    public void RemoveRow(StepNodeRow row)
    {
        RemoveChild(row);
    }

    public override void _Process(double delta)
    {
    }

    public void OnNodeSelected()
    {
    }

    public void OnNodeDeselected()
    {
    }

    public void OnGuiInput(InputEvent @event)
    {
        if (Selected)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Keycode == Key.Delete)
                    GetParent<ProcessGraph>().RemoveStep(this);
        }
    }

    public void UpdateTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort)
    {
        rows[fromPort].Title = "Transition to: " + toNode.Title;
    }

    public void RemoveTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort)
    {
        rows[fromPort].Title = "Chapter End";
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public void UpdateOutputPortName(Port port, GraphElement node)
    {
        throw new NotImplementedException();
    }

    public Port? GetOutPutPortForSlot(int indexOf)
    {
        throw new NotImplementedException();
    }

    public Port? GetInPutPortForSlot(int indexOf)
    {
        throw new NotImplementedException();
    }
}