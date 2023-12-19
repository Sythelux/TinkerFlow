using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Utils;
using VRBuilder.Editor.Configuration;
using VRBuilder.Editor.ProcessValidation;

namespace VRBuilder.Editor.UI.Drawers;

[DefaultProcessDrawer(typeof(object))]
public partial class ObjectFactory : AbstractProcessFactory
{
    public override Control Create<T>(T currentValue, Action<object> changeValueCallback, string label)
    {
        if (currentValue == null)
        {
            return new Label
            {
                Text = label
            };
        }

        var container = new VBoxContainer();
        container.AddChild(new Label
        {
            Text = label
        });

        foreach (MemberInfo memberInfoToDraw in GetMembersToDraw(currentValue))
        {
            MemberInfo closuredMemberInfo = memberInfoToDraw;
            if (closuredMemberInfo.GetAttributes<MetadataAttribute>(true).Any())
            {
                container.AddChild(CreateAndDrawMetadataWrapper(currentValue, closuredMemberInfo, changeValueCallback));
            }
            else
            {
                IProcessFactory memberDrawer = DrawerLocator.GetDrawerForMember(closuredMemberInfo, currentValue);

                object? memberValue = ReflectionUtils.GetValueFromPropertyOrField(currentValue, closuredMemberInfo);

                Label displayName = memberDrawer.GetLabel(closuredMemberInfo, currentValue);

                CheckValidationForValue(currentValue, closuredMemberInfo, displayName);

                container.AddChild(memberDrawer.Create(memberValue, (value) =>
                {
                    ReflectionUtils.SetValueToPropertyOrField(currentValue, closuredMemberInfo, value);
                    changeValueCallback(currentValue);
                }, displayName.Text));
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

    protected virtual Label AddValidationInformation(Label guiContent, List<EditorReportEntry> entries)
    {
        // guiContent.image = EditorGUIUtility.IconContent("Warning").image;
        guiContent.TooltipText = ValidationTooltipGenerator.CreateTooltip(entries);
        return guiContent;
    }

    protected virtual List<EditorReportEntry> GetValidationReportsFor(IData data, MemberInfo memberInfo)
    {
        if (EditorConfigurator.Instance.Validation.LastReport != null)
        {
            return EditorConfigurator.Instance.Validation.LastReport.GetEntriesFor(data, memberInfo);
        }

        return new List<EditorReportEntry>();
    }

    protected virtual IEnumerable<MemberInfo> GetMembersToDraw(object value)
    {
        return EditorReflectionUtils.GetFieldsAndPropertiesToDraw(value);
    }


    private Control CreateAndDrawMetadataWrapper(object ownerObject, MemberInfo drawnMemberInfo, Action<object> changeValueCallback)
    {
        return new Container();
    }
}