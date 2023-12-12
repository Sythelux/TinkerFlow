using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using VRBuilder.Core;
using VRBuilder.Core.Configuration;
using VRBuilder.Editor;
using VRBuilder.Editor.UI.Graphics;
using VRBuilder.Editor.UI.Windows;

namespace TinkerFlow.Godot.Editor;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[Tool]
public partial class ProcessGraph : ProcessEditorWindow //ProcessGraphView.cs
{
    [Export]
    public PackedScene? StepNode { get; set; }

    private PopupMenu? addNodeMenu;
    public PopupMenu AddNodeMenu => addNodeMenu ??= GetNode<PopupMenu>("PopupMenu");
    private PopupMenu? editNodeMenu;
    private Vector2 popUpPosition;
    private RuntimeConfigurator? lastConfigurator;
    public PopupMenu EditNodeMenu => editNodeMenu ??= GetNode<PopupMenu>("NodePopupMenu");
    public Node? SelectedNode { get; set; }
    private ProcessGraphNode entryNode;
    private IChapter currentChapter;
    private List<IStepNodeInstantiator> instantiators = new();
    private Dictionary<IChapter, Rect2> storedViewTransforms = new Dictionary<IChapter, Rect2>();

    [Signal]
    public delegate void ModifiedEventHandler();

    public override void _Ready()
    {
        NodeSelected += OnNodeSelected;
        NodeDeselected += OnNodeDeselected;
        ConnectionRequest += OnConnectionRequest;
        DisconnectionRequest += OnDisconnectionRequest;
    }

    private void OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        (GetNode(fromNode + "") as ProcessGraphNode)?.RemoveTransitionTo((int)fromPort, GetNode<ProcessGraphNode>(toNode + ""), (int)toPort);
    }

    private void OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        (GetNode(fromNode + "") as ProcessGraphNode)?.UpdateTransitionTo((int)fromPort, GetNode<ProcessGraphNode>(toNode + ""), (int)toPort);
    }

    private void OnNodeDeselected(Node node)
    {
        SelectedNode = null;
    }

    private void OnNodeSelected(Node node)
    {
        SelectedNode = node;
    }

    public IEnumerable<string> GetNext(string nodeName)
    {
        return GetConnectionList().Where(c => c["from"].AsString() == nodeName).Select(c => c["to"].AsString());
    }

    public void OnModified(int a = 0, int b = 0, int c = 0, int d = 0)
    {
        EmitSignal(SignalName.Modified);
    }

    public override void _Process(double delta)
    {
        var configurator = EditorInterface.Singleton.GetEditedSceneRoot().GetChildren().OfType<RuntimeConfigurator>().FirstOrDefault();
        if (configurator?.GetSelectedProcess() != lastConfigurator?.GetSelectedProcess())
        {
        }
    }

    public void AddStep(int id)
    {
        var step = StepNode?.Instantiate<ProcessGraphNode>();
        switch (id)
        {
            case 0:
                if (step != null)
                {
                    step.Position = popUpPosition;
                    AddChild(step);
                }

                break;
        }

        // AddNodeMenu.Hide();
    }

    public void RemoveStep(ProcessGraphNode processGraphNode)
    {
        RemoveChild(processGraphNode);
    }

    public void ModifyStep(int menuId)
    {
        switch (menuId)
        {
            case 0:
                RemoveChild(SelectedNode);
                if (SelectedNode != null)
                    OnNodeDeselected(SelectedNode);
                break;
        }
    }

    internal override void SetProcess(IProcess currentProcess)
    {
        throw new System.NotImplementedException();
    }

    internal override IChapter GetChapter()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Displays the specified chapter.
    /// </summary>        
    internal override void SetChapter(IChapter chapter)
    {
        IChapter previousChapter = GlobalEditorHandler.GetCurrentChapter();

        if (chapter != previousChapter)
        {
            if (previousChapter != null)
            {
                if (storedViewTransforms.ContainsKey(previousChapter))
                {
                    storedViewTransforms[previousChapter] = GetViewportRect();
                }
                else
                {
                    storedViewTransforms.Add(previousChapter, GetViewportRect());
                }
            }

            GlobalEditorHandler.SetCurrentChapter(chapter);

            if (storedViewTransforms.ContainsKey(chapter))
            {
                // TODO: SetViewportRect(storedViewTransforms[chapter]);
            }
            else
            {
                // TODO: viewTransform.scale = Vector3.one;
                //
                // if (contentRect.height > 0)
                // {
                //     viewTransform.position = new Vector2(defaultViewTransform.x, (int)(contentRect.height / 2)) - chapter.ChapterMetadata.EntryNodePosition;
                // }
                // else
                // {
                //     viewTransform.position = defaultViewTransform - chapter.ChapterMetadata.EntryNodePosition;
                // }
            }
        }

        currentChapter = chapter;

        GetChildren().Clear(); //ForEach(RemoveElement);
        ClearConnections(); // edges.ForEach(RemoveElement);

        entryNode = new EntryPointNode();
        AddChild(entryNode);

        GenerateNodes(currentChapter);

        foreach (ProcessGraphNode node in GetChildren().ToList().OfType<ProcessGraphNode>())
        {
            RefreshNode(node);
        }
    }

    private IEnumerable<ProcessGraphNode> GenerateNodes(IChapter chapter)
    {
        return chapter.Data.Steps.Select(CreateStepNode).ToList();
    }

    private void RefreshNode(ProcessGraphNode node)
    {
        node.Refresh();

        LinkNode(node);

        foreach (ProcessGraphNode leadingNode in GetLeadingNodes(node))
        {
            foreach (IStep output in leadingNode.Outputs)
            {
                if (output != node)
                {
                    continue;
                }


                // Port port = leadingNode.outputContainer[Array.IndexOf(leadingNode.Outputs, output)].Q<Port>();
                Port? port = leadingNode.GetOutPutPortForSlot(Array.IndexOf(leadingNode.Outputs, output));

                if (port.HasValue)
                    leadingNode.UpdateOutputPortName(port.Value, node);
            }
        }
    }

    private ProcessGraphNode CreateStepNode(IStep step)
    {
        if (string.IsNullOrEmpty(step.StepMetadata.StepType))
        {
            step.StepMetadata.StepType = "default";
        }

        IStepNodeInstantiator instantiator = instantiators.FirstOrDefault(i => i.StepType == step.StepMetadata.StepType);

        if (instantiator == null)
        {
            GD.PushError($"Impossible to find correct visualization for type '{step.StepMetadata.StepType}' used in step '{step.Data.Name}'. Things might not look as expected.");
            instantiator = instantiators.First(i => i.StepType == "default");
        }

        ProcessGraphNode node = instantiator.InstantiateNode(step);
        AddChild(node);
        return node;
    }

    internal override void RefreshChapterRepresentation()
    {
        throw new System.NotImplementedException();
    }

    private void LinkNodes(Port output, Port input)
    {
        Edge edge = new Edge
        {
            Output = output,
            Input = input,
        };

        ConnectNode(edge);
        // edge.input.Connect(edge);
        // edge.output.Connect(edge);

        ((ProcessGraphNode)output.Node).UpdateOutputPortName(output, input.Node);
    }

    private void ConnectNode(Edge edge)
    {
        //StringName fromNode, int fromPort, StringName toNode, int toPort
        ConnectNode(edge.Input.PortName, edge.Input.PortId, edge.Output.PortName, edge.Output.PortId);
    }

    private ProcessGraphNode FindStepNode(IStep step)
    {
        if (step == null)
        {
            return null;
        }

        return GetChildren().ToList().FirstOrDefault(n => n is ProcessGraphNode graphNode && graphNode.EntryPoint == step) as ProcessGraphNode;
    }

    private void LinkNode(ProcessGraphNode node)
    {
        if (node.EntryPoint != null)
        {
            LinkStepNode(node.EntryPoint);
        }
        else if (node is EntryPointNode)
        {
            ProcessGraphNode firstNode = FindStepNode(currentChapter.Data.FirstStep);

            if (firstNode != null)
            {
                // LinkNodes(node.outputContainer[0].Query<Port>(), firstNode.inputContainer[0].Query<Port>());
                Port? outPutPortForSlot = node.GetOutPutPortForSlot(0);
                Port? inPutPortForSlot = firstNode.GetInPutPortForSlot(0);
                if (outPutPortForSlot.HasValue && inPutPortForSlot.HasValue)
                    LinkNodes(outPutPortForSlot.Value, inPutPortForSlot.Value);
            }
        }
    }

    private void LinkStepNode(IStep step)
    {
        foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
        {
            Port? outputPort = FindStepNode(step).GetOutPutPortForSlot(step.Data.Transitions.Data.Transitions.IndexOf(transition));

            if (transition.Data.TargetStep != null && outputPort.HasValue)
            {
                ProcessGraphNode target = FindStepNode(transition.Data.TargetStep);
                Port? inPutPortForSlot = target.GetInPutPortForSlot(0);
                if (inPutPortForSlot.HasValue)
                    LinkNodes(outputPort.Value, inPutPortForSlot.Value);
            }
        }
    }

    public void ShowPopup(Vector2 pos)
    {
        popUpPosition = pos + GlobalPosition;
        if (SelectedNode == null)
            AddNodeMenu.PopupOnParent(new Rect2I((int)popUpPosition.X, (int)popUpPosition.Y, AddNodeMenu.Size.X, AddNodeMenu.Size.Y));
        else
            EditNodeMenu.PopupOnParent(new Rect2I((int)popUpPosition.X, (int)popUpPosition.Y, AddNodeMenu.Size.X, AddNodeMenu.Size.Y));
    }

    private IEnumerable<ProcessGraphNode> GetLeadingNodes(ProcessGraphNode targetNode)
    {
        List<ProcessGraphNode> leadingNodes = new();

        if (targetNode.EntryPoint == null)
        {
            return leadingNodes;
        }

        foreach (Node node in GetChildren().ToList())
        {
            ProcessGraphNode processGraphNode = node as ProcessGraphNode;

            if (processGraphNode != null && processGraphNode.Outputs.Contains(targetNode.EntryPoint))
            {
                leadingNodes.Add(processGraphNode);
            }
        }

        return leadingNodes;
    }
}

public struct Port
{
    public string PortName;
    public int PortId;
    public GraphElement Node;
}

public class Edge
{
    public Port Output;
    public Port Input;
}