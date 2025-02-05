using Godot;
using System;
using VRBuilder.Core;
using VRBuilder.Core.Editor.UI.GraphView.Nodes;

namespace VRBuilder.Core.Editor.UI.Graphics
{
    /// <summary>
    /// Entry point node in a graph view editor.
    /// </summary>
    public partial class EntryPointNode : ProcessGraphNode
    {
        protected readonly Label label;

        /// <inheritdoc/>
        public override IStep[] Outputs => new[] { GlobalEditorHandler.GetCurrentChapter().Data.FirstStep };

        /// <inheritdoc/>
        public override IStep EntryPoint => null;


        public EntryPointNode() : base()
        {
            Title = "Start";
            label = new Label();
            label.Text = emptyOutputPortText;
            AddChild(label);
            // IsEntryPoint = true;

            // capabilities = Capabilities.Selectable;
            // capabilities |= Capabilities.Movable;

            // titleButtonContainer.Clear();


            // AddTransitionPort(false);
            //
            // SetPosition(new Rect(GlobalEditorHandler.GetCurrentChapter().ChapterMetadata.EntryNodePosition, defaultNodeSize));
        }

        public override void UpdateTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
        {
            label.Text = "Transition to: " + toNode.Title;
        }

        public override void RemoveTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
        {
            label.Text = emptyOutputPortText;
        }


        // /// <inheritdoc/>
        // public override Vector2 Position { get => GlobalEditorHandler.GetCurrentChapter().ChapterMetadata.EntryNodePosition; set => GlobalEditorHandler.GetCurrentChapter().ChapterMetadata.EntryNodePosition = value; }
        //
        // /// <inheritdoc/>
        // public override string Name { get => title; set => title = value; }
        //
        // public override void OnSelected()
        // {
        //     base.OnSelected();
        //
        //     GlobalEditorHandler.ChangeCurrentStep(null);
        // }
        //
        // /// <inheritdoc/>
        // public override void AddToChapter(IChapter chapter)
        // {
        // }
        //
        // /// <inheritdoc/>
        // public override void Refresh()
        // {
        //     base.Refresh();
        // }
        //
        // /// <inheritdoc/>
        // public override void RemoveFromChapter(IChapter chapter)
        // {
        // }
        //
        // /// <inheritdoc/>
        // public override void SetOutput(int index, IStep output)
        // {
        //     GlobalEditorHandler.GetCurrentChapter().Data.FirstStep = output;
        // }
        //
        // /// <inheritdoc/>
        // protected override void RemovePortWithUndo(Port port)
        // {
        // }
    }
}
