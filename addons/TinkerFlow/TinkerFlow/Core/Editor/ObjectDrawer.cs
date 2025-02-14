using System.Linq;
using Godot;
using VRBuilder.Core.SceneObjects;

[Tool]
public partial class ObjectDrawer : Control
{
	private Button? objectLabel;
	public Button ObjectLabel => objectLabel ??= GetNode<Button>("%Object");
	private OptionButton? options;
	private Node? selectedObject;
	public OptionButton Options => options ??= GetNode<OptionButton>("%Options");

	[Signal]
	public delegate void SelectedObjectChangedEventHandler(Variant selectedObject);

	[Export]
	public bool AllowSceneObjects { get; set; }

	public override void _Ready()
	{
		Options.ItemSelected += OptionsOnItemSelected;
	}

	private void OptionsOnItemSelected(long index)
	{
		//TODO: throw item choose
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		//TODO
		return true;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.AsGodotDictionary<Variant, Variant>().TryGetValue("nodes", out Variant nodes))
		{
			if (EditorInterface.Singleton.GetBaseControl().GetNode(nodes.AsStringArray().First()) is ProcessSceneObject reference)
			{
				ObjectLabel.Text = reference.GetPath();
				selectedObject = reference;
				EmitSignal(SignalName.SelectedObjectChanged, Variant.From(reference));
				return;
			}
		}

		selectedObject = null;
		ObjectLabel.Text = "<empty>";
	}
}

