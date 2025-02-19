using Godot;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Core.Editor.UI;

internal class EditorDrawingHelper
{
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
        var button = new Button();
        button.Text = description;
        button.Icon = GetIcon("icon_add");
        button.Name = "AddButton";
        return button;
    }

    public static Button DrawHelpButton()
    {
        var button = new Button();
        button.Icon = GetIcon("icon_help");
        button.Name = "HelpButton";
        return button;
    }

    public static Button DrawPasteButton()
    {
        var button = new Button();
        button.Icon = GetIcon("icon_paste");
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
