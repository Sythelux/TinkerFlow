using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace TinkerFlow.Godot.Editor;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[Tool]
public partial class ProcessGraph : GraphEdit
{
    [Export]
    public PackedScene? StepNode { get; set; }

    private PopupMenu? addNodeMenu;
    public PopupMenu AddNodeMenu => addNodeMenu ??= GetNode<PopupMenu>("PopupMenu");
    private PopupMenu? editNodeMenu;
    private Vector2 popUpPosition;
    public PopupMenu EditNodeMenu => editNodeMenu ??= GetNode<PopupMenu>("NodePopupMenu");
    public Node? SelectedNode { get; set; }

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
        (GetNode(fromNode + "") as StepNode)?.RemoveTransitionTo((int)fromPort, GetNode<StepNode>(toNode + ""), (int)toPort);
    }

    private void OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        (GetNode(fromNode + "") as StepNode)?.UpdateTransitionTo((int)fromPort, GetNode<StepNode>(toNode + ""), (int)toPort);
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


    public void AddStep(int id)
    {
        var step = StepNode?.Instantiate<StepNode>();
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

    public void RemoveStep(StepNode stepNode)
    {
        RemoveChild(stepNode);
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

    public void ShowPopup(Vector2 pos)
    {
        popUpPosition = pos + GlobalPosition;
        if (SelectedNode == null)
            AddNodeMenu.PopupOnParent(new Rect2I((int)popUpPosition.X, (int)popUpPosition.Y, AddNodeMenu.Size.X, AddNodeMenu.Size.Y));
        else
            EditNodeMenu.PopupOnParent(new Rect2I((int)popUpPosition.X, (int)popUpPosition.Y, AddNodeMenu.Size.X, AddNodeMenu.Size.Y));
    }
}