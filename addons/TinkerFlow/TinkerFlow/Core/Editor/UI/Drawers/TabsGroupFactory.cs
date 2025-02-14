// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Editor.Tabs;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    [DefaultProcessDrawer(typeof(ITabsGroup))]
    internal class TabsGroupFactory : AbstractProcessFactory
    {
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            ITabsGroup tabsGroup = (ITabsGroup)currentValue;

            // Draw tabs selector.
            var tabs = DrawToolbox(tabsGroup, changeValueCallback);

            // Get drawer for the object under the tab.
            IProcessDrawer tabValueDrawer = DrawerLocator.GetDrawerForValue(tabsGroup.Tabs[tabsGroup.Selected].GetValue(), typeof(object));

            void ChangeValueCallback(object newValue)
            {
                tabsGroup.Tabs[tabsGroup.Selected].SetValue(newValue);
                changeValueCallback(tabsGroup);
            }

            tabs.AddChild(tabValueDrawer.Create(tabsGroup.Tabs[tabsGroup.Selected].GetValue(), ChangeValueCallback, String.Empty));

            return tabs;
        }

        private TabContainer DrawToolbox(ITabsGroup tabsGroup, Action<object> changeValueCallback)
        {
            Label[] labels = tabsGroup.Tabs.Select(tab => new Label{Text = tab.Label}).ToArray();

            var tabContainer = new TabContainer();
            tabContainer.TabSelected += tab =>
            {
                if (tabsGroup.Selected != tab)
                {
                    tabsGroup.Tabs[tabsGroup.Selected].OnUnselect();
                    tabsGroup.Tabs[(int)tab].OnSelected();
                    tabsGroup.Selected = (int)tab;
                    changeValueCallback(tabsGroup);
                }
            };
            foreach (Label label in labels)
            {
                tabContainer.AddChild(label);
            }

            return tabContainer;
        }
    }
}
