using Godot;

namespace VRBuilder.Core.Editor.Util
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    public class Application
    {
        public static string dataPath = "user://";
        public static string streamingAssetsPath = "res://StreamingAssets";

        /// <summary>
        /// TODO: after the port is done it can be inlined with only OS.ShellOpen();
        /// </summary>
        /// <param name="helpLink"></param>
        public static void OpenURL(string helpLink)
        {
            OS.ShellOpen(helpLink);
        }
    }
}
