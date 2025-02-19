using Godot;
using System;
using System.Linq;

namespace TinkerFlow.Core.Editor.UI
{
    /// <summary>
    /// source: https://forum.godotengine.org/t/how-to-make-folding-menu/24416/3
    /// </summary>
    [GlobalClass]
    public partial class ExpandableVBoxContainer : VBoxContainer
    {
        [Export]
        public bool Expanded { get; set; }
        private bool initialized = false;
        private STATE state;
        private Vector2 maxSize;
        private Vector2 lastSize;


        public override void _Ready()
        {
            state = STATE.OPEN;
        }

        public void SetToggled(bool toggledOn = false)
        {
            Expanded = !toggledOn;
            state = Expanded ? STATE.OPENING : STATE.CLOSING;
        }

        public void Toggle()
        {
            Expanded = !Expanded;
            state = Expanded ? STATE.OPENING : STATE.CLOSING;
        }

        public override void _Process(double delta)
        {
            if (!initialized)
            {
                maxSize = Size;
                lastSize = maxSize;
                initialized = true;
            }
            switch (state)
            {
                case STATE.CLOSING:
                    SizeFlagsVertical = SizeFlags.ShrinkBegin;
                    if (CustomMinimumSize.Y > 0.001)
                    {
                        CustomMinimumSize = CustomMinimumSize with
                        {
                            Y = 0
                        };
                        lastSize = CustomMinimumSize;
                    }
                    else if (CustomMinimumSize.Y <= 0.001)
                    {
                        SizeFlagsVertical = SizeFlags.Fill;
                        foreach (Control child in GetChildren().OfType<Control>())
                        {
                            child.Visible = Expanded;
                        }
                        state = STATE.CLOSE;
                    }
                    break;
                case STATE.OPENING:
                {
                    SizeFlagsVertical = SizeFlags.ShrinkBegin;
                    foreach (Control child in GetChildren().OfType<Control>())
                    {
                        child.Visible = Expanded;
                    }
                    if (Math.Abs(CustomMinimumSize.Y - maxSize.Y) >= 0.999)
                    {
                        CustomMinimumSize = CustomMinimumSize with
                        {
                            Y = maxSize.Y
                        };
                        lastSize = CustomMinimumSize;
                    }
                    else if (Math.Abs(CustomMinimumSize.Y - maxSize.Y) < 0.001)
                    {
                        SizeFlagsVertical = SizeFlags.Fill;
                        state = STATE.OPEN;
                    }
                    break;
                }
            }
        }

    }

    enum STATE { OPEN, CLOSE, OPENING, CLOSING }
}
