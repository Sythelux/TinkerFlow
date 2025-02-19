// // Copyright (c) 2013-2019 Innoactive GmbH
// // Licensed under the Apache License, Version 2.0
// // Modifications copyright (c) 2021-2024 MindPort GmbH
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Godot;
// using VRBuilder.Core.Configuration.Modes;
// using VRBuilder.Core.Properties;
// using VRBuilder.Core.SceneObjects;
// using VRBuilder.Core.Utils;
//
// namespace VRBuilder.Core.Configuration
// {
//     /// <summary>
//     /// Process runtime configuration which is used if no other was implemented.
//     /// </summary>
//     [Tool]
//     public partial class DefaultRuntimeConfiguration2 : BaseRuntimeConfiguration
//     {
//         /// <summary>
//         /// Default mode which white lists everything.
//         /// </summary>
//         public static readonly IMode DefaultMode = new Mode("Default", new WhitelistTypeRule<IOptional>());
//
//         private IProcessAudioPlayer? processAudioPlayer;
//         private ISceneObjectManager? sceneObjectManager;
//
//         public DefaultRuntimeConfiguration2()
//         {
//             Modes = new BaseModeHandler(new List<IMode> { DefaultMode });
//         }
//
//         /// <inheritdoc />
//         public override ProcessSceneObject User => LocalUser;
//
//         /// <inheritdoc />
//         public override UserSceneObject LocalUser
//         {
//             get
//             {
//                 UserSceneObject user = Users.FirstOrDefault();
//
//                 if (user == null) GD.PushError(new Exception("Could not find a UserSceneObject in the scene."));
//
//                 return user;
//             }
//         }
//
//         /// <inheritdoc />
//         public override AudioStreamPlayer /*AudioSource*/ InstructionPlayer => ProcessAudioPlayer.FallbackAudioSource;
//
//         /// <inheritdoc />
//         public override IProcessAudioPlayer ProcessAudioPlayer
//         {
//             get
//             {
//                 if (processAudioPlayer == null) processAudioPlayer = new DefaultAudioPlayer();
//
//                 return processAudioPlayer;
//             }
//         }
//
//         /// <inheritdoc />
//         public override IEnumerable<UserSceneObject> Users => NodeExtensions.FindObjectsOfType<UserSceneObject>();
//
//         /// <inheritdoc />
//         public override ISceneObjectManager SceneObjectManager => sceneObjectManager ??= new DefaultSceneObjectManager();
//
//         /// <inheritdoc />
//         public override ISceneObjectRegistry SceneObjectRegistry => sceneObjectRegistry ??= new GodotSceneObjectRegistry();
//     }
// }