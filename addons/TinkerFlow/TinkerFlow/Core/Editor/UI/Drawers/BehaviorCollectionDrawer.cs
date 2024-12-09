// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

#if UNITY_5_3_OR_NEWER

using System;
using System.Reflection;
using VRBuilder.Core;
namespace VRBuilder.Core.Editor.UI.Drawers
{
    [DefaultProcessDrawer(typeof(BehaviorCollection))]
    internal class BehaviorCollectionDrawer : DataOwnerFactory
    {
        public override GUIContent GetLabel(MemberInfo memberInfo, object memberOwner)
        {
            return null;
        }

        public override GUIContent GetLabel(object value, Type declaredType)
        {
            return null;
        }
    }
}
#endif
