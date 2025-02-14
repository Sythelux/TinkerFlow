using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core.Editor.UI.GraphView.Nodes;

namespace VRBuilder.Core.Editor.UI.GraphView.Instantiators
{
    /// <summary>
    /// Instantiator for the End Chapter node.
    /// </summary>
    public class EndChapterNodeInstantiator : IStepNodeInstantiator
    {
        public readonly PackedScene ProcessGraphNodePrefab = ResourceLoader.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/Steps/EndChapterNode.tscn"));

        /// <inheritdoc/>
        public string Name => "End Chapter";

        /// <inheritdoc/>
        public bool IsInNodeMenu => true;

        /// <inheritdoc/>
        public int Priority => 150;

        /// <inheritdoc/>
        public string StepType => "endChapter";

        /// <inheritdoc/>
        public ProcessGraphNode InstantiateNode(IStep step)
        {
            return ProcessGraphNodePrefab.Instantiate<ProcessGraphNode>();
        }
    }
}
