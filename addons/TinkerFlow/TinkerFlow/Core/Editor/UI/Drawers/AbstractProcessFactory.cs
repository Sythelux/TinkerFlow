#if UNITY_5_3_OR_NEWER
#elif GODOT
using Godot;
#endif
using System;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    public abstract partial class AbstractProcessFactory : IProcessDrawer
    {

        #region IProcessDrawer Members

        public abstract Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text);

        // public virtual Control DebugCreate<T>(T currentValue, Action<object> changeValueCallback, string text)
        // {
        //     PrintDebugger.Indent();
        //     var tmp = Create(currentValue, changeValueCallback, text);
        //     PrintDebugger.UnIndent();
        //     return tmp;
        // }

        public virtual Label? GetLabel(MemberInfo memberInfo, object memberOwner)
        {
            GD.Print($"AbstractProcessFactory.GetLabel({memberInfo.Name}, {memberOwner})");
            // Type memberType = Core.Utils.ReflectionUtils.GetDeclaredTypeOfPropertyOrField(memberInfo);
            object value = ReflectionUtils.GetValueFromPropertyOrField(memberOwner, memberInfo);

            // if (value != null)
            // {
            //     memberType = value.GetType();
            // }

            Label? valueLabel = GetLabel(value);
            if (valueLabel == null)
                return null;

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

        public virtual Label? GetLabel<T>(T value)
        {
            GD.Print($"AbstractProcessFactory.GetLabel({value})");
            var l = new Label();
            switch (value)
            {
                case INamedData nameable:
                    l.Text = string.IsNullOrEmpty(nameable.Name) ? string.Empty : nameable.Name;
                    break;
                case string stringValue:
                    l.Text = stringValue;
                    break;
                case null:
                    return null;
                default:
                    GD.PrintErr("AbstractProcessFactory.GetLabel: Unknown label type: " + value.GetType());
                    l.Text = l.Text;
                    break;
            }
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
}
