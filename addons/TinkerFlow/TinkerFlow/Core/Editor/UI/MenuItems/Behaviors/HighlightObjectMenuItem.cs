#if UNITY_6000_0_OR_NEWER
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Core.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class HighlightObjectMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Guidance/Highlight Objects";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new HighlightObjectBehavior();
        }
    }
}

#endif
