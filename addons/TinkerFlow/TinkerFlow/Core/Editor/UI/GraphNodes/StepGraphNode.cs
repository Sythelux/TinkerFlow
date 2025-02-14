using Godot;
using System.Linq;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Core.Editor.UI.GraphView.Nodes
{
    /// <summary>
    /// Step node in a graph view editor.
    /// </summary>
    [Tool]
    public partial class StepGraphNode : ProcessGraphNode
    {
        protected IStep step;
        Button addButton;

        /// <inheritdoc/>
        public override IStep EntryPoint
        {
            get => step;
            internal set
            {
                step = value;
                SetTitle(step.Data.Name);
                SetPosition(step.StepMetadata.Position);
            }
        }

        /// <inheritdoc/>
        public override IStep[] Outputs => step.Data.Transitions.Data.Transitions.Select(t => t.Data.TargetStep).ToArray();

        /// <inheritdoc/>
        public override void Refresh()
        {
            SetTitle(step.Data.Name);
            base.Refresh();
        }

        public override void UpdateTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
        {
            var ccwad = GetChildCount() - 1; //childCountWithoutAddButton
            if (fromPort > ccwad - 1)
            {
                AddRow();
            }
        }

        public override void RemoveTransitionTo(int fromPort, ProcessGraphNode toNode, int toPort = 0)
        {
        }

        // /// <summary>
        // /// Creates a transition port supporting undo.
        // /// </summary>
        // protected virtual void CreatePortWithUndo()
        // {
        //     ITransition transition = EntityFactory.CreateTransition();
        //
        //     RevertableChangesHandler.Do(new ProcessCommand(
        //         () =>
        //         {
        //             step.Data.Transitions.Data.Transitions.Add(transition);
        //             AddTransitionPort();
        //         },
        //         () =>
        //         {
        //             RemovePort(outputContainer[step.Data.Transitions.Data.Transitions.IndexOf(transition)] as Port);
        //         }
        //     ));
        // }
        //
        // /// <summary>
        // /// Removes the specified output port.
        // /// </summary>
        // protected void RemovePort(Port port)
        // {
        //     Edge edge = port.connections.FirstOrDefault();
        //
        //     if (edge != null)
        //     {
        //         edge.input.Disconnect(edge);
        //         edge.parent.Remove(edge);
        //     }
        //
        //     int index = outputContainer.IndexOf(port);
        //     step.Data.Transitions.Data.Transitions.RemoveAt(index);
        //
        //     outputContainer.Remove(port);
        //
        //     if (outputContainer.childCount == 0)
        //     {
        //         CreatePortWithUndo();
        //     }
        //
        //     RefreshPorts();
        //     RefreshExpandedState();
        // }
        //
        // /// <summary>
        // /// Removes the specified output port supporting undo.
        // /// </summary>
        // protected override void RemovePortWithUndo(Port port)
        // {
        //     int index = outputContainer.IndexOf(port);
        //     ITransition removedTransition = step.Data.Transitions.Data.Transitions[index];
        //     IChapter storedChapter = GlobalEditorHandler.GetCurrentChapter();
        //
        //     RevertableChangesHandler.Do(new ProcessCommand(
        //         () =>
        //         {
        //             RemovePort(port);
        //         },
        //         () =>
        //         {
        //             step.Data.Transitions.Data.Transitions.Insert(index, removedTransition);
        //             AddTransitionPort(true, index);
        //             GlobalEditorHandler.RequestNewChapter(storedChapter);
        //         }
        //     ));
        // }

        public override void _EnterTree()
        {
            ChildEnteredTree += OnChildAdded;
            ChildExitingTree += OnChildRemoved;
        }

        private void OnChildRemoved(Node node)
        {
            if (node is StepNodeRow stepNodeRow)
            {
                stepNodeRow.AddTransition -= AddRow;
                stepNodeRow.RemoveTransition -= RemoveRow;
            }
        }

        private void OnChildAdded(Node node)
        {
            if (node is StepNodeRow stepNodeRow)
            {
                stepNodeRow.AddTransition += AddRow;
                stepNodeRow.RemoveTransition += RemoveRow;
                SetSlotEnabledRight(GetChildCount() - 1, true);
            }
        }

        public void AddRow(bool isDeletablePort = true, bool supportsAddingNewRows = true)
        {
            AddRow(null, isDeletablePort, supportsAddingNewRows);
        }

        public void AddRow(StepNodeRow? sourceRow)
        {
            AddRow(sourceRow, true, true);
        }

        public void AddRow(StepNodeRow? sourceRow, bool isDeletablePort, bool supportsAddingNewRows)
        {
            // var newRow = RowPrefab.Instantiate<StepNodeRow>();
            // newRow.Removable = isDeletablePort;
            // newRow.Addable = supportsAddingNewRows;
            // RemoveChild(addButton);
            // AddChild(newRow);
            // AddChild(addButton);
        }

        public void RemoveRow(StepNodeRow row)
        {
            RemoveChild(row);
        }

        // /// <inheritdoc/>
        // public override void OnSelected()
        // {
        //     base.OnSelected();
        //
        //     GlobalEditorHandler.ChangeCurrentStep(step);
        //     GlobalEditorHandler.StartEditingStep();
        // }

        // /// <inheritdoc/>
        // public override void SetOutput(int index, IStep output)
        // {
        //     step.Data.Transitions.Data.Transitions[index].Data.TargetStep = output;
        // }
        //
        // /// <inheritdoc/>
        // public override void AddToChapter(IChapter chapter)
        // {
        //     chapter.Data.Steps.Add(step);
        // }
        //
        // /// <inheritdoc/>
        // public override void RemoveFromChapter(IChapter chapter)
        // {
        //     if (chapter.ChapterMetadata.LastSelectedStep == step)
        //     {

        //         chapter.ChapterMetadata.LastSelectedStep = null;
        //         GlobalEditorHandler.ChangeCurrentStep(null);
        //     }
        //
        //     chapter.Data.Steps.Remove(step);
        // }

        // /// <inheritdoc/>
        // public override void UpdateOutputPortName(Port outputPort, Node input)
        // {
        //     int index = outputContainer.IndexOf(outputPort);
        //     if (index >= 0 && string.IsNullOrEmpty(step.Data.Transitions.Data.Transitions[index].Data.Name) == false)
        //     {
        //         outputPort.portName = step.Data.Transitions.Data.Transitions[index].Data.Name;
        //     }
        //     else
        //     {
        //         base.UpdateOutputPortName(outputPort, input);
        //     }
        // }
    }
}
