using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Configuration;

/// <summary>
/// Default single-user implementation of <see cref="ISceneObjectManager"/>.
/// </summary>
public class DefaultSceneObjectManager : ISceneObjectManager
{
    /// <inheritdoc/>
    public void SetSceneObjectActive(ISceneObject sceneObject, bool isActive)
    {
        sceneObject.GameObject.SetProcess(isActive);
    }

    /// <inheritdoc/>
    public void SetComponentActive(ISceneObject sceneObject, string componentTypeName, bool isActive)
    {
        IEnumerable<Node> components = sceneObject.GameObject.GetComponents<Node>().Where(c => c.GetType().Name == componentTypeName);

        foreach (Node component in components)
        {
            Type componentType = component.GetType();

            if (componentType.GetProperty("enabled") != null) componentType.GetProperty("enabled")?.SetValue(component, isActive, null);
        }
    }

    /// <inheritdoc/>
    public Node3D InstantiatePrefab(PackedScene prefab, Vector3 position, Quaternion rotation)
    {
        var instantiatePrefab = prefab.Instantiate<Node3D>();
        instantiatePrefab.Position = position;
        instantiatePrefab.Quaternion = rotation;
        return instantiatePrefab;
    }

    /// <inheritdoc/>
    public void RequestAuthority(ISceneObject sceneObject)
    {
    }
}