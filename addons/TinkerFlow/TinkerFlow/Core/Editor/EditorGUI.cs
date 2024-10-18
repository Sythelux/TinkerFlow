using System;
using Godot;
using TinkerFlow.Godot.Editor;

namespace VRBuilder.Editor.Godot
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    public static class EditorGUI
    {
        public static PackedScene objectDrawerPrefab = GD.Load<PackedScene>(TinkerFlowPlugin.ResourcePath("Prefabs/ObjectDrawer.tscn"));

        /// <summary>
        ///   <para>User message types.</para>
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            ///   <para>Neutral message.</para>
            /// </summary>
            None,

            /// <summary>
            ///   <para>Info message.</para>
            /// </summary>
            Info,

            /// <summary>
            ///   <para>Warning message.</para>
            /// </summary>
            Warning,

            /// <summary>
            ///   <para>Error message.</para>
            /// </summary>
            Error,
        }

        public static ObjectDrawer ObjectField(Label label, Node obj, Type objType, bool allowSceneObjects)
        {
            var od = objectDrawerPrefab.Instantiate<ObjectDrawer>();
            od.AllowSceneObjects = allowSceneObjects;

            // https://docs.godotengine.org/en/latest/classes/class_control.html#class-control-private-method-get-drag-data
            return od;
        }

        public static Control HelpBox(string message, MessageType type)
        {
            var richTextLabel = new RichTextLabel();
            richTextLabel.Text = type switch
            {
                MessageType.None => $"{message}",
                MessageType.Info => $"[color=white]{message}[/color]",
                MessageType.Warning => $"[color=yellow]{message}[/color]",
                MessageType.Error => $"[color=red]{message}[/color]",
                _ => $"{message}"
            };
            return richTextLabel;
        }
    }
}