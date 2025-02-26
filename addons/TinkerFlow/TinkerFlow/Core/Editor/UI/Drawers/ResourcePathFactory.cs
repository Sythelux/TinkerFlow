using System;
using System.Reflection;
using Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Draws an object field and sets the Resources path of the dragged object.
    /// </summary>
    public abstract class ResourcePathFactory<T> : AbstractProcessFactory where T : Resource
    {
        public override Control? Create<T1>(T1 currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print(
                $"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (currentValue is not string oldPath)
                return new Control { Name = GetType().Name };

            var editorResourcePicker = new EditorResourcePicker();
            editorResourcePicker.BaseType = "Resource";
            editorResourcePicker.EditedResource = ResourceLoader.Load<Resource>(oldPath);
            editorResourcePicker.ResourceChanged += EditorResourcePickerOnResourceChanged;

            return editorResourcePicker;

            void EditorResourcePickerOnResourceChanged(Resource resource)
            {
                var newPath = resource?.ResourcePath;
                if (string.IsNullOrEmpty(newPath) == false)
                {
                    /*if (newPath.Contains("Resources"))
                    {
                        newPath = newPath.Remove(0, newPath.IndexOf("Resources", StringComparison.Ordinal) + 10);
                    }
                    else
                    {
                        GD.PushError("The object is not in the path of a 'Resources' folder.");
                        newPath = "";
                    }*/

                    if (newPath.Contains('.'))
                    {
                        newPath = newPath.Remove(newPath.LastIndexOf('.'));
                    }
                }

                if (oldPath != newPath)
                {
                    ChangeValue(() => newPath, () => oldPath, changeValueCallback);
                }
            }
        }
    }
}