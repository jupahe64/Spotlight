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
        SM3DWorldLevel currentLevel;
        public LevelEditorForm()
        {
            InitializeComponent();
            
            LevelGLControlModern.CameraDistance = 20;
            LevelGLControlModern.KeyDown += LevelGL_ControlModern_KeyDown;
            
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


        private void SceneListView1_ListExited(object sender, ListEventArgs e)
        {
            currentLevel.scene.CurrentList = e.List;
            //fetch availible properties for list
            ObjectUIControl.CurrentObjectUIProvider = currentLevel.scene.GetObjectUIProvider();
        }

        private void SceneListView1_ItemsMoved(object sender, ItemsMovedEventArgs e)
        {
            currentLevel.scene.ReorderObjects(sceneListView1.CurrentList, e.OriginalIndex, e.Count, e.Offset);
            e.Handled = true;
            LevelGLControlModern.Refresh();
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
            if (currentLevel == null)
                return;

            foreach (object obj in e.ItemsToDeselect)
                currentLevel.scene.ToogleSelected((EditableObject)obj, false);

            foreach (object obj in e.ItemsToSelect)
                currentLevel.scene.ToogleSelected((EditableObject)obj, true);

            e.Handled = true;
            LevelGLControlModern.Refresh();

            Scene_SelectionChanged(this, null);
        }

        private void LevelGL_ControlModern_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && currentLevel!=null)
            {
                string DeletedObjects = "";
                for (int i = 0; i < currentLevel.scene.SelectedObjects.Count; i++)
                    DeletedObjects += currentLevel.scene.SelectedObjects.ElementAt(i).ToString()+(i+1 == currentLevel.scene.SelectedObjects.Count ? ".":", ");
                SpotlightToolStripStatusLabel.Text = $"Deleted {DeletedObjects}";

                currentLevel.scene.DeleteSelected();
                LevelGLControlModern.Refresh();
                sceneListView1.UpdateAutoScrollHeight();
                Scene_SelectionChanged(this, null);
            }
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            if (currentLevel.scene.SelectedObjects.Count > 1)
            { 
                lblCurrentObject.Text = "Multiple objects selected";
                string SelectedObjects = "";
                for (int i = 0; i < currentLevel.scene.SelectedObjects.Count; i++)
                    SelectedObjects += currentLevel.scene.SelectedObjects.ElementAt(i).ToString() + (i + 1 == currentLevel.scene.SelectedObjects.Count ? "." : ", ");
                SpotlightToolStripStatusLabel.Text = $"Selected {SelectedObjects}";
            }
            else if (currentLevel.scene.SelectedObjects.Count == 0)
                lblCurrentObject.Text = SpotlightToolStripStatusLabel.Text = "Nothing selected";
            else
            {
                lblCurrentObject.Text = currentLevel.scene.SelectedObjects.First().ToString() + " selected";
                SpotlightToolStripStatusLabel.Text = $"Selected {currentLevel.scene.SelectedObjects.First().ToString()}";
            }
            sceneListView1.Refresh();

            ObjectUIControl.CurrentObjectUIProvider = currentLevel.scene.GetObjectUIProvider();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "3DW Levels|*.szs", InitialDirectory = Program.StageDataPath };
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName != "")
            {
                if (SM3DWorldLevel.TryOpen(ofd.FileName, LevelGLControlModern, sceneListView1, out SM3DWorldLevel level))
                {
                    SpotlightToolStripProgressBar.Maximum = 100;
                    SpotlightToolStripProgressBar.Value = 0;
                    currentLevel = level;
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

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentLevel == null)
                return;

            currentLevel.Save();
            SpotlightToolStripStatusLabel.Text = "Level saved!";
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentLevel == null)
                return;

            if (currentLevel.SaveAs())
                SpotlightToolStripStatusLabel.Text = "Level saved!";
            else
                SpotlightToolStripStatusLabel.Text = "Save Cancelled or Failed.";
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm SF = new SettingsForm();
            SF.ShowDialog();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentLevel == null)
                return;
            currentLevel.scene.Undo();
            LevelGLControlModern.Refresh();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentLevel == null)
                return;
            currentLevel.scene.Redo();
            LevelGLControlModern.Refresh();
        }
    }
}
