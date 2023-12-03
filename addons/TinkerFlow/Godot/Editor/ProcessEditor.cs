using Godot;
using System;

namespace TinkerFlow.Godot.Editor;

[Tool]
public partial class ProcessEditor : Control
{
    [Export]
    public int MyPropertyTest { get; set; }

    private ProcessGraph? graph;

    [Signal]
    public delegate void StepSelectedEventHandler(StepNode stepNode);

    [Signal]
    public delegate void StepDeselectedEventHandler(StepNode stepNode);

    public override void _Ready()
    {
        graph = GetNode<ProcessGraph>("Main/Workspace/Graph");
        graph.NodeSelected += OnNodeSelected;
        graph.NodeDeselected += OnNodeDeselected;
    }

    private void OnNodeDeselected(Node node)
    {
        if (node is StepNode stepNode)
            EmitSignal(SignalName.StepDeselected, stepNode);
    }

    private void OnNodeSelected(Node node)
    {
        if (node is StepNode stepNode)
            EmitSignal(SignalName.StepSelected, stepNode);
    }

    public override void _Process(double delta)
    {
    }

    public void Save()
    {
        // throw new NotImplementedException();
    }

    public Color BaseColor { get; set; }
}