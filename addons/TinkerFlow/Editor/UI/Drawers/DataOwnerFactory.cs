﻿// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Linq;
using Godot;
using VRBuilder.Core;

namespace VRBuilder.Editor.UI.Drawers;

[DefaultProcessDrawer(typeof(IDataOwner))]
internal class DataOwnerFactory : AbstractProcessFactory
{
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, Control label)
    {
        if (currentValue == null) throw new NullReferenceException("Attempting to draw null object.");

        IData? data = (currentValue as IDataOwner)?.Data;
        IProcessFactory dataDrawer = DrawerLocator.GetDrawerForMember(EditorReflectionUtils.GetFieldsAndPropertiesToDraw(currentValue).First(member => member.Name == "Data"), currentValue);

        return dataDrawer.Create(data, _ => changeValueCallback(currentValue), label);
    }

    public override Label GetLabel<T>(T value)
    {
        IData? data = (value as IDataOwner)?.Data;

        if (value != null)
        {
            IProcessFactory dataDrawer = DrawerLocator.GetDrawerForMember(EditorReflectionUtils.GetFieldsAndPropertiesToDraw(value).First(member => member.Name == "Data"), value);
            return data != null
                ? dataDrawer.GetLabel(data)
                : new Label { Text = $"{nameof(T)}.Data is null" };
        }

        return new Label { Text = $"{nameof(T)} is null" };
    }
}