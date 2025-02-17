using Godot;
using System;

namespace TinkerFlow.Core.Editor.Util
{
    public partial class ProcessCommand : GodotObject
    {
        public ProcessCommand()
        {
        }

        public ProcessCommand(Action doCallback, Action undoCallback)
        {
            DoCallback = doCallback;
            UndoCallback = undoCallback;
        }

        public Action? UndoCallback { get; init; }
        public Action? DoCallback { get; init; }

        public void Do() => DoCallback?.Invoke();
        public void Undo() => DoCallback?.Invoke();

    }
}
