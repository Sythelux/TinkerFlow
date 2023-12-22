using System.Runtime.Serialization;
using Godot;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;

namespace VRBuilder.Core.Conditions;

/// <summary>
/// A condition that completes when a certain amount of time has passed.
/// </summary>
[DataContract(IsReference = true)]
[HelpLink("https://www.mindport.co/vr-builder/manual/default-conditions/timeout-condition")]
public partial class TimeoutCondition : Condition<TimeoutCondition.EntityData>
{
    [JsonConstructor] //Preserve
    public TimeoutCondition() : this(0)
    {
    }

    public TimeoutCondition(float timeout)
    {
        Data.Timeout = timeout;
    }

    /// <inheritdoc />
    public override IStageProcess GetActiveProcess()
    {
        return new ActiveProcess(Data);
    }

    #region Nested type: ActiveProcess

    private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
    {
        private float timeStarted;

        public ActiveProcess(EntityData data) : base(data)
        {
        }

        /// <inheritdoc />
        protected override bool CheckIfCompleted()
        {
            return Time.GetTicksMsec() - timeStarted >= Data.Timeout;
        }

        /// <inheritdoc />
        public override void Start()
        {
            timeStarted = Time.GetTicksMsec();
            base.Start();
        }
    }

    #endregion

    #region Nested type: EntityData

    /// <summary>
    /// The data for timeout condition.
    /// </summary>
    [DisplayName("Timeout")]
    public class EntityData : IConditionData
    {
        /// <summary>
        /// The delay before the condition completes.
        /// </summary>
        [DataMember]
        [DisplayName("Wait (in seconds)")]
        public float Timeout { get; set; }

        #region IConditionData Members

        /// <inheritdoc />
        public bool IsCompleted { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        [HideInProcessInspector]
        public string Name => $"Complete after {Timeout.ToString()} seconds";

        /// <inheritdoc />
        public IMetadata Metadata { get; set; }

        #endregion
    }

    #endregion
}