using Godot;
using System;
using Godot.Collections;
using TinkerFlow.Godot.Editor;

[Tool]
public partial class StepNode : GraphNode
{
    private readonly Array<StepNodeRow> rows = new();

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

    public void UpdateTransitionTo(int fromPort, StepNode toNode, int toPort)
    {
        rows[fromPort].Title = "Transition to: " + toNode.Title;
    }

    public void RemoveTransitionTo(int fromPort, StepNode toNode, int toPort)
    {
        rows[fromPort].Title = "Chapter End";
    }
}