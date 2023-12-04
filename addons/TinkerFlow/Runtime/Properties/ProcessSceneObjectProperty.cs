// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System.Linq;
using Godot;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    // [RequireComponent(typeof(ProcessSceneObject))]
    public abstract partial class ProcessSceneObjectProperty : Node, ISceneObjectProperty
    {
        private ISceneObject sceneObject;

        public ISceneObject SceneObject
        {
            get
            {
                if (sceneObject == null)
                {
                    sceneObject = FindChildren("*", recursive: true)?.OfType<ISceneObject>().First();
                }

                return sceneObject;
            }
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void Reset()
        {
            //TODO: this.AddProcessPropertyExtensions();
        }
    }
}