// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System.Runtime.Serialization;
using Godot;
using VRBuilder.Core.Utils.Logging;
using VRBuilder.Unity;

namespace VRBuilder.Core.Behaviors;

/// <summary>
/// Inherit from this abstract class when creating your own behaviors.
/// </summary>
/// <typeparam name="TData">The type of the behavior's data.</typeparam>
[DataContract(IsReference = true)]
public abstract partial class Behavior<TData> : Entity<TData>, IBehavior where TData : class, IBehaviorData, new()
{
    protected Behavior()
    {
        if (LifeCycleLoggingConfig.Instance.LogBehaviors)
            LifeCycle.StageChanged += (sender, args) => { GD.Print("{0}<b>Behavior</b> <i>'{1} ({2})'</i> is <b>{3}</b>.\n", ConsoleUtils.GetTabs(2), Data.Name, GetType().Name, LifeCycle.Stage); };
    }

    #region IBehavior Members

    /// <inheritdoc />
    IBehaviorData IDataOwner<IBehaviorData>.Data => Data;

    /// <inheritdoc />
    public virtual IBehavior Clone()
    {
        return MemberwiseClone() as IBehavior;
    }

    #endregion
}