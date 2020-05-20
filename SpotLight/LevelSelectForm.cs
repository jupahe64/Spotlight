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
    public partial class LevelSelectForm : Form
    {
        ListViewItem[][] itemsBySection = new ListViewItem[13][];

        private StageList stagelist;

        public LevelSelectForm(StageList stagelist, bool showMisc = false)
        {
            InitializeComponent();
            CenterToScreen();

            Text = Program.CurrentLanguage.GetTranslation("LevelSelectTitle") ?? "Spotlight - Choose a Level";
            CourseIDColumnHeader.Text = Program.CurrentLanguage.GetTranslation("CourseIDColumnHeader") ?? "CourseID";
            LevelNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("LevelNameColumnHeader") ?? "Level Name";

            this.stagelist = stagelist;

            for (int i = 0; i < SectionComboBox.Items.Count-1; i++)
            {
                if (Program.CurrentLanguage.Translations.TryGetValue("WorldName"+(i+1), out string value))
                    SectionComboBox.Items[i] = value;
            }

            if (!showMisc)
                SectionComboBox.Items.RemoveAt(12);

            SectionComboBox.SelectedIndex = 0;
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

        private void SectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = SectionComboBox.SelectedIndex;

            ListViewItem[] items = itemsBySection[i];

            if (i == 12) //misc stages
            {
                if (LevelsListView.Columns.Contains(CourseIDColumnHeader))
                    LevelsListView.Columns.Remove(CourseIDColumnHeader);
            }
            else
            {
                if (!LevelsListView.Columns.Contains(CourseIDColumnHeader))
                    LevelsListView.Columns.Insert(0, CourseIDColumnHeader);
            }

            if (items == null)
            {
                if (i == 12) //misc stages
                {
                    #region misc stages
                    items = new ListViewItem[]
                    {
                        new ListViewItem(new string[] { Program.CurrentLanguage.GetTranslation("CourseSelectStage") ?? "CourseSelectStage" }) { Tag = "CourseSelectStage" }
                    };
                    #endregion
                }
                else
                {
                    items = new ListViewItem[stagelist.Worlds[i].Levels.Count];
                    for (int j = 0; j < stagelist.Worlds[i].Levels.Count; j++)
                    {
                        items[j] = new ListViewItem(new string[] 
                        {
                            stagelist.Worlds[i].Levels[j].StageID.ToString(),
                            Program.CurrentLanguage.GetTranslation(stagelist.Worlds[i].Levels[j].StageName) ?? stagelist.Worlds[i].Levels[j].StageName

                        }) { Tag = stagelist.Worlds[i].Levels[j].StageName };
                    }
                }

                itemsBySection[i] = items;
            }

            LevelsListView.Items.Clear();
            LevelsListView.Items.AddRange(items);
        }
    }
}
