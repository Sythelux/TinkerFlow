using Godot;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Core.Editor.UI;

internal class EditorDrawingHelper
{
    private static Texture2D AddIcon(string path)
    {
        path += EditorInterface.Singleton.GetEditorSettings().Get("interface/theme/base_color").AsColor().Luminance > 0.5
            ? "_dark"
            : "_light";
        path += ".png";
        return TinkerFlowPlugin.GetIcon(path);
    }

    public static Button DrawAddButton(string description)
    {
        return new Button
        {
            Text = description,
            Icon = AddIcon("icon_add")
        };
    }

    public static Button DrawHelpButton()
    {
        return new Button
        {
            Icon = AddIcon("icon_help")
        };
    }

    public static Button DrawPasteButton()
    {
        return new Button
        {
            Icon = AddIcon("icon_paste")
        };
    }
}
