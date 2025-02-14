using Godot;
using System;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core.Editor.UI.GraphView.Nodes;

namespace VRBuilder.Core.Editor.UI.GraphView.Instantiators
{
    /// <summary>
    /// Instantiator for a default <see cref="IStep"/> node.
    /// </summary>
    public class DefaultStepNodeInstantiator : IStepNodeInstantiator
    {
        public readonly PackedScene ProcessGraphNodePrefab = ResourceLoader.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/Steps/ProcessGraphNode.tscn"));

        /// <inheritdoc/>
        public string Name => "Step";

        /// <inheritdoc/>
        public bool IsInNodeMenu => true;

        /// <inheritdoc/>
        public string StepType => "default";

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public ProcessGraphNode InstantiateNode(IStep step)
        {
            StepGraphNode processGraphNode = ProcessGraphNodePrefab.Instantiate() as StepGraphNode
                                             ?? throw new InvalidOperationException($"Root Element of {ProcessGraphNodePrefab.ResourcePath} needs to have {nameof(StepGraphNode)} attached.");
            processGraphNode.EntryPoint = step;
            return processGraphNode;

        }
    }
}
