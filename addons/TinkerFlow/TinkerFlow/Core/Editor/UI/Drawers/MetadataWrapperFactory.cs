// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using TinkerFlow.Core.Editor.UI;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.Util;
using VRBuilder.Core.UI.Drawers.Metadata;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// This drawer receives a data structure which contains an actual object to draw and additional drawing information.
    /// It takes metadata entries one by one and recursively calls its Draw method, until no unprocessed metadata left.
    /// After that, an actual object is drawn.
    /// </summary>
    [DefaultProcessDrawer(typeof(MetadataWrapper))]
    internal class MetadataWrapperFactory : AbstractProcessFactory
    {
        private readonly string reorderableName = "ReorderableElement";
        private readonly string separatedName = typeof(SeparatedAttribute).FullName;
        private readonly string deletableName = typeof(DeletableAttribute).FullName;
        private readonly string foldableName = typeof(FoldableAttribute).FullName;
        private readonly string drawIsBlockingToggleName = typeof(DrawIsBlockingToggleAttribute).FullName;
        private readonly string extendableListName = typeof(ExtendableListAttribute).FullName;
        private readonly string keepPopulatedName = typeof(KeepPopulatedAttribute).FullName;
        private readonly string reorderableListOfName = typeof(ReorderableListOfAttribute).FullName;
        private readonly string listOfName = typeof(ListOfAttribute).FullName;
        private readonly string showHelpName = typeof(HelpAttribute).FullName;
        private readonly string showMenuName = typeof(MenuAttribute).FullName;


        /// <inheritdoc />
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            // GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not MetadataWrapper wrapper)
                return new Control { Name = GetType().Name + "." + text };

            // If the drawn object is a ITransition, IBehavior or ICondition the list object will be part of a header.
            bool isPartOfHeader = wrapper.ValueDeclaredType == typeof(ITransition) || wrapper.ValueDeclaredType == typeof(IBehavior) || wrapper.ValueDeclaredType == typeof(ICondition);

            if (wrapper.Metadata.ContainsKey(deletableName))
            {
                return DrawDeletable(wrapper, changeValueCallback, text, isPartOfHeader);
            }

            if (wrapper.Metadata.ContainsKey(showMenuName))
            {
                return DrawMenu(wrapper, changeValueCallback, text, isPartOfHeader);
            }

            if (wrapper.Metadata.ContainsKey(showHelpName))
            {
                return DrawHelp(wrapper, changeValueCallback, text, isPartOfHeader);
            }

            if (wrapper.Metadata.ContainsKey(reorderableName))
            {
                return DrawReorderable(wrapper, changeValueCallback, text, isPartOfHeader);
            }

            if (wrapper.Metadata.ContainsKey(separatedName))
            {
                return DrawSeparated(wrapper, changeValueCallback, text);
            }

            if (wrapper.Metadata.ContainsKey(foldableName))
            {
                return DrawFoldable(wrapper, changeValueCallback, text, isPartOfHeader);
            }

            if (wrapper.Metadata.ContainsKey(drawIsBlockingToggleName))
            {
                return DrawIsBlockingToggle(wrapper, changeValueCallback, text);
            }

            if (wrapper.Metadata.ContainsKey(extendableListName))
            {
                return DrawExtendableList<T>(wrapper, changeValueCallback, text);
            }

            if (wrapper.Metadata.ContainsKey(keepPopulatedName))
            {
                return HandleKeepPopulated(wrapper, changeValueCallback, text);
            }

            if (wrapper.Metadata.ContainsKey(reorderableListOfName))
            {
                return DrawReorderableListOf(wrapper, changeValueCallback, text);
            }

            if (wrapper.Metadata.ContainsKey(listOfName))
            {
                return DrawListOf(wrapper, changeValueCallback, text);
            }

            GD.PushError(new NotImplementedException($"Wrapper drawer for this kind of metadata is not implemented:{string.Join(", ", wrapper.Metadata.Keys)}"));
            return new Control { Name = GetType().Name + ".NotImplementedException" };
        }

        /// <inheritdoc />
        public override Label? GetLabel(MemberInfo memberInfo, object memberOwner)
        {
            return GetLabel(ReflectionUtils.GetValueFromPropertyOrField(memberOwner, memberInfo));
        }

        /// <inheritdoc />
        public override Label? GetLabel<T>(T value)
        {
            // Assert that value is never null, as we always call MetadataWrapper on freshly created instance.
            var wrapper = value as MetadataWrapper;
            IProcessDrawer valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);

            return valueDrawer.GetLabel(wrapper.Value);
        }

        // private GUIStyle GetStyle(bool isPartOfHeader = false)
        // {
        //     GUIStyle style = new GUIStyle(GUI.skin.button)
        //     {
        //         fontStyle = FontStyle.Bold
        //     };
        //
        //     if (isPartOfHeader)
        //     {
        //         Texture2D normal = new Texture2D(1, 1);
        //         normal.SetPixels(new Color[]{ new Color(1, 1, 1, 0)  });
        //         normal.Apply();
        //
        //         Texture2D active = new Texture2D(1, 1);
        //         active.SetPixels(new Color[]{ new Color(1, 1, 1, 0.05f)  });
        //         active.Apply();
        //
        //         style.normal.background = normal;
        //         style.hover.background = active;
        //         style.active.background = active;
        //     }
        //
        //     return style;
        // }

        private Control DrawHelp(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}|{(isPartOfHeader ? "header:yes" : "header:no")}");

            var control = DrawRecursively(wrapper, showHelpName, changeValueCallback, text);
            if (wrapper.Value?.GetType() != null)
            {
                if (wrapper.Value.GetType().GetCustomAttribute(typeof(HelpLinkAttribute)) is HelpLinkAttribute helpLinkAttribute)
                {
                    var button = DrawHelpButton();
                    button.Pressed += () => Application.OpenURL(helpLinkAttribute.HelpLink);
                    control.AddChild(button);
                }
            }

            return control;
        }

        private static Button DrawHelpButton()
        {
            return new Button
            {
                Icon = EditorDrawingHelper.HELP_ICON,
                Name = "DrawHelp.HelpButton",
                Flat = true
            };
        }

        private Control DrawMenu(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}|{(isPartOfHeader ? "header:yes" : "header:no")}");

            var control = DrawRecursively(wrapper, showMenuName, changeValueCallback, text);
            if (wrapper.Value?.GetType() != null)
            {
                var button = DrawMenuButton();
                // menuAttribute.
                // button.Pressed += () => DrawEntityMenu;
                control.AddChild(button);
            }

            return control;
        }

        private static Button DrawMenuButton()
        {
            return new Button
            {
                Icon = EditorDrawingHelper.MENU_ICON,
                Name = "DrawHelp.MenuButton",
                Flat = true
            };
        }

        // private void DrawEntityMenu(MetadataWrapper wrapper, Action<object> changeValueCallback)
        // {
        //     GenericMenu menu = new GenericMenu();
        //
        //     menu.AddItem(new GUIContent("Remove"), false, () => Delete(wrapper, changeValueCallback));
        //     menu.AddItem(new GUIContent("Copy"), false, () => Copy(wrapper, changeValueCallback));
        //
        //     if (CanPaste(wrapper))
        //     {
        //         menu.AddItem(new GUIContent("Paste"), false, () => Paste(wrapper, changeValueCallback));
        //     }
        //     else
        //     {
        //         menu.AddDisabledItem(new GUIContent("Paste"));
        //     }
        //
        //     menu.ShowAsContext();
        // }

        private Control DrawReorderable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}|{(isPartOfHeader ? "header:yes" : "header:no")}");

            var control = DrawRecursively(wrapper, reorderableName, changeValueCallback, text);

            var reorderDownButton = ReorderDownButton(wrapper);
            reorderDownButton.Pressed += () =>
            {
                object oldValue = wrapper.Value;
                ChangeValue(() =>
                    {
                        ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).MoveDown = true;
                        return wrapper;
                    },
                    () =>
                    {
                        ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).MoveDown = false;
                        wrapper.Value = oldValue;
                        return wrapper;
                    },
                    changeValueCallback);
            };
            control.AddChild(reorderDownButton);

            var reorderUpButton = ReorderUpButton(wrapper);
            reorderUpButton.Pressed += () =>
            {
                object oldValue = wrapper.Value;
                ChangeValue(() =>
                    {
                        ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).MoveUp = true;
                        return wrapper;
                    },
                    () =>
                    {
                        ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).MoveUp = false;
                        wrapper.Value = oldValue;
                        return wrapper;
                    },
                    changeValueCallback);
            };
            control.AddChild(reorderUpButton);

            return control;
        }

        private Button ReorderDownButton(MetadataWrapper wrapper)
        {
            return new Button
            {
                Icon = EditorDrawingHelper.ARROW_DOWN_ICON,
                Disabled = ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).IsLast,
                Name = "Reorderable.ReorderDownButton",
                Flat = true
            };
        }

        private Button ReorderUpButton(MetadataWrapper wrapper)
        {
            return new Button
            {
                Icon = EditorDrawingHelper.ARROW_UP_ICON,
                Disabled = ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).IsFirst,
                Name = "Reorderable.ReorderUpButton",
                Flat = true
            };
        }

        private Control DrawSeparated(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            //TODO:
            // EditorDrawingHelper.DrawRect(new Rect(0f, rect.y - 1f, rect.x + rect.width, 1f), Color.grey);
            //
            // Rect wrappedRect = rect;
            // wrappedRect.y += EditorDrawingHelper.VerticalSpacing;
            //
            // wrappedRect = DrawRecursively(wrappedwrapper, separatedName, changeValueCallback, label);
            //
            // wrappedRect.height += EditorDrawingHelper.VerticalSpacing;
            //
            // EditorDrawingHelper.DrawRect(new Rect(0f, wrappedRect.y + wrappedRect.height - 1f, wrappedRect.x + wrappedRect.width, 1f), Color.grey);
            //
            // rect.height = wrappedRect.height;
            // return rect;
            return new Control { Name = "DrawSeparated" };
        }

        private Control DrawDeletable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}|{(isPartOfHeader ? "header:yes" : "header:no")}");

            Control control = DrawRecursively(wrapper, deletableName, changeValueCallback, text);

            var deleteButton = DeleteButton(wrapper);
            deleteButton.Pressed += () =>
            {
                object oldValue = wrapper.Value;
                ChangeValue(() =>
                    {
                        wrapper.Value = null;
                        return wrapper;
                    },
                    () =>
                    {
                        wrapper.Value = oldValue;
                        return wrapper;
                    },
                    changeValueCallback);
            };
            control.AddChild(deleteButton);

            return control;
        }

        private static Button DeleteButton(MetadataWrapper wrapper)
        {
            return new Button
            {
                Icon = EditorDrawingHelper.DELETE_ICON,
                // Disabled = wrapper.Value,
                Name = "DrawDeletable.DeleteButton",
                Flat = true
            };
        }

        private Control DrawFoldable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}|{(isPartOfHeader ? "header:yes" : "header:no")}");

            if (wrapper.Metadata[foldableName] == null)
            {
                wrapper.Metadata[foldableName] = true;
                changeValueCallback(wrapper);
            }

            Control control;
            if (isPartOfHeader)
                control = new HBoxContainer { Name = "Foldable.Container" };
            else
                control = new VBoxContainer { Name = "Foldable.Container" };

            var isToggledInValue = !(bool)wrapper.Metadata[foldableName];

            Button collapseButton = EditorDrawingHelper.DrawCollapseButton(collapsed: isToggledInValue);
            collapseButton.Flat = true;
            var toggleLabel = new Label
            {
                Name = "Foldable.ToggleLabel",
                Text = text,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            var expandableContainer = new ExpandableVBoxContainer { Name = "Foldable.ExpandableContainer" };
            // expandableContainer.AddChild(new Label { Text = "Some" });
            // expandableContainer.AddChild(new Label { Text = "Junk" });
            // expandableContainer.AddChild(new Label { Text = "To" });
            // expandableContainer.AddChild(new Label { Text = "Test" });
            expandableContainer.SetToggled(isToggledInValue);
            collapseButton.Toggled += OnCollapseButtonOnToggled;
            collapseButton.Toggled += OnToggledEventHandler;

            control.AddChild(collapseButton);
            control.AddChild(toggleLabel);
            control.AddChild(expandableContainer);

            expandableContainer.AddChild(DrawRecursively(wrapper, foldableName, (newWrapper) =>
            {
                // We want the user to be aware that value has changed even if the foldable was collapsed (for example, undo/redo).
                wrapper.Metadata[foldableName] = true;
                changeValueCallback(wrapper);
            }, text));
            return control;

            void OnToggledEventHandler(bool newIsToggledInValue)
            {
                var oldIsToggledInValue = !(bool)wrapper.Metadata[foldableName];
                if (newIsToggledInValue != oldIsToggledInValue)
                {
                    wrapper.Metadata[foldableName] = !newIsToggledInValue;
                    changeValueCallback(wrapper);
                }
            }

            void OnCollapseButtonOnToggled(bool toggled) => expandableContainer.SetToggled(toggled);
        }

        private Control DrawIsBlockingToggle(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            var dataOwner = wrapper.Value as IDataOwner;

            var control = DrawRecursively(wrapper, drawIsBlockingToggleName, changeValueCallback, text);

            if (dataOwner == null)
            {
                GD.PushError("The target property of the DrawIsBlockingToggleAttribute has to implement IDataOwner.");
                return control;
            }

            if (dataOwner.Data is not IBackgroundBehaviorData backgroundBehaviorData)
            {
                return control;
            }

            IProcessDrawer? boolDrawer = DrawerLocator.GetDrawerForValue(backgroundBehaviorData.IsBlocking, typeof(bool));
            control.AddChild(boolDrawer?.Create(backgroundBehaviorData.IsBlocking, (newValue) =>
            {
                backgroundBehaviorData.IsBlocking = (bool)newValue;
                changeValueCallback(wrapper);
            }, "Wait for completion"));

            return control;
        }

        private Control DrawExtendableList<T>(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            if (wrapper.Value is IList == false)
            {
                GD.PushWarning("ExtendableListAttribute can be used only with IList members.");
                return new Control { Name = "DrawExtendableList" };
            }

            Type? elementType = (wrapper.Metadata[extendableListName] as ExtendableListAttribute.SerializedTypeWrapper)?.Type;
            var list = (IList)wrapper.Value;


            var control = DrawRecursively(wrapper, extendableListName, changeValueCallback, text);

            IProcessDrawer addThingsDrawer = DrawerLocator.GetInstantiatorDrawer(elementType);

            if (addThingsDrawer != null)
            {
                control.AddChild(addThingsDrawer.Create<T>(default, (newValue) =>
                {
                    if (newValue == null)
                    {
                        ReflectionUtils.RemoveFromList(ref list, list.Count - 1);
                    }
                    else
                    {
                        ReflectionUtils.InsertIntoList(ref list, list.Count, newValue);
                    }

                    if (wrapper.Metadata.ContainsKey(listOfName))
                    {
                        var temp = (ListOfAttribute.Metadata)wrapper.Metadata[listOfName];
                        temp.ChildMetadata.Add(temp.ChildAttributes.ToDictionary(attribute => attribute.Name, attribute => attribute.GetDefaultMetadata(null)));
                        wrapper.Metadata[listOfName] = temp;
                    }

                    wrapper.Value = list;
                    changeValueCallback(wrapper);
                }, ""));
            }

            return control;
        }

        private Control HandleKeepPopulated(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            if (wrapper.Value is IList == false)
            {
                GD.PushWarning("KeepPopulated can be used only with IList members.");
                return new Control { Name = "HandleKeepPopulated" };
            }

            var list = (IList)wrapper.Value;

            if (list.Count == 0)
            {
                var entryType = (Type)wrapper.Metadata[keepPopulatedName];
                if (entryType != null)
                {
                    Type listType = ReflectionUtils.GetEntryType(list);
                    if (listType.IsAssignableFrom(entryType))
                    {
                        ReflectionUtils.InsertIntoList(ref list, 0, ReflectionUtils.CreateInstanceOfType(entryType));
                    }
                    else
                    {
                        GD.PushError("Trying to add an keep populuated entry with type {0} to list filled {1}", entryType.Name, listType.Name);
                    }
                }
                else
                {
                    GD.PushError("No Type found to create default instance with");
                }
            }

            return DrawRecursively(wrapper, keepPopulatedName, changeValueCallback, text);
        }

        private IList<MetadataWrapper> ConvertListOfMetadataToList(MetadataWrapper wrapper)
        {
            if (CheckListOfMetadata(wrapper) == false)
            {
                return new List<MetadataWrapper>();
            }

            if (wrapper.Metadata.Count > 1)
            {
                GD.PushError(new NotImplementedException($"ListOfAttribute attribute should have the lowest priority. Check MetadataWrapperDrawer.Draw method."));
            }

            var wrapperMetadata = (wrapper.Metadata[listOfName] as ListOfAttribute.Metadata);
            List<Dictionary<string, object>> listOfMetadata = wrapperMetadata.ChildMetadata;

            var list = (IList)wrapper.Value;

            if (listOfMetadata == null)
            {
                listOfMetadata = new List<Dictionary<string, object>>(list.Count);
            }

            if (listOfMetadata.Count != list.Count)
            {
                listOfMetadata.Clear();
                for (var i = 0; i < list.Count; i++)
                {
                    listOfMetadata.Add(wrapperMetadata.ChildAttributes.ToDictionary(attribute => attribute.Name, attribute => attribute.GetDefaultMetadata(null)));
                }
            }

            return GetListOfWrappers(wrapper, listOfMetadata);
        }

        private IList<MetadataWrapper> ConvertReorderableListOfMetadataToList(MetadataWrapper wrapper)
        {
            if (CheckListOfMetadata(wrapper) == false)
            {
                return new List<MetadataWrapper>();
            }

            if (wrapper.Metadata.Count > 1)
            {
                GD.PushError(new NotImplementedException($"ReorderableListOfAttribute attribute should have the lowest priority. Check MetadataWrapperDrawer.Draw method."));
            }

            var wrapperMetadata = (wrapper.Metadata[reorderableListOfName] as ListOfAttribute.Metadata);
            List<Dictionary<string, object>> listOfMetadata = wrapperMetadata.ChildMetadata;

            int wrapperCount = ((IList)wrapper.Value).Count;

            if (listOfMetadata == null)
            {
                listOfMetadata = new List<Dictionary<string, object>>(wrapperCount);
            }

            if (listOfMetadata.Count != wrapperCount)
            {
                listOfMetadata.Clear();
                for (var i = 0; i < wrapperCount; i++)
                {
                    listOfMetadata.Add(wrapperMetadata.ChildAttributes.ToDictionary(attribute => attribute.Name, attribute => attribute.GetDefaultMetadata(null)));
                    listOfMetadata[i].Add(reorderableName, new ReorderableElementMetadata());
                }
            }

            return GetListOfWrappers(wrapper, listOfMetadata);
        }

        private bool CheckListOfMetadata(MetadataWrapper wrapper)
        {
            if (wrapper.Value == null || (wrapper.Value is IList == false))
            {
                if (wrapper.Value != null)
                {
                    GD.PushWarning($"ListOfAttribute can be used only with IList members.");
                }

                return false;
            }

            return true;
        }

        private IList<MetadataWrapper> GetListOfWrappers(MetadataWrapper wrapper, List<Dictionary<string, object>> listOfMetadata)
        {
            Type entryType = ReflectionUtils.GetEntryType(wrapper.Value);
            var wrapperValueList = (IList)wrapper.Value;

            List<MetadataWrapper> listOfWrappers = new List<MetadataWrapper>();
            for (var i = 0; i < wrapperValueList.Count; i++)
            {
                listOfWrappers.Add(new MetadataWrapper()
                {
                    Metadata = listOfMetadata[i],
                    ValueDeclaredType = entryType,
                    Value = wrapperValueList[i],
                });
            }

            return listOfWrappers;
        }

        private Control DrawListOf(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            IList<MetadataWrapper> listOfWrappers = ConvertListOfMetadataToList(wrapper);

            IProcessDrawer valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);
            var list = (IList)wrapper.Value;

            return valueDrawer?.Create(listOfWrappers, (newValue) =>
            {
                List<MetadataWrapper> newListOfWrappers = ((List<MetadataWrapper>)newValue).ToList();

                ReflectionUtils.ReplaceList(ref list, newListOfWrappers.Select(childWrapper => childWrapper.Value));
                wrapper.Value = list;

                ((ListOfAttribute.Metadata)wrapper.Metadata[listOfName]).ChildMetadata = newListOfWrappers.Select(childWrapper => childWrapper.Metadata).ToList();
                changeValueCallback(wrapper);
            }, text) ?? new Control { Name = "DrawListOf" };
        }

        private Control DrawReorderableListOf(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:{text}");

            IList<MetadataWrapper> listOfWrappers = ConvertReorderableListOfMetadataToList(wrapper);

            IProcessDrawer? valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);
            var list = (IList)wrapper.Value;

            for (var i = 0; i < listOfWrappers.Count; i++)
            {
                var metadata = (ReorderableElementMetadata)listOfWrappers[i].Metadata[reorderableName];
                metadata.IsFirst = i == 0;
                metadata.IsLast = i == listOfWrappers.Count - 1;
            }

            return valueDrawer.Create(listOfWrappers, (newValue) =>
            {
                List<MetadataWrapper> newListOfWrappers = ((List<MetadataWrapper>)newValue).ToList();

                for (var i = 0; i < newListOfWrappers.Count; i++)
                {
                    var metadata = (ReorderableElementMetadata)newListOfWrappers[i].Metadata[reorderableName];

                    switch (metadata.MoveDown)
                    {
                        case true when metadata.MoveUp == false:
                        {
                            metadata.MoveDown = false;
                            if (i < newListOfWrappers.Count - 1)
                            {
                                (newListOfWrappers[i], newListOfWrappers[i + 1]) = (newListOfWrappers[i + 1], newListOfWrappers[i]);
                            }

                            // Repeat at same index because unprocessed element switched position to i.
                            i--;
                            break;
                        }
                        case false when metadata.MoveUp:
                        {
                            metadata.MoveUp = false;
                            if (i > 0)
                            {
                                (newListOfWrappers[i], newListOfWrappers[i - 1]) = (newListOfWrappers[i - 1], newListOfWrappers[i]);
                            }

                            break;
                        }
                        default:
                            // Reset, if both actions are true
                            metadata.MoveDown = false;
                            metadata.MoveUp = false;
                            break;
                    }
                }

                ReflectionUtils.ReplaceList(ref list, newListOfWrappers.Select(childWrapper => childWrapper.Value));
                wrapper.Value = list;

                ((ListOfAttribute.Metadata)wrapper.Metadata[reorderableListOfName]).ChildMetadata = newListOfWrappers.Select(childWrapper => childWrapper.Metadata).ToList();
                changeValueCallback(wrapper);
            }, text) ?? new Control { Name = "DrawReorderableListOf" };
        }

        private Control DrawRecursively(MetadataWrapper wrapper, string currentDrawerName, Action<object> changeValueCallback, string text)
        {
            // There are more metadata information to handle, pass it to the next iteration.
            Control rect;
            if (wrapper.Metadata.Count > 1)
            {
                rect = DrawWrapperRecursively(wrapper, changeValueCallback, currentDrawerName, text);
            }
            else
            {
                // Draw an actual object.
                IProcessDrawer? valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);

                void ValueChanged(object newValue)
                {
                    wrapper.Value = newValue;
                    changeValueCallback(wrapper);
                }

                rect = valueDrawer?.Create(wrapper.Value, ValueChanged, text) ?? new Control { Name = "DrawRecursively" };
            }

            return rect;
        }

        private Control DrawWrapperRecursively(MetadataWrapper parentWrapper, Action<object> changeValueCallback, string removedMetadataName, string text)
        {
            var wrappedWrapper = new MetadataWrapper()
            {
                Value = parentWrapper.Value,
                ValueDeclaredType = parentWrapper.ValueDeclaredType,
                Metadata = parentWrapper.Metadata.Where(kvp => kvp.Key != removedMetadataName).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            return Create(wrappedWrapper, WrappedWrapperChanged, text) ?? new Control { Name = "DrawWrapperRecursively" };

            void WrappedWrapperChanged(object newValue)
            {
                var newWrapper = (MetadataWrapper)newValue;

                foreach (string key in newWrapper.Metadata.Keys)
                {
                    parentWrapper.Metadata[key] = wrappedWrapper.Metadata[key];
                }

                parentWrapper.Value = newWrapper.Value;

                changeValueCallback(parentWrapper);
            }
        }
    }
}