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
using SpotLight.ObjectParamDatabase;
using SpotLight.Level;

namespace SpotLight
{
    public partial class LevelEditorForm : Form
    {
        LevelParameterForm LPF;
        SM3DWorldScene currentScene;
        public ObjectParameterDatabase ObjectDatabase = null;
        public LevelEditorForm()
        {
            InitializeComponent();
            tabControl1.SelectedTab = tabPageObjects;
            LevelGLControlModern.CameraDistance = 20;

            MainSceneListView.SelectionChanged += MainSceneListView_SelectionChanged;
            MainSceneListView.ItemsMoved += MainSceneListView_ItemsMoved;

            LevelGLControlModern.Visible = false;

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

            if (!File.Exists("ParameterDatabase.sopd"))
            {
                bool Breakout = false;
                while (!Breakout)
                {
                    DialogResult DR = MessageBox.Show(
@"Spotlight could not find the Object Parameter Database (ParameterDatabase.sopd)

Spotlight needs an Object Parameter Database in order for you to add objects.

Would you like to generate a new object Database from your 3DW Directory?",
"Database Missing", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

                    switch (DR)
                    {
                        case DialogResult.Yes:
                            ObjectDatabase = new ObjectParameterDatabase();
                            ObjectDatabase.Create(Program.StageDataPath);
                            ObjectDatabase.Save("ParameterDatabase.sopd");
                            Breakout = true;
                            break;
                        case DialogResult.No:
                            DialogResult DR2 = MessageBox.Show("Are you sure? You cannot add objects without it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            switch (DR2)
                            {
                                case DialogResult.None:
                                case DialogResult.Yes:
                                    ObjectDatabase = null;
                                    Breakout = true;
                                    break;
                                case DialogResult.No:
                                    break;
                            }
                            break;
                        case DialogResult.None:
                        case DialogResult.Cancel:
                            Environment.Exit(1);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                ObjectDatabase = new ObjectParameterDatabase("ParameterDatabase.sopd");
                ObjectParameterDatabase Ver = new ObjectParameterDatabase();

                if (Ver.Version > ObjectDatabase.Version)
                {
                    bool Breakout = false;
                    while (!Breakout)
                    {
                        DialogResult DR = MessageBox.Show(
$@"The Loaded Database is outdated ({ObjectDatabase.Version.ToString()}).
The latest Database version is {ObjectParameterDatabase.LatestVersion}.
Would you like to rebuild the database from your 3DW Files?",
    "Database Outdated", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

                        switch (DR)
                        {
                            case DialogResult.Yes:
                                ObjectDatabase = new ObjectParameterDatabase();
                                ObjectDatabase.Create(Program.StageDataPath);
                                ObjectDatabase.Save("ParameterDatabase.sopd");
                                Breakout = true;
                                break;
                            case DialogResult.No:
                                DialogResult DR2 = MessageBox.Show("Are you sure? You cannot add objects without it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                                switch (DR2)
                                {
                                    case DialogResult.None:
                                    case DialogResult.Yes:
                                        ObjectDatabase = null;
                                        Breakout = true;
                                        break;
                                    case DialogResult.No:
                                        break;
                                }
                                break;
                            case DialogResult.None:
                            case DialogResult.Cancel:
                                Environment.Exit(1);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


        private void MainSceneListView_ListExited(object sender, ListEventArgs e)
        {
            currentScene.CurrentList = e.List;
            //fetch availible properties for list
            currentScene.SetupObjectUIControl(ObjectUIControl);
        }

        private void MainSceneListView_ItemsMoved(object sender, ItemsMovedEventArgs e)
        {
            currentScene.ReorderObjects(MainSceneListView.CurrentList, e.OriginalIndex, e.Count, e.Offset);
            e.Handled = true;
            LevelGLControlModern.Refresh();
        }

        private void Scene_ListChanged(object sender, GL_EditorFramework.EditorDrawables.ListChangedEventArgs e)
        {
            if (e.Lists.Contains(MainSceneListView.CurrentList))
            {
                MainSceneListView.UpdateAutoScrollHeight();
                MainSceneListView.Refresh();
            }
        }

        private void MainSceneListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentScene == null)
                return;

            //apply selection changes to scene
            if (e.SelectionChangeMode == SelectionChangeMode.SET)
            {
                currentScene.SelectedObjects.Clear();

                foreach (ISelectable obj in e.Items)
                    obj.SelectDefault(LevelGLControlModern);
            }
            else if (e.SelectionChangeMode == SelectionChangeMode.ADD)
            {
                foreach (ISelectable obj in e.Items)
                    obj.SelectDefault(LevelGLControlModern);
            }
            else //SelectionChangeMode.SUBTRACT
            {
                foreach (ISelectable obj in e.Items)
                    obj.DeselectAll(LevelGLControlModern);
            }

            e.Handled = true;
            LevelGLControlModern.Refresh();

            Scene_SelectionChanged(this, null);
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            if (currentScene.SelectedObjects.Count > 1)
            {
                lblCurrentObject.Text = "Multiple objects selected";
                string SelectedObjects = "";
                string previousobject = "";
                int multi = 1;

                List<string> selectedobjectnames = new List<string>();
                for (int i = 0; i < currentScene.SelectedObjects.Count; i++)
                    selectedobjectnames.Add(currentScene.SelectedObjects.ElementAt(i).ToString());

                selectedobjectnames.Sort();
                for (int i = 0; i < selectedobjectnames.Count; i++)
                {
                    string currentobject = selectedobjectnames[i];
                    if (previousobject == currentobject)
                    {
                        SelectedObjects = SelectedObjects.Remove(SelectedObjects.Length - (multi.ToString().Length + 1));
                        multi++;
                        SelectedObjects += $"x{multi}";
                    }
                    else if (multi > 1)
                    {
                        SelectedObjects += ", " + $"\"{currentobject}\"" + ", ";
                        multi = 1;
                    }
                    else
                    {
                        SelectedObjects += $"\"{currentobject}\"" + ", ";
                        multi = 1;
                    }
                    previousobject = currentobject;
                }
                SelectedObjects = multi > 1 ? SelectedObjects+"." : SelectedObjects.Remove(SelectedObjects.Length-2) + ".";
                SpotlightToolStripStatusLabel.Text = $"Selected {SelectedObjects}";
            }
            else if (currentScene.SelectedObjects.Count == 0)
                lblCurrentObject.Text = SpotlightToolStripStatusLabel.Text = "Nothing selected.";
            else
            {
                lblCurrentObject.Text = currentScene.SelectedObjects.First().ToString() + " selected";
                SpotlightToolStripStatusLabel.Text = $"Selected \"{currentScene.SelectedObjects.First().ToString()}\".";
            }
            MainSceneListView.Refresh();

            currentScene.SetupObjectUIControl(ObjectUIControl);
        }

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            General3dWorldObject obj = (currentScene.SelectedObjects.First() as General3dWorldObject);
            Rail rail = (currentScene.SelectedObjects.First() as Rail);
            Debugger.Break();
        }

        #region Toolstrip Items

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = 
                "Level Files (Map)|*Map1.szs|" +
                "Level Files (Design)|*Design1.szs|" +
                "Level Files (Sound)|*Sound1.szs|" +
                "All Level Files|*Map1.szs;*Design1.szs;*Sound1.szs",
                InitialDirectory = currentScene?.EditZone.Directory ?? Program.StageDataPath };

            SpotlightToolStripStatusLabel.Text = "Waiting...";

            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName != "")
                LoadLevel(ofd.FileName);
            else
                SpotlightToolStripStatusLabel.Text = "Open Cancelled";
        }

        private void Scene_ObjectsMoved(object sender, EventArgs e)
        {
            ObjectUIControl.Refresh();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;

            currentScene.Save();
            SpotlightToolStripStatusLabel.Text = "Level saved!";
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;

            if (currentScene.SaveAs())
                SpotlightToolStripStatusLabel.Text = "Level saved!";
            else
                SpotlightToolStripStatusLabel.Text = "Save Cancelled or Failed.";
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm SF = new SettingsForm() { Home = this };
            SF.ShowDialog();
            SpotlightToolStripStatusLabel.Text = "Settings Saved.";
        }

        //----------------------------------------------------------------------------

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;
            currentScene.Undo();
            LevelGLControlModern.Refresh();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;
            currentScene.Redo();
            LevelGLControlModern.Refresh();
        }

        #endregion

        private void LevelParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Program.GamePath + "\\SystemData") && File.Exists(Program.GamePath + "\\SystemData\\StageList.szs"))
            {
                LPF = new LevelParameterForm(currentScene == null ? "" : currentScene.ToString());
                LPF.Show();
            }
            else
            {
                MessageBox.Show("StageList.szs is missing from "+Program.GamePath+"\\SystemData", "Missing File",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void DuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene.SelectedObjects.Count == 0)
                SpotlightToolStripStatusLabel.Text = "Can't duplicate nothing!";
            else
            {
                string SelectedObjects = "";
                string previousobject = "";
                int multi = 1;

                List<string> selectedobjectnames = new List<string>();
                for (int i = 0; i < currentScene.SelectedObjects.Count; i++)
                    selectedobjectnames.Add(currentScene.SelectedObjects.ElementAt(i).ToString());

                selectedobjectnames.Sort();
                for (int i = 0; i < selectedobjectnames.Count; i++)
                {
                    string currentobject = selectedobjectnames[i];
                    if (previousobject == currentobject)
                    {
                        SelectedObjects = SelectedObjects.Remove(SelectedObjects.Length - (multi.ToString().Length + 1));
                        multi++;
                        SelectedObjects += $"x{multi}";
                    }
                    else if (multi > 1)
                    {
                        SelectedObjects += ", " + $"\"{currentobject}\"" + ", ";
                        multi = 1;
                    }
                    else
                    {
                        SelectedObjects += $"\"{currentobject}\"" + ", ";
                        multi = 1;
                    }
                    previousobject = currentobject;
                }
                SelectedObjects = multi > 1 ? SelectedObjects + "." : SelectedObjects.Remove(SelectedObjects.Length - 2) + ".";

                currentScene.DuplicateSelectedObjects();

                SpotlightToolStripStatusLabel.Text = $"Duplicated {SelectedObjects}";
            }
        }

        private void OpenExToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpotlightToolStripStatusLabel.Text = "Waiting...";
            StageList STGLST = new StageList(Program.GamePath + "\\SystemData\\StageList.szs");
            LevelParamSelectForm LPSF = new LevelParamSelectForm(STGLST);
            LPSF.ShowDialog();
            if (LPSF.levelname == "")
            {
                SpotlightToolStripStatusLabel.Text = "Open Cancelled";
                return;
            }
            LoadLevel($"{Program.GamePath}\\StageData\\{LPSF.levelname}Map1.szs");
        }

        private void LoadLevel(string Filename)
        {
            SpotlightToolStripProgressBar.Value = 0;
            SpotlightToolStripStatusLabel.Text = "Loading Level...";
            if (LevelIO.TryOpenLevel(Filename, this, out SM3DWorldScene scene))
            {
                currentScene = scene;
                SpotlightToolStripProgressBar.Value = 50;
                SetupScene(scene);

                documentTabControl1.AddTab(new DocumentTabControl.DocumentTab(scene.EditZone.LevelName, scene), true);

                MainSceneListView.Enabled = true;
                MainSceneListView.SetRootList("ObjectList");
                MainSceneListView.ListExited += MainSceneListView_ListExited;
                MainSceneListView.Refresh();
                SpotlightToolStripProgressBar.Value = 100;

                SpotlightToolStripStatusLabel.Text = $"\"{scene.ToString()}\" has been Loaded successfully.";
            }
        }

        private void SetupScene(SM3DWorldScene scene)
        {
            scene.SelectionChanged += Scene_SelectionChanged;
            scene.ListChanged += Scene_ListChanged;
            scene.ListEntered += Scene_ListEntered;
            scene.ObjectsMoved += Scene_ObjectsMoved;

            ObjectUIControl.ClearObjectUIContainers();
            ObjectUIControl.Refresh();
        }

        private void Scene_ListEntered(object sender, ListEventArgs e)
        {
            MainSceneListView.EnterList(e.List);
        }

        private void EditObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene.GetType() != typeof(SM3DWorldScene))
            {
                currentScene = currentScene.ConvertToOtherSceneType<SM3DWorldScene>();
                SetupScene(currentScene);
                LevelGLControlModern.MainDrawable = currentScene;
                LevelGLControlModern.Refresh();
            }
        }

        private void EditLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene.GetType() != typeof(LinkEdit3DWScene))
            {
                currentScene = currentScene.ConvertToOtherSceneType<LinkEdit3DWScene>();
                SetupScene(currentScene);
                LevelGLControlModern.MainDrawable = currentScene;
                LevelGLControlModern.Refresh();
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;

            string DeletedObjects = "";
            for (int i = 0; i < currentScene.SelectedObjects.Count; i++)
                DeletedObjects += currentScene.SelectedObjects.ElementAt(i).ToString() + (i + 1 == currentScene.SelectedObjects.Count ? "." : ", ");
            SpotlightToolStripStatusLabel.Text = $"Deleted {DeletedObjects}";

            currentScene.DeleteSelected();
            LevelGLControlModern.Refresh();
            MainSceneListView.UpdateAutoScrollHeight();
            Scene_SelectionChanged(this, null);
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;

            foreach (I3dWorldObject obj in currentScene.Objects)
                obj.SelectAll(LevelGLControlModern);

            LevelGLControlModern.Refresh();
            MainSceneListView.Refresh();
        }

        private void DeselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene == null)
                return;

            foreach (I3dWorldObject obj in currentScene.Objects)
                obj.DeselectAll(LevelGLControlModern);

            LevelGLControlModern.Refresh();
            MainSceneListView.Refresh();
        }

        private void MainSceneListView_ItemDoubleClicked(object sender, ItemDoubleClickedEventArgs e)
        {
            if (e.Item is I3dWorldObject obj)
                currentScene.FocusOn(obj);
        }

        private void AddObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjectDatabase == null)
            {
                MessageBox.Show(
@"Y o u  c h o s e  n o t  t o  g e n e r a t e
a  v a l i d  d a t a b a s e  r e m e m b e r ?
= )");

                DialogResult DR = MessageBox.Show("The Database is invalid, and you cannot add objects without one. Would you like to generate one from your SM3DW Files?","Invalid Database", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (DR == DialogResult.Yes)
                {
                    ObjectDatabase = new ObjectParameterDatabase();
                    ObjectDatabase.Create(Program.StageDataPath);
                    ObjectDatabase.Save("ParameterDatabase.sopd");
                }
                MessageBox.Show("Database Created", "Operation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AddObjectForm AOF = new AddObjectForm(ObjectDatabase);
            if (AOF.ShowDialog() == DialogResult.OK)
            {
                currentScene.ObjectPlaceDelegate = (p, z) =>
                {
                    var entry = ObjectDatabase.ObjectParameters[0];
                    return new (I3dWorldObject, ObjectList)[] { (
                        entry.ToGeneral3DWorldObject(z.NextObjID()), 
                        z.ObjLists[SM3DWorldZone.MAP_PREFIX + "ObjectList"]
                        )};
                };

                //@Super Hackio, now it's your turn to make it so it puts it in the right object list and in the right position
                //also make sure to actually USE the objID parameter in ToGeneral3DWorldObject()
            }
        }

        public void LevelZoneTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SM3DWorldZone zone = (SM3DWorldZone)e.Node.Tag;

            MainSceneListView.RootLists.Clear();

            foreach (KeyValuePair<string, ObjectList> keyValuePair in zone.ObjLists)
            {
                MainSceneListView.RootLists.Add(keyValuePair.Key, keyValuePair.Value);
            }

            MainSceneListView.UpdateComboBoxItems();

            MainSceneListView.SetRootList("ObjectList");
        }

        private void BtnEditIndividual_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void DocumentTabControl1_SelectedTabChanged(object sender, EventArgs e)
        {
            if (documentTabControl1.SelectedTab == null)
            {
                LevelGLControlModern.Visible = false;
                return;
            }

            currentScene = (SM3DWorldScene)documentTabControl1.SelectedTab.Tag;

            LevelGLControlModern.Visible = true;

            LevelGLControlModern.MainDrawable = currentScene;
        }

        private void DocumentTabControl1_TabClosing(object sender, HandledEventArgs e)
        {
            //TODO: ask to save unsaved changes
        }
    }
}
