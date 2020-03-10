using Microsoft.WindowsAPICodePack.Dialogs;
using SpotLight.ObjectParamDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(LevelEditorForm home)
        {
            Home = home;
            InitializeComponent();
            CenterToParent();
            Localize();
            GamePathTextBox.Text = Program.GamePath;
            ProjectPathTextBox.Text = Program.ProjectPath;

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
                ver = Check.Equals(Spotlight.ObjectInformationDatabase.ObjectInformationDatabase.LatestVersion) ? Spotlight.ObjectInformationDatabase.ObjectInformationDatabase.LatestVersion.ToString() : Check.ToString() + $" ({DatabaseOutdated})";
                FS.Close();
            }
            else
                ClearDescriptionsButton.Enabled = false;
            date = File.GetLastWriteTime(Program.SODDPath);
            DescriptionInfoLabel.Text = DescriptionInfoLabel.Text.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? FileDoesntExist : date.ToLongDateString()).Replace("[VER]", ver);

            #endregion

            RenderAreaCheckBox.Checked = Properties.Settings.Default.DrawAreas;
            RenderSkyboxesCheckBox.Checked = Properties.Settings.Default.DrawSkyBoxes;
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
                        LanguageComboBox.Items.Add(Files[i].Replace(Program.LanguagePath+"\\", "").Replace(".txt", ""));
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

        }

        private LevelEditorForm Home;
        private bool Loading = false;

        private void GamePathButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = DatabasePickerTitle,
                IsFolderPicker = true
            };
            Program.GamePath = "";
            while (!Program.GamePathIsValid())
            {
                if (Program.GamePath != "")
                    MessageBox.Show(InvalidFolder, InvalidGamePath);

                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Program.GamePath = ofd.FileName;
                }
                else
                    Program.GamePath = GamePathTextBox.Text;
            }
            GamePathTextBox.Text = Program.GamePath;
        }

        private void GamePathTextBox_TextChanged(object sender, EventArgs e) => Program.GamePath = GamePathTextBox.Text;

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.GamePathIsValid())
            {
                e.Cancel = true;
                MessageBox.Show(InvalidFolder, InvalidGamePath, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Program.ProjectPath.Equals("") && !Program.ProjectPathIsValid())
            {
                e.Cancel = true;
                MessageBox.Show(InvalidFolder, InvalidProjectPath, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
            if (BfresModelCache.Contains("CheckpointFlag"))
            {
                BfresModelCache.ReloadModel("CheckpointFlag", Home.LevelGLControlModern);
                Home.LevelGLControlModern.Refresh();
            }
        }

        private void UniqueIDsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UniqueIDs = UniqueIDsCheckBox.Checked;
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

        private void ProjectPathButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = DatabasePickerTitle,
                IsFolderPicker = true
            };
            Program.ProjectPath = "";
            while (!Program.ProjectPathIsValid())
            {
                if (Program.ProjectPath != "")
                    MessageBox.Show(InvalidFolder, InvalidProjectPath);
                CommonFileDialogResult DR = ofd.ShowDialog();
                if (DR == CommonFileDialogResult.Ok)
                {
                    Program.ProjectPath = ofd.FileName;
                }
                else if (DR == CommonFileDialogResult.Cancel)
                {
                    Program.ProjectPath = ProjectPathTextBox.Text;
                    return;
                }
                else
                    Program.ProjectPath = ProjectPathTextBox.Text;
            }
            ProjectPathTextBox.Text = Program.ProjectPath;
        }

        private void ProjectPathTextBox_TextChanged(object sender, EventArgs e) => Program.ProjectPath = ProjectPathTextBox.Text;

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LanguageComboBox.SelectedItem.ToString().Equals(Properties.Settings.Default.Language))
                return;

            Properties.Settings.Default.Language = LanguageComboBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            Localize();
            Home.Localize();
        }

        #region Translations
        public void Localize()
        {
            Text = Program.CurrentLanguage.GetTranslation("SettingsTitle") ?? "Spotlight - Settings";
            DatabaseVersionBase = Program.CurrentLanguage.GetTranslation("DatabaseVersionBase") ?? "Database Last Built on: [DATABASEGENDATE].    Version: [VER]";
            DescriptionVersionBase = Program.CurrentLanguage.GetTranslation("DescriptionVersionBase") ?? "Descriptions Last Edited on: [DATABASEGENDATE].    Version: [VER]";
            FileDoesntExist = Program.CurrentLanguage.GetTranslation("FileDoesntExist") ?? "N/A (File doesn't exist)";
            FileDeleted = Program.CurrentLanguage.GetTranslation("FileDeleted") ?? "N/A (File was deleted)";
            SettingsInvalid = Program.CurrentLanguage.GetTranslation("SettingsInvalid") ?? "Invalid";
            DatabaseOutdated = Program.CurrentLanguage.GetTranslation("DatabaseOutdated") ?? "Outdated";
            DatabasePickerTitle = Program.CurrentLanguage.GetTranslation("DatabasePickerTitle") ?? "Select the Game Directory of Super Mario 3D World";
            InvalidFolder = Program.CurrentLanguage.GetTranslation("InvalidFolder") ?? "The Directory doesn't contain ObjectData and StageData.";
            InvalidGamePath = Program.CurrentLanguage.GetTranslation("DatabaseOutdated") ?? "The GamePath is invalid";
            InvalidProjectPath = Program.CurrentLanguage.GetTranslation("DatabaseOutdated") ?? "The ProjectPath is invalid";
            DatabaseRebuildSuccessText = Program.CurrentLanguage.GetTranslation("DatabaseRebuildSuccessText") ?? "Database has been rebuilt!";
            DatabaseRebuildSuccessHeader = Program.CurrentLanguage.GetTranslation("DatabaseRebuildSuccessHeader") ?? "Success";
            DescriptionClearText = Program.CurrentLanguage.GetTranslation("DescriptionClearText") ??
@"Are you sure you want to empty out your description database?
(This action cannot be undone)";
            DescriptionClearHeader = Program.CurrentLanguage.GetTranslation("DescriptionClearHeader") ?? "Confirmation";
            DescriptionClearSuccessText = Program.CurrentLanguage.GetTranslation("DescriptionClearSuccessText") ?? "Descriptions Deleted";
            DescriptionClearSuccessHeader = Program.CurrentLanguage.GetTranslation("DescriptionClearSuccessHeader") ?? "Success";
            ResetWarningText = Program.CurrentLanguage.GetTranslation("ResetWarningText") ?? "This will reset all your customizations, delete your Object Database and Description Database, and quit spotlight without saving.\nAre you sure you want to continue?";
            ResetWarningHeader = Program.CurrentLanguage.GetTranslation("ResetWarningHeader") ?? "Warning";

            GamePathLabel.Text = Program.CurrentLanguage.GetTranslation("GamePathLabel") ?? "Game Directory:";
            ProjectPathLabel.Text = Program.CurrentLanguage.GetTranslation("ProjectPathLabel") ?? "Project Directory:";
            ObjectParameterGroupBox.Text = Program.CurrentLanguage.GetTranslation("ObjectParameterGroupBox") ?? "Databases";
            RebuildDatabaseButton.Text = Program.CurrentLanguage.GetTranslation("RebuildDatabaseButton") ?? "Rebuild";
            ClearDescriptionsButton.Text = Program.CurrentLanguage.GetTranslation("ClearDescriptionsButton") ?? "Clear";
            RenderingGroupBox.Text = Program.CurrentLanguage.GetTranslation("RenderingGroupBox") ?? "Rendering";
            RenderAreaCheckBox.Text = Program.CurrentLanguage.GetTranslation("RenderAreaCheckBox") ?? "Render Areas";
            RenderSkyboxesCheckBox.Text = Program.CurrentLanguage.GetTranslation("RenderSkyboxesCheckBox") ?? "Render Skyboxes";
            PlayerLabel.Text = Program.CurrentLanguage.GetTranslation("PlayerLabel") ?? "Player:";
            Loading = true;
            PlayerComboBox.Items.Clear();
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerNone") ?? "None");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerMario") ?? "Mario");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerLuigi") ?? "Luigi");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerPeach") ?? "Peach");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerToad") ?? "Toad");
            PlayerComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("PlayerRosalina") ?? "Rosalina");
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
            Loading = false;
            LoadingAndSavingGroupBox.Text = Program.CurrentLanguage.GetTranslation("LoadingAndSavingGroupBox") ?? "Loading and Saving";
            UniqueIDsCheckBox.Text = Program.CurrentLanguage.GetTranslation("UniqueIDsCheckBox") ?? "Only load unique ObjectIDs (disable if objects disappear when loading a custom level)";
            EditingGroupBox.Text = Program.CurrentLanguage.GetTranslation("EditingGroupBox") ?? "Editing";
            IDEditingCheckBox.Text = Program.CurrentLanguage.GetTranslation("IDEditingCheckBox") ?? "Enable ID Editing";
            MiscellaneousGroupBox.Text = Program.CurrentLanguage.GetTranslation("MiscellaneousGroupBox") ?? "Miscellaneous";
            LanguageLabel.Text = Program.CurrentLanguage.GetTranslation("LanguageLabel") ?? "Language:";
            ResetSpotlightButton.Text = Program.CurrentLanguage.GetTranslation("ResetSpotlightButton") ?? "Reset";
        }

        private string DatabaseVersionBase { get; set; }
        private string DescriptionVersionBase { get; set; }
        private string FileDoesntExist { get; set; }
        private string FileDeleted { get; set; }
        private string SettingsInvalid { get; set; }
        private string DatabaseOutdated { get; set; }
        private string DatabasePickerTitle { get; set; }
        private string InvalidFolder { get; set; }
        private string InvalidGamePath { get; set; }
        private string InvalidProjectPath { get; set; }
        private string DatabaseRebuildSuccessHeader { get; set; }
        private string DatabaseRebuildSuccessText { get; set; }
        private string DescriptionClearHeader { get; set; }
        private string DescriptionClearText { get; set; }
        private string DescriptionClearSuccessHeader { get; set; }
        private string DescriptionClearSuccessText { get; set; }
        private string ResetWarningHeader { get; set; }
        private string ResetWarningText { get; set; }

        #endregion
    }
}
