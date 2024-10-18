#if UNITY_5_3_OR_NEWER
using UnityEditor
// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;

namespace VRBuilder.Editor.UI.Drawers
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class InstantiatorProcessDrawerAttribute : Attribute
    {
        public Type Type { get; private set; }

        public InstantiatorProcessDrawerAttribute(Type type)
        {
            Type = type;
        }
    }
}

#elif GODOT
using Godot;
//TODO
#endif
