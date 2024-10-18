#if GODOT
using System.Collections.Generic;
using System.Linq;

namespace VRBuilder.Core.Godot
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    public static class SceneUtils
    {
        //TODO: somehow merge with NodeExtension and or completely refactor to be Godot conform
        public static IEnumerable<T> GetActiveAndInactiveComponents<T>()
        {
            return SceneManager.Instance.GetChildren(true).OfType<T>();
        }
    }
}
#endif