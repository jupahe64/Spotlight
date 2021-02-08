using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight
{
    public partial class LevelSelectForm : Form
    {
        ListViewItem[][] itemsBySection = new ListViewItem[14][];

        private StageList stagelist;

        //Bowsers Fury
        List<Level.LevelIO.ObjectInfo> islandInfos;

        //Captain Toad
        bool displaysSeasons = false;

        /// <summary>
        /// For Captain Toad
        /// </summary>
        /// <param name=""></param>
        public LevelSelectForm(List<List<string>> seasons)
        {
            InitializeComponent();
            CenterToScreen();

            Text = Program.CurrentLanguage.GetTranslation("LevelSelectTitle") ?? "Spotlight - Choose a Level";
            CourseIDColumnHeader.Text = Program.CurrentLanguage.GetTranslation("CourseIDColumnHeader") ?? "CourseID";
            LevelNameColumnHeader.Text = Program.CurrentLanguage.GetTranslation("LevelNameColumnHeader") ?? "Level Name";

            itemsBySection = new ListViewItem[seasons.Count][];


            SectionComboBox.Items.Clear();

            for (int i = 0; i < seasons.Count; i++)
            {
                itemsBySection[i] = seasons[i].Select(x => ItemFromStageName(x)).ToArray();

                SectionComboBox.Items.Add(Program.CurrentLanguage.GetTranslation("SeasonName" + (i + 1)) ?? "Season " + (i + 1));
            }

            displaysSeasons = true;

            SectionComboBox.SelectedIndex = 0;
        }

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
            {
                SectionComboBox.Items.RemoveAt(12); //Misc
                SectionComboBox.Items.RemoveAt(12); //Bowsers Fury
            }
            else
            {
                //Misc
                SectionComboBox.Items[12] = Program.CurrentLanguage.GetTranslation("WorldNameMisc") ?? "Miscellaneous";
                SectionComboBox.Items[13] = Program.CurrentLanguage.GetTranslation("WorldNameFury") ?? "Bowser's Fury";

                //Bowsers Fury
                string oceanStagepath = Program.TryGetPathViaProject("StageData", "SingleModeOceanStage.szs");
                if (File.Exists(oceanStagepath))
                {
                    islandInfos = new List<Level.LevelIO.ObjectInfo>();

                    Level.LevelIO.GetObjectInfosCombined(oceanStagepath, new Dictionary<string, List<Level.LevelIO.ObjectInfo>>()
                    {
                        ["IslandList"] = islandInfos
                    }, new Dictionary<string, List<Level.LevelIO.ObjectInfo>>(), new Dictionary<string, List<Level.LevelIO.ObjectInfo>>());
                }
                else
                {
                    SectionComboBox.Items.RemoveAt(13); //Bowsers Fury
                }
            }

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

        static ListViewItem ItemFromStageName(string name) => new ListViewItem(new string[] { Program.CurrentLanguage.GetTranslation(name) ?? name }) { Tag = name };

        private void SectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = SectionComboBox.SelectedIndex;

            ListViewItem[] items = itemsBySection[i];

            if (displaysSeasons    ||    i >= 12 /*misc stages, bowsers fury*/ )
            {
                if (LevelsListView.Columns.Contains(CourseIDColumnHeader))
                    LevelsListView.Columns.Remove(CourseIDColumnHeader);
            }
            else
            {
                if (!LevelsListView.Columns.Contains(CourseIDColumnHeader))
                {
                    LevelsListView.Columns.Insert(0, CourseIDColumnHeader);
                    CourseIDColumnHeader.Width = 65;
                }
            }

            if (items == null)
            {
                if (i == 12) //misc stages
                {
                    #region misc stages
                    items = new ListViewItem[]
                    {
                        ItemFromStageName("CourseSelectStage"),
                        ItemFromStageName("TitleDemo00Stage"),
                        ItemFromStageName("TitleDemo01Stage"),
                        ItemFromStageName("TitleDemo02Stage"),
                        ItemFromStageName("TitleDemo03Stage"),
                        ItemFromStageName("TitleDemo04Stage"),
                        ItemFromStageName("TitleDemo05Stage")
                    };
                    #endregion
                }
                else if (i == 13) //bowsers fury
                {
                    #region bowsers fury
                    items = new ListViewItem[islandInfos.Count+3];

                    items[0] = ItemFromStageName("SingleModeOceanStage");
                    items[1] = ItemFromStageName("SingleModeOceanPhase0Stage");
                    items[2] = ItemFromStageName("SingleModeBossStage");

                    for (int _i = 0; _i < islandInfos.Count; _i++)
                    {
                        items[_i+3] = ItemFromStageName(islandInfos[_i].ObjectName);
                    }
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
