using System.Reflection;
using Godot;
using VRBuilder.Core;
using VRBuilder.Editor;
using VRBuilder.Editor.UI.Drawers;
using VRBuilder.Editor.UI.Windows;

namespace TinkerFlow.Godot.Editor;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
[Tool]
public partial class StepWindow : Control, IStepView
{
    private Control? stepDrawer;
    private IStep? step;
    private LineEdit? stepName;

    public StepWindow()
    {
        Instance = this;
    }

    public static StepWindow Instance { get; private set; }

    public LineEdit StepName => stepName ??= GetNode<LineEdit>("StepName");

    #region IStepView Members

    public void SetStep(IStep? newStep)
    {
        if (step == null)
            OnStepDeselected(null);
        else
            OnStepSelected(step);
    }


    public void ResetStepView()
    {
        if (Instance.IsVisibleInTree() || step == null) return;

        // Dictionary<string, object> dict = step.Data.Metadata.GetMetadata(typeof(TabsGroup));
        // if (dict.ContainsKey(TabsGroup.SelectedKey))
        // {
        //     dict[TabsGroup.SelectedKey] = 0;
        // }
    }

    #endregion

    public override void _Draw() //OnGUI
    {
        if (step == null)
        {
            return;
        }

        // GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
    }

    public override void _EnterTree()
    {
        // GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());

        GlobalEditorHandler.StepWindowOpened(this);
    }

    public override void _ExitTree()
    {
        // GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());

        GlobalEditorHandler.StepWindowClosed(this);
    }

    public override void _Notification(int what)
    {
        // if (!new[] { NotificationProcess }.Contains(what))
        //     GD.Print($"{GetType().Name}: {MethodBase.GetCurrentMethod()?.Name}({Enum.GetName(typeof(Notifications), what)}:{what})");
    }

    public override void _Process(double delta)
    {
        //called even when invisible
        // GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
    }

    public override void _Ready()
    {
        GD.Print(GetType().Name + ": " + MethodBase.GetCurrentMethod());
        OnStepDeselected(null);
    }

    public void OnStepSelected(IStep? newStep)
    {
        if (newStep == null)
            return;
        step = newStep;
        StepName.Text = step.Data.Name;
        foreach (Node? node in GetChildren())
        {
            var child = (Control)node;
            child.Visible = true;
        }

        if (stepDrawer != null)
        {
            RemoveChild(stepDrawer);
            stepDrawer.Free();
            stepDrawer = null;
        }

        stepDrawer = DrawerLocator.GetDrawerForValue(step, typeof(Step))?.Create(step, ModifyStep, "Step");
        if (stepDrawer != null)
            AddChild(stepDrawer);
    }

    private void ModifyStep(object newStep)
    {
        step = newStep as IStep;
        GlobalEditorHandler.CurrentStepModified(step);
    }

    public void OnStepDeselected(Step? oldStep)
    {
        StepName.Text = "None Selected";

        if (stepDrawer != null)
        {
            RemoveChild(stepDrawer);
            stepDrawer.Free();
            stepDrawer = null;
        }

        foreach (Node? node in GetChildren())
        {
            var child = (Control)node;
            child.Visible = false;
        }

        StepName.Visible = true;
    }

    public static void ShowInspector()
    {
        // Instance.Show();
    }
}