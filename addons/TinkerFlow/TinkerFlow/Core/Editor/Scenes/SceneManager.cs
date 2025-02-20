using Godot;
using System;
using System.Threading.Tasks;
using VRBuilder.Core.Utils;

namespace TinkerFlow.Core.Editor.Scenes
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [Tool]
    public partial class SceneManager : Node
    {
        public static int sceneCountInBuildSettings;
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
        public static async Task<Node> LoadSceneAsync(int sceneIndex, LoadSceneMode loadSceneMode)
        {
            GD.PushError(new NotImplementedException());
            return await Task.FromResult(new Node());
        }

        public static void LoadScene(int sceneIndex, LoadSceneMode loadSceneMode)
        {
            GD.PushError(new NotImplementedException());
        }
    }
}
