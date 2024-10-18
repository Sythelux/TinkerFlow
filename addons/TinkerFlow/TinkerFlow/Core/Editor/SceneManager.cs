using System;
using Godot;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Godot
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [Tool]
    public partial class SceneManager : Node
    {
        public static Action<Node> sceneUnloaded;

        public SceneManager()
        {
            Instance = this;
        }

        public static SceneManager Instance { get; set; } = null!;

        public override void _Ready()
        {
            NodeExtensions.Root = Instance.GetTree().Root;
        }
    }
}