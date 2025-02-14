// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2024 MindPort GmbH

using System;
using System.Reflection;
using VRBuilder.Core.Behaviors;
using Godot;
using TinkerFlow.Godot.Editor;
using VRBuilder.Core.Editor.Godot;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Process drawer for <see cref="BehaviorExecutionStages"/> members.
    /// </summary>
    [DefaultProcessDrawer(typeof(BehaviorExecutionStages))]
    internal class BehaviorExecutionStagesFactory : AbstractProcessFactory
    {
        private enum ExecutionStages
        {
            BeforeStepExecution = 1 << 0,
            AfterStepExecution = 1 << 1,
            BeforeAndAfterStepExecution = ~0
        }

        /// <inheritdoc />
        public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");
            //TODO:
            //
            // rect.height = EditorDrawingHelper.SingleLineHeight;
            //
            // BehaviorExecutionStages oldBehaviorExecutionStages = (BehaviorExecutionStages)currentValue;
            // BehaviorExecutionStages newBehaviorExecutionStages;
            // ExecutionStages oldExecutionStages;
            // ExecutionStages newExecutionStages;
            //
            // oldExecutionStages = (ExecutionStages)(int)currentValue;
            // newExecutionStages = (ExecutionStages)EditorGUI.EnumPopup(rect, label, oldExecutionStages);
            //
            // if (newExecutionStages != oldExecutionStages)
            // {
            //     switch (newExecutionStages)
            //     {
            //         case ExecutionStages.AfterStepExecution:
            //             newBehaviorExecutionStages = BehaviorExecutionStages.Deactivation;
            //             break;
            //         case ExecutionStages.BeforeAndAfterStepExecution:
            //             newBehaviorExecutionStages = BehaviorExecutionStages.ActivationAndDeactivation;
            //             break;
            //         default:
            //             newBehaviorExecutionStages = BehaviorExecutionStages.Activation;
            //             break;
            //     }
            //
            //     ChangeValue(() => newBehaviorExecutionStages, () => oldBehaviorExecutionStages, changeValueCallback);
            // }
            //
            // return rect;
            return new Control
            {
                Name = GetType().Name + "." + text
            };
        }
    }
}
