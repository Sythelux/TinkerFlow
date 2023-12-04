using System;

namespace Godot;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
/// TO BE REPLACES, when EXPORT_HIDDEN_INSPECTOR EXISTS
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class HideInInspector : Attribute
{
    /// <summary>
    /// Optional hint that determines how the property should be handled by the editor.
    /// </summary>
    public PropertyHint Hint { get; }

    /// <summary>
    /// Optional string that can contain additional metadata for the <see cref="P:Godot.ExportAttribute.Hint" />.
    /// </summary>
    public string HintString { get; }

    /// <summary>Constructs a new ExportAttribute Instance.</summary>
    /// <param name="hint">The hint for the exported property.</param>
    /// <param name="hintString">A string that may contain additional metadata for the hint.</param>
    public HideInInspector(PropertyHint hint = PropertyHint.None, string hintString = "")
    {
        this.Hint = hint;
        this.HintString = hintString;
    }
}