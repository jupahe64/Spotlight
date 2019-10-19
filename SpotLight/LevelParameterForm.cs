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
            Loading = false;
            StageList = new StageList(Program.GamePath + "\\SystemData\\StageList.szs");

            WorldIDNumericUpDown.Maximum = StageList.Worlds.Count;

            if (LevelName != "")
            {
                bool Breakout = false;
                for (int x = 0; x < StageList.Worlds.Count; x++)
                {
                    for (int y = 0; y < StageList.Worlds[x].Levels.Count; y++)
                    {
                        if (StageList.Worlds[x].Levels[y].StageName == LevelName)
                        {
                            LoadLevelData(StageList.Worlds[x].Levels[y],StageList.Worlds[x].WorldID,y);
                            Breakout = true;
                            break;
                        }
                    }
                    if (Breakout)
                        break;
                }
            }
        }

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
            };

        public bool Loading = false, Changed = false;
        public StageList StageList;
        public int[] LevelID = new int[2];

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
            "妖精の家"
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
            FairyStampHouse = 3
        }

        private void ChangeLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelParamSelectForm LPSF = new LevelParamSelectForm(StageList);
            LPSF.ShowDialog();
            if (LPSF.levelname == "")
                return;
            
            bool Breakout = false;
            for (int x = 0; x < StageList.Worlds.Count; x++)
            {
                for (int y = 0; y < StageList.Worlds[x].Levels.Count; y++)
                {
                    if (StageList.Worlds[x].Levels[y].StageName == LPSF.levelname)
                    {
                        LoadLevelData(StageList.Worlds[x].Levels[y], StageList.Worlds[x].WorldID,y);
                        Breakout = true;
                        break;
                    }
                }
                if (Breakout)
                    break;
            }
        }

        private void LoadLevelData(LevelParam LV, int worldID, int levelID)
        {
            LevelID[0] = worldID;
            LevelID[1] = levelID;

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
            CourseLabel.Text = $"World {worldID}-{LV.StageID}";
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
            SaveToolStripMenuItem.Enabled = true;
            #endregion
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StageList.Save();
            Changed = false;
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
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].StageName = StageNameTextBox.Text;
            Changed = true;
        }
        
        private void StageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]-1].Levels[LevelID[1]].StageType = JapaneseStageTypes[Convert.ToInt32(((KeyValuePair<int, StageTypes>)StageTypeComboBox.Items[StageTypeComboBox.SelectedIndex]).Key)];
            Changed = true;
        }

        private void WorldIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;
            
        }

        private void LevelIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].StageID = (int)LevelIDNumericUpDown.Value;
            Changed = true;
        }

        private void CourseIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].CourseID = (int)CourseIDNumericUpDown.Value;
            Changed = true;
        }

        private void TimerNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].Timer = (int)TimerNumericUpDown.Value;
            Changed = true;
        }

        private void GreenStarsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].GreenStarNum = (int)GreenStarsNumericUpDown.Value;
            Changed = true;
        }

        private void GreenStarLockNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].GreenStarLock = (int)GreenStarLockNumericUpDown.Value;
            Changed = true;
        }

        private void DoubleMarioNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].DoubleMario = (int)DoubleMarioNumericUpDown.Value;
            Changed = true;
        }

        private void StampCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].IllustItemNum = StampCheckBox.Checked ? 1 : 0;
            Changed = true;
        }

        private void GhostBaseTimeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].GhostBaseTime = (int)GhostBaseTimeNumericUpDown.Value;
            Changed = true;
        }

        private void GhostIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Loading == true)
                return;

            StageList.Worlds[LevelID[0]].Levels[LevelID[1]].GhostID = (int)GhostIDNumericUpDown.Value;
            Changed = true;
        }
    }
}