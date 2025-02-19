using Godot;

namespace VRBuilder.Editor.UI.Drawers;

internal class EditorDrawingHelper
{
    public static Button DrawAddButton(string description)
    {
        return new Button
        {
            Text = description,
            Icon = AddIcon("+")
        };
    }

    private static Texture2D AddIcon(string path)
    {
        GD.PushError(new System.NotImplementedException());
        var noiseTexture2D = new NoiseTexture2D
        {
            Height = 16,
            Width = 16
        };
        return noiseTexture2D;
    }

    public static Button DrawHelpButton()
    {
        return new Button
        {
            Icon = AddIcon("?")
        };
    }
}