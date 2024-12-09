#if UNITY_6000_0_OR_NEWER
using VRBuilder.Core.Conditions;

namespace VRBuilder.Core.Editor.ProcessUpgradeTool.Updaters
{
    /// <summary>
    /// Concrete implementation of <see cref="ListUpdater{T}"/>.
    /// </summary>
    public class ConditionListUpdater : ListUpdater<ICondition>
    {
    }
}

#endif
