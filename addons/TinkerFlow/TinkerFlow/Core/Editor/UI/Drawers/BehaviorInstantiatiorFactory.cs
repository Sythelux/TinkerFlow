#if UNITY_5_3_OR_NEWER
using UnityEditor
// // Copyright (c) 2013-2019 Innoactive GmbH
// // Licensed under the Apache License, Version 2.0
// // Modifications copyright (c) 2021-2024 MindPort GmbH
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Godot;
// using VRBuilder.Core.Behaviors;
// using VRBuilder.Editor.Configuration;
//
// namespace VRBuilder.Editor.UI.Drawers
// {
//     /// <summary>
//     /// Draws a dropdown button with all <see cref="InstantiationOption{IBehavior}"/> in the project, and creates a new instance of choosen behavior on click.
//     /// </summary>
//     [InstantiatorProcessDrawer(typeof(IBehavior))]
//     internal partial class BehaviorInstantiatiorFactory : AbstractInstantiatorFactory<IBehavior>
//     {
//         /// <inheritdoc />
//         public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
//         {
//             EditorGUI.BeginDisabledGroup(EditorConfigurator.Instance.AllowedMenuItemsSettings.GetBehaviorMenuOptions().Any() == false);
//             if (EditorDrawingHelper.DrawAddButton(ref rect, "Add Behavior"))
//             {
//                 IList<TestableEditorElements.MenuOption> options = ConvertFromConfigurationOptionsToGenericMenuOptions(EditorConfigurator.Instance.BehaviorsMenuContent.ToList(), currentValue, changeValueCallback);
//                 TestableEditorElements.DisplayContextMenu(options);
//             }
//
//             EditorGUI.EndDisabledGroup();
//
//             if (EditorDrawingHelper.DrawHelpButton(ref rect))
//             {
//                 Application.OpenURL("https://www.mindport.co/vr-builder/manual/default-behaviors");
//             }
//
//             if (EditorConfigurator.Instance.AllowedMenuItemsSettings.GetBehaviorMenuOptions().Any() == false)
//             {
//                 rect.y += rect.height + EditorDrawingHelper.VerticalSpacing;
//                 rect.width -= EditorDrawingHelper.IndentationWidth;
//                 EditorGUI.HelpBox(rect, "Your project does not contain any Behaviors. Either create one or import a VR Builder Component.", MessageType.Error);
//                 rect.height += rect.height + EditorDrawingHelper.VerticalSpacing;
//             }
//
//             return rect;
//         }
//     }
// }


#elif GODOT
using Godot;
//TODO
#endif
