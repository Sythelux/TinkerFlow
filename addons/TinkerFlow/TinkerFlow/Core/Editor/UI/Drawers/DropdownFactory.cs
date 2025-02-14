using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Generic drawer for dropdowns. Implement by providing your possible options.
    /// </summary>
    /// <remarks>
    /// In case of a null or invalid value, the drawer will automatically select the first
    /// available value. You can create a null entry for the first value if you want it
    /// to default to null.
    /// </remarks>
    /// <typeparam name="T">Type of value provided by the dropdown.</typeparam>
    public abstract class DropdownFactory<T> : AbstractProcessFactory where T : IEquatable<T>
    {
        /// <summary>
        /// List of elements displayed in the dropdown.
        /// </summary>
        protected abstract IList<DropDownElement<T>> PossibleOptions { get; }

        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (PossibleOptions.Count == 0)
            {

                return new Label { Text = "No values can be selected." };
            }

            T oldValue = (T)currentValue;

            int oldIndex = PossibleOptions.IndexOf(PossibleOptions.FirstOrDefault(item => item.Value != null && item.Value.Equals(oldValue)));

            if (oldIndex < 0)
            {
                oldIndex = 0;
            }

            MenuButton menuButton = new MenuButton { Text = text, Name = GetType().Name + "." + text };
            PopupMenu menu = menuButton.GetPopup();

            foreach (var po in PossibleOptions.Select(item => item.Label))
            {
                menu.AddItem(po);
            }

            menu.IndexPressed += idx =>
            {
                var newIndex = (int)idx;

                if (PossibleOptions[newIndex].Value == null)
                {
                    if (oldValue != null)
                    {
                        ChangeValue(() => PossibleOptions[newIndex].Value, () => oldValue, changeValueCallback);
                    }
                }
                else if (PossibleOptions[newIndex].Value.Equals(oldValue) == false)
                {
                    ChangeValue(() => PossibleOptions[newIndex].Value, () => oldValue, changeValueCallback);
                }
            };

            return menuButton;
        }
    }
}
