using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTK;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static GL_EditorFramework.Framework;
using SpotLight.ObjectParamDatabase;
using SpotLight.Level;
using System.Threading;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace SpotLight
{
    public partial class LevelEditorForm : Form
    {
        LevelParameterForm LPF;
        SM3DWorldScene currentScene;
        public LevelEditorForm()
        {
            InitializeComponent();
            MainTabControl.SelectedTab = ObjectsTabPage;
            LevelGLControlModern.CameraDistance = 20;

            MainSceneListView.SelectionChanged += MainSceneListView_SelectionChanged;
            MainSceneListView.ItemsMoved += MainSceneListView_ItemsMoved;
            MainSceneListView.ListExited += MainSceneListView_ListExited;

            LevelGLControlModern.Visible = false;
            Localize();

            //Properties.Settings.Default.Reset(); //Used when testing to make sure the below part works

            if (Program.GamePath == "")
            {
                DialogResult DR = MessageBox.Show(WelcomeMessageText, WelcomeMessageHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SpotlightToolStripStatusLabel.Text = StatusWelcomeMessage;
            }
            else
                SpotlightToolStripStatusLabel.Text = StatusWelcomeBackMessage;

            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = DatabasePickerTitle,
                IsFolderPicker = true
            };

            while (!Program.GamePathIsValid())
            {
                if (Program.GamePath != "")
                    MessageBox.Show(InvalidGamepathText, InvalidGamepathHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    DialogResult DR = MessageBox.Show(DatabaseMissingText, DatabaseMissingHeader, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

                    switch (DR)
                    {
                        case DialogResult.Yes:
                            Thread DatabaseGenThread = new Thread(() =>
                            {
                                LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                                LLF.ShowDialog();
                            });
                            DatabaseGenThread.Start();
                            Program.ParameterDB = new ObjectParameterDatabase();
                            Program.ParameterDB.Create(Program.BaseStageDataPath);
                            Program.ParameterDB.Save(Program.SOPDPath);
                            if (DatabaseGenThread.IsAlive)
                                LoadLevelForm.DoClose = true;
                            Breakout = true;
                            break;
                        case DialogResult.No:
                            DialogResult DR2 = MessageBox.Show(DatabaseExcludeText, DatabaseExcludeHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            switch (DR2)
                            {
                                case DialogResult.None:
                                case DialogResult.Yes:
                                    Program.ParameterDB = null;
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
                Program.ParameterDB = new ObjectParameterDatabase(Program.SOPDPath);
                ObjectParameterDatabase Ver = new ObjectParameterDatabase();

                if (Ver.Version > Program.ParameterDB.Version)
                {
                    bool Breakout = false;
                    while (!Breakout)
                    {
                        DialogResult DR = MessageBox.Show(string.Format(DatabaseOutdatedText, Program.ParameterDB.Version.ToString(), ObjectParameterDatabase.LatestVersion), DatabaseOutdatedHeader, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

                        switch (DR)
                        {
                            case DialogResult.Yes:
                                Thread DatabaseGenThread = new Thread(() =>
                                {
                                    LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                                    LLF.ShowDialog();
                                });
                                DatabaseGenThread.Start();
                                Program.ParameterDB = new ObjectParameterDatabase();
                                Program.ParameterDB.Create(Program.BaseStageDataPath);
                                Program.ParameterDB.Save(Program.SOPDPath);
                                if (DatabaseGenThread.IsAlive)
                                    LoadLevelForm.DoClose = true;
                                Breakout = true;
                                break;
                            case DialogResult.No:
                                DialogResult DR2 = MessageBox.Show(DatabaseExcludeText, DatabaseExcludeHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                                switch (DR2)
                                {
                                    case DialogResult.None:
                                    case DialogResult.Yes:
                                        Program.ParameterDB = null;
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

            if (CheckForUpdates(out Version useless, false))
            {
                AboutToolStripMenuItem.BackColor = System.Drawing.Color.LawnGreen;
                CheckForUpdatesToolStripMenuItem.BackColor = System.Drawing.Color.LawnGreen;
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

        private void Scene_ListChanged(object sender, ListChangedEventArgs e)
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
            ObjectUIControl.Refresh();
        }

        private void SplitContainer2_Panel2_Click(object sender, EventArgs e)
        {
            if (currentScene == null || currentScene.SelectedObjects.Count == 0)
                return;
            General3dWorldObject obj = (currentScene.SelectedObjects.First() as General3dWorldObject);
            Rail rail = (currentScene.SelectedObjects.First() as Rail);
            Debugger.Break();
        }

        private void Scene_ObjectsMoved(object sender, EventArgs e)
        {
            ObjectUIControl.Refresh();
        }

        #region Toolstrip Items

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter =
                "Level Files (Map)|*Map1.szs|" +
                "Level Files (Design)|*Design1.szs|" +
                "Level Files (Sound)|*Sound1.szs|" +
                "All Level Files|*Map1.szs;*Design1.szs;*Sound1.szs",
                InitialDirectory = currentScene?.EditZone.Directory ?? Program.BaseStageDataPath };

            SpotlightToolStripStatusLabel.Text = "Waiting...";

            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName != "")
                OpenLevel(ofd.FileName);
            else
                SpotlightToolStripStatusLabel.Text = "Open Cancelled";
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
            {
                SpotlightToolStripStatusLabel.Text = "Level saved!";
                ZoneDocumentTabControl.SelectedTab.Name = currentScene.ToString(); //level name might have changed
            }
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
                currentScene.SetupObjectUIControl(ObjectUIControl);
                MainSceneListView.Refresh();
                ObjectUIControl.Invalidate();

                SpotlightToolStripStatusLabel.Text = $"Duplicated {SelectedObjects}";
            }
        }

        private void OpenExToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string stagelistpath = Program.TryGetPathViaProject("SystemData", "StageList.szs");
            if (!File.Exists(stagelistpath))
            {
                MessageBox.Show($"StageList.szs Could not be found inside {stagelistpath}, so this feature cannot be used.", "404", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SpotlightToolStripStatusLabel.Text = "Open Failed";
                return;
            }
            SpotlightToolStripStatusLabel.Text = "Waiting...";
            StageList STGLST = new StageList(stagelistpath);
            LevelParamSelectForm LPSF = new LevelParamSelectForm(STGLST, true);
            LPSF.ShowDialog();
            if (LPSF.levelname.Equals(""))
            {
                SpotlightToolStripStatusLabel.Text = "Open Cancelled";
                return;
            }
            if (!Program.ProjectPath.Equals("") && !File.Exists(System.IO.Path.Combine(Program.ProjectPath, "StageData", $"{LPSF.levelname}Map1.szs")))
            {
                DialogResult DR = MessageBox.Show($"{LPSF.levelname} isn't inside your project directory. Would you like to create a copy in your project directory?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DR == DialogResult.Yes)
                {
                    File.Copy(System.IO.Path.Combine(Program.GamePath, "StageData", $"{LPSF.levelname}Map1.szs"), System.IO.Path.Combine(Program.ProjectPath, "StageData", $"{LPSF.levelname}Map1.szs"));
                    if (File.Exists(System.IO.Path.Combine(Program.GamePath, "StageData", $"{LPSF.levelname}Design1.szs")))
                        File.Copy(System.IO.Path.Combine(Program.GamePath, "StageData", $"{LPSF.levelname}Design1.szs"), System.IO.Path.Combine(Program.ProjectPath, "StageData", $"{LPSF.levelname}Design1.szs"), true);
                    if (File.Exists(System.IO.Path.Combine(Program.GamePath, "StageData", $"{LPSF.levelname}Sound1.szs")))
                        File.Copy(System.IO.Path.Combine(Program.GamePath, "StageData", $"{LPSF.levelname}Sound1.szs"), System.IO.Path.Combine(Program.ProjectPath, "StageData", $"{LPSF.levelname}Sound1.szs"), true);
                }
                else if (DR == DialogResult.Cancel)
                {
                    SpotlightToolStripStatusLabel.Text = "Open Cancelled";
                    return;
                }
            }
            OpenLevel(Program.TryGetPathViaProject("StageData", $"{LPSF.levelname}Map1.szs"));
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

        private void AddObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ParameterDB == null)
            {
//                MessageBox.Show(
//@"Y o u  c h o s e  n o t  t o  g e n e r a t e
//a  v a l i d  d a t a b a s e  r e m e m b e r ?
//= )"); //As much as I wish we could keep this, we can't.

                DialogResult DR = MessageBox.Show("The Database is invalid, and you cannot add objects without one. Would you like to generate one from your SM3DW Files?","Invalid Database", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (DR == DialogResult.Yes)
                {
                    Thread DatabaseGenThread = new Thread(() =>
                    {
                        LoadLevelForm LLF = new LoadLevelForm("GenDatabase", "GenDatabase");
                        LLF.ShowDialog();
                    });
                    DatabaseGenThread.Start();
                    Program.ParameterDB = new ObjectParameterDatabase();
                    Program.ParameterDB.Create(Program.BaseStageDataPath);
                    Program.ParameterDB.Save(Program.SOPDPath);
                    if (DatabaseGenThread.IsAlive)
                        LoadLevelForm.DoClose = true;
                }
                MessageBox.Show("Database Created", "Operation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            new AddObjectForm(currentScene, LevelGLControlModern).ShowDialog(this);
            AddObjectTimer.Start();
        }

        private void AddZoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentScene.EditZoneIndex != 0)
                return;

            AddZoneForm azf = new AddZoneForm();

            if (azf.ShowDialog() == DialogResult.Cancel)
                return;

            if (azf.SelectedFileName == null)
                return;

            if (SM3DWorldZone.TryOpen(System.IO.Path.Combine(Program.BaseStageDataPath,azf.SelectedFileName), out SM3DWorldZone zone))
            {
                currentScene.ZonePlacements.Add(new ZonePlacement(Vector3.Zero, Vector3.Zero, Vector3.One, zone));
                ZoneDocumentTabControl_SelectedTabChanged(null, null);
            }
        }

        private void MoveToLinkedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdditionManager additionManager = new AdditionManager();
            DeletionManager deletionManager = new DeletionManager();

            var linkedObjs = currentScene.EditZone.LinkedObjects;
            
            foreach (var objList in currentScene.EditZone.ObjLists.Values)
            {
                var selected = objList.Where(x => x.IsSelectedAll()).ToArray();

                if (selected.Length == 0)
                    continue;

                additionManager.Add(linkedObjs, selected);
                deletionManager.Add(objList, selected);
            }
            currentScene.BeginUndoCollection();
            currentScene.ExecuteAddition(additionManager);
            currentScene.ExecuteDeletion(deletionManager);
            currentScene.EndUndoCollection();
            SpotlightToolStripStatusLabel.Text = "Moved to the Link List";
        }

        private void MoveToAppropriateListsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdditionManager additionManager = new AdditionManager();
            DeletionManager deletionManager = new DeletionManager();

            var linkedObjs = currentScene.EditZone.LinkedObjects;

            foreach (var objList in currentScene.EditZone.ObjLists.Values)
            {
                var selected = objList.Where(x => x.IsSelectedAll()).ToArray();

                if (selected.Length == 0)
                    continue;

                for (int i = 0; i < selected.Length; i++)
                {
                    if(!selected[i].TryGetObjectList(currentScene.EditZone, out ObjectList _objList))
                        _objList = linkedObjs;


                    if (objList != _objList)
                    {
                        additionManager.Add(_objList, selected[i]);
                        deletionManager.Add(objList, selected[i]);
                    }
                }
            }

            {
                var objList = linkedObjs;

                var selected = objList.Where(x => x.IsSelectedAll()).ToArray();

                if (selected.Length != 0)
                {
                    for (int i = 0; i < selected.Length; i++)
                    {
                        if (!selected[i].TryGetObjectList(currentScene.EditZone, out ObjectList _objList))
                            _objList = linkedObjs;


                        if (objList != _objList)
                        {
                            additionManager.Add(_objList, selected[i]);
                            deletionManager.Add(objList, selected[i]);
                        }
                    }
                }
            }

            currentScene.BeginUndoCollection();
            currentScene.ExecuteAddition(additionManager);
            currentScene.ExecuteDeletion(deletionManager);
            currentScene.EndUndoCollection();
            SpotlightToolStripStatusLabel.Text = "Moved to the Appropriate List";
        }

        private void SpotlightWikiToolStripMenuItem_Click(object sender, EventArgs e) => Process.Start("https://github.com/jupahe64/Spotlight/wiki");

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForUpdates(out Version Latest, true))
            {
                if (MessageBox.Show($"Spotlight Version {Latest.ToString()} is currently available for download! Would you like to visit the Download Page?", "Update Ready!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    Process.Start("https://github.com/jupahe64/Spotlight/releases");
            }
            else if (!Latest.Equals(new Version(0, 0, 0, 0)))
                MessageBox.Show($"You are using the Latest version of Spotlight ({new Version(Application.ProductVersion).ToString()})", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
        
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

        public void OpenZone(SM3DWorldZone zone)
        {
            SM3DWorldScene scene = new SM3DWorldScene(zone);

            AssignSceneEvents(scene);

            scene.EditZoneIndex = 0;

            //indirectly calls ZoneDocumentTabControl_SelectedTabChanged
            //which already sets up a lot
            ZoneDocumentTabControl.AddTab(new DocumentTabControl.DocumentTab(zone.LevelName, scene), true);

            Scene_IsSavedChanged(null, null);

            #region focus on player if it exists
            const string playerListName = SM3DWorldZone.MAP_PREFIX + "PlayerList";

            if (zone.ObjLists.ContainsKey(playerListName) && zone.ObjLists[playerListName].Count > 0)
            {
                scene.GL_Control.CamRotX = 0;
                scene.GL_Control.CamRotY = HALF_PI / 4;
                scene.FocusOn(zone.ObjLists[playerListName][0]);
            }
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
            foreach (var tab in ZoneDocumentTabControl.Tabs)
            {
                SM3DWorldScene scene = (SM3DWorldScene)tab.Document;

                tab.Name = scene.ToString() + (scene.IsSaved ? string.Empty : "*");
            }

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

        private void MainSceneListView_ItemDoubleClicked(object sender, ItemDoubleClickedEventArgs e)
        {
            if (e.Item is I3dWorldObject obj)
                currentScene.FocusOn(obj);
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

            currentScene = (SM3DWorldScene)ZoneDocumentTabControl.SelectedTab.Document;

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

        private void ZoneDocumentTabControl_TabClosing(object sender, DocumentTabClosingEventArgs e)
        {
            SM3DWorldScene scene = (SM3DWorldScene)e.Tab.Document;

            #region find out which zones will remain after the zones in this scene gets unloaded
            HashSet<SM3DWorldZone> remainingZones = new HashSet<SM3DWorldZone>();

            foreach(var tab in ZoneDocumentTabControl.Tabs)
            {
                if (tab == e.Tab)
                    continue; //wont remain

                foreach (var zone in ((SM3DWorldScene)tab.Document).GetZones())
                    remainingZones.Add(zone);
            }
            #endregion

            #region find out which zones need to get unloaded
            HashSet<SM3DWorldZone> unsavedZones = new HashSet<SM3DWorldZone>();

            HashSet<SM3DWorldZone> zonesToUnload = new HashSet<SM3DWorldZone>();

            foreach (var zone in scene.GetZones())
            {
                if (!remainingZones.Contains(zone))
                {
                    zonesToUnload.Add(zone);

                    if (!zone.IsSaved && !remainingZones.Contains(zone))
                        unsavedZones.Add(zone);
                }
            }
            #endregion

            #region ask to save unsaved changes
            if (unsavedZones.Count>0)
            {
                StringBuilder sb = new StringBuilder("You have unsaved changes in:\n");

                foreach (var zone in unsavedZones)
                    sb.AppendLine(zone.LevelFileName);

                sb.AppendLine("Do you want to save them ? ");

                switch (MessageBox.Show(sb.ToString(), "Unsaved Changes!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        foreach (var zone in unsavedZones)
                            zone.Save();

                        Scene_IsSavedChanged(null, null);
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            #endregion

            //unload all zones that need to be unloaded
            foreach (var zone in zonesToUnload)
                zone.Unload();


            #region disable all controls if this is the last tab
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
            #endregion
        }

        private void ZoneListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SM3DWorldZone zone = (SM3DWorldZone)ZoneListBox.SelectedItem;

            currentScene.EditZoneIndex = ZoneListBox.SelectedIndex;

            MainSceneListView.RootLists.Clear();

            MainSceneListView.RootLists.Add("Common_Linked", currentScene.EditZone.LinkedObjects);

            if (ZoneListBox.SelectedIndex == 0) //main zone selected
                MainSceneListView.RootLists.Add("Common_ZoneList", currentScene.ZonePlacements);

            foreach (KeyValuePair<string, ObjectList> keyValuePair in zone.ObjLists)
            {
                MainSceneListView.RootLists.Add(keyValuePair.Key, keyValuePair.Value);
            }

            MainSceneListView.UpdateComboBoxItems();
            currentScene.SetupObjectUIControl(ObjectUIControl);
            LevelGLControlModern.Refresh();

            if(zone.HasCategoryMap)
                MainSceneListView.SetRootList(SM3DWorldZone.MAP_PREFIX+"ObjectList");
            else if(zone.HasCategoryDesign)
                MainSceneListView.SetRootList(SM3DWorldZone.DESIGN_PREFIX + "ObjectList");
            else
                MainSceneListView.SetRootList(SM3DWorldZone.SOUND_PREFIX + "ObjectList");

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
            AddZoneToolStripMenuItem.Enabled = Trigger;
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

        private void LevelEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ZoneDocumentTabControl.TryClearTabs();
        }

        private void AddObjectTimer_Tick(object sender, EventArgs e)
        {
            MainSceneListView.Refresh();
            if (currentScene.ObjectPlaceDelegate == null)
                AddObjectTimer.Stop();
        }


        /// <summary>
        /// Checks for Updates
        /// </summary>
        /// <returns></returns>
        public static bool CheckForUpdates(out Version Latest, bool ShowError = false)
        {
            System.Net.WebClient Client = new System.Net.WebClient();
            Latest = null;
            try
            {
                Client.DownloadFile("https://raw.githubusercontent.com/jupahe64/Spotlight/master/SpotLight/LatestVersion.txt", @AppDomain.CurrentDomain.BaseDirectory + "VersionCheck.txt");
            }
            catch (Exception e)
            {
                if (ShowError)
                    MessageBox.Show($"Failed to retrieve update information.\n{e.Message}", "Connection Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning); //No internet lol
                Latest = new Version(0, 0, 0, 0);
                return false;
            }
            if (File.Exists(@AppDomain.CurrentDomain.BaseDirectory + "VersionCheck.txt"))
            {
                Version Internet = new Version(File.ReadAllText(@AppDomain.CurrentDomain.BaseDirectory + "VersionCheck.txt"));
                File.Delete(@AppDomain.CurrentDomain.BaseDirectory + "VersionCheck.txt");
                Version Local = new Version(Application.ProductVersion);
                if (Local.CompareTo(Internet) < 0)
                {
                    Latest = Internet;
                    return true;
                }
                else
                {
                    Latest = Local;
                    return false;
                }
            }
            else return false;
        }

        #region Translations
        public void Localize()
        {
            WelcomeMessageHeader = Program.CurrentLanguage.GetTranslation("WelcomeMessageHeader") ?? "Introduction";
            WelcomeMessageText = Program.CurrentLanguage.GetTranslation("WelcomeMessageText") ??
@"Welcome to Spotlight!

In order to use this program, you will need the folders ""StageData"" and ""ObjectData"" from Super Mario 3D World

Please select the folder than contains these folders";
            StatusWelcomeMessage = Program.CurrentLanguage.GetTranslation("StatusWelcomeMessage") ?? "Welcome to Spotlight!";
            StatusWelcomeBackMessage = Program.CurrentLanguage.GetTranslation("StatusWelcomeBackMessage") ?? "Welcome back!";
            DatabasePickerTitle = Program.CurrentLanguage.GetTranslation("DatabasePickerTitle") ?? "Select the Game Directory of Super Mario 3D World";
            InvalidGamepathText = Program.CurrentLanguage.GetTranslation("InvalidGamepathText") ?? "The Directory doesn't contain ObjectData and StageData.";
            InvalidGamepathHeader = Program.CurrentLanguage.GetTranslation("InvalidGamepathHeader") ?? "The GamePath is invalid";
            DatabaseMissingText = Program.CurrentLanguage.GetTranslation("DatabaseMissingText") ??
@"Spotlight could not find the Object Parameter Database (ParameterDatabase.sopd)

Spotlight needs an Object Parameter Database in order for you to add objects.

Would you like to generate a new object Database from your 3DW Directory?";
            DatabaseMissingHeader = Program.CurrentLanguage.GetTranslation("DatabaseMissinHeader") ?? "Database Missing";
            DatabaseExcludeText = Program.CurrentLanguage.GetTranslation("DatabaseExcludeText") ?? "Are you sure? You cannot add objects without it.";
            DatabaseExcludeHeader = Program.CurrentLanguage.GetTranslation("DatabaseExcludeHeader") ?? "Confirmation";
            DatabaseOutdatedHeader = Program.CurrentLanguage.GetTranslation("DatabaseOutdatedText") ??
@"The Loaded Database is outdated ({0}).
The latest Database version is {1}.
Would you like to rebuild the database from your 3DW Files?";
            DatabaseOutdatedHeader = Program.CurrentLanguage.GetTranslation("DatabaseOutdatedHeader") ?? "Database Outdated";
        }

        private string WelcomeMessageHeader { get; set; }
        private string WelcomeMessageText { get; set; }
        private string StatusWelcomeMessage { get; set; }
        private string StatusWelcomeBackMessage { get; set; }
        private string DatabasePickerTitle { get; set; }
        private string InvalidGamepathHeader { get; set; }
        private string InvalidGamepathText { get; set; }
        private string DatabaseMissingHeader { get; set; }
        private string DatabaseMissingText { get; set; }
        private string DatabaseExcludeHeader { get; set; }
        private string DatabaseExcludeText { get; set; }
        private string DatabaseOutdatedHeader { get; set; }
        private string DatabaseOutdatedText { get; set; }

        #endregion

    }
}
