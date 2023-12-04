using System;
using System.Collections.Generic;
using System.Reflection;
using Godot.Collections;

namespace Godot;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
/// <summary>
/// Exports the annotated member as a property of the Godot Object.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SerializeFieldAttribute : Attribute
{
    /// <summary>
    /// Optional hint that determines how the property should be handled by the editor.
    /// </summary>
    public PropertyHint Hint { get; }

    /// <summary>
    /// Optional string that can contain additional metadata for the <see cref="P:Godot.ExportAttribute.Hint" />.
    /// </summary>
    public string HintString { get; }


    public Variant.Type VariantType { get; }

    /// <summary>Constructs a new ExportAttribute Instance.</summary>
    /// <param name="hint">The hint for the exported property.</param>
    /// <param name="hintString">A string that may contain additional metadata for the hint.</param>
    /// <param name="variantType">Variant Type</param>
    public SerializeFieldAttribute(Variant.Type variantType = Variant.Type.Nil, PropertyHint hint = PropertyHint.None, string hintString = "")
    {
        Hint = hint;
        HintString = hintString;
        VariantType = variantType;
    }

    public static IEnumerable<Dictionary> GetPropertyValue<T>(T runtimeConfigurator)
    {
        foreach (FieldInfo memberInfo in typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var att = (SerializeFieldAttribute)GetCustomAttribute(memberInfo, typeof(SerializeFieldAttribute));
            if (att != null)
                yield return PropertyInfo(memberInfo.Name, att.VariantType, att.Hint, att.HintString);
        }

        foreach (PropertyInfo memberInfo in typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var att = (SerializeFieldAttribute)GetCustomAttribute(memberInfo, typeof(SerializeFieldAttribute));
            if (att != null)
                yield return PropertyInfo(memberInfo.Name, att.VariantType, att.Hint, att.HintString);
        }
    }

    public static Dictionary PropertyInfo(string name, Variant.Type type, PropertyHint hint, string hintStr = "")
    {
        var d = new Dictionary();
        d.Add("name", name);
        d.Add("type", (int)type);
        d.Add("hint", (int)hint);
        d.Add("hint_string", hintStr);
        return d;
    }
}