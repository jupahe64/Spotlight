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
    public partial class LevelParameterForm : Form
    {
        public LevelParameterForm(string LevelName = "")
        {
            InitializeComponent();
            CenterToParent();
            Loading = true;

            StageTypeComboBox.DataSource = new BindingSource(comboSource, null);
            StageTypeComboBox.DisplayMember = "Value";
            StageTypeComboBox.ValueMember = "Key";



            StageList = new StageList(Program.TryGetPathViaProject("SystemData", "StageList.szs"));

            WorldIDNumericUpDown.Maximum = StageList.Worlds.Count;

            Localize();

            if (!LevelName.Equals(""))
            {
                bool Breakout = false;
                for (int x = 0; x < StageList.Worlds.Count; x++)
                {
                    for (int y = 0; y < StageList.Worlds[x].Levels.Count; y++)
                    {
                        if (StageList.Worlds[x].Levels[y].StageName == LevelName)
                        {
                            WorldComboBox.SelectedIndex = x;
                            LevelsListView.Items[y].Selected = true;
                            Breakout = true;
                            break;
                        }
                    }
                    if (Breakout)
                        break;
                }
            }
            else
                WorldComboBox.SelectedIndex = 0;

            Loading = false;
        }

        ListViewItem[][] itemsByWorld = new ListViewItem[13][];

        Dictionary<int, StageTypes> comboSource = new Dictionary<int, StageTypes>
            {
                { 15, (StageTypes)0 },
                { 1, (StageTypes)1 },
                { 13, (StageTypes)2 },
                { 16, (StageTypes)3 },
                { 0, (StageTypes)4 },
                { 2, (StageTypes)5 },
                { 12, (StageTypes)6 },
                { 14, (StageTypes)7 },
                { 8, (StageTypes)8 },
                { 9, (StageTypes)9 },
                { 10, (StageTypes)10 },
                { 3, (StageTypes)11 },
                { 4, (StageTypes)12 },
                { 5, (StageTypes)13 },
                { 6, (StageTypes)14 },
                { 7, (StageTypes)15 },
                { 11, (StageTypes)16 },
                { 17, (StageTypes)17 },
            };

        public bool Loading = false, Changed = false;
        public StageList StageList;
        public int currentWorld;
        public int currentLvlIndex;

        string[] JapaneseStageTypes = new string[]
        {
            "カジノ部屋",
            "キノピオの家",
            "キノピオ探検隊",
            "クッパ城",
            "クッパ城[砦]",
            "クッパ城[戦車]",
            "クッパ城[列車]",
            "クッパ城[列車通常]",
            "ゲートキーパー",
            "ゲートキーパー[GPあり]",
            "ゴールデンエクスプレス",
            "チャンピオンシップ",
            "ミステリーハウス",
            "隠しキノピオの家",
            "隠し土管",
            "通常",
            "妖精の家",
            "DRC専用"
        };
        public enum StageTypes
        {
            CasinoRoom = 4,
            ToadHouse = 1,
            CaptainToad = 5,
            BowserCastle = 11,
            BowserCastleFort = 12,
            BowserCastleTrain = 13,
            BowserCastleTank = 14,
            BowserCastleTrainNormal = 15,
            Gatekeeper = 8,
            GatekeeperGoalPole = 9,
            GoldenExpress = 10,
            Championship = 16,
            ChallengeHouse = 6,
            ToadHouseHidden = 2,
            HiddenPipe = 7,
            Normal = 0,
            FairyStampHouse = 3,
            GamePadRequired = 17
        }

        private void LoadLevelData(LevelParam LV, int worldID, int levelID)
        {
            currentWorld = worldID-1;
            currentLvlIndex = levelID;

            Loading = true;
            for (int i = 0; i < JapaneseStageTypes.Length; i++)
            { 
                if (LV.StageType == JapaneseStageTypes[i])
                {
                    StageTypeComboBox.SelectedIndex = (int)comboSource[i];
                    break;
                }
            }
            StageNameTextBox.Text = LV.StageName;
            UpdateCourseLabel();
            WorldIDNumericUpDown.Value = worldID;
            LevelIDNumericUpDown.Value = LV.StageID;
            CourseIDNumericUpDown.Value = LV.CourseID;
            TimerNumericUpDown.Value = LV.Timer;
            GreenStarsNumericUpDown.Value = LV.GreenStarNum;
            GreenStarLockNumericUpDown.Value = LV.GreenStarLock;
            DoubleMarioNumericUpDown.Value = LV.DoubleMario;
            GhostBaseTimeNumericUpDown.Value = LV.GhostBaseTime;
            GhostIDNumericUpDown.Value = LV.GhostID;

            if (LV.IllustItemNum == 1)
                StampCheckBox.Checked = true;
            else if (LV.IllustItemNum > 1)
                throw new Exception();
            else
                StampCheckBox.Checked = false;

            Loading = false;

            #region Enable the form
            StageTypeComboBox.Enabled = true;
            StageNameTextBox.Enabled = true;
            WorldIDNumericUpDown.Enabled = true;
            LevelIDNumericUpDown.Enabled = true;
            CourseIDNumericUpDown.Enabled = true;
            TimerNumericUpDown.Enabled = true;
            GreenStarsNumericUpDown.Enabled = true;
            GreenStarLockNumericUpDown.Enabled = true;
            DoubleMarioNumericUpDown.Enabled = true;
            GhostBaseTimeNumericUpDown.Enabled = true;
            GhostIDNumericUpDown.Enabled = true;
            StampCheckBox.Enabled = true;
            DeleteLevelButton.Enabled = true;
            #endregion
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StageList.Save();
            Changed = false;

            bool Breakout = false;
            for (int x = 0; x < StageList.Worlds.Count; x++)
            {
                for (int y = 0; y < StageList.Worlds[x].Levels.Count; y++)
                {
                    if (StageList.Worlds[x].Levels[y].StageName.Equals(StageNameTextBox.Text))
                    {
                        LoadLevelData(StageList.Worlds[x].Levels[y], StageList.Worlds[x].WorldID, y);
                        Breakout = true;
                        break;
                    }
                }
                if (Breakout)
                    break;
            }

            MessageBox.Show("Save Complete!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LevelParameterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Changed)
            {
                DialogResult tmp = MessageBox.Show("You have Unsaved changes!\nWould you like to save?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (tmp)
                {
                    case DialogResult.Yes:
                        StageList.Save();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void StageNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].StageName = StageNameTextBox.Text;

            LevelsListView.BeginUpdate();
            itemsByWorld[currentWorld] = null;
            WorldComboBox_SelectedIndexChanged(null, null);

            LevelsListView.SelectedItems.Clear();
            LevelsListView.Items[currentLvlIndex].Selected = true;
            LevelsListView.EndUpdate();

            Changed = true;
        }
        
        private void StageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].StageType = JapaneseStageTypes[Convert.ToInt32(((KeyValuePair<int, StageTypes>)StageTypeComboBox.Items[StageTypeComboBox.SelectedIndex]).Key)];
            Changed = true;
        }

        private void UpdateCourseLabel() =>
            CourseLabel.Text = $"{Program.CurrentLanguage.GetTranslation("WorldName" + (currentWorld + 1)) ?? "World " + (currentWorld + 1)}-{currentLvlIndex + 1}";

        private void WorldIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            itemsByWorld[currentWorld] = null;
            
            LevelParam current = StageList.Worlds[currentWorld].Levels[currentLvlIndex];

            StageList.Worlds[currentWorld].RemoveAt(currentLvlIndex);

            currentWorld = (int)WorldIDNumericUpDown.Value-1;
            currentLvlIndex = StageList.Worlds[(int)WorldIDNumericUpDown.Value-1].Add(current);
            Changed = true;

            itemsByWorld[currentWorld] = null;

            LevelsListView.BeginUpdate();
            WorldComboBox.SelectedIndex = (int)WorldIDNumericUpDown.Value - 1;

            Loading = true;
            LevelsListView.SelectedItems.Clear();
            LevelsListView.Items[currentLvlIndex].Selected = true;
            Loading = false;
            LevelsListView.EndUpdate();

            UpdateCourseLabel();
        }

        private void LevelIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].StageID = (int)LevelIDNumericUpDown.Value;
            currentLvlIndex = StageList.Worlds[currentWorld].UpdateLevelIndex(StageList.Worlds[currentWorld].Levels[currentLvlIndex]);

            LevelsListView.BeginUpdate();
            itemsByWorld[currentWorld] = null;
            WorldComboBox_SelectedIndexChanged(null, null);

            LevelsListView.SelectedItems.Clear();
            LevelsListView.Items[currentLvlIndex].Selected = true;
            LevelsListView.EndUpdate();

            Changed = true;
            UpdateCourseLabel();
        }

        private void CourseIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].CourseID = (int)CourseIDNumericUpDown.Value;
            Changed = true;
        }

        private void TimerNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].Timer = (int)TimerNumericUpDown.Value;
            Changed = true;
        }

        private void GreenStarsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].GreenStarNum = (int)GreenStarsNumericUpDown.Value;
            Changed = true;
        }

        private void GreenStarLockNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].GreenStarLock = (int)GreenStarLockNumericUpDown.Value;
            Changed = true;
        }

        private void DoubleMarioNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].DoubleMario = (int)DoubleMarioNumericUpDown.Value;
            Changed = true;
        }

        private void StampCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].IllustItemNum = StampCheckBox.Checked ? 1 : 0;
            Changed = true;
        }

        private void GhostBaseTimeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].GhostBaseTime = (int)GhostBaseTimeNumericUpDown.Value;
            Changed = true;
        }

        private void GhostIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            StageList.Worlds[currentWorld].Levels[currentLvlIndex].GhostID = (int)GhostIDNumericUpDown.Value;
            Changed = true;
        }

        private void WorldComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = WorldComboBox.SelectedIndex;
            currentWorld = i;

            ListViewItem[] items = itemsByWorld[i];

            if (items == null)
            {
                items = new ListViewItem[StageList.Worlds[i].Levels.Count];

                int previousStageID = -1;

                for (int j = 0; j < StageList.Worlds[i].Levels.Count; j++)
                {
                    LevelParam levelParam = StageList.Worlds[i].Levels[j];

                    items[j] = new ListViewItem(new string[]
                    {
                        levelParam.StageID.ToString(),
                        Program.CurrentLanguage.GetTranslation(levelParam.StageName) ?? levelParam.StageName

                    })
                    { Tag = StageList.Worlds[i].Levels[j] };

                    if (previousStageID == levelParam.StageID)
                    {
                        items[j-1].ForeColor = Color.Red;
                        items[j].ForeColor = Color.Red;
                    }

                    previousStageID = levelParam.StageID;
                }

                itemsByWorld[i] = items;
            }

            LevelsListView.Items.Clear();
            LevelsListView.Items.AddRange(items);
        }

        private void LevelsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            if (LevelsListView.SelectedItems.Count > 0)
            {
                Loading = true;

                int x = WorldComboBox.SelectedIndex;
                int y = LevelsListView.SelectedIndices[0];

                LoadLevelData(StageList.Worlds[x].Levels[y], StageList.Worlds[x].WorldID, y);

                Loading = false;
            }
        }

        private void AddLevelButton_Click(object sender, EventArgs e)
        {
            currentWorld = WorldComboBox.SelectedIndex;
            int index = StageList.Worlds[currentWorld].Add("", StageList.GetNextCourseID());

            LevelsListView.BeginUpdate();
            itemsByWorld[currentWorld] = null;
            WorldComboBox_SelectedIndexChanged(null, null);

            LevelsListView.SelectedItems.Clear();
            LevelsListView.Items[index].Selected = true;
            LevelsListView.EndUpdate();

            Changed = true;
        }

        private void DeleteLevelButton_Click(object sender, EventArgs e)
        {
            StageList.Worlds[currentWorld].RemoveAt(currentLvlIndex);

            LevelsListView.BeginUpdate();
            itemsByWorld[currentWorld] = null;
            WorldComboBox_SelectedIndexChanged(null, null);

            LevelsListView.SelectedItems.Clear();
            LevelsListView.EndUpdate();

            Changed = true;

            #region Disable the form
            StageTypeComboBox.Enabled = false;
            StageNameTextBox.Enabled = false;
            WorldIDNumericUpDown.Enabled = false;
            LevelIDNumericUpDown.Enabled = false;
            CourseIDNumericUpDown.Enabled = false;
            TimerNumericUpDown.Enabled = false;
            GreenStarsNumericUpDown.Enabled = false;
            GreenStarLockNumericUpDown.Enabled = false;
            DoubleMarioNumericUpDown.Enabled = false;
            GhostBaseTimeNumericUpDown.Enabled = false;
            GhostIDNumericUpDown.Enabled = false;
            StampCheckBox.Enabled = false;
            DeleteLevelButton.Enabled = false;
            #endregion
        }

        #region Localization
        public void Localize()
        {
            this.Localize(
            AddLevelButton,
            DeleteLevelButton,
            SaveToolStripMenuItem,
            StageNameLabel,
            StageNameTextBox,
            StageTypeLabel,
            GlobalIDLabel,
            StampCheckBox,
            TimeLabel,
            GreenStarUnlockCountLabel,
            GreenStarLabel,
            DoubleCherryLabel,
            GhostTimeLabel,
            GhostIDLabel,

            CourseIDColumnHeader,
            LevelNameColumnHeader
            );

            for (int i = 0; i < WorldComboBox.Items.Count - 1; i++)
            {
                if (Program.CurrentLanguage.Translations.TryGetValue("WorldName" + (i + 1), out string value))
                    WorldComboBox.Items[i] = value;
            }
        }

        //[Program.Localized]
        //string SaveSuccessHeader = "Notice";
        //[Program.Localized]
        //string SaveSuccessText = "Save Successful!";
        //[Program.Localized]
        //string SaveFailedHeader = "Warning";
        //[Program.Localized]
        //string SaveFailedText = "Save Failed!";
        #endregion
    }
}