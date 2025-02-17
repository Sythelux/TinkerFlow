using Godot;
using Godot.Collections;
using System.IO;
using System.Reflection;
using VRBuilder.Core.Editor;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.UI.Windows;

namespace TinkerFlow.Godot.Editor
{
    [Tool]
    public partial class TinkerFlowPlugin : EditorPlugin
    {
        private static readonly Dictionary<string, Texture2D> iconCache = new Dictionary<string, Texture2D>();
        private static string? defaultBasePath; // should be: "res://addons/TinkerFlow/TinkerFlow/Core/"

        // private MyInspectorPlugin _plugin;
        public ProcessEditor? ProcessEditor { get; set; }
        public StepWindow? ProcessInspector { get; set; }
        public static TinkerFlowPlugin? Instance { get; private set; }

        public override void _EnterTree()
        {
            Instance = this;
            ProcessInspector = GD.Load<PackedScene>(ResourcePath("Scenes/ProcessInspector.tscn")).Instantiate<StepWindow>();
            AddControlToDock(DockSlot.LeftUl, ProcessInspector);

            ProcessEditor = ResourceLoader.Load<PackedScene>(ResourcePath("Scenes/ProcessEditor.tscn")).Instantiate<ProcessEditor>();
            if (ProcessEditor != null)
            {
                EditorInterface.Singleton.GetEditorMainScreen().AddChild(ProcessEditor);
                var baseColor = EditorInterface.Singleton.Get("interface/theme/base_color").As<Color>();
                ProcessEditor.BaseColor = baseColor.V < 0.5 ? Colors.White : Colors.Black;
                if (ProcessInspector != null)
                {
                    ProcessEditor.StepSelected += ProcessInspector.OnStepSelected;
                    ProcessEditor.StepDeselected += ProcessInspector.OnStepDeselected;
                }
            }

            EditorReflectionUtils.OnScriptsReload();
            AllowedMenuItemsSettings.Instance.UpdateWithAllBehaviorsAndConditionsInProject();

            _MakeVisible(false);
            // _plugin = new MyInspectorPlugin();
            // AddInspectorPlugin(_plugin);
            // AddCustomType("ProcessEditor", "Panel", ResourceLoader.Load<CSharpScript>(""), new ImageTexture());
        }

        public override string _GetPluginName()
        {
            return "Tinker Flow";
        }

        public override Texture2D _GetPluginIcon()
        {
            return ResourceLoader.Load<Texture2D>(ResourcePath("Resources/TinkerFlow.svg"));
        }

        public override bool _HasMainScreen()
        {
            return true;
        }

        public override void _MakeVisible(bool visible)
        {
            if (ProcessEditor != null) ProcessEditor.Visible = visible;
        }

        public override void _SaveExternalData()
        {
            ProcessEditor?.Save();
        }

        public override void _ExitTree()
        {
            // RemoveInspectorPlugin(_plugin);
            ProcessEditor?.QueueFree();
            RemoveControlFromDocks(ProcessInspector);
            ProcessInspector?.Free();
        }

        public static string ResourcePath(string subPath)
        {
            defaultBasePath ??= typeof(TinkerFlowPlugin).GetCustomAttribute<ScriptPathAttribute>()?.Path
                .Replace($"{nameof(TinkerFlowPlugin)}.cs", "");
            return Path.Join(defaultBasePath, subPath);
        }

        public static Texture2D GetIcon(string iconName)
        {
            if (iconCache.TryGetValue(iconName, out var texture))
                return texture;

            if (EditorInterface.Singleton is EditorInterface editorInterface)
            {
                Theme? theme = editorInterface.GetEditorTheme();
                foreach (string themeType in theme.GetIconTypeList())
                    if (theme.HasIcon(iconName, themeType))
                        return theme.GetIcon(iconName, themeType);
            }

            if (ResourceLoader.Exists(ResourcePath(iconName)))
                return ResourceLoader.Load<Texture2D>(ResourcePath(iconName));
            const string vrBuilderLiteResourcePath = "../VR-Builder-Lite/Source/Core/Resources";
            if (ResourceLoader.Exists(ResourcePath(Path.Join(vrBuilderLiteResourcePath, iconName))))
                return ResourceLoader.Load<Texture2D>(ResourcePath(Path.Join(vrBuilderLiteResourcePath, iconName)));
            foreach (string suffix in new[] { "png", "jpg", "gif", "svg" })
            {
                string fullName = string.Join('.', iconName, suffix);
                if (ResourceLoader.Exists(ResourcePath(fullName)))
                    return ResourceLoader.Load<Texture2D>(ResourcePath(fullName));
                if (ResourceLoader.Exists(ResourcePath(Path.Join(vrBuilderLiteResourcePath, fullName))))
                    return ResourceLoader.Load<Texture2D>(ResourcePath(Path.Join(vrBuilderLiteResourcePath, fullName)));
            }
            GD.PushWarning($"icon not found{iconName}");
            return new Texture2D();
        }
    }
}
