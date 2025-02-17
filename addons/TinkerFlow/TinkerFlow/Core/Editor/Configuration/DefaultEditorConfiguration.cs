// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2023 MindPort GmbH

using System.Collections.ObjectModel;
using System.Linq;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.ProcessValidation;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Core.IO;
using VRBuilder.Core.Serialization;

namespace VRBuilder.Core.Editor.Configuration
{
    /// <summary>
    /// Default editor configuration definition which is used if no other was implemented.
    /// </summary>
    public class DefaultEditorConfiguration : IEditorConfiguration
    {
        private AllowedMenuItemsSettings? allowedMenuItemsSettings;

        /// <inheritdoc />
        public virtual string ProcessStreamingAssetsSubdirectory => "Processes";

        /// <inheritdoc />
        public virtual string AllowedMenuItemsSettingsAssetPath => "TinkerFlow/Editor/Config/AllowedMenuItems.json";

        /// <inheritdoc />
        public virtual IProcessSerializer Serializer => new NewtonsoftJsonProcessSerializerV4();

        /// <inheritdoc />
        public IProcessAssetStrategy ProcessAssetStrategy => new SingleFileProcessAssetStrategy();

        /// <inheritdoc />
        public virtual ReadOnlyCollection<MenuOption<IBehavior>> BehaviorsMenuContent => AllowedMenuItemsSettings.GetBehaviorMenuOptions().Cast<MenuOption<IBehavior>>().ToList().AsReadOnly();

        /// <inheritdoc />
        public virtual ReadOnlyCollection<MenuOption<ICondition>> ConditionsMenuContent => AllowedMenuItemsSettings.GetConditionMenuOptions().Cast<MenuOption<ICondition>>().ToList().AsReadOnly();

        /// <inheritdoc />
        public virtual AllowedMenuItemsSettings AllowedMenuItemsSettings
        {
            get => allowedMenuItemsSettings ??= AllowedMenuItemsSettings.Instance;
            set => allowedMenuItemsSettings = value;
        }

        internal virtual IValidationHandler Validation { get; }

        protected DefaultEditorConfiguration()
        {
#if CREATOR_PRO
            Validation = new DefaultValidationHandler();
#else
            Validation = new DisabledValidationHandler();
#endif
        }
    }
}
