using System;
using Godot;

namespace TinkerFlow.Core.Editor.StepInspector;

public partial class CurveEditor : Control
{
    private const double DEFAULT_SNAP = 10;
    private const double BASE_SPACING = 10;
    private Button snapButton;
    private EditorSpinSlider snapCountEdit;
    private MenuButton presetsButton;
    private CurveEdit curveEditorRect;
    private float spacing;

    public override void _EnterTree()
    {
        HFlowContainer toolbar = new HFlowContainer();
        AddChild(toolbar);

        snapButton = new Button();
        snapButton.SetTooltipText(Tr("Toggle Grid Snap"));
        snapButton.SetToggleMode(true);
        toolbar.AddChild(snapButton);

        toolbar.AddChild(new VSeparator());

        snapCountEdit = new EditorSpinSlider();
        snapCountEdit.SetMin(2);
        snapCountEdit.SetMax(100);
        snapCountEdit.SetValue(DEFAULT_SNAP);
        snapCountEdit.SetCustomMinimumSize(new Vector2(65 * GetThemeDefaultBaseScale(), 0)); //TOD: check if GetThemeDefaultBaseScale is also ok instead of EDSCALE
        toolbar.AddChild(snapCountEdit);
        snapCountEdit.ValueChanged += SetSnapCount;

        presetsButton = new MenuButton();
        presetsButton.SetText(Tr("Presets"));
        presetsButton.SetSwitchOnHover(true);
        presetsButton.SetHSizeFlags(SizeFlags.Expand | SizeFlags.ShrinkEnd);
        toolbar.AddChild(presetsButton);
        presetsButton.GetPopup().IdPressed += OnPresetItemSelected;

        curveEditorRect = new CurveEdit();
        AddChild(curveEditorRect);

        Control emptySpace = new Control();
        emptySpace.CustomMinimumSize = new Vector2(0, spacing);
        AddChild(emptySpace);

        SetMouseFilter(MouseFilterEnum.Stop);
        SetSnapEnabled(snapButton.IsPressed());
        SetSnapCount(snapCountEdit.GetValue());
    }

    private void SetSnapEnabled(bool isPressed)
    {
        GD.PushError(new NotImplementedException());
    }

    private void OnPresetItemSelected(long id)
    {
        GD.PushError(new System.NotImplementedException());
    }

    private void SetSnapCount(double value)
    {
        GD.PushError(new System.NotImplementedException());
    }

    public override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationThemeChanged:
                spacing = (float)Math.Round(BASE_SPACING * GetThemeDefaultBaseScale());
                // snap_button->set_icon(get_editor_theme_icon(SNAME("SnapGrid")));
                // PopupMenu* p = presets_button->get_popup();
                // p->clear();
                // p->add_icon_item(get_editor_theme_icon(SNAME("CurveConstant")), TTR("Constant"), CurveEdit::PRESET_CONSTANT);
                // p->add_icon_item(get_editor_theme_icon(SNAME("CurveLinear")), TTR("Linear"), CurveEdit::PRESET_LINEAR);
                // p->add_icon_item(get_editor_theme_icon(SNAME("CurveIn")), TTR("Ease In"), CurveEdit::PRESET_EASE_IN);
                // p->add_icon_item(get_editor_theme_icon(SNAME("CurveOut")), TTR("Ease Out"), CurveEdit::PRESET_EASE_OUT);
                // p->add_icon_item(get_editor_theme_icon(SNAME("CurveInOut")), TTR("Smoothstep"), CurveEdit::PRESET_SMOOTHSTEP);
                break;
        }
    }
}