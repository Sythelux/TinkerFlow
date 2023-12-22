using Godot;

[Tool]
public partial class StepNodeRow : Node
{
    #region Delegates

    [Signal]
    public delegate void AddTransitionEventHandler(StepNodeRow row);

    [Signal]
    public delegate void RemoveTransitionEventHandler(StepNodeRow row);

    #endregion

    private Button? addButton;

    private Button? removeButton;
    private Label? titleLabel;

    public string Title
    {
        get => titleLabel?.Text ?? string.Empty;
        set
        {
            if (titleLabel != null)
                titleLabel.Text = value;
        }
    }

    public bool Removable
    {
        get => removeButton != null && !removeButton.Disabled;
        set
        {
            if (removeButton != null)
                removeButton.Disabled = !value;
        }
    }

    public bool Addable
    {
        get => addButton != null && !addButton.Disabled;
        set
        {
            if (addButton != null)
                addButton.Disabled = !value;
        }
    }

    public override void _Ready()
    {
        addButton = GetNode<Button>("AddTransition");
        removeButton = GetNode<Button>("RemoveTransition");
        titleLabel = GetNode<Label>("Title");
    }

    public void OnAddButton()
    {
        EmitSignal(SignalName.AddTransition, this);
    }

    public void OnRemoveButton()
    {
        EmitSignal(SignalName.RemoveTransition, this);
    }
}