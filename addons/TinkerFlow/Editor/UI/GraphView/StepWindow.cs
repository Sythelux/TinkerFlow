using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using VRBuilder.Core;
using VRBuilder.Editor;
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
    private IStep? step;
    public static StepWindow Instance { get; private set; }

    [Export]
    public PackedScene? TransitionUiBase { get; set; }

    public LineEdit StepName => stepName ??= GetNode<LineEdit>("StepName");
    public LineEdit StepDescription => stepDescription ??= GetNode<LineEdit>("StepDescription");
    public VBoxContainer Behaviors => behaviors ??= GetNode<VBoxContainer>("TabContainer/Behaviors");
    public VBoxContainer Transitions => transitions ??= GetNode<VBoxContainer>("TabContainer/Conditions");
    public VBoxContainer UnlockedElements => unlockedElements ??= GetNode<VBoxContainer>("TabContainer/LockedObjects");

    public StepWindow()
    {
        Instance = this;
    }

    public override void _Draw()
    {
        GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
    }

    public override void _EnterTree()
    {
        GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());

        GlobalEditorHandler.StepWindowOpened(this);
    }

    public override void _ExitTree()
    {
        GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());

        GlobalEditorHandler.StepWindowClosed(this);
    }

    public override void _Notification(int what)
    {
        // if (!new[] { NotificationProcess }.Contains(what))
        //     GD.Print($"{GetType().Name}: {MethodBase.GetCurrentMethod()?.Name}({Enum.GetName(typeof(Notifications), what)}:{what})");
    }

    public override void _Process(double delta)
    {
        //called even when invisible
        // GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
    }

    public override void _Ready()
    {
        GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
    }

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
            if (transitionUiBase != null)
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

    public void SetStep(IStep? newStep)
    {
        step = newStep;
    }

    public void ResetStepView()
    {
        if (Instance.IsVisibleInTree() || step == null) return;

        // Dictionary<string, object> dict = step.Data.Metadata.GetMetadata(typeof(TabsGroup));
        // if (dict.ContainsKey(TabsGroup.SelectedKey))
        // {
        //     dict[TabsGroup.SelectedKey] = 0;
        // }
    }

    public static void ShowInspector()
    {
        // Instance.Show();
    }
}