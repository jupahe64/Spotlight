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
using System.Threading;

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
            MainTabControl.SelectedTab = ObjectsTabPage;
            LevelGLControlModern.CameraDistance = 20;

            MainSceneListView.SelectionChanged += MainSceneListView_SelectionChanged;
            MainSceneListView.ItemsMoved += MainSceneListView_ItemsMoved;
            MainSceneListView.ListExited += MainSceneListView_ListExited;

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

            if (!File.Exists(Program.SOPDPath))
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
                            Thread DatabaseGenThread = new Thread(() =>
                            {
                                LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                                LLF.ShowDialog();
                            });
                            DatabaseGenThread.Start();
                            ObjectDatabase = new ObjectParameterDatabase();
                            ObjectDatabase.Create(Program.StageDataPath);
                            ObjectDatabase.Save(Program.SOPDPath);
                            if (DatabaseGenThread.IsAlive)
                                LoadLevelForm.DoClose = true;
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
                ObjectDatabase = new ObjectParameterDatabase(Program.SOPDPath);
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
                                Thread DatabaseGenThread = new Thread(() =>
                                {
                                    LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                                    LLF.ShowDialog();
                                });
                                DatabaseGenThread.Start();
                                ObjectDatabase = new ObjectParameterDatabase();
                                ObjectDatabase.Create(Program.StageDataPath);
                                ObjectDatabase.Save(Program.SOPDPath);
                                if (DatabaseGenThread.IsAlive)
                                    LoadLevelForm.DoClose = true;
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
                CurrentObjectLabel.Text = "Multiple objects selected";
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
                SpotlightToolStripStatusLabel.Text = $"Selected {SelectedObjects}";
            }
            else if (currentScene.SelectedObjects.Count == 0)
                CurrentObjectLabel.Text = SpotlightToolStripStatusLabel.Text = "Nothing selected.";
            else
            {
                CurrentObjectLabel.Text = currentScene.SelectedObjects.First().ToString() + " selected";
                SpotlightToolStripStatusLabel.Text = $"Selected \"{currentScene.SelectedObjects.First().ToString()}\".";
            }
            MainSceneListView.Refresh();

            currentScene.SetupObjectUIControl(ObjectUIControl);
        }

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            if (currentScene == null || currentScene.SelectedObjects.Count == 0)
                return;
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
                OpenLevel(ofd.FileName);
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
            SettingsForm SF = new SettingsForm(this);
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
                MessageBox.Show("StageList.szs is missing from " + Program.GamePath + "\\SystemData", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LevelParamSelectForm LPSF = new LevelParamSelectForm(STGLST,true);
            LPSF.ShowDialog();
            if (LPSF.levelname == "")
            {
                SpotlightToolStripStatusLabel.Text = "Open Cancelled";
                return;
            }
            OpenLevel($"{Program.GamePath}\\StageData\\{LPSF.levelname}Map1.szs");
        }

        private void OpenLevel(string Filename)
        {

            if (SM3DWorldZone.TryOpen(Filename, out SM3DWorldZone zone))
            {
                Thread LoadingThread = new Thread((n) =>
                {
                    LoadLevelForm LLF = new LoadLevelForm(n.ToString(), "LoadLevel");
                    LLF.ShowDialog();
                });
                LoadingThread.Start(zone.LevelName);
                OpenZone(zone);
                if (LoadingThread.IsAlive)
                    LoadLevelForm.DoClose = true;
                SpotlightToolStripStatusLabel.Text = $"\"{zone.LevelName}\" has been Loaded successfully.";

                SetAppStatus(true);
            }
        }

        const string objectListName = SM3DWorldZone.MAP_PREFIX + "ObjectList";

        public void OpenZone(SM3DWorldZone zone)
        {
            SM3DWorldScene scene = new SM3DWorldScene(zone);

            AssignSceneEvents(scene);

            scene.EditZoneIndex = 0;

            //indirectly calls DocumentTabControl1_SelectedTabChanged
            //which already sets up a lot
            ZoneDocumentTabControl.AddTab(new DocumentTabControl.DocumentTab(zone.LevelName, scene), true);

            #region focus on player if it exists
            const string playerListName = SM3DWorldZone.MAP_PREFIX + "PlayerList";

            if (zone.ObjLists.ContainsKey(playerListName) && zone.ObjLists[playerListName].Count > 0)
            {
                scene.GL_Control.CamRotX = 0;
                scene.GL_Control.CamRotY = HALF_PI / 4;
                scene.FocusOn(zone.ObjLists[playerListName][0]);
            }
            #endregion

            #region setup UI
            MainSceneListView.Enabled = true;
            MainSceneListView.SetRootList(objectListName);
            MainSceneListView.Refresh();

            #endregion
        }

        private void AssignSceneEvents(SM3DWorldScene scene)
        {
            scene.SelectionChanged += Scene_SelectionChanged;
            scene.ListChanged += Scene_ListChanged;
            scene.ListEntered += Scene_ListEntered;
            scene.ObjectsMoved += Scene_ObjectsMoved;
            scene.ZonePlacementsChanged += Scene_ZonePlacementsChanged;
            scene.Reverted += Scene_Reverted;
            scene.IsSavedChanged += Scene_IsSavedChanged;
        }

        private void Scene_IsSavedChanged(object sender, EventArgs e)
        {
            ZoneDocumentTabControl.SelectedTab.Name = currentScene.ToString() + (currentScene.IsSaved ? string.Empty : "*");
            ZoneDocumentTabControl.Invalidate();
        }

        private void Scene_Reverted(object sender, RevertedEventArgs e)
        {
            UpdateZoneList();
            MainSceneListView.Refresh();
            currentScene.SetupObjectUIControl(ObjectUIControl);
        }

        private void Scene_ZonePlacementsChanged(object sender, EventArgs e)
        {
            UpdateZoneList();
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
                AssignSceneEvents(currentScene);
                LevelGLControlModern.MainDrawable = currentScene;
                LevelGLControlModern.Refresh();
            }
        }

        private void EditLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene.GetType() != typeof(LinkEdit3DWScene))
            {
                currentScene = currentScene.ConvertToOtherSceneType<LinkEdit3DWScene>();
                AssignSceneEvents(currentScene);
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
                    Thread DatabaseGenThread = new Thread(() =>
                    {
                        LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                        LLF.ShowDialog();
                    });
                    DatabaseGenThread.Start();
                    ObjectDatabase = new ObjectParameterDatabase();
                    ObjectDatabase.Create(Program.StageDataPath);
                    ObjectDatabase.Save(Program.SOPDPath);
                    if (DatabaseGenThread.IsAlive)
                        LoadLevelForm.DoClose = true;
                }
                MessageBox.Show("Database Created", "Operation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AddObjectForm AOF = new AddObjectForm(ObjectDatabase);
            if (AOF.ShowDialog() == DialogResult.OK)
            {
                currentScene.ObjectPlaceDelegate = (Position, Zone) =>
                {
                    Parameter entry = ObjectDatabase.ObjectParameters[AOF.SelectedIndex];
                    Category category = (Category)entry.CategoryID;

                    ObjectList objList;
                    if (category == Category.Link)
                        objList = Zone.LinkedObjects;
                    else
                        objList = Zone.ObjLists[SM3DWorldZone.MAP_PREFIX + category.ToString() + "List"];

                    General3dWorldObject currentobject = entry.ToGeneral3DWorldObject(Zone.NextObjID(), -Position);
                    currentobject.DoModelLoad(LevelGLControlModern);
                    return new (I3dWorldObject, ObjectList)[] { (currentobject, objList) };
                };
            }
        }

        enum Category : byte
        {
            Area,
            CheckPoint,
            Demo,
            Goal,
            Object,
            Player,
            Sky,
            Link
        }

        private void EditIndividualButton_Click(object sender, EventArgs e)
        {
            OpenZone((SM3DWorldZone)ZoneListBox.SelectedItem);
        }

        private void ZoneDocumentTabControl_SelectedTabChanged(object sender, EventArgs e)
        {
            if (ZoneDocumentTabControl.SelectedTab == null)
            {
                LevelGLControlModern.Visible = false;
                return;
            }

            currentScene = (SM3DWorldScene)ZoneDocumentTabControl.SelectedTab.Tag;

            #region setup UI
            ObjectUIControl.ClearObjectUIContainers();
            ObjectUIControl.Refresh();

            MainSceneListView.SelectedItems = currentScene.SelectedObjects;
            MainSceneListView.Refresh();

            UpdateZoneList();

            LevelGLControlModern.Visible = true;

            LevelGLControlModern.MainDrawable = currentScene;
            #endregion
        }

        private void UpdateZoneList()
        {
            ZoneListBox.BeginUpdate();
            ZoneListBox.Items.Clear();

            foreach (var item in currentScene.GetZones())
            {
                ZoneListBox.Items.Add(item);
            }

            int prevSel = ZoneListBox.SelectedIndex;

            ZoneListBox.SelectedIndex = currentScene.EditZoneIndex;

            if(prevSel== ZoneListBox.SelectedIndex)
                ZoneListBox_SelectedIndexChanged(null, null);

            ZoneListBox.EndUpdate();
        }

        private void ZoneDocumentTabControl_TabClosing(object sender, HandledEventArgs e)
        {
            SM3DWorldScene scene = (SM3DWorldScene)ZoneDocumentTabControl.SelectedTab.Tag;

            if (!scene.IsSaved)
            {
                switch(MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        scene.Save();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Handled = true;
                        return;
                }
            }

            if (ZoneDocumentTabControl.Tabs.Count == 1)
            {
                ZoneListBox.Items.Clear();
                MainSceneListView.UnselectCurrentList();
                MainSceneListView.RootLists.Clear();
                ObjectUIControl.ClearObjectUIContainers();
                ObjectUIControl.Refresh();

                SetAppStatus(false);
                string levelname = currentScene.EditZone.LevelName;
                currentScene = null;
                SpotlightToolStripStatusLabel.Text = $"{levelname} was closed.";
            }
        }

        private void ZoneListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SM3DWorldZone zone = (SM3DWorldZone)ZoneListBox.SelectedItem;

            currentScene.EditZoneIndex = ZoneListBox.SelectedIndex;

            MainSceneListView.RootLists.Clear();

            if (ZoneListBox.SelectedIndex == 0) //main zone selected
                MainSceneListView.RootLists.Add("Zones", currentScene.ZonePlacements);

            foreach (KeyValuePair<string, ObjectList> keyValuePair in zone.ObjLists)
            {
                MainSceneListView.RootLists.Add(keyValuePair.Key, keyValuePair.Value);
            }

            MainSceneListView.UpdateComboBoxItems();
            currentScene.SetupObjectUIControl(ObjectUIControl);
            LevelGLControlModern.Refresh();

            MainSceneListView.SetRootList(objectListName);
            MainSceneListView.Refresh();

            EditIndividualButton.Enabled = ZoneListBox.SelectedIndex > 0;
        }

        /// <summary>
        /// Sets the status of the application. This changes the Enabled property of controls that need it.
        /// </summary>
        /// <param name="Trigger">The state to set the Enabled property to</param>
        private void SetAppStatus(bool Trigger)
        {
            SaveToolStripMenuItem.Enabled = Trigger;
            SaveAsToolStripMenuItem.Enabled = Trigger;
            UndoToolStripMenuItem.Enabled = Trigger;
            RedoToolStripMenuItem.Enabled = Trigger;
            AddObjectToolStripMenuItem.Enabled = Trigger;
            DuplicateToolStripMenuItem.Enabled = Trigger;
            DeleteToolStripMenuItem.Enabled = Trigger;
            SelectAllToolStripMenuItem.Enabled = Trigger;
            DeselectAllToolStripMenuItem.Enabled = Trigger;
            ModeToolStripMenuItem.Enabled = Trigger;
            EditObjectsToolStripMenuItem.Enabled = Trigger;
            EditLinksToolStripMenuItem.Enabled = Trigger;
            EditIndividualButton.Enabled = Trigger;
            MainSceneListView.Enabled = Trigger;
        }
    }
}
