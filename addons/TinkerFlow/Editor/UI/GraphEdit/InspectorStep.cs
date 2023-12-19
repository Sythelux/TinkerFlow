using Godot;

namespace VRBuilder.Editor.UI.Windows;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public partial class InspectorStep : Control
{
    private LineEdit? stepDescription;
    private VBoxContainer? behaviors;
    private VBoxContainer? transitions;
    private VBoxContainer? unlockedElements;

    public LineEdit StepDescription => stepDescription ??= GetNode<LineEdit>("StepDescription");
    public VBoxContainer Behaviors => behaviors ??= GetNode<VBoxContainer>("%BehaviorTab");
    public VBoxContainer Transitions => transitions ??= GetNode<VBoxContainer>("%ConditionTab");
    public VBoxContainer UnlockedElements => unlockedElements ??= GetNode<VBoxContainer>("%UnlockedObjects");
    
    
}