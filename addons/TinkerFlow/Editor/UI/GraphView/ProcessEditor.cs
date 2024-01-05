using Godot;
using VRBuilder.Core;
using VRBuilder.Editor;

namespace TinkerFlow.Godot.Editor;

[Tool]
public partial class ProcessEditor : Control
{
	#region Delegates

	[Signal]
	public delegate void StepDeselectedEventHandler(Step processGraphNode);

	[Signal]
	public delegate void StepSelectedEventHandler(Step processGraphNode);

	#endregion

	private HBoxContainer chapterHierarchy;

	private ProcessGraph? graph;

	[Export]
	public int MyPropertyTest { get; set; }

	public ProcessGraph ProcessGraph => graph ??= GetNode<ProcessGraph>("%ProcessGraph");

	public Color BaseColor { get; set; }

	public override void _Draw()
	{
		// GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
	}

	public override void _ExitTree()
	{
		GlobalEditorHandler.ProcessWindowClosed(ProcessGraph);
	}

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationVisibilityChanged:
				if (IsVisibleInTree())
					GlobalEditorHandler.ProcessWindowOpened(ProcessGraph);
				else
					GlobalEditorHandler.ProcessWindowClosed(ProcessGraph);
				break;
		}
	}

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
			if (stepNode.EntryPoint is Step step)
				EmitSignal(SignalName.StepDeselected, step);
	}

	private void OnNodeSelected(Node node)
	{
		if (node is ProcessGraphNode stepNode)
			if (stepNode.EntryPoint is Step step)
				EmitSignal(SignalName.StepSelected, step);
	}

	public override void _Process(double delta)
	{
	}

	public void Save()
	{
		// throw new NotImplementedException();
	}
}
