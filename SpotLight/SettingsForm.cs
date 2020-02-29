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
            GamePathTextBox.Text = Program.GamePath;

            #region Databases

            string ver = "Invalid";
            if (File.Exists(Program.SOPDPath))
            {
                FileStream FS = new FileStream(Program.SOPDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SOPD")
                    throw new Exception("Invalid Database File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Home.ObjectDatabase != null ? Home.ObjectDatabase.Version.ToString() : Check.ToString() + " (Outdated)";
                FS.Close();
            }
            DateTime date = File.GetLastWriteTime(Program.SOPDPath);
            DatabaseInfoLabel.Text = DatabaseInfoLabel.Text.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018,1,1).Year) < 0 ? "N/A (File doesn't exist)" : date.ToLongDateString()).Replace("[VER]", ver);

            ver = "Invalid";
            if (File.Exists(Program.SODDPath))
            {
                FileStream FS = new FileStream(Program.SODDPath, FileMode.Open);
                byte[] Read = new byte[4];
                FS.Read(Read, 0, 4);
                if (Encoding.ASCII.GetString(Read) != "SODD")
                    throw new Exception("Invalid Description File");

                Version Check = new Version(FS.ReadByte(), FS.ReadByte());
                ver = Check.Equals(Spotlight.ObjectInformationDatabase.ObjectInformationDatabase.LatestVersion) ? Spotlight.ObjectInformationDatabase.ObjectInformationDatabase.LatestVersion.ToString() : Check.ToString() + " (Outdated)";
                FS.Close();
            }
            else
                ClearDescriptionsButton.Enabled = false;
            date = File.GetLastWriteTime(Program.SODDPath);
            DescriptionInfoLabel.Text = DescriptionInfoLabel.Text.Replace("[DATABASEGENDATE]", date.Year.CompareTo(new DateTime(2018, 1, 1).Year) < 0 ? "N/A (File doesn't exist)" : date.ToLongDateString()).Replace("[VER]", ver);

            #endregion

            RenderAreaCheckBox.Checked = Properties.Settings.Default.DrawAreas;
            RenderSkyboxesCheckBox.Checked = Properties.Settings.Default.DrawSkyBoxes;
            PlayerComboBox.SelectedIndex = PlayerComboBox.FindStringExact(Properties.Settings.Default.PlayerChoice);
            UniqueIDsCheckBox.Checked = Properties.Settings.Default.UniqueIDs;
        }

        private LevelEditorForm Home;

        public static string UserName
        {
            get => Properties.Settings.Default.UserName;
            set
            {
                Properties.Settings.Default.UserName = value;
                Properties.Settings.Default.Save();
            }
        }

        private void GamePathButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog()
            {
                Title = "Select the Game Directory of Super Mario 3D World",
                IsFolderPicker = true
            };
            Program.GamePath = "";
            while (!Program.GamePathIsValid())
            {
                if (Program.GamePath != "")
                    MessageBox.Show("The Directory doesn't contain ObjectData and StageData.", "The GamePath is invalid");

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
                MessageBox.Show("The game path is invalid.\nPlease try again","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
            Home.ObjectDatabase = new ObjectParameterDatabase();
            Home.ObjectDatabase.Create(Program.StageDataPath);
            Home.ObjectDatabase.Save(Program.SOPDPath);
            if (DatabaseGenThread.IsAlive)
                LoadLevelForm.DoClose = true;
            DatabaseInfoLabel.Text = "Database Last Built on: [DATABASEGENDATE].    Version: [VER]".Replace("[DATABASEGENDATE]", File.GetLastWriteTime(Program.SOPDPath).ToLongDateString()).Replace("[VER]", Home.ObjectDatabase.Version.ToString());
            MessageBox.Show("Database has been rebuilt!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearDescriptionsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
@"Are you sure you want to empty out your description database?
(This action cannot be undone)", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                File.Delete(Program.SODDPath);
                DescriptionInfoLabel.Text = "Descriptions Last Edited on: [DATABASEGENDATE].    Version: [VER]".Replace("[DATABASEGENDATE]", "N/A (File was deleted)").Replace("[VER]", "Invalid");
                MessageBox.Show("Descriptions Deleted","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Properties.Settings.Default.PlayerChoice = PlayerComboBox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void UniqueIDsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UniqueIDs = UniqueIDsCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void ResetSpotlightButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset all your customizations, delete your Object Database and Description Database, and quit spotlight without saving.\nAre you sure you want to continue?","Warning",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (File.Exists(Program.SOPDPath))
                    File.Delete(Program.SOPDPath);
                if (File.Exists(Program.SODDPath))
                    File.Delete(Program.SODDPath);
                Properties.Settings.Default.Reset();
                Environment.FailFast("It was nice knowing you...");
            }
        }
    }
}
