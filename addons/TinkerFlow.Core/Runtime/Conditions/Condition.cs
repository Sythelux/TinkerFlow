// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using VRBuilder.Core.RestrictiveEnvironment;
using VRBuilder.Core.Utils.Logging;
using VRBuilder.Unity;

namespace VRBuilder.Core.Conditions;

/// <summary>
/// An implementation of <see cref="ICondition"/>. Use it as the base class for your custom conditions.
/// </summary>
[DataContract(IsReference = true)]
public abstract partial class Condition<TData> : CompletableEntity<TData>, ICondition, ILockablePropertiesProvider where TData : class, IConditionData, new()
{
    protected Condition()
    {
        if (LifeCycleLoggingConfig.Instance.LogConditions)
            LifeCycle.StageChanged += (sender, args) => { GD.Print("{0}<b>Condition</b> <i>'{1} ({2})'</i> is <b>{3}</b>.\n", ConsoleUtils.GetTabs(2), Data.Name, GetType().Name, LifeCycle.Stage); };
    }

    #region ICondition Members

    /// <inheritdoc />
    IConditionData IDataOwner<IConditionData>.Data => Data;

    /// <inheritdoc />
    public virtual ICondition Clone()
    {
        return MemberwiseClone() as ICondition;
    }

    #endregion

    #region ILockablePropertiesProvider Members

    /// <inheritdoc />
    public virtual IEnumerable<LockablePropertyData> GetLockableProperties()
    {
        return PropertyReflectionHelper.ExtractLockablePropertiesFromConditions(Data)
            .Union(PropertyReflectionHelper.ExtractLockablePropertiesFromConditionTags(Data));
    }

    #endregion
}