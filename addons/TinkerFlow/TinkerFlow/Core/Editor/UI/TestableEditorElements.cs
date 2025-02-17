// // Copyright (c) 2013-2019 Innoactive GmbH
// // Licensed under the Apache License, Version 2.0
// // Modifications copyright (c) 2021-2024 MindPort GmbH
//
// using System;
// using System.Collections.Generic;

using Godot;
using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Editor
{
    public static class TestableEditorElements
    {
        public enum DisplayMode
        {
            Normal,
            Recording,
            Playback
        }

        public class MenuOption
        {
            public Label Label { get; private set; }

            protected MenuOption(Label label)
            {
                Label = label;
            }
        }

        public sealed class DisabledMenuItem : MenuOption
        {
            public DisabledMenuItem(Label label) : base(label)
            {
            }
        }

        public sealed class MenuItem : MenuOption
        {
            public Action Func { get; private set; }
            public Action<object> Func2 { get; private set; }
            public bool On { get; private set; }
            public object UserData { get; private set; }

            public MenuItem(Label label, bool on, Action func) : base(label)
            {
                On = on;
                Func = func;
            }

            public MenuItem(Label label, bool on, Action<object> func, object userData) : base(label)
            {
                On = on;
                Func2 = func;
                UserData = userData;
            }
        }

        public sealed class MenuSeparator : MenuOption
        {
            public MenuSeparator(string pathToSubmenu) : base(new Label { Text = pathToSubmenu })
            {
            }
        }

//
//         private static readonly Queue<string> recordedSelections = new Queue<string>();
        private static readonly Queue<string> prepickedSelections = new Queue<string>();
//
        public static DisplayMode Mode { get; private set; }
//
//         public static void StartRecording()
//         {
//             AssertDisplayMode(DisplayMode.Normal);
//
//             Mode = DisplayMode.Recording;
//             recordedSelections.Clear();
//         }
//
//         public static List<string> StopRecording()
//         {
//             AssertDisplayMode(DisplayMode.Recording);
//
//             List<string> result = recordedSelections.ToList();
//             recordedSelections.Clear();
//             Mode = DisplayMode.Normal;
//             return result;
//         }
//
//         public static void StartPlayback(List<string> selections)
//         {
//             AssertDisplayMode(DisplayMode.Normal);
//
//             prepickedSelections.Clear();
//             foreach (string selection in selections)
//             {
//                 prepickedSelections.Enqueue(selection);
//             }
//
//             Mode = DisplayMode.Playback;
//         }
//
//         public static void StopPlayback()
//         {
//             AssertDisplayMode(DisplayMode.Playback);
//
//             prepickedSelections.Clear();
//             Mode = DisplayMode.Normal;
//         }
//
        public static PopupMenu DisplayContextMenu(IList<MenuOption> options)
        {
            var mousePos = EditorInterface.Singleton.GetEditorViewport2D().GetMousePosition();
            return DisplayDropdownMenu(mousePos, options);
        }
//
        public static PopupMenu DisplayDropdownMenu(Vector2 position, IList<MenuOption> options)
        {
            PopupMenu menu = new PopupMenu();

            if (Mode == DisplayMode.Playback)
            {
                if (prepickedSelections.Count == 0)
                {
                    return menu;
                }

                int index;
                if (int.TryParse(prepickedSelections.Dequeue(), out index) == false)
                {
                    return menu;
                }

                if (index == -1)
                {
                    return menu;
                }

                MenuItem item = options[index] as MenuItem;
                if (item == null)
                {
                    return menu;
                }

                if (item.Func != null)
                {
                    item.Func();
                }
                else if (item.Func2 != null)
                {
                    item.Func2(item.UserData);
                }

                return menu;
            }


            for (int i = 0; i < options.Count; i++)
            {
                MenuOption closuredOption = options[i];

                if (closuredOption is MenuSeparator)
                {
                    menu.AddSeparator(closuredOption.Label.Text);
                }
                else if (closuredOption is DisabledMenuItem)
                {
                    menu.AddItem(closuredOption.Label.Text);
                    menu.SetItemDisabled(menu.ItemCount - 1, true);
                }
                else
                {
                    MenuItem item = closuredOption as MenuItem;

                    menu.AddItem(closuredOption.Label.Text, i);
                    int i1 = i;

                    if (item.Func2 != null)
                    {
                        menu.IndexPressed += index =>
                        {
                            if (i1 == index) item.Func2(item.UserData);
                        };
                    }
                    else
                    {
                        menu.IndexPressed += index =>
                        {
                            if (i1 == index) item.Func();
                        };
                    }

                    // GenericMenu.MenuFunction finalCallback = new GenericMenu.MenuFunction(itemCallback);
                    //
                    // if (Mode == DisplayMode.Recording)
                    // {
                    //     int closuredIndex = i;
                    //     finalCallback = () =>
                    //     {
                    //         recordedSelections.Enqueue(closuredIndex.ToString());
                    //         itemCallback();
                    //     };
                    // }
                    // menu.AddItem(closuredOption.Label.Text, item.On, finalCallback);
                }
            }
            // menu.DropDown(position);
            return menu;
        }
//
//         public static void ClearProgressBar()
//         {
//             EditorUtility.ClearProgressBar();
//         }
//
//         /// <summary>
//         /// If you want to be able to test your editor windows, you should use EditorUtils instead of EditorUtility.
//         /// If EditorUtils.Mode is set to Recording, it saves the user input.
//         /// If EditorUtils.Mode is set to Playback, it returns a stored value instead of actually calling its EditorUtility counterpart.
//         /// IF EditorUtils.Mode is set to Normal, it just calls its EditorUtility counterpart.
//         /// </summary>
//         public static bool DisplayCancelableProgressBar(string title, string info, float progress)
//         {
//             return DisplayBoolPicker(() => EditorUtility.DisplayCancelableProgressBar(title, info, progress));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static bool DisplayDialog(string title, string message, string ok, string cancel = "")
//         {
//             return DisplayBoolPicker(() => EditorUtility.DisplayDialog(title, message, ok, cancel));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt)
//         {
//             return DisplayIntPicker(() => EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static void DisplayProgressBar(string title, string info, float progress)
//         {
//             EditorUtility.DisplayProgressBar(title, info, progress);
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string OpenFilePanel(string title, string directory, string extension)
//         {
//             return DisplayStringPicker(() => EditorUtility.OpenFilePanel(title, directory, extension));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string OpenFilePanelWithFilters(string title, string directory, string[] filters)
//         {
//             return DisplayStringPicker(() => EditorUtility.OpenFilePanelWithFilters(title, directory, filters));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string OpenFolderPanel(string title, string folder, string defaultName)
//         {
//             return DisplayStringPicker(() => EditorUtility.OpenFolderPanel(title, folder, defaultName));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string SaveFilePanel(string title, string directory, string defaultName, string extension)
//         {
//             return DisplayStringPicker(() => EditorUtility.SaveFilePanel(title, directory, defaultName, extension));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message)
//         {
//             return DisplayStringPicker(() => EditorUtility.SaveFilePanelInProject(title, defaultName, extension, message));
//         }
//
//         /// <summary>
//         /// <see cref="DisplayCancelableProgressBar"/>
//         /// </summary>
//         public static string SaveFolderPanel(string title, string folder, string defaultName)
//         {
//             return DisplayStringPicker(() => EditorUtility.SaveFolderPanel(title, folder, defaultName));
//         }
//
//         private static void AssertDisplayMode(DisplayMode expected)
//         {
//             if (Mode != expected)
//             {
//                 throw new InvalidStateException(string.Format("{0} was expected to be in {1} mode, but was in {2}.", typeof(TestableEditorElements).Name, Mode, expected));
//             }
//         }
//
//         private static int DisplayIntPicker(Func<int> stuff)
//         {
//             return DisplayPicker(obj => recordedSelections.Enqueue(obj.ToString()), () => int.Parse(prepickedSelections.Dequeue()), stuff);
//         }
//
//         private static bool DisplayBoolPicker(Func<bool> stuff)
//         {
//             return DisplayPicker(obj => recordedSelections.Enqueue(obj.ToString()), () => bool.Parse(prepickedSelections.Dequeue()), stuff);
//         }
//
//         private static string DisplayStringPicker(Func<string> stuff)
//         {
//             return DisplayPicker(obj => recordedSelections.Enqueue(obj), () => prepickedSelections.Dequeue(), stuff);
//         }
//
//         private static T DisplayPicker<T>(Action<T> record, Func<T> prepick, Func<T> draw)
//         {
//             if (Mode == DisplayMode.Playback)
//             {
//                 return prepick();
//             }
//
//             T result = draw();
//             if (Mode == DisplayMode.Recording)
//             {
//                 record(result);
//             }
//
//             return result;
//         }
//
//         /// <summary>
//         /// Stop recording/replaying user actions and clear the stack.
//         /// </summary>
//         public static void Panic()
//         {
//             recordedSelections.Clear();
//             prepickedSelections.Clear();
//             Mode = DisplayMode.Normal;
//         }
    }
}
