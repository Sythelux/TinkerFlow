using System;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// An element in a dropdown.
    /// </summary>
    /// <typeparam name="T">Type of value returned by the dropdown.</typeparam>
    public struct DropDownElement<T>(T value, string label) where T : IEquatable<T>
    {
        /// <summary>
        /// Display name of the element.
        /// </summary>
        public string Label { get; set; } = label;

        /// <summary>
        /// Selectable value.
        /// </summary>
        public T Value { get; set; } = value;

        public DropDownElement(T value) : this(value, $"{value}") { }
    }
}
