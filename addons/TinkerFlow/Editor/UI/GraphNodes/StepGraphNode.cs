using System.Linq;
using System.Reflection;
using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;

namespace VRBuilder.Editor.UI.Graphics;

/// <summary>
/// Step node in a graph view editor.
/// </summary>
public partial class StepGraphNode : ProcessGraphNode
{
    protected IStep step;

    public StepGraphNode(IStep step) : base()
    {
        Title = step.Data.Name;
        this.step = step;

        AddRow(false);
        SetSlotEnabledLeft(0, true);

        for (var i = 0; i < step.Data.Transitions.Data.Transitions.Count; i++)
        {
            // ITransition transition = step.Data.Transitions.Data.Transitions[i];
            SetSlotEnabledRight(i, true);
            if (i > 0) // firstrow exists because of input side.
                AddRow();
        }

        NodeSelected += OnSelected;
    }

    /// <inheritdoc/>
    public override IStep? EntryPoint => step;

    /// <inheritdoc/>
    public override IStep[] Outputs => step.Data.Transitions.Data.Transitions.Select(t => t.Data.TargetStep).ToArray();

    public override void _Ready()
    {
        GD.Print($"{GetType().Name}: {MethodBase.GetCurrentMethod()}");
        PositionOffset = step.StepMetadata.Position;
    }

    /// <inheritdoc/>
    public override void Refresh()
    {
        GD.Print($"{GetType().Name}: {MethodBase.GetCurrentMethod()}");
        Title = step?.Data.Name;
        base.Refresh();
    }

    /// <summary>
    /// Creates a transition port supporting undo.
    /// </summary>
    protected virtual void CreatePortWithUndo(StepNodeRow row)
    {
    }

    /// <summary>
    /// Removes the specified output port supporting undo.
    /// </summary>        
    protected override void RemovePortWithUndo(int port)
    {
    }

    /// <inheritdoc/>
    public void OnSelected()
    {
        GlobalEditorHandler.ChangeCurrentStep(step);
        GlobalEditorHandler.StartEditingStep();
    }

    /// <inheritdoc/>
    public override void SetOutput(int index, IStep output)
    {
        step.Data.Transitions.Data.Transitions[index].Data.TargetStep = output;
    }

    /// <inheritdoc/>
    public override void AddToChapter(IChapter chapter)
    {
        chapter.Data.Steps.Add(step);
    }

    /// <inheritdoc/>
    public override void RemoveFromChapter(IChapter chapter)
    {
        if (chapter.ChapterMetadata.LastSelectedStep == step)
        {
            chapter.ChapterMetadata.LastSelectedStep = null;
            GlobalEditorHandler.ChangeCurrentStep(null);
        }

        chapter.Data.Steps.Remove(step);
    }
}