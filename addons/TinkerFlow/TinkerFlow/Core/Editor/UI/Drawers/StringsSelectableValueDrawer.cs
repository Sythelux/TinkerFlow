#if UNITY_5_3_OR_NEWER
using UnityEditor
namespace VRBuilder.Editor.UI.Drawers
{
    /// <summary>
    /// Selectable value drawer letting the user choose between two string values.
    /// </summary>
    public class StringsSelectableValueDrawer : SelectableValueDrawer<string, string>
    {
    }
}

#elif GODOT
using Godot;
//TODO
#endif
