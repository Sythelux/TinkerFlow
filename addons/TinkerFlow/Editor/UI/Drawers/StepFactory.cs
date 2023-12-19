using System;
using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;

namespace VRBuilder.Editor.UI.Drawers;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[DefaultProcessDrawer(typeof(Step.EntityData))]
public partial class StepFactory : ObjectFactory
{
    public PackedScene inspectorStepPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/InspectorStep.tscn"));

    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string label)
    {
        // var parent = base.Create(currentValue, changeValueCallback, label);
        var container = inspectorStepPrefab.Instantiate<Control>();
        // //// remove after IProcessDrawer
        // foreach (Node child in Transitions.GetChildren())
        // {
        //     Transitions.RemoveChild(child);
        //     child.QueueFree();
        // }
        //
        // foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
        // {
        //     var transitionUiBase = TransitionUiBase?.Instantiate<TransitionUiBase>();
        //     if (transitionUiBase != null)
        //         Transitions.AddChild(transitionUiBase);
        // }
        // //Remove to here        // var button = new Button();
        // // button.Text = "Add Transition";
        // // button.Pressed += () => step.Data.Transitions.Data.Transitions.Add(new Transition());
        // // Transitions.AddChild(button);
        // parent.AddChild(container);
        return container;
    }
}