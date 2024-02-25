using System.Linq;

namespace VRBuilder.Editor.UI.Drawers;

public class PrintDebugger
{
    private static int indent;

    public static void Indent()
    {
        indent++;
    }

    public static string Get()
    {
        return string.Concat(Enumerable.Repeat("\t", indent));
    }

    public static void UnIndent()
    {
        indent--;
    }
}