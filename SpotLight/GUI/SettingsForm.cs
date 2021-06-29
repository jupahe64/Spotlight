using Microsoft.WindowsAPICodePack.Dialogs;
using Spotlight.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GL_EditorFramework;
using System.Threading.Tasks;
using Spotlight.ObjectRenderers;

namespace Spotlight.GUI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(LevelEditorForm home)
        {
            Home = home;
            InitializeComponent();
            CenterToParent();
            Localize();

            if (Properties.Settings.Default.GamePaths == null)
                Properties.Settings.Default.GamePaths = new StringCollection();

            if (Properties.Settings.Default.ProjectPaths == null)
                Properties.Settings.Default.ProjectPaths = new StringCollection();

            if (!string.IsNullOrEmpty(Program.GamePath))
                Properties.Settings.Default.GamePaths.AddUnique(Program.GamePath);

            if (!string.IsNullOrEmpty(Program.ProjectPath))
                Properties.Settings.Default.ProjectPaths.AddUnique(Program.ProjectPath);

            Properties.Settings.Default.Save();

            GamePathTextBox.Text = Program.GamePath;
            GamePathTextBox.PossibleSuggestions = Properties.Settings.Default.GamePaths.ToArray();

            ProjectPathTextBox.Text = Program.ProjectPath;
            ProjectPathTextBox.PossibleSuggestions = Properties.Settings.Default.ProjectPaths.ToArray();

            #region Databases

            string ver = SettingsInvalid;
            if (File.Exists(Program.SOPDPath))
            {
                FileStream FS = new FileStream(Program.SOPDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SOPD")
                    throw new Exception("Invalid Database File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Program.ParameterDB != null ? Program.ParameterDB.Version.ToString() : Check.ToString() + $" ({DatabaseOutdated})";
                FS.Close();
            }
            DateTime date = File.GetLastWriteTime(Program.SOPDPath);
            DatabaseInfoLabel.Text = DatabaseVersionBase.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? FileDoesntExist : date.ToLongDateString()).Replace("[VER]", ver);

            ver = SettingsInvalid;
            if (File.Exists(Program.SODDPath))
            {
                FileStream FS = new FileStream(Program.SODDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SODD")
                    throw new Exception("Invalid Description File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Check.Equals(Database.ObjectInformationDatabase.LatestVersion) ? Database.ObjectInformationDatabase.LatestVersion.ToString() : Check.ToString() + $" ({DatabaseOutdated})";
                FS.Close();
            }
            else
                ClearDescriptionsButton.Enabled = false;
            date = File.GetLastWriteTime(Program.SODDPath);
            DescriptionInfoLabel.Text = DescriptionVersionBase.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? FileDoesntExist : date.ToLongDateString()).Replace("[VER]", ver);

            #endregion

            RenderAreaCheckBox.Checked = Properties.Settings.Default.DrawAreas;
            RenderSkyboxesCheckBox.Checked = Properties.Settings.Default.DrawSkyBoxes;
            RenderTransparentWallsCheckBox.Checked = Properties.Settings.Default.DrawTransparentWalls;

            switch (Properties.Settings.Default.PlayerChoice)
            {
                case "None":
                    PlayerComboBox.SelectedIndex = 0;
                    break;
                case "Mario":
                    PlayerComboBox.SelectedIndex = 1;
                    break;
                case "Luigi":
                    PlayerComboBox.SelectedIndex = 2;
                    break;
                case "Peach":
                    PlayerComboBox.SelectedIndex = 3;
                    break;
                case "Toad":
                    PlayerComboBox.SelectedIndex = 4;
                    break;
                case "Rosalina":
                    PlayerComboBox.SelectedIndex = 5;
                    break;
            }

            UniqueIDsCheckBox.Checked = Properties.Settings.Default.UniqueIDs;
            DoNotLoadTexturesCheckBox.Checked = Properties.Settings.Default.DoNotLoadTextures;
            IDEditingCheckBox.Checked = Properties.Settings.Default.AllowIDEdits;
            if (!Directory.Exists(Program.LanguagePath))
            {
                LanguageComboBox.Enabled = false;
                LanguageComboBox.SelectedIndex = 0;
            }
            else
            {
                string[] Files = Directory.GetFiles(Program.LanguagePath);
                int langcount = Files.Length;
                for (int i = 0; i < langcount; i++)
                {
                    if (!LanguageComboBox.Items.Contains(Files[i].Replace(Program.LanguagePath + "\\", "").Replace(".txt", "")))
                        LanguageComboBox.Items.Add(Files[i].Replace(Program.LanguagePath + "\\", "").Replace(".txt", ""));
                }

                if (File.Exists(Path.Combine(Program.LanguagePath, Properties.Settings.Default.Language + ".txt")))
                {
                    for (int i = 0; i < LanguageComboBox.Items.Count; i++)
                    {
                        if (LanguageComboBox.Items[i].Equals(Properties.Settings.Default.Language))
                        {
                            LanguageComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                    LanguageComboBox.SelectedIndex = 0;
            }

            string tmp = Properties.Settings.Default.SplashSize.Width + "x" + Properties.Settings.Default.SplashSize.Height;
            SplashSizeComboBox.SelectedItem = tmp;
        }

        private LevelEditorForm Home;
        private bool Loading = false;

        private void RebuildDatabaseButton_Click(object sender, EventArgs e)
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
            DatabaseInfoLabel.Text = DatabaseVersionBase.Replace("[DATABASEGENDATE]", File.GetLastWriteTime(Program.SOPDPath).ToLongDateString()).Replace("[VER]", Program.ParameterDB.Version.ToString());
            MessageBox.Show(DatabaseRebuildSuccessText, DatabaseRebuildSuccessHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearDescriptionsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(DescriptionClearText, DescriptionClearHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                File.Delete(Program.SODDPath);
                DescriptionInfoLabel.Text = DescriptionVersionBase.Replace("[DATABASEGENDATE]", FileDeleted).Replace("[VER]", SettingsInvalid);
                MessageBox.Show(DescriptionClearSuccessText, DescriptionClearSuccessHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearDescriptionsButton.Enabled = false;
            }
        }

        private void RenderAreaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrawAreas = RenderAreaCheckBox.Checked;
            Properties.Settings.Default.Save();
            Home.LevelGLControlModern.Refresh();
        }

        private void RenderSkyboxesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrawSkyBoxes = RenderSkyboxesCheckBox.Checked;
            Properties.Settings.Default.Save();
            Home.LevelGLControlModern.Refresh();
        }

        private void RenderTransparentWallsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrawTransparentWalls = RenderTransparentWallsCheckBox.Checked;
            Properties.Settings.Default.Save();
            Home.LevelGLControlModern.Refresh();
        }

        private void PlayerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || PlayerComboBox.SelectedItem.ToString().Equals(Properties.Settings.Default.PlayerChoice))
                return;

            switch (PlayerComboBox.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.PlayerChoice = "None";
                    break;
                case 1:
                    Properties.Settings.Default.PlayerChoice = "Mario";
                    break;
                case 2:
                    Properties.Settings.Default.PlayerChoice = "Luigi";
                    break;
                case 3:
                    Properties.Settings.Default.PlayerChoice = "Peach";
                    break;
                case 4:
                    Properties.Settings.Default.PlayerChoice = "Toad";
                    break;
                case 5:
                    Properties.Settings.Default.PlayerChoice = "Rosalina";
                    break;
            }
            Properties.Settings.Default.Save();
            if (BfresModelRenderer.Contains("CheckpointFlag"))
            {
                BfresModelRenderer.ReloadModel("CheckpointFlag");
                Home.LevelGLControlModern.Refresh();
            }
        }

        private void UniqueIDsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UniqueIDs = UniqueIDsCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void DoNotLoadTexturesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DoNotLoadTextures = DoNotLoadTexturesCheckBox.Checked;
            Properties.Settings.Default.Save();
        }


        private void ResetSpotlightButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(ResetWarningText, ResetWarningHeader,MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (File.Exists(Program.SOPDPath))
                    File.Delete(Program.SOPDPath);
                if (File.Exists(Program.SODDPath))
                    File.Delete(Program.SODDPath);
                Properties.Settings.Default.Reset();
                Environment.FailFast("It was nice knowing you...");
            }
        }

        private void IDEditingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AllowIDEdits = IDEditingCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void GamePathButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = DatabasePickerTitle,
                IsFolderPicker = true
            };

            string lastValidPath = GamePathTextBox.Text;

            while (true)
            {
                if (ofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    GamePathTextBox.Text = lastValidPath; //reset the path to make sure it's still valid
                    return;
                }
                else
                {
                    GamePathTextBox.Text = ofd.FileName;

                    CancelEventArgs cancelEventArgs = new CancelEventArgs();

                    //will set everything 
                    GamePathTextBox_ValueEntered(null, cancelEventArgs);

                    if (cancelEventArgs.Cancel)
                        MessageBox.Show(InvalidFolder, InvalidGamePath);
                    else
                        return;
                }
            }
        }

        private void GamePathTextBox_ValueEntered(object sender, CancelEventArgs e)
        {
            string path = GamePathTextBox.Text;

            if (!Program.IsGamePathValid(path))
            {
                e.Cancel = true;
                return;
            }

            Program.GamePath = path;

            Properties.Settings.Default.GamePaths.AddUnique(GamePathTextBox.Text);

            GamePathTextBox.PossibleSuggestions = Properties.Settings.Default.GamePaths.ToArray();

            Properties.Settings.Default.Save();
        }

        private void ProjectPathButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = DatabasePickerTitle,
                IsFolderPicker = true
            };

            string lastValidPath = ProjectPathTextBox.Text;

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ProjectPathTextBox.Text = ofd.FileName;

                CancelEventArgs cancelEventArgs = new CancelEventArgs();

                //will set everything and show a message box if it's invalid
                ProjectPathTextBox_ValueEntered(null, cancelEventArgs);

                if (cancelEventArgs.Cancel)
                    ProjectPathTextBox.Text = lastValidPath; //reset the path to make sure it's still valid
            }
        }

        private void ProjectPathTextBox_ValueEntered(object sender, CancelEventArgs e)
        {
            string path = ProjectPathTextBox.Text;

            if(string.IsNullOrEmpty(path))
            {
                //empty project paths are valid but we don't want to save them
                Program.ProjectPath = path;
                return;
            }


            if (!Program.IsGamePathValid(path))
            {
                if(MessageBox.Show("The entered path doesn't contain an ObjectData or StageData folder, would you like to create them?","Missing ObjectData / StageData",MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    Program.MakeGamePathValid(path);
                }
            }

            Program.ProjectPath = path;

            Properties.Settings.Default.ProjectPaths.AddUnique(path);

            ProjectPathTextBox.PossibleSuggestions = Properties.Settings.Default.ProjectPaths.ToArray();

            Properties.Settings.Default.Save();
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LanguageComboBox.SelectedItem.ToString().Equals(Properties.Settings.Default.Language))
                return;

            Properties.Settings.Default.Language = LanguageComboBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            Program.CurrentLanguage = File.Exists(Path.Combine(Program.LanguagePath, Properties.Settings.Default.Language + ".txt")) ? new Language(Properties.Settings.Default.Language, Path.Combine(Program.LanguagePath, Properties.Settings.Default.Language + ".txt")) : new Language();

            Localize();
            Home.Localize();
        }

        #region Translations
        public void Localize()
        {
            this.Localize(
                GamePathLabel,
                ProjectPathLabel,
                ObjectParameterGroupBox,
                RebuildDatabaseButton,
                ClearDescriptionsButton,
                RenderingGroupBox,
                RenderAreaCheckBox,
                RenderSkyboxesCheckBox,
                PlayerLabel,

                LoadingAndSavingGroupBox,
                UniqueIDsCheckBox,
                EditingGroupBox,
                IDEditingCheckBox,
                MiscellaneousGroupBox,
                LanguageLabel,
                SplashSizeLabel,
                SplashTestButton,
                ResetSpotlightButton,

                LoadingAndSavingGroupBox,
                UniqueIDsCheckBox,
                EditingGroupBox,
                IDEditingCheckBox,
                MiscellaneousGroupBox,
                LanguageLabel,
                SplashSizeLabel,
                SplashTestButton,
                ResetSpotlightButton
            );

            Loading = true;
            int tempplayerid = PlayerComboBox.SelectedIndex;
            PlayerComboBox.Items.Clear();
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerNone") ?? "None");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerMario") ?? "Mario");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerLuigi") ?? "Luigi");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerPeach") ?? "Peach");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerToad") ?? "Toad");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerRosalina") ?? "Rosalina");
            PlayerComboBox.SelectedIndex = tempplayerid;
            Loading = false;

            #region Databases
            string ver = SettingsInvalid;
            if (File.Exists(Program.SOPDPath))
            {
                FileStream FS = new FileStream(Program.SOPDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SOPD")
                    throw new Exception("Invalid Database File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Program.ParameterDB != null ? Program.ParameterDB.Version.ToString() : Check.ToString() + $" ({DatabaseOutdated})";
                FS.Close();
            }
            DateTime date = File.GetLastWriteTime(Program.SOPDPath);
            DatabaseInfoLabel.Text = DatabaseVersionBase.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? FileDoesntExist : date.ToLongDateString()).Replace("[VER]", ver);

            ver = SettingsInvalid;
            if (File.Exists(Program.SODDPath))
            {
                FileStream FS = new FileStream(Program.SODDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SODD")
                    throw new Exception("Invalid Description File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Check.Equals(Spotlight.Database.ObjectInformationDatabase.LatestVersion) ? Spotlight.Database.ObjectInformationDatabase.LatestVersion.ToString() : Check.ToString() + $" ({DatabaseOutdated})";
                FS.Close();
            }
            else
                ClearDescriptionsButton.Enabled = false;
            date = File.GetLastWriteTime(Program.SODDPath);
            DescriptionInfoLabel.Text = DescriptionVersionBase.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? FileDoesntExist : date.ToLongDateString()).Replace("[VER]", ver);
            #endregion

            switch (PlayerComboBox.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.PlayerChoice = "None";
                    break;
                case 1:
                    Properties.Settings.Default.PlayerChoice = "Mario";
                    break;
                case 2:
                    Properties.Settings.Default.PlayerChoice = "Luigi";
                    break;
                case 3:
                    Properties.Settings.Default.PlayerChoice = "Peach";
                    break;
                case 4:
                    Properties.Settings.Default.PlayerChoice = "Toad";
                    break;
                case 5:
                    Properties.Settings.Default.PlayerChoice = "Rosalina";
                    break;
            }
        }

        [Program.Localized]
        string DatabaseVersionBase = "Database Last Built on: [DATABASEGENDATE].    Version: [VER]";
        [Program.Localized]
        string DescriptionVersionBase = "Descriptions Last Edited on: [DATABASEGENDATE].    Version: [VER]";
        [Program.Localized]
        string FileDoesntExist = "N/A (File doesn't exist)";
        [Program.Localized]
        string FileDeleted = "N/A (File was deleted)";
        [Program.Localized]
        string SettingsInvalid = "Invalid";
        [Program.Localized]
        string DatabaseOutdated = "Outdated";
        [Program.Localized]
        string DatabasePickerTitle = "Select the Game Directory of Super Mario 3D World";
        [Program.Localized]
        string InvalidFolder = "The Directory doesn't contain ObjectData and StageData.";
        [Program.Localized]
        string InvalidGamePath = "The GamePath is invalid";
        [Program.Localized]
        string InvalidProjectPath = "The ProjectPath is invalid";
        [Program.Localized]
        string DatabaseRebuildSuccessText = "Database has been rebuilt!";
        [Program.Localized]
        string DatabaseRebuildSuccessHeader = "Success";
        [Program.Localized]
        string DescriptionClearText =
        @"Are you sure you want to empty out your description database?
(This action cannot be undone)";
        [Program.Localized]
        string DescriptionClearHeader = "Confirmation";
        [Program.Localized]
        string DescriptionClearSuccessText = "Descriptions Deleted";
        [Program.Localized]
        string DescriptionClearSuccessHeader = "Success";
        [Program.Localized]
        string ResetWarningText = "This will reset all your customizations, delete your Object Database and Description Database, and quit spotlight without saving.\nAre you sure you want to continue?";
        [Program.Localized]
        string ResetWarningHeader = "Warning";

        #endregion

        private void SplashSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            Properties.Settings.Default.SplashSize = new Size(int.Parse(SplashSizeComboBox.SelectedItem.ToString().Split('x')[0]), int.Parse(SplashSizeComboBox.SelectedItem.ToString().Split('x')[1]));
            Properties.Settings.Default.Save();
        }

        private void SplashTestButton_Click(object sender, EventArgs e)
        {
            Program.IsProgramReady = false;
            Task SplashTask = Task.Run(() =>
            {
                SplashForm SF = new SplashForm(10);
                SF.ShowDialog();
                SF.Dispose();
            });
            Thread.Sleep(9999);
            Program.IsProgramReady = true;
        }
    }

    public static class StringCollectionExtensions
    {
        public static void AddUnique(this StringCollection collection, string value)
        {
            if (!collection.Contains(value))
                collection.Add(value);
        }

        public static string[] ToArray(this StringCollection collection)
        {
            string[] arr = new string[collection.Count];

            for (int i = 0; i < collection.Count; i++)
            {
                arr[i] = collection[i];
            }

            return arr;
        }
    }
}
