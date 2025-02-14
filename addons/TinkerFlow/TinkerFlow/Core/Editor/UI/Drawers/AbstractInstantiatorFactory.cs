// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// An abstract drawer for Step Inspector UI elements that create new instances of objects.
    /// Drawers for "Add Behavior", "Add Condition", "Add Transition" buttons inherit from this class.
    /// </summary>
    /// <typeparam name="T">A type of an object to instantiate.</typeparam>
    internal abstract partial class AbstractInstantiatorFactory<T> : AbstractProcessFactory
    {
        /// <summary>
        /// Converts a list of <seealso cref="StepInspector.Menu.MenuOption"/> <paramref name="options"/> to <seealso cref="MenuOption{T}"/> options.
        /// </summary>
        protected IList<TestableEditorElements.MenuOption> ConvertFromConfigurationOptionsToGenericMenuOptions(IList<MenuOption<T>> options, object currentValue, Action<object> changeValueCallback)
        {
            return options.Select<MenuOption<T>, TestableEditorElements.MenuOption>(menuOption =>
            {
                MenuSeparator<T> separator = menuOption as MenuSeparator<T>;
                DisabledMenuItem<T> disabled = menuOption as DisabledMenuItem<T>;
                MenuItem<T> item = menuOption as MenuItem<T>;

                if (separator != null)
                {
                    return new TestableEditorElements.MenuSeparator(separator.PathToSubmenu);
                }

                if (disabled != null)
                {
                    return new TestableEditorElements.DisabledMenuItem(new Label { Text = disabled.Label });
                }

                if (item != null)
                {
                    return new TestableEditorElements.MenuItem(new Label { Text = item.DisplayedName }, false, () => ChangeValue(() => item.GetNewItem(), () => currentValue, changeValueCallback));
                }

                throw new InvalidCastException("There is a closed list of implementations of AddItemMenuOption.");
            }).ToList();
        }
    }
}
