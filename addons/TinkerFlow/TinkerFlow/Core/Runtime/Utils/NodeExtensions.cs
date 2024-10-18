#if GODOT
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VRBuilder.Core.Utils
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    public static class NodeExtensions
    {
        public static Node? Root;

        public static IEnumerable<T> FindObjectsOfType<T>(this Node self) where T : GodotObject
        {
            return FindObjectsOfType<T>();
        }

        public static IEnumerable<T> FindObjectsOfType<T>() where T : GodotObject
        {
            if (Engine.IsEditorHint())
                return EditorInterface.Singleton.GetEditedSceneRoot()?.GetComponentsInChildren<T>() ?? Array.Empty<T>();
            return Root != null
                ? Root.GetComponentsInChildren<T>()
                : Array.Empty<T>();
        }

        public static T? GetComponentInChildren<T>(this Node self)
        {
            return self.GetComponentsInChildren<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetComponentsInChildren<T>(this Node self)
        {
            return self.GetChildren(true).OfType<T>();
        }

        public static T? GetComponent<T>(this Node self)
        {
            return self.GetComponents<T>().FirstOrDefault();
        }

        public static T? GetComponent<T>(this Node self, T child)
        {
            return self.GetComponents<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetComponents<T>(this Node self)
        {
            return self.GetChildren().OfType<T>();
        }

        public static T AddComponent<T>(this Node self) where T : Node, new()
        {
            var node = new T();
            self.AddChild(node);
            return node;
        }

        public static T AddComponent<T>(this Node self, T component)
        {
            if (component is Node node)
                self.AddChild(node);
            return component;
        }
    }
}
#endif