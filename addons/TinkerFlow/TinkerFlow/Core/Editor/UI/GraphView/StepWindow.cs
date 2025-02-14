using System.Collections.Generic;
using System.Reflection;
using Godot;
using VRBuilder.Core.Editor.UI.Drawers;
using VRBuilder.Core.Editor.UI.GraphView;

namespace VRBuilder.Core.Editor.UI.Windows
{
    ///<author email="Sythelux Rikd">Sythelux Rikd</author>
    [Tool]
    public partial class StepWindow : VBoxContainer, IStepView
    {
        private Control? stepDrawer;
        private IStep? step;
        // private LineEdit? stepName;

        public StepWindow()
        {
            Instance = this;
        }

        public static StepWindow Instance { get; private set; }

        // public LineEdit StepName => stepName ??= GetNode<LineEdit>("StepName");

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
            // StepName.Text = step.Data.Name;
            // StepName.Editable = true;
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
            foreach (Node child in GetAllChildren(stepDrawer))
            {
                // if (child is Control control)
                //     control.SizeFlagsVertical = SizeFlags.ExpandFill;
                child.Owner = stepDrawer;
            }

            var packedScene = new PackedScene();
            packedScene.Pack(stepDrawer);
            ResourceSaver.Save(packedScene, "res://tmp/stepDrawer.tscn");
            if (stepDrawer != null)
            {
                AddChild(stepDrawer);
            }
        }

        private void ModifyStep(object newStep)
        {
            step = newStep as IStep;
            GlobalEditorHandler.CurrentStepModified(step);
        }

        public void OnStepDeselected(Step? oldStep)
        {
            // StepName.Text = "None Selected";
            // StepName.Editable = false;

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

            // StepName.Visible = true;
        }

        public IEnumerable<Node> GetAllChildren(Node n)
        {
            yield return n;
            PrintDebugger.Indent();
            GD.Print(PrintDebugger.Get() + n.Name);
            foreach (Node child in n.GetChildren(false))
            {
                foreach (Node allChild in GetAllChildren(child))
                {
                    yield return allChild;
                }
            }
            PrintDebugger.UnIndent();
        }

        public static void ShowInspector()
        {
            // Instance.Show();
        }
    }
}
