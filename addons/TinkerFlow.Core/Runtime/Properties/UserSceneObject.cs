// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System.Diagnostics;
using Godot;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Properties;

/// <summary>
/// Used to identify the user within the scene.
/// </summary>
public partial class UserSceneObject : ProcessSceneObject
{
    [SerializeField]
    private Node3D? head;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private Transform3D? leftHand, rightHand;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    /// <summary>
    /// Returns the user's head transform.
    /// </summary>
    public Node3D Head
    {
        get
        {
            return head ??= this.GetComponentInChildren<Camera3D>();
            //TODO: Debug.LogWarning("User head object is not referenced on User Scene Object component. The rig's camera will be used, if available.");
        }
    }

    /// <summary>
    /// Returns the user's left hand transform.
    /// </summary>
    public Transform3D? LeftHand => leftHand;

    /// <summary>
    /// Returns the user's right hand transform.
    /// </summary>
    public Transform3D? RightHand => rightHand;

    public override void _Ready()
    {
        base._Ready();
        uniqueName = "User";
    }
}