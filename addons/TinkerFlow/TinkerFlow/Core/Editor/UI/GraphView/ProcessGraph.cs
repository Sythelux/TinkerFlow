using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using VRBuilder.Core;
using VRBuilder.Core.Configuration;
using VRBuilder.Editor;
using VRBuilder.Editor.UI.Graphics;
using VRBuilder.Editor.UI.Windows;
using VRBuilder.Editor.Util;

namespace TinkerFlow.Godot.Editor
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [Tool]
    public partial class ProcessGraph : ProcessEditorWindow //ProcessGraphView.cs
    {
        #region Delegates

        [Signal]
        public delegate void ModifiedEventHandler();

        #endregion

        private readonly Dictionary<IChapter, Rect2> storedViewTransforms = new();
        private readonly List<IStepNodeInstantiator> instantiators = new();
        private IChapter? currentChapter;
        private IProcess? currentProcess;

        private PopupMenu? addNodeMenu;
        private PopupMenu? editNodeMenu;
        private ProcessGraphNode? entryNode;
        private ProcessMenuView? chapterMenu;
        private RuntimeConfigurator? lastConfigurator;
        private RuntimeConfigurator? runtimeConfigurator;
        private VBoxContainer? chapterViewContainer;
        private Vector2 popUpPosition;

        [Export]
        public PackedScene? StepNode { get; set; }

        public RuntimeConfigurator? RuntimeConfigurator => runtimeConfigurator ??= EditorInterface.Singleton.GetEditedSceneRoot()?.GetChildren().OfType<RuntimeConfigurator>().FirstOrDefault();
        [Export]
        public PopupMenu AddNodeMenu
        {
            get { return addNodeMenu ??= GetNode<PopupMenu>("../PopupMenu"); }
            set => addNodeMenu = value;
        }

        [Export]
        public PopupMenu EditNodeMenu
        {
            get { return editNodeMenu ??= GetNode<PopupMenu>("../NodePopupMenu"); }
            set => editNodeMenu = value;
        }

        public Node? SelectedNode { get; set; }
        public VBoxContainer ChapterViewContainer => chapterViewContainer ??= GetNode<VBoxContainer>("%ChapterView");

        public override void _EnterTree()
        {
            SetupInstantiators();
        }

        public override void _Ready()
        {
            NodeSelected += OnNodeSelected;
            NodeDeselected += OnNodeDeselected;
            ConnectionRequest += OnConnectionRequest;
            DisconnectionRequest += OnDisconnectionRequest;
            chapterMenu ??= new ProcessMenuView();

            // chapterMenu.MenuExtendedChanged += (sender, args) => { chapterViewContainer.style.width = args.IsExtended ? ProcessMenuView.ExtendedMenuWidth : ProcessMenuView.MinimizedMenuWidth; };
            // chapterMenu.RefreshRequested += (sender, args) => { chapterViewContainer.MarkDirtyLayout(); };
        }

        private void SetupInstantiators()
        {
            IEnumerable<Type> instantiatorTypes = TypesUtils.GetTypesDerivedFrom<IStepNodeInstantiator>();
            foreach (Type instantiatorType in instantiatorTypes)
                if (Activator.CreateInstance(instantiatorType) is IStepNodeInstantiator val)
                    instantiators.Add(val);
        }

        private void OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            (GetNode(fromNode + "") as ProcessGraphNode)?.RemoveTransitionTo((int)fromPort, GetNode<ProcessGraphNode>(toNode + ""), (int)toPort);
        }

        private void OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            Error err = ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            if (err != Error.Ok)
                GD.PrintErr(err);
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
            if (RuntimeConfigurator?.GetSelectedProcess() != lastConfigurator?.GetSelectedProcess())
            {
                lastConfigurator = RuntimeConfigurator;
                if (RuntimeConfigurator != null)
                {
                    GlobalEditorHandler.SetCurrentProcess(ProcessAssetUtils.GetProcessNameFromPath(RuntimeConfigurator.GetSelectedProcess()));
                    GlobalEditorHandler.StartEditingProcess();
                }
            }

            foreach (ProcessGraphNode node in GetChildren().ToList().OfType<ProcessGraphNode>())
                if (node.Dirty)
                    RefreshNode(node);
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

        internal override void SetProcess(IProcess? process)
        {
            GD.Print($"{GetType().Name}: {MethodBase.GetCurrentMethod()}");

            // TODO: RevertableChangesHandler.FlushStack();

            currentProcess = process;

            if (currentProcess == null) return;

            chapterMenu?.Initialise(currentProcess, this);
            // chapterViewContainer.onGUIHandler = () => chapterMenu.Draw();

            // if (chapterMenu != null)
            //     chapterMenu.ChapterChanged += (sender, args) => { SetChapter(args.CurrentChapter); };

            SetChapter(currentProcess.Data.FirstChapter);
        }

        internal override IChapter? GetChapter()
        {
            return currentChapter;
        }

        /// <summary>
        /// Displays the specified chapter.
        /// </summary>        
        internal override void SetChapter(IChapter chapter)
        {
            IChapter? previousChapter = GlobalEditorHandler.GetCurrentChapter();

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

            ClearConnections(); // edges.ForEach(RemoveElement);
            foreach (Node node in GetChildren())
                node.Free();
            GetChildren().Clear();

            entryNode = new EntryPointNode();
            AddChild(entryNode);

            _ = GenerateNodes(currentChapter).ToList();

            foreach (ProcessGraphNode node in GetChildren().OfType<ProcessGraphNode>())
            {
                RefreshNode(node);
            }
        }

        private IEnumerable<ProcessGraphNode> GenerateNodes(IChapter chapter)
        {
            return chapter.Data.Steps.Select(CreateStepNode).OfType<ProcessGraphNode>();
        }

        private void RefreshNode(ProcessGraphNode node)
        {
            node.Refresh();

            LinkNode(node);

            foreach (ProcessGraphNode leadingNode in GetLeadingNodes(node))
            {
                foreach (IStep output in leadingNode.Outputs)
                {
                    leadingNode.UpdateTransitionTo(Array.IndexOf(leadingNode.Outputs, output), node); // Port port = leadingNode.outputContainer[Array.IndexOf(leadingNode.Outputs, output)].Q<Port>();
                }
            }
        }

        private ProcessGraphNode? CreateStepNode(IStep step)
        {
            if (string.IsNullOrEmpty(step.StepMetadata.StepType))
            {
                step.StepMetadata.StepType = "default";
            }

            IStepNodeInstantiator? instantiator = instantiators.FirstOrDefault(i => i.StepType == step.StepMetadata.StepType);

            if (instantiator == null)
            {
                GD.PushError($"Impossible to find correct visualization for type '{step.StepMetadata.StepType}' used in step '{step.Data.Name}'. Things might not look as expected.");
                instantiator = instantiators.FirstOrDefault(i => i.StepType == "default");
            }

            ProcessGraphNode? node = instantiator?.InstantiateNode(step);
            AddChild(node);
            return node;
        }

        internal override void RefreshChapterRepresentation()
        {
            GD.PushError(new NotImplementedException());
        }


        private ProcessGraphNode? FindStepNode(IStep? step)
        {
            return step == null
                ? null
                : GetChildren().OfType<ProcessGraphNode>().FirstOrDefault(n => n.EntryPoint == step);
        }

        private void LinkNode(ProcessGraphNode node)
        {
            if (node.EntryPoint != null)
            {
                LinkStepNode(node.EntryPoint);
            }
            else if (node is EntryPointNode)
            {
                ProcessGraphNode? firstNode = FindStepNode(currentChapter?.Data.FirstStep);

                if (firstNode != null)
                {
                    // LinkNodes(node.outputContainer[0].Query<Port>(), firstNode.inputContainer[0].Query<Port>());
                    OnConnectionRequest(node.Name, 0, firstNode.Name, 0);
                    node.UpdateTransitionTo(0, firstNode);
                }
            }
        }

        private void LinkStepNode(IStep step)
        {
            foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
            {
                int slotIndex = step.Data.Transitions.Data.Transitions.IndexOf(transition);
                ProcessGraphNode? processGraphNode = FindStepNode(step);

                if (transition.Data.TargetStep != null)
                {
                    ProcessGraphNode? target = FindStepNode(transition.Data.TargetStep);
                    if (processGraphNode != null && target != null)
                    {
                        OnConnectionRequest(processGraphNode.Name, slotIndex, target.Name, 0);
                        processGraphNode.UpdateTransitionTo(0, target);
                    }
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
                if (node is ProcessGraphNode processGraphNode && processGraphNode.Outputs.Contains(targetNode.EntryPoint))
                {
                    leadingNodes.Add(processGraphNode);
                }
            }

            return leadingNodes;
        }
    }
}