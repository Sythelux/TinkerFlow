using Godot;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Core.Editor.UI;

internal class EditorDrawingHelper
{
    public static readonly Texture2D DELETE_ICON = TinkerFlowPlugin.GetIcon("Remove");
    public static readonly Texture2D ARROW_UP_ICON = TinkerFlowPlugin.GetIcon("ArrowUp");
    public static readonly Texture2D ARROW_DOWN_ICON = TinkerFlowPlugin.GetIcon("ArrowDown");
    public static readonly Texture2D HELP_ICON = TinkerFlowPlugin.GetIcon("Help");
    public static readonly Texture2D MENU_ICON = TinkerFlowPlugin.GetIcon("GuiTabMenuHl");
    public static readonly Texture2D EDIT_ICON = TinkerFlowPlugin.GetIcon("Edit");
    public static readonly Texture2D SHOW_ICON = TinkerFlowPlugin.GetIcon("Info");
    public static readonly Texture2D ADD_ICON = TinkerFlowPlugin.GetIcon("Add");
    public static readonly Texture2D PASTE_ICON = TinkerFlowPlugin.GetIcon("ActionPaste");


    //TODO: get all references to this function and unify it.
    public static Texture2D GetIcon(string path)
    {
        path += EditorInterface.Singleton.GetEditorSettings().Get("interface/theme/base_color").AsColor().Luminance > 0.5
            ? "_dark"
            : "_light";
        path += ".png";
        return TinkerFlowPlugin.GetIcon(path);
    }

    public static Button DrawAddButton(string description)
    {
        var button = new Button
        {
            Text = description,
            Icon = ADD_ICON,
            Name = "AddButton"
        };
        return button;
    }

    public static Button DrawHelpButton()
    {
        var button = new Button();
        button.Icon = HELP_ICON;
        button.Name = "HelpButton";
        return button;
    }

    public static Button DrawPasteButton()
    {
        var button = new Button();
        button.Icon = PASTE_ICON;
        button.Name = "PasteButton";
        return button;
    }

    public static Button DrawCollapseButton(string? label = null, bool collapsed = false)
    {
        var guiTreeArrowRight = TinkerFlowPlugin.GetIcon("GuiTreeArrowRight");
        var guiTreeArrowDown = TinkerFlowPlugin.GetIcon("GuiTreeArrowDown");

        var button = new Button();
        button.Icon = collapsed ? guiTreeArrowRight : guiTreeArrowDown;
        button.Name = "CollapseButton";
        button.ToggleMode = true;
        if (label != null)
            button.Text = label;
        button.Toggled += toggledOn => button.Icon = toggledOn ? guiTreeArrowRight : guiTreeArrowDown;
        return button;
    }
}
