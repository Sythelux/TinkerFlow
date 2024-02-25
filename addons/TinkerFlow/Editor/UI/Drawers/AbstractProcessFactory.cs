using System;
using System.Linq;
using System.Reflection;
using Godot;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;

namespace VRBuilder.Editor.UI.Drawers;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public abstract partial class AbstractProcessFactory : IProcessFactory
{
    #region IProcessFactory Members

    public abstract Control Create<T>(T currentValue, Action<object> changeValueCallback, string text);

    // public virtual Control DebugCreate<T>(T currentValue, Action<object> changeValueCallback, string text)
    // {
    //     PrintDebugger.Indent();
    //     var tmp = Create(currentValue, changeValueCallback, text);
    //     PrintDebugger.UnIndent();
    //     return tmp;
    // }

    public virtual Label GetLabel(MemberInfo memberInfo, object memberOwner)
    {
        GD.Print($"AbstractProcessFactory.GetLabel({memberInfo.Name}, {memberOwner})");
        // Type memberType = Core.Utils.ReflectionUtils.GetDeclaredTypeOfPropertyOrField(memberInfo);
        object? value = Core.Utils.ReflectionUtils.GetValueFromPropertyOrField(memberOwner, memberInfo);

        // if (value != null)
        // {
        //     memberType = value.GetType();
        // }

        Label valueLabel = GetLabel(value);

        DisplayNameAttribute? displayNameAttribute = memberInfo.GetAttributes<DisplayNameAttribute>(true).FirstOrDefault();
        DisplayTooltipAttribute? displayTooltipAttribute = memberInfo.GetAttributes<DisplayTooltipAttribute>(true).FirstOrDefault();

        if (displayNameAttribute?.Name != null)
        {
            valueLabel.Text = displayNameAttribute.Name;
        }

        if (displayTooltipAttribute?.Tooltip != null)
        {
            valueLabel.TooltipText = displayTooltipAttribute.Tooltip;
        }

        if (string.IsNullOrEmpty(valueLabel.Text))
        {
            valueLabel.Text = memberInfo.Name;
        }

        return valueLabel;
    }

    public virtual Label GetLabel<T>(T value)
    {
        GD.Print($"AbstractProcessFactory.GetLabel({value})");
        var nameable = value as INamedData;
        var l = new Label();

        l.Text = nameable == null || string.IsNullOrEmpty(nameable.Name)
            ? string.Empty
            : nameable.Name;

        return l;
    }

    public virtual void ChangeValue<T>(Func<T> getNewValueCallback, Func<T> getOldValueCallback, Action<T> assignValueCallback)
    {
        // ReSharper disable once ImplicitlyCapturedClosure
        Action doCallback = () => assignValueCallback(getNewValueCallback());
        // ReSharper disable once ImplicitlyCapturedClosure
        Action undoCallback = () => assignValueCallback(getOldValueCallback());
        // TODO: RevertableChangesHandler.Do(new ProcessCommand(doCallback, undoCallback));
    }

    #endregion
}