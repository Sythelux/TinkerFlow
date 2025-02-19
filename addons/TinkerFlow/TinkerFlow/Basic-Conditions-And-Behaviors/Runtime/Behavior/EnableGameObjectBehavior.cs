using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Behaviors;

/// <summary>
/// Enables gameObject of target ISceneObject.
/// </summary>
[DataContract(IsReference = true)]
[HelpLink("https://www.mindport.co/vr-builder/manual/default-behaviors/enable-object")]
public partial class EnableGameObjectBehavior : Behavior<EnableGameObjectBehavior.EntityData>
{
    [JsonConstructor] //Preserve
    public EnableGameObjectBehavior() : this("")
    {
    }

    /// <param name="targetObject">Object to enable.</param>
    public EnableGameObjectBehavior(ISceneObject targetObject) : this(ProcessReferenceUtils.GetNameFrom(targetObject))
    {
    }

    /// <param name="targetObject">Name of the object to enable.</param>
    public EnableGameObjectBehavior(string targetObject)
    {
        Data.Target = new SceneObjectReference(targetObject);
    }

    /// <inheritdoc />
    public override IStageProcess GetActivatingProcess()
    {
        return new ActivatingProcess(Data);
    }

    public override IStageProcess GetDeactivatingProcess()
    {
        return new DeactivatingProcess(Data);
    }

    #region Nested type: ActivatingProcess

    private class ActivatingProcess : InstantProcess<EntityData>
    {
        public ActivatingProcess(EntityData data) : base(data)
        {
        }

        /// <inheritdoc />
        public override void Start()
        {
            RuntimeConfigurator.Configuration.SceneObjectManager.SetSceneObjectActive(Data.Target.Value, true);
        }
    }

    #endregion

    #region Nested type: DeactivatingProcess

    private class DeactivatingProcess : InstantProcess<EntityData>
    {
        public DeactivatingProcess(EntityData data) : base(data)
        {
        }

        /// <inheritdoc />
        public override void Start()
        {
            if (Data.DisableOnDeactivating) RuntimeConfigurator.Configuration.SceneObjectManager.SetSceneObjectActive(Data.Target.Value, false);
        }
    }

    #endregion

    #region Nested type: EntityData

    /// <summary>
    /// "Enable game object" behavior's data.
    /// </summary>
    [DisplayName("Enable Object")]
    [DataContract(IsReference = true)]
    public class EntityData : IBehaviorData
    {
        /// <summary>
        /// The object to enable.
        /// </summary>
        [DataMember]
        [DisplayName("Object")]
        public SceneObjectReference? Target { get; set; }

        [DataMember]
        [DisplayName("Disable Object after step is complete")]
        public bool DisableOnDeactivating { get; set; }

        #region IBehaviorData Members

        /// <inheritdoc />
        public Metadata Metadata { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public string Name
        {
            get
            {
                try
                {
                    string target = Target?.IsEmpty() == false ? Target.Value?.GameObject.Name : "[NULL]";
                    return $"Enable {target}";
                }
                catch
                {
                    return "[Disposed]";
                }
            }
        }

        #endregion
    }

    #endregion
}