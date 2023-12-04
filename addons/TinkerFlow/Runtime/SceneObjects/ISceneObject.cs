// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using Godot;
using VRBuilder.Core.Properties;

namespace VRBuilder.Core.SceneObjects
{
    public class SceneObjectNameChanged : EventArgs
    {
        public readonly string NewName;
        public readonly string PreviousName;

        public SceneObjectNameChanged(string newName, string previousName)
        {
            NewName = newName;
            PreviousName = previousName;
        }
    }

    /// <summary>
    /// Gives the possibility to easily identify targets for Conditions, Behaviors and so on.
    /// </summary>
    public interface ISceneObject : ILockable
    {
        event EventHandler<SceneObjectNameChanged> UniqueNameChanged;

        /// <summary>
        /// Unique Guid for each entity, which is required
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Unique name which is not required
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Target GameObject, used for applying stuff.
        /// </summary>
        Node GameObject { get; } //was GameObject

        /// <summary>
        /// Properties on the scene object.
        /// </summary>
        IEnumerable<ISceneObjectProperty> Properties { get; }

        /// <summary>
        /// True if the scene object has a property of the specified type.
        /// </summary>
        bool CheckHasProperty<T>() where T : ISceneObjectProperty;

        /// <summary>
        /// True if the scene object has a property of the specified type.
        /// </summary>
        bool CheckHasProperty(Type type);

        void ValidateProperties(IEnumerable<Type> properties);

        /// <summary>
        /// Returns a property of the specified type.
        /// </summary>
        T GetProperty<T>() where T : ISceneObjectProperty;

        /// <summary>
        /// Attempts to change the scene object's unique name to the specified name.
        /// </summary>        
        void ChangeUniqueName(string newName);
    }
}
