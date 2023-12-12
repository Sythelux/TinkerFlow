using Godot;
using VRBuilder.Core;
using VRBuilder.Editor.UI.Windows;

namespace TinkerFlow.Godot.Editor;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[Tool]
public partial class StepWindow : Control, IStepView
{
    private LineEdit? stepName;
    private LineEdit? stepDescription;
    private VBoxContainer? behaviors;
    private VBoxContainer? transitions;
    private VBoxContainer? unlockedElements;

    [Export]
    public PackedScene? TransitionUiBase { get; set; }

    public LineEdit StepName => stepName ??= GetNode<LineEdit>("StepName");
    public LineEdit StepDescription => stepDescription ??= GetNode<LineEdit>("StepDescription");
    public VBoxContainer Behaviors => behaviors ??= GetNode<VBoxContainer>("TabContainer/Behaviors");
    public VBoxContainer Transitions => transitions ??= GetNode<VBoxContainer>("TabContainer/Conditions");
    public VBoxContainer UnlockedElements => unlockedElements ??= GetNode<VBoxContainer>("TabContainer/LockedObjects");

    public void OnStepSelected(ProcessGraphNode processGraphNode)
    {
        StepName.Text = processGraphNode.Name;
        foreach (Node? node in GetChildren())
        {
            var child = (Control)node;
            child.Visible = true;
        }

        foreach (Node child in Transitions.GetChildren())
        {
            Transitions.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var node in processGraphNode.GetChildren())
        {
            var child = (StepNodeRow)node;
            var transitionUiBase = TransitionUiBase?.Instantiate<TransitionUiBase>();
            Transitions.AddChild(transitionUiBase);
        }

        var button = new Button();
        button.Text = "Add Transition";
        button.Pressed += () => processGraphNode.AddRow(new StepNodeRow());
        Transitions.AddChild(button);
    }

    public void OnStepDeselected(ProcessGraphNode processGraphNode)
    {
        StepName.Text = "None Selected";
        foreach (Node? node in GetChildren())
        {
            var child = (Control)node;
            child.Visible = false;
        }

        StepName.Visible = true;
    }

    public void SetStep(IStep newStep)
    {
        throw new System.NotImplementedException();
    }

    public void ResetStepView()
    {
        throw new System.NotImplementedException();
    }

    public static void ShowInspector()
    {
        throw new System.NotImplementedException();
    }
}