// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// This wrapper is used for <see cref="IRuntimeConfiguration"/> configurations, which
    /// ensures that the old interface based configurations can still be used.
    /// </summary>
    [Obsolete("Helper class to ensure backwards compatibility.")]
    public class RuntimeConfigWrapper : BaseRuntimeConfiguration
    {
        /// <summary>
        /// Wrapped IRuntimeConfiguration.
        /// </summary>
        public readonly IRuntimeConfiguration Configuration;

        public RuntimeConfigWrapper(IRuntimeConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc />
        public override ProcessSceneObject User => Configuration.LocalUser;

        /// <inheritdoc />
        public override AudioStreamPlayer /*AudioSource*/ InstructionPlayer => Configuration.InstructionPlayer;

        /// <inheritdoc />
        public override ISceneObjectRegistry SceneObjectRegistry => Configuration.SceneObjectRegistry;

        /// <inheritdoc />
        public override IEnumerable<UserSceneObject> Users => Configuration.Users;

        /// <inheritdoc />
        public override IProcessAudioPlayer ProcessAudioPlayer => Configuration.ProcessAudioPlayer;

        /// <inheritdoc />
        public override ISceneObjectManager SceneObjectManager => Configuration.SceneObjectManager;

        /// <inheritdoc />
        public override ISceneConfiguration SceneConfiguration => Configuration.SceneConfiguration;

        /// <inheritdoc />
        public override UserSceneObject LocalUser => Configuration.LocalUser;

        /// <inheritdoc />
        public override Task<IProcess> LoadProcess(string path)
        {
            return Configuration.LoadProcess(path);
        }
    }
}
