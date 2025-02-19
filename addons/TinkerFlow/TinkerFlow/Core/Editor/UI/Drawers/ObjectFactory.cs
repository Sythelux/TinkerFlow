using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.ProcessValidation;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    [DefaultProcessDrawer(typeof(object))]
    public partial class ObjectFactory : AbstractProcessFactory
    {
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            var label = new Label { Text = text };
            if (currentValue == null)
            {
                return label;
            }

            var container = new VBoxContainer { Name = GetType().Name + "." + text };
            // container.AddChild(CreateLabel(currentValue, changeValueCallback, label));

            //TODO: some issues with: GroupsToUnlock, which is IDictionary, IBehaviorCollection is not there?
            foreach (MemberInfo memberInfoToDraw in GetMembersToDraw(currentValue))
            {
                GD.Print("memberInfoToDraw: " + memberInfoToDraw.Name);
                MemberInfo closuredMemberInfo = memberInfoToDraw;
                if (closuredMemberInfo.GetAttributes<MetadataAttribute>(true).Any())
                {
                    Control andDrawMetadataWrapper = CreateAndDrawMetadataWrapper(currentValue, closuredMemberInfo, changeValueCallback);
                    container.AddChild(andDrawMetadataWrapper);
                }
                else
                {
                    IProcessDrawer memberDrawer = DrawerLocator.GetDrawerForMember(closuredMemberInfo, currentValue);

                    object? memberValue = ReflectionUtils.GetValueFromPropertyOrField(currentValue, closuredMemberInfo);

                    Label displayName = memberDrawer.GetLabel(closuredMemberInfo, currentValue);

                    CheckValidationForValue(currentValue, closuredMemberInfo, displayName);

                    Control control = memberDrawer.Create(memberValue, (value) =>
                    {
                        ReflectionUtils.SetValueToPropertyOrField(currentValue, closuredMemberInfo, value);
                        changeValueCallback(currentValue);
                    }, displayName?.Text ?? null);
                    container.AddChild(control);
                }
            }

            return container;
        }

        protected virtual void CheckValidationForValue(object currentValue, MemberInfo info, Label label)
        {
            if (currentValue is IData data && EditorConfigurator.Instance.Validation.IsAllowedToValidate())
            {
                List<EditorReportEntry> entries = GetValidationReportsFor(data, info);
                if (entries.Count > 0)
                {
                    AddValidationInformation(label, entries);
                }
            }
        }

        protected virtual Label AddValidationInformation(Label Control, List<EditorReportEntry> entries)
        {
            // Control.image = EditorGUIUtility.IconContent("Warning").image;
            Control.TooltipText = ValidationTooltipGenerator.CreateTooltip(entries);
            return Control;
        }

        protected virtual List<EditorReportEntry> GetValidationReportsFor(IData data, MemberInfo memberInfo)
        {
            if (EditorConfigurator.Instance.Validation.LastReport != null)
            {
                return EditorConfigurator.Instance.Validation.LastReport.GetEntriesFor(data, memberInfo);
            }

            return new List<EditorReportEntry>();
        }

        protected virtual Control CreateLabel<T>(T currentValue, Action<object> changeValueCallback, Label label)
        {
            return label;
        }

        /// <inheritdoc />
        public override Label GetLabel(MemberInfo memberInfo, object memberOwner)
        {
            return MergeControls(base.GetLabel(memberInfo, memberOwner), GetTypeNameLabel(ReflectionUtils.GetValueFromPropertyOrField(memberOwner, memberInfo))); //, ReflectionUtils.GetDeclaredTypeOfPropertyOrField(memberInfo)
        }

        /// <inheritdoc />
        public override Label GetLabel<T>(T value)
        {
            return MergeControls(base.GetLabel(value), GetTypeNameLabel(value));
        }

        protected virtual IEnumerable<MemberInfo> GetMembersToDraw(object value)
        {
            return EditorReflectionUtils.GetFieldsAndPropertiesToDraw(value);
        }

        private Label MergeControls(Label? name, Label? typeName)
        {
            Label result;
            if (name == null || string.IsNullOrEmpty(name.Text))
                result = new Label
                {
                    Text = $"{typeName?.Text}", TooltipText
                        // image = name.image,
                        = name?.TooltipText
                };
            else
                result = new Label
                {
                    Text = $"{name.Text}"
                };

            // if (result.image == null)
            // {
            //     result.image = typeName.image;
            // }

            result.TooltipText ??= typeName?.TooltipText;

            return result;
        }

        protected virtual Label GetTypeNameLabel<T>(T value)
        {
            Type actualType = typeof(T);
            if (value != null) actualType = value.GetType();

            DisplayNameAttribute? typeNameAttribute = actualType.GetAttributes<DisplayNameAttribute>(true).FirstOrDefault();
            return typeNameAttribute != null
                ? new Label { Text = typeNameAttribute.Name }
                : new Label { Text = actualType.FullName };
        }

        private Control CreateAndDrawMetadataWrapper(object ownerObject, MemberInfo drawnMemberInfo, Action<object> changeValueCallback)
        {
            PropertyInfo? metadataProperty = ownerObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(property => typeof(Metadata).IsAssignableFrom(property.PropertyType));
            FieldInfo? metadataField = ownerObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(field => typeof(Metadata).IsAssignableFrom(field.FieldType));
            Metadata? ownerObjectMetadata;

            if (metadataProperty != null)
                ownerObjectMetadata = metadataProperty.GetValue(ownerObject, null) as Metadata ?? new Metadata();
            else if (metadataField != null)
                ownerObjectMetadata = metadataField.GetValue(ownerObject) as Metadata ?? new Metadata();
            else
            {
                GD.PushError(new MissingFieldException($"No metadata property on object {ownerObject}."));
                return new Control { Name = $"{ownerObject}.Missing" };
            }

            object? memberValue = ReflectionUtils.GetValueFromPropertyOrField(ownerObject, drawnMemberInfo);
            IProcessDrawer memberDrawer = DrawerLocator.GetDrawerForMember(drawnMemberInfo, ownerObject);

            var wrapper = new MetadataWrapper
            {
                Metadata = ownerObjectMetadata.GetMetadata(drawnMemberInfo),
                ValueDeclaredType = ReflectionUtils.GetDeclaredTypeOfPropertyOrField(drawnMemberInfo),
                Value = memberValue
            };

            void WrapperChangedCallback(object newValue)
            {
                var newWrapper = (MetadataWrapper)newValue;
                foreach (string key in newWrapper.Metadata.Keys.ToList()) wrapper.Metadata[key] = newWrapper.Metadata[key];

                ownerObjectMetadata.Clear();
                foreach (string key in newWrapper.Metadata.Keys) ownerObjectMetadata.SetMetadata(drawnMemberInfo, key, newWrapper.Metadata[key]);

                if (metadataField != null) metadataField.SetValue(ownerObject, ownerObjectMetadata);

                if (metadataProperty != null) metadataProperty.SetValue(ownerObject, ownerObjectMetadata, null);

                ReflectionUtils.SetValueToPropertyOrField(ownerObject, drawnMemberInfo, newWrapper.Value);

                changeValueCallback(ownerObject);
            }

            var isMetadataDirty = false;

            List<MetadataAttribute> declaredAttributes = drawnMemberInfo.GetAttributes<MetadataAttribute>(true).ToList();

            Dictionary<string, object> obsoleteMetadataRemoved = wrapper.Metadata.Keys.ToList().Where(key => declaredAttributes.Any(attribute => attribute.Name == key)).ToDictionary(key => key, key => wrapper.Metadata[key]);

            if (obsoleteMetadataRemoved.Count < wrapper.Metadata.Count)
            {
                wrapper.Metadata = obsoleteMetadataRemoved;
                isMetadataDirty = true;
            }

            foreach (MetadataAttribute metadataAttribute in declaredAttributes)
                if (wrapper.Metadata.ContainsKey(metadataAttribute.Name) == false)
                {
                    wrapper.Metadata[metadataAttribute.Name] = metadataAttribute.GetDefaultMetadata(drawnMemberInfo);
                    isMetadataDirty = true;
                }
                else if (metadataAttribute.IsMetadataValid(wrapper.Metadata[metadataAttribute.Name]) == false)
                {
                    wrapper.Metadata[metadataAttribute.Name] = metadataAttribute.GetDefaultMetadata(drawnMemberInfo);
                    isMetadataDirty = true;
                }

            if (isMetadataDirty) WrapperChangedCallback(wrapper);

            IProcessDrawer? wrapperDrawer = DrawerLocator.GetDrawerForValue(wrapper, typeof(MetadataWrapper));

            Label displayName = memberDrawer.GetLabel(drawnMemberInfo, ownerObject);

            return wrapperDrawer?.Create(wrapper, (Action<object>)WrapperChangedCallback, displayName.Text) ?? new Label { Text = "No Drawer for MetadataWrapper" };
        }
    }
}
