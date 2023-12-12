using Godot;
using System;

namespace TinkerFlow.Godot.Editor;

[Tool]
public partial class ProcessEditor : Control
{
    [Export]
    public int MyPropertyTest { get; set; }

    private ProcessGraph? graph;
    private HBoxContainer chapterHierarchy;

    public ProcessGraph ProcessGraph => graph ??= GetNode<ProcessGraph>("Main/Workspace/Graph");

    [Signal]
    public delegate void StepSelectedEventHandler(ProcessGraphNode processGraphNode);

    [Signal]
    public delegate void StepDeselectedEventHandler(ProcessGraphNode processGraphNode);

    public override void _Ready()
    {
        if (ProcessGraph != null)
        {
            ProcessGraph.NodeSelected += OnNodeSelected;
            ProcessGraph.NodeDeselected += OnNodeDeselected;
        }
    }

    private void OnNodeDeselected(Node node)
    {
        if (node is ProcessGraphNode stepNode)
            EmitSignal(SignalName.StepDeselected, stepNode);
    }

    private void OnNodeSelected(Node node)
    {
        if (node is ProcessGraphNode stepNode)
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