using System.Collections.Generic;
using System.Reflection;
using Godot;
using VRBuilder.Core.Editor.Godot;
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
        private LineEdit? noStepLabel;

        public StepWindow()
        {
            Instance = this;
        }

        public static StepWindow Instance { get; private set; }

        public LineEdit NoStepLabel => noStepLabel ??= GetNode<LineEdit>("StepName");

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
            NoStepLabel.Visible = true;
            if (newStep == null)
                return;
            step = newStep;
            foreach (Node? node in GetChildren())
            {
                var child = (Control)node;
                child.Visible = true;
            }
            NoStepLabel.Visible = false;

            if (stepDrawer != null)
            {
                RemoveChild(stepDrawer);
                stepDrawer.Free();
                stepDrawer = null;
            }

            stepDrawer = DrawerLocator.GetDrawerForValue(step, typeof(Step))?.Create(step, ModifyStep, "Step");
            if (stepDrawer != null)
            {
                foreach (var child in GetAllChildren(stepDrawer))
                    if (child.Owner != stepDrawer)
                        child.Owner = stepDrawer;

                var packedScene = new PackedScene();
                packedScene.Pack(stepDrawer);
                ResourceSaver.Save(packedScene, "res://tmp/stepDrawer.tscn");
                if (stepDrawer != null)
                {
                    AddChild(stepDrawer);
                }
            }
            else
            {
                AddChild(EditorGUI.HelpBox("No Stepdrawer", EditorGUI.MessageType.Warning));
            }
        }

        private void ModifyStep(object newStep)
        {
            step = newStep as IStep;
            GlobalEditorHandler.CurrentStepModified(step);
        }

        public void OnStepDeselected(Step? oldStep)
        {
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

            NoStepLabel.Visible = true;
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