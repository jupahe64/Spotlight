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
    public partial class LevelParamSelectForm : Form
    {
        public LevelParamSelectForm(StageList stagelist, bool ShowCourseSelect = false)
        {
            InitializeComponent();
            CenterToScreen();

            Text = Program.CurrentLanguage.GetTranslation("LevelSelectTitle") ?? "Spotlight - Choose a Level";
            WorldIDColumnHeader.Text = Program.CurrentLanguage.GetTranslation("WorldIDColumnHeader") ?? "World";
            LevelNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("LevelNameColumnHeader") ?? "Level Name";
            CourseIDColumnHeader.Text = Program.CurrentLanguage.GetTranslation("CourseIDColumnHeader") ?? "ID";


            if (ShowCourseSelect)
            {
                LevelsListView.Items.Add(new ListViewItem(new string[] { "0", Program.CurrentLanguage.GetTranslation("CourseSelectStage") ?? "CourseSelectStage", "0" }) { Tag = "CourseSelectStage" });
            }
            for (int i = 0; i < stagelist.Worlds.Count; i++)
                for (int j = 0; j < stagelist.Worlds[i].Levels.Count; j++)
                    LevelsListView.Items.Add(new ListViewItem(new string[] { (i + 1).ToString(), Program.CurrentLanguage.GetTranslation(stagelist.Worlds[i].Levels[j].StageName) ?? stagelist.Worlds[i].Levels[j].StageName, stagelist.Worlds[i].Levels[j].CourseID.ToString() }) { Tag = stagelist.Worlds[i].Levels[j].StageName });
        }
        public string levelname = "";
        public bool Selected = false;

        private void LevelsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LevelsListView.SelectedItems.Count > 0)
                levelname = LevelsListView.SelectedItems[0].Tag.ToString();
        }

        private void ChooseLevelButton_Click(object sender, EventArgs e)
        {
            Selected = true;
            Close();
        }

        private void LevelParamSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Selected)
                levelname = "";
        }

        private void LevelsListView_DoubleClick(object sender, EventArgs e)
        {
            if (LevelsListView.SelectedItems.Count > 0 && levelname != "")
            {
                Selected = true;
                Close();
            }
        }
    }
}
