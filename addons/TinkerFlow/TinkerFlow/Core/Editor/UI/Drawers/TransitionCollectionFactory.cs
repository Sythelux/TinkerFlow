// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using Godot;
using System.Reflection;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    [DefaultProcessDrawer(typeof(TransitionCollection))]
    internal class TransitionCollectionFactory : DataOwnerFactory;
}
