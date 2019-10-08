using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTK;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GL_EditorFramework.Framework;

namespace SpotLight
{
    public partial class LevelEditorForm : Form
    {
        SM3DWorldLevel currenLevel;
        public LevelEditorForm()
        {
            InitializeComponent();
            
            gL_ControlModern1.CameraDistance = 20;
            gL_ControlModern1.KeyDown += GL_ControlModern1_KeyDown;
            
            sceneListView1.SelectionChanged += SceneListView1_SelectionChanged;
            sceneListView1.ItemsMoved += SceneListView1_ItemsMoved;

            //Properties.Settings.Default.Reset();

            if (Program.GamePath == "")
            {
                MessageBox.Show(
@"Welcome to Spotlight!

In order to use this program, you will need the folders ""StageData"" and ""ObjectData"" from Super Mario 3D World

Please select the folder than contains these folders", "Introduction", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SpotlightToolStripStatusLabel.Text = "Welcome to Spotlight!";
            }
            else
                SpotlightToolStripStatusLabel.Text = "Welcome back!";

            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = "Select the Game Directory of Super Mario 3D World",
                IsFolderPicker = true
            };

            while (!Program.GamePathIsValid())
            {
                if (Program.GamePath != "")
                    MessageBox.Show("The Directory doesn't contain ObjectData and StageData.", "The GamePath is invalid");

                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Program.GamePath = ofd.FileName;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "3DW Levels|*.szs", InitialDirectory = Program.StageDataPath };
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName != "")
            {
                if(SM3DWorldLevel.TryOpen(ofd.FileName, gL_ControlModern1, sceneListView1, out SM3DWorldLevel level))
                {
                    SpotlightToolStripProgressBar.Maximum = 100;
                    SpotlightToolStripProgressBar.Value = 0;
                    currenLevel = level;
                    SpotlightToolStripProgressBar.Value = 50;
                    level.scene.SelectionChanged += Scene_SelectionChanged;
                    level.scene.ListChanged += Scene_ListChanged;

                    sceneListView1.Enabled = true;
                    sceneListView1.SetRootList("ObjectList");
                    sceneListView1.ListExited += SceneListView1_ListExited;
                    SpotlightToolStripProgressBar.Value = 100;
                    SpotlightToolStripStatusLabel.Text = $"\"{level.ToString()}\" has been Loaded successfully.";
                }
            }
        }

        private void SceneListView1_ListExited(object sender, ListEventArgs e)
        {
            currenLevel.scene.CurrentList = e.List;
            //fetch availible properties for list
            ObjectUIControl.CurrentObjectUIProvider = currenLevel.scene.GetObjectUIProvider();
        }

        private void SceneListView1_ItemsMoved(object sender, ItemsMovedEventArgs e)
        {
            currenLevel.scene.ReorderObjects(sceneListView1.CurrentList, e.OriginalIndex, e.Count, e.Offset);
            e.Handled = true;
            gL_ControlModern1.Refresh();
        }

        private void Scene_ListChanged(object sender, GL_EditorFramework.EditorDrawables.ListChangedEventArgs e)
        {
            if (e.Lists.Contains(sceneListView1.CurrentList))
            {
                sceneListView1.UpdateAutoScrollHeight();
                sceneListView1.Refresh();
            }
        }

        private void SceneListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currenLevel == null)
                return;

            foreach (object obj in e.ItemsToDeselect)
                currenLevel.scene.ToogleSelected((EditableObject)obj, false);

            foreach (object obj in e.ItemsToSelect)
                currenLevel.scene.ToogleSelected((EditableObject)obj, true);

            e.Handled = true;
            gL_ControlModern1.Refresh();

            Scene_SelectionChanged(this, null);
        }

        private void GL_ControlModern1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && currenLevel!=null)
            {
                string DeletedObjects = "";
                for (int i = 0; i < currenLevel.scene.SelectedObjects.Count; i++)
                    DeletedObjects += currenLevel.scene.SelectedObjects.ElementAt(i).ToString()+(i+1 == currenLevel.scene.SelectedObjects.Count ? ".":", ");
                SpotlightToolStripStatusLabel.Text = $"Deleted {DeletedObjects}";

                currenLevel.scene.DeleteSelected();
                gL_ControlModern1.Refresh();
                sceneListView1.UpdateAutoScrollHeight();
                Scene_SelectionChanged(this, null);
            }
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            if (currenLevel.scene.SelectedObjects.Count > 1)
            { 
                lblCurrentObject.Text = "Multiple objects selected";
                string SelectedObjects = "";
                for (int i = 0; i < currenLevel.scene.SelectedObjects.Count; i++)
                    SelectedObjects += currenLevel.scene.SelectedObjects.ElementAt(i).ToString() + (i + 1 == currenLevel.scene.SelectedObjects.Count ? "." : ", ");
                SpotlightToolStripStatusLabel.Text = $"Selected {SelectedObjects}";
            }
            else if (currenLevel.scene.SelectedObjects.Count == 0)
                lblCurrentObject.Text = SpotlightToolStripStatusLabel.Text = "Nothing selected";
            else
            {
                lblCurrentObject.Text = currenLevel.scene.SelectedObjects.First().ToString() + " selected";
                SpotlightToolStripStatusLabel.Text = $"Selected {currenLevel.scene.SelectedObjects.First().ToString()}";
            }
            sceneListView1.Refresh();

            ObjectUIControl.CurrentObjectUIProvider = currenLevel.scene.GetObjectUIProvider();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currenLevel == null)
                return;

            currenLevel.Save();
            SpotlightToolStripStatusLabel.Text = "Level saved!";
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currenLevel == null)
                return;

            if (currenLevel.SaveAs())
                SpotlightToolStripStatusLabel.Text = "Level saved!";
            else
                SpotlightToolStripStatusLabel.Text = "Save Cancelled or Failed.";
        }

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm SF = new SettingsForm();
            SF.ShowDialog();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currenLevel == null)
                return;
            currenLevel.scene.Undo();
            gL_ControlModern1.Refresh();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currenLevel == null)
                return;
            currenLevel.scene.Redo();
            gL_ControlModern1.Refresh();
        }
    }
}
