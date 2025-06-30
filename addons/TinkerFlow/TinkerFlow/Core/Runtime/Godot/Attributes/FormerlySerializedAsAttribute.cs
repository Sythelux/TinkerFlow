using System;

namespace VRBuilder.Core.Godot.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormerlySerializedAsAttribute : Attribute
{
    public string Message { get; }

    public FormerlySerializedAsAttribute(string message)
    {
        Message = message;
    }
}