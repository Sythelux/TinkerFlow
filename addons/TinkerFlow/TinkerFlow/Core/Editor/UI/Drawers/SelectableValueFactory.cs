using Godot;
using System;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.UI.SelectableValues;

namespace VRBuilder.Core.Editor.UI.Drawers
{
    /// <summary>
    /// Template drawer for selectable values. A concrete implementation of this drawer is required for each use case.
    /// </summary>
    public abstract class SelectableValueFactory<TFirst, TSecond> : AbstractProcessFactory
    {
        public override Control? Create<T>(T currentValue, Action<object> changeValueCallback, string text)
        {
            GD.Print($"{PrintDebugger.Get()}{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}({currentValue?.GetType().Name}, {text})");

            if (RuntimeConfigurator.Exists == false)
            {
                return new Control();
            }

            if (currentValue is not SelectableValue<TFirst, TSecond> selectableValue)
                return new Control();

            var toggleContainer = new VBoxContainer();
            var group = new ButtonGroup();
            var firstValueToggle = new Button
            {
                ToggleMode = true,
                ButtonGroup = group,
                Text = selectableValue.FirstValueLabel
            };
            firstValueToggle.SetPressed(selectableValue.IsFirstValueSelected);
            firstValueToggle.Pressed += () => changeValueCallback?.Invoke(firstValueToggle.IsPressed());
            toggleContainer.AddChild(firstValueToggle);

            var secondValueToggle = new Button
            {
                ToggleMode = true,
                ButtonGroup = group,
                Text = selectableValue.SecondValueLabel
            };
            secondValueToggle.SetPressed(!selectableValue.IsFirstValueSelected);
            //secondValueToggle.Pressed += () => changeValueCallback?.Invoke(!firstValueToggle.IsPressed());
            toggleContainer.AddChild(secondValueToggle);

            //TODO:
            // if (selectableValue.IsFirstValueSelected)
            // {
            //     MemberInfo firstMemberInfo = selectableValue.GetType().GetMember(nameof(selectableValue.FirstValue)).First();
            //     rect.height += DrawerLocator.GetDrawerForMember(firstMemberInfo, selectableValue).Draw(rect, selectableValue.FirstValue, (value) => ChangeValue(() => value, () => selectableValue.FirstValue, (newValue) =>
            //     {
            //         selectableValue.FirstValue = (TFirst)newValue;
            //         changeValueCallback(selectableValue);
            //     }), label).height;
            // }
            // else
            // {
            //     MemberInfo secondMemberInfo = selectableValue.GetType().GetMember(nameof(selectableValue.SecondValue)).First();
            //     rect.height += DrawerLocator.GetDrawerForMember(secondMemberInfo, selectableValue).Draw(rect, selectableValue.SecondValue, (value) => ChangeValue(() => value, () => selectableValue.SecondValue, (newValue) =>
            //     {
            //         selectableValue.SecondValue = (TSecond)newValue;
            //         changeValueCallback(selectableValue);
            //     }), label).height;
            // }
            //
            // rect.height += EditorDrawingHelper.VerticalSpacing;

            return toggleContainer;
        }

        /*protected Rect AddNewRectLine(ref Rect currentRect)
        {
            Rect newRectLine = currentRect;
            newRectLine.height = EditorDrawingHelper.SingleLineHeight;
            newRectLine.y += currentRect.height + EditorDrawingHelper.VerticalSpacing;

            currentRect.height += EditorDrawingHelper.SingleLineHeight + EditorDrawingHelper.VerticalSpacing;
            return newRectLine;
        }*/
    }
}
