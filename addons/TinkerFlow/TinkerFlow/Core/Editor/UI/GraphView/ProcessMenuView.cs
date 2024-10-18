using System;
using Godot;
using VRBuilder.Core;

namespace TinkerFlow.Godot.Editor
{
    public partial class ProcessMenuView : Resource
    {
        private int activeChapter;
        protected IProcess Process { get; private set; }
        public ProcessGraph ParentWindow { get; set; }

        #region Events

        [Signal]
        public delegate void ChapterChangedEventHandler(Chapter chapterRid); //was IChapter chapter. IChapter can't be passed and I need a unique identifier of it. Rid might be wrong

        [Signal]
        public delegate void MenuExtendedChangedEventHandler(bool isExtended);

        [Signal]
        public delegate void RefreshRequestedEventHandler();

        #endregion

        public void Initialise(IProcess process, ProcessGraph parent)
        {
            Process = process;
            ParentWindow = parent;

            activeChapter = 0;
        }

        private void EmitChapterChanged()
        {
            // if (ChapterChanged != null)
            // {
            //     ChapterChanged.Invoke(this, new ChapterChangedEventArgs(CurrentChapter));
            // }
        }
    }
}