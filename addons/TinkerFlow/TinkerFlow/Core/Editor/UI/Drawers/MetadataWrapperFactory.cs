// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core;
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
        private static readonly Texture2D deleteIcon = TinkerFlowPlugin.GetIcon("icon_delete");
        private static readonly Texture2D arrowUpIcon = TinkerFlowPlugin.GetIcon("icon_arrow_up");
        private static readonly Texture2D arrowDownIcon = TinkerFlowPlugin.GetIcon("icon_arrow_down");
        private static readonly Texture2D helpIcon = TinkerFlowPlugin.GetIcon("icon_help");


        /// <inheritdoc />
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not MetadataWrapper wrapper)
                return new Control();

            // If the drawn object is a ITransition, IBehavior or ICondition the list object will be part of a header.
            bool isPartOfHeader = wrapper.ValueDeclaredType == typeof(ITransition) || wrapper.ValueDeclaredType == typeof(IBehavior) || wrapper.ValueDeclaredType == typeof(ICondition);

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

            if (wrapper.Metadata.ContainsKey(deletableName))
            {
                return DrawDeletable(wrapper, changeValueCallback, text, isPartOfHeader);
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

            throw new NotImplementedException("Wrapper drawer for this kind of metadata is not implemented.");
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
            MetadataWrapper wrapper = value as MetadataWrapper;
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
            var control = DrawRecursively(wrapper, showHelpName, changeValueCallback, text);
            if (wrapper.Value?.GetType() != null)
            {
                HelpLinkAttribute helpLinkAttribute = wrapper.Value.GetType().GetCustomAttribute(typeof(HelpLinkAttribute)) as HelpLinkAttribute;
                if (helpLinkAttribute != null)
                {
                    var button = new Button
                    {
                        Icon = helpIcon,
                    };
                    button.Pressed += () => Application.OpenURL(helpLinkAttribute.HelpLink);
                    control.AddChild(button);
                }
            }
            return control;
        }

        private Control DrawReorderable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            var control = DrawRecursively(wrapper, reorderableName, changeValueCallback, text);

            var reorderDownButton = new Button
            {
                Icon = arrowDownIcon,
                Disabled = ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).IsLast
            };
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

            var reorderUpButton = new Button
            {
                Icon = arrowUpIcon,
                Disabled = ((ReorderableElementMetadata)wrapper.Metadata[reorderableName]).IsFirst
            };
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

        private Control DrawSeparated(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            //TODO:
            // EditorDrawingHelper.DrawRect(new Rect(0f, rect.y - 1f, rect.x + rect.width, 1f), Color.grey);
            //
            // Rect wrappedRect = rect;
            // wrappedRect.y += EditorDrawingHelper.VerticalSpacing;
            //
            // wrappedRect = DrawRecursively(wrappedRect, wrapper, separatedName, changeValueCallback, label);
            //
            // wrappedRect.height += EditorDrawingHelper.VerticalSpacing;
            //
            // EditorDrawingHelper.DrawRect(new Rect(0f, wrappedRect.y + wrappedRect.height - 1f, wrappedRect.x + wrappedRect.width, 1f), Color.grey);
            //
            // rect.height = wrappedRect.height;
            // return rect;
            return new Control();
        }

        private Control DrawDeletable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            Control control = DrawRecursively(wrapper, deletableName, changeValueCallback, text);

            var deleteButton = new Button
            {
                Icon = deleteIcon,
            };
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

        private Control DrawFoldable(MetadataWrapper wrapper, Action<object> changeValueCallback, string text, bool isPartOfHeader)
        {
            //TODO:
            // if (wrapper.Metadata[foldableName] == null)
            // {
            //     wrapper.Metadata[foldableName] = true;
            //     changeValueCallback(wrapper);
            // }
            //
            // bool oldIsFoldedOutValue = (bool)wrapper.Metadata[foldableName];
            //
            // GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            // {
            //     fontStyle = FontStyle.Bold,
            //     fontSize = 12
            // };
            //
            // GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            // {
            //     fontStyle = FontStyle.Bold,
            //     fontSize = 12
            // };
            //
            // Rect foldoutRect = new Rect(rect.x, rect.y, rect.width, EditorDrawingHelper.HeaderLineHeight);
            //
            // if (isPartOfHeader)
            // {
            //     EditorGUI.DrawRect(new Rect(0, foldoutRect.y, foldoutRect.width + foldoutRect.x + 8, foldoutRect.height), new Color(62f / 256f, 62f / 256f, 62f / 256f));
            //     EditorGUI.DrawRect(new Rect(0, foldoutRect.y, foldoutRect.width + foldoutRect.x + 8, 1), new Color(26f / 256f, 26f / 256f, 26f / 256f));
            //     EditorGUI.DrawRect(new Rect(0, foldoutRect.y + foldoutRect.height, foldoutRect.width + foldoutRect.x + 8, 1), new Color(48f / 256f, 48f / 256f, 48f / 256f));
            // }
            //
            // bool newIsFoldedOutValue = EditorDrawingHelper.DrawFoldoutWithReducedFocusArea(foldoutRect, oldIsFoldedOutValue, oldIsFoldedOutValue ? new GUIContent() : label, foldoutStyle, labelStyle);
            //
            // if (newIsFoldedOutValue != oldIsFoldedOutValue)
            // {
            //     wrapper.Metadata[foldableName] = newIsFoldedOutValue;
            //     changeValueCallback(wrapper);
            // }
            //
            // // Collapsed
            // if (newIsFoldedOutValue == false)
            // {
            //     rect.height = EditorDrawingHelper.HeaderLineHeight;
            //     return rect;
            // }
            //
            // rect.height = 0f;
            //
            // Rect wrappedRect = rect;
            // wrappedRect.x += EditorDrawingHelper.IndentationWidth;
            // wrappedRect.width -= EditorDrawingHelper.IndentationWidth;
            //
            // return DrawRecursively(wrappedRect, wrapper, foldableName, (newWrapper) =>
            // {
            //     // We want the user to be aware that value has changed even if the foldable was collapsed (for example, undo/redo).
            //     wrapper.Metadata[foldableName] = true;
            //     changeValueCallback(wrapper);
            // }, label);
            return new Control();
        }

        private Control DrawIsBlockingToggle(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            IDataOwner? dataOwner = wrapper.Value as IDataOwner;

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
            if (wrapper.Value is IList == false)
            {
                GD.PushWarning("ExtendableListAttribute can be used only with IList members.");
                return new Control();
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
                        ListOfAttribute.Metadata temp = (ListOfAttribute.Metadata)wrapper.Metadata[listOfName];
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
            if (wrapper.Value is IList == false)
            {
                GD.PushWarning("KeepPopulated can be used only with IList members.");
                return new Control();
            }

            IList list = (IList)wrapper.Value;

            if (list.Count == 0)
            {
                Type? entryType = (Type)wrapper.Metadata[keepPopulatedName];
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
                throw new NotImplementedException($"ListOfAttribute attribute should have the lowest priority. Check MetadataWrapperDrawer.Draw method.");
            }

            ListOfAttribute.Metadata wrapperMetadata = (wrapper.Metadata[listOfName] as ListOfAttribute.Metadata);
            List<Dictionary<string, object>> listOfMetadata = wrapperMetadata.ChildMetadata;

            IList list = (IList)wrapper.Value;

            if (listOfMetadata == null)
            {
                listOfMetadata = new List<Dictionary<string, object>>(list.Count);
            }

            if (listOfMetadata.Count != list.Count)
            {
                listOfMetadata.Clear();
                for (int i = 0; i < list.Count; i++)
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
                throw new NotImplementedException($"ReorderableListOfAttribute attribute should have the lowest priority. Check MetadataWrapperDrawer.Draw method.");
            }

            ListOfAttribute.Metadata wrapperMetadata = (wrapper.Metadata[reorderableListOfName] as ListOfAttribute.Metadata);
            List<Dictionary<string, object>> listOfMetadata = wrapperMetadata.ChildMetadata;

            int wrapperCount = ((IList)wrapper.Value).Count;

            if (listOfMetadata == null)
            {
                listOfMetadata = new List<Dictionary<string, object>>(wrapperCount);
            }

            if (listOfMetadata.Count != wrapperCount)
            {
                listOfMetadata.Clear();
                for (int i = 0; i < wrapperCount; i++)
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
            IList wrapperValueList = (IList)wrapper.Value;

            List<MetadataWrapper> listOfWrappers = new List<MetadataWrapper>();
            for (int i = 0; i < wrapperValueList.Count; i++)
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
            IList<MetadataWrapper> listOfWrappers = ConvertListOfMetadataToList(wrapper);

            IProcessDrawer valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);
            IList list = (IList)wrapper.Value;

            return valueDrawer?.Create(listOfWrappers, (newValue) =>
            {
                List<MetadataWrapper> newListOfWrappers = ((List<MetadataWrapper>)newValue).ToList();

                ReflectionUtils.ReplaceList(ref list, newListOfWrappers.Select(childWrapper => childWrapper.Value));
                wrapper.Value = list;

                ((ListOfAttribute.Metadata)wrapper.Metadata[listOfName]).ChildMetadata = newListOfWrappers.Select(childWrapper => childWrapper.Metadata).ToList();
                changeValueCallback(wrapper);
            }, text) ?? new Control();
        }

        private Control DrawReorderableListOf(MetadataWrapper wrapper, Action<object> changeValueCallback, string text)
        {
            IList<MetadataWrapper> listOfWrappers = ConvertReorderableListOfMetadataToList(wrapper);

            IProcessDrawer? valueDrawer = DrawerLocator.GetDrawerForValue(wrapper.Value, wrapper.ValueDeclaredType);
            IList list = (IList)wrapper.Value;

            for (int i = 0; i < listOfWrappers.Count; i++)
            {
                ReorderableElementMetadata metadata = (ReorderableElementMetadata)listOfWrappers[i].Metadata[reorderableName];
                metadata.IsFirst = i == 0;
                metadata.IsLast = i == listOfWrappers.Count - 1;
            }

            return valueDrawer.Create(listOfWrappers, (newValue) =>
            {
                List<MetadataWrapper> newListOfWrappers = ((List<MetadataWrapper>)newValue).ToList();

                for (int i = 0; i < newListOfWrappers.Count; i++)
                {
                    ReorderableElementMetadata metadata = (ReorderableElementMetadata)newListOfWrappers[i].Metadata[reorderableName];

                    if (metadata.MoveDown && metadata.MoveUp == false)
                    {
                        metadata.MoveDown = false;
                        if (i < newListOfWrappers.Count - 1)
                        {
                            (newListOfWrappers[i], newListOfWrappers[i + 1]) = (newListOfWrappers[i + 1], newListOfWrappers[i]);
                        }

                        // Repeat at same index because unprocessed element switched position to i.
                        i--;
                    }
                    else if (metadata.MoveDown == false && metadata.MoveUp)
                    {
                        metadata.MoveUp = false;
                        if (i > 0)
                        {
                            (newListOfWrappers[i], newListOfWrappers[i - 1]) = (newListOfWrappers[i - 1], newListOfWrappers[i]);
                        }
                    }
                    else
                    {
                        // Reset, if both actions are true
                        metadata.MoveDown = false;
                        metadata.MoveUp = false;
                    }
                }

                ReflectionUtils.ReplaceList(ref list, newListOfWrappers.Select(childWrapper => childWrapper.Value));
                wrapper.Value = list;

                ((ListOfAttribute.Metadata)wrapper.Metadata[reorderableListOfName]).ChildMetadata = newListOfWrappers.Select(childWrapper => childWrapper.Metadata).ToList();
                changeValueCallback(wrapper);
            }, text) ?? new Control();
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

                rect = valueDrawer?.Create(wrapper.Value, ValueChanged, text) ?? new Control();
            }

            return rect;
        }

        private Control DrawWrapperRecursively(MetadataWrapper parentWrapper, Action<object> changeValueCallback, string removedMetadataName, string text)
        {
            MetadataWrapper wrappedWrapper = new MetadataWrapper()
            {
                Value = parentWrapper.Value,
                ValueDeclaredType = parentWrapper.ValueDeclaredType,
                Metadata = parentWrapper.Metadata.Where(kvp => kvp.Key != removedMetadataName).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            void WrappedWrapperChanged(object newValue)
            {
                MetadataWrapper newWrapper = (MetadataWrapper)newValue;

                foreach (string key in newWrapper.Metadata.Keys)
                {
                    parentWrapper.Metadata[key] = wrappedWrapper.Metadata[key];
                }

                parentWrapper.Value = newWrapper.Value;

                changeValueCallback(parentWrapper);
            }

            return Create(wrappedWrapper, WrappedWrapperChanged, text) ?? new Control();
        }
    }
}
