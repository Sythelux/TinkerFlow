using System;
using System.Reflection;
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

    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
    {
        GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

        Control parent = base.Create(currentValue, changeValueCallback, text);
        parent.Name = GetType().Name + "." + text;


        if (currentValue is Step.EntityData step)
        {
            step.Metadata ??= new Metadata();
            var container = inspectorStepPrefab.Instantiate<InspectorStep>();
            container.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            //if ( lastStep != step )
            {
                container.Step = step;
                ClearContainer(container.Behaviors);
                UpdateBehaviors(container.Behaviors, step.Behaviors, changeValueCallback, text);
                // ClearContainer(container.Transitions);
                // UpdateTransitions(container.Transitions, step.Transitions, changeValueCallback, label);
                // ClearContainer(container.UnlockedElements);
                // UpdateUnlockedElements(container.UnlockedElements, new LockableObjectsCollection(step), changeValueCallback, label);
                lastStep = step;
                parent.AddChild(container);
            }
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

    private void UpdateBehaviors(VBoxContainer container, IBehaviorCollection stepBehaviors, Action<object> changeValueCallback, string text)
    {
        //TODO: rethink
        foreach (IBehavior behavior in stepBehaviors.Data.Behaviors)
        {
            var behaviorUi = processInspectorBehaviorUIPrefab.Instantiate<VBoxContainer>();
            if (behaviorUi.GetNode("%Label") is Label labelUi)
                labelUi.Text = behavior.Data.Name;
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(behavior, typeof(object));
            if (factory != null)
            {
                Control control = factory.Create(behavior, changeValueCallback, text);
                behaviorUi.GetNode("%Values").AddChild(control);
            }

            container.AddChild(behaviorUi);
        }
    }

    private void UpdateTransitions(VBoxContainer container, ITransitionCollection stepTransitions, Action<object> changeValueCallback, string text)
    {
        foreach (ITransition transition in stepTransitions.Data.Transitions)
        {
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(transition, typeof(object));
            if (factory != null)
            {
                Control control = factory.Create(transition, changeValueCallback, text);
                container.AddChild(control);
            }
        }
    }

    private void UpdateUnlockedElements(VBoxContainer container, LockableObjectsCollection lockableObjectsCollection, Action<object> changeValueCallback, string text)
    {
        foreach (ISceneObject sceneObjects in lockableObjectsCollection.SceneObjects)
        {
            IProcessFactory? factory = DrawerLocator.GetDrawerForValue(sceneObjects, typeof(object));
            if (factory != null)
            {
                Control control = factory.Create(sceneObjects, changeValueCallback, text);
                container.AddChild(control);
            }
        }
    }
}