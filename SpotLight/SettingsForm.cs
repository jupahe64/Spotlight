using Microsoft.WindowsAPICodePack.Dialogs;
using SpotLight.ObjectParamDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            CenterToParent();
            GamePathTextBox.Text = Program.GamePath;
        }

        public LevelEditorForm Home;

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
            Home.ObjectDatabase = new ObjectParameterDatabase();
            Home.ObjectDatabase.Create(Program.StageDataPath);
            Home.ObjectDatabase.Save("ParameterDatabase.sopd");
            MessageBox.Show("Database has been rebuilt!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
