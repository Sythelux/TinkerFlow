using System.IO;
using System.Reflection;
using Godot;

namespace TinkerFlow.Godot.Editor
{
    [Tool]
    public partial class TinkerFlowPlugin : EditorPlugin
    {
        private static string? defaultBasePath; // "res://addons/TinkerFlow/"

        // private MyInspectorPlugin _plugin;
        public ProcessEditor? ProcessEditor { get; set; }
        public StepWindow? ProcessInspector { get; set; }
        public static TinkerFlowPlugin? Instance { get; private set; }

        public override void _EnterTree()
        {
            Instance = this;
            ProcessInspector = GD.Load<PackedScene>(ResourcePath("Scenes/ProcessInspector.tscn"))
                .Instantiate<StepWindow>();
            AddControlToDock(DockSlot.LeftUl, ProcessInspector);

            ProcessEditor = ResourceLoader.Load<PackedScene>(ResourcePath("Scenes/ProcessEditor.tscn"))
                .Instantiate<ProcessEditor>();
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
            // const string DefaultAddonBasePath = "res://addons/TinkerFlowPlugin/";
            defaultBasePath ??= typeof(TinkerFlowPlugin).GetCustomAttribute<ScriptPathAttribute>()?.Path
                .Replace($"{nameof(TinkerFlowPlugin)}.cs", "");
            return Path.Join(defaultBasePath, subPath);
        }
    }
}