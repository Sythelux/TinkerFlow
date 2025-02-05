using Godot;
using Godot.Collections;

namespace VRBuilder.Core.SceneObjects;

public partial class ProcessSceneObject
{
    private bool resetObjectId;

    [Export]
    public bool ResetObjectID
    {
        get => resetObjectId;
        private set
        {
            if (value)
                MakeUnique();
        }
    }

    public override Variant _Get(StringName property)
    {
        var variant = base._Get(property);
        serializedGuid ??= new SerializableGuid();
        var serializableGuid = serializedGuid as SerializableGuid;
        // if(property == nameof(serializedGuid))
        //     if (serializedGuid == null!)
        //         serializedGuid = new SerializableGuid();
        if (IsGuidAssigned())
        {
            /// Restore Guid:
            /// - Editor Prefab Overrides -> Revert
            serializableGuid?.SetGuid(guid);
        }
        else if (SerializableGuid.IsValid(serializableGuid))
        {
            /// Apply Serialized Guid:
            /// - Open scene
            /// - Recompile
            /// - Editor Prefab Overrides -> Apply
            /// - Start Playmode
            guid = serializableGuid.Guid;
        }
        else
        {
            /// - New GameObject we initialize guid lazy
            /// - Drag and drop prefab into scene
            /// - Interacting with the prefab outside of the scene
        }

        return variant;
    }

    // [ContextMenu("Reset Object ID")]
    protected void MakeUnique()
    {
        var dialog = new ConfirmationDialog();
        EditorInterface.Singleton.GetEditorViewport3D().AddChild(dialog);
        dialog.DialogText = "Warning! This will change the object's unique ID.\n" +
                            "All reference to this object in the Process Editor will become invalid.\n" +
                            "Proceed?";
        dialog.Title = "Reset Object ID";
        dialog.CancelButtonText = "No";
        dialog.OkButtonText = "Yes";
        dialog.Confirmed += ResetUniqueId;
        dialog.PopupCentered();
    }

    public override bool _Set(StringName property, Variant value)
    {
        return base._Set(property, value);
    }

    // public override Array<Dictionary> _GetPropertyList()
    // {
    //     return base._GetPropertyList();
    // }
}