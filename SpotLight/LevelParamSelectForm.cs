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
        public LevelParamSelectForm(StageList stagelist)
        {
            InitializeComponent();
            CenterToScreen();
            for (int i = 0; i < stagelist.Worlds.Count; i++)
                for (int j = 0; j < stagelist.Worlds[i].Levels.Count; j++)
                    LevelsListView.Items.Add(new ListViewItem(new string[] { (i+1).ToString(), stagelist.Worlds[i].Levels[j].StageName, stagelist.Worlds[i].Levels[j].CourseID.ToString() }));
        }
        public string levelname = "";
        public bool Selected = false;

        private void LevelsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LevelsListView.SelectedItems.Count > 0)
                levelname = LevelsListView.SelectedItems[0].SubItems[1].Text;
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
