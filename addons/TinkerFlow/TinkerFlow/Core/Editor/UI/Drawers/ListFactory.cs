// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TinkerFlow.Core.Editor.UI;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// A default Process drawer for types implementing `IList`.
    /// </summary>
    [DefaultProcessDrawer(typeof(IList))]
    internal class ListFactory : AbstractProcessFactory
    {
        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not IList list)
                return new Control { Name = GetType().Name + "." + text };

            Type entryDeclaredType = ReflectionUtils.GetEntryType(currentValue);

            var control = new VBoxContainer { Name = GetType().Name + "." + text };

            // if (!string.IsNullOrEmpty(text))
            //     control.AddChild(new Label { Text = text });

            object[] entries = new object[list.Count];
            list.CopyTo(entries, 0);

            int closuredLength = entries.Length;
            for (int index = 0; index < entries.Length; index++)
            {
                int closuredIndex = index;
                object entry = entries[index];

                IProcessDrawer entryDrawer = DrawerLocator.GetDrawerForValue(entry, entryDeclaredType);

                Action<object> entryValueChangedCallback = newValue =>
                {
                    if (list.Count < closuredLength)
                    {
                        ReflectionUtils.InsertIntoList(ref list, closuredIndex, newValue);
                    }
                    else
                    {
                        list[closuredIndex] = newValue;
                    }

                    MetadataWrapper wrapper = newValue as MetadataWrapper;
                    // if new value is null, or the value is wrapper with null value, remove it from list.
                    if (newValue == null || (wrapper != null && wrapper.Value == null))
                    {
                        ReflectionUtils.RemoveFromList(ref list, closuredIndex);
                    }

                    changeValueCallback(list);
                };

                var block = new VBoxContainer { Name = $"ListFactory.Block:{index}" };
                var bg = new PanelContainer { Name = $"ListFactory.Block.Bg:{index}" };
                var styleBoxFlat = new StyleBoxFlat
                {
                    BgColor = new Color(1f, 1f, 1f, 0.1f),
                };
                bg.AddThemeStyleboxOverride("panel", styleBoxFlat);

                Label entryLabel = entryDrawer.GetLabel(entry);
                Control? header = entryDrawer.Create(entry, entryValueChangedCallback, entryLabel.Text);
                ExpandableVBoxContainer body = header.GetChildren(true).OfType<ExpandableVBoxContainer>().First();

                // block.AddChild(new HSeparator());
                block.AddChild(bg);
                bg.AddChild(header);
                header.RemoveChild(body);
                block.AddChild(body);

                control.AddChild(block);
            }
            return control;
        }
    }
}
