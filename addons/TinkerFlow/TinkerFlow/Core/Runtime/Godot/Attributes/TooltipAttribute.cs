using System;

namespace VRBuilder.Core.Godot.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TooltipAttribute : Attribute
    {
        public string Message { get; }

        public TooltipAttribute(string message)
        {
            Message = message;
        }
    }
}
