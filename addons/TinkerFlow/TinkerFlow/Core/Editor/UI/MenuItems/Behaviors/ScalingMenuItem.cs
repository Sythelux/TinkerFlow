#if UNITY_6000_0_OR_NEWER
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Core.Editor.UI.MenuItems.Behaviors
{
    public class ScalingMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Scale Object";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new ScalingBehavior();
        }
    }
}

#endif
