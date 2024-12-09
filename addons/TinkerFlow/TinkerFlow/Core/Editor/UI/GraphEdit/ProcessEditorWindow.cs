using Godot;
using VRBuilder.Core.Editor;
using VRBuilder.Core.Editor.UI.GraphView.Windows;

namespace VRBuilder.Core.Editor.UI.Windows
{
    /// <summary>
    /// Base class for a process GUI editor window.
    /// </summary>
    public abstract partial class ProcessEditorWindow : GraphEdit, IProcessEditorWindow
    {
        /// <summary>
        /// Sets the process to be displayed.
        /// </summary>        
        internal abstract void SetProcess(IProcess? currentProcess);

        /// <summary>
        /// Gets the current chapter.
        /// </summary>        
        internal abstract IChapter? GetChapter();

        /// <summary>
        /// Switches to the specified chapter.
        /// </summary>        
        internal abstract void SetChapter(IChapter chapter);

        /// <summary>
        /// Updates the chapter view.
        /// </summary>
        internal abstract void RefreshChapterRepresentation();
    }
}
