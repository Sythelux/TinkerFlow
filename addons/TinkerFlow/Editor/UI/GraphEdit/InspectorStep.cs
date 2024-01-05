using Godot;
using VRBuilder.Core;

namespace VRBuilder.Editor.UI.Windows;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[Tool]
public partial class InspectorStep : VBoxContainer
{
	private Step.EntityData? step;
	private LineEdit? stepDescription;
	private VBoxContainer? behaviors;
	private VBoxContainer? transitions;
	private VBoxContainer? unlockedElements;

	public VBoxContainer Behaviors => behaviors ??= GetNode<VBoxContainer>("%Behaviors");
	public VBoxContainer Transitions => transitions ??= GetNode<VBoxContainer>("%Conditions");
	public VBoxContainer UnlockedElements => unlockedElements ??= GetNode<VBoxContainer>("%UnlockedObjects");

	public Step.EntityData? Step
	{
		get => step;
		set => step = value;
	}
}
