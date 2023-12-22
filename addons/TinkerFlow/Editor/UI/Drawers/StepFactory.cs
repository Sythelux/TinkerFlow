using System;
using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Editor.UI.Windows;

namespace VRBuilder.Editor.UI.Drawers;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[DefaultProcessDrawer(typeof(Step.EntityData))]
public partial class StepFactory : ObjectFactory
{
    private Step.EntityData? lastStep;
    protected PackedScene inspectorStepPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/InspectorStep.tscn"));
    protected PackedScene processInspectorBehaviorUIPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/ProcessInspectorBehaviorUI.tscn"));

    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
    {
        Control parent = base.Create(currentValue, changeValueCallback, label);

        if (currentValue is Step.EntityData step)
        {
            step.Metadata ??= new Metadata();
            var container = inspectorStepPrefab.Instantiate<InspectorStep>();
            container.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            if (lastStep != step && container != null)
            {
                container.Step = step;
                ClearContainer(container.Behaviors);
                UpdateBehaviors(container.Behaviors, step.Behaviors, changeValueCallback, label);
                // ClearContainer(container.Transitions);
                // UpdateTransitions(container.Transitions, step.Transitions, changeValueCallback, label);
                // ClearContainer(container.UnlockedElements);
                // UpdateUnlockedElements(container.UnlockedElements, new LockableObjectsCollection(step), changeValueCallback, label);
                lastStep = step;
            }

            parent.AddChild(container);
        }

        return parent;
    }

    private void ClearContainer(VBoxContainer container)
    {
        foreach (Node child in container.GetChildren())
        {
            container.RemoveChild(child);
            child.Free();
        }
    }

    private void UpdateBehaviors(VBoxContainer container, IBehaviorCollection stepBehaviors, Action<object> changeValueCallback, Control label)
    {
        foreach (IBehavior behavior in stepBehaviors.Data.Behaviors)
        {
            var behaviorUi = processInspectorBehaviorUIPrefab.Instantiate<VBoxContainer>();
            if (behaviorUi.GetNode("%Label") is Label labelUi)
                labelUi.Text = behavior.Data.Name;
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(behavior, typeof(object));
            if (factory != null)
                behaviorUi.GetNode("%Values").AddChild(factory.Create(behavior, changeValueCallback, label));
            container.AddChild(behaviorUi);
        }
    }

    private void UpdateTransitions(VBoxContainer container, ITransitionCollection stepTransitions, Action<object> changeValueCallback, Control label)
    {
        foreach (ITransition transition in stepTransitions.Data.Transitions)
        {
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(transition, typeof(object));
            if (factory != null) container.AddChild(factory.Create(transition, changeValueCallback, label));
        }
    }

    private void UpdateUnlockedElements(VBoxContainer container, LockableObjectsCollection lockableObjectsCollection, Action<object> changeValueCallback, Control label)
    {
        foreach (ISceneObject sceneObjects in lockableObjectsCollection.SceneObjects)
        {
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(sceneObjects, typeof(object));
            if (factory != null) container.AddChild(factory.Create(sceneObjects, changeValueCallback, label));
        }
    }
}