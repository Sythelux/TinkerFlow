#if UNITY_5_3_OR_NEWER
using UnityEditor
using UnityEngine.Video;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Implementation of asset path drawer for <see cref="VideoClip"/> assets.
    /// </summary>
    public class VideoClipResourceDrawer : ResourcePathDrawer<VideoClip>
    {
    }
}

#elif GODOT
using Godot;
//TODO
#endif
