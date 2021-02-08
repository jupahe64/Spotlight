using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotlight
{
    public partial class LoadLevelForm : Form
    {
        public LoadLevelForm(string levelname, string scenario)
        {
            InitializeComponent();
            CenterToParent();
            Localize();
            LevelName = levelname;
            DoClose = false;
            LevelTimer.Start();
            TalkTimer.Start();
            CloseTimer.Start();
            rand = new Random((int)StringToHash(LevelName));
            Scenario = scenario;
            if (Scenario.Equals("LoadLevel"))
                Text = LoadingLevelText;
            else if (Scenario.Equals("GenDatabase"))
                Text = DatabaseGeneratingText;
        }
        string LevelName;
        string Scenario;
        //int counter = 0;
        bool reset = false;
        Random rand = new Random();
        private void LevelTimer_Tick(object sender, EventArgs e)
        {
            if (reset)
            {
                MainProgressBar.Value = 0;
                reset = false;
                MainProgressBar.PerformStep();
            }
            else
                MainProgressBar.PerformStep();

            if (MainProgressBar.Value + MainProgressBar.Step > MainProgressBar.Maximum)
                reset = true;

            if (Scenario.Equals("LoadLevel"))
            {
                LevelTimer.Interval = rand.Next(1,3)*1000;
                MainProgressBar.Step = rand.Next(1,10);
            }
        }

        private void Converse(int count)
        {
            switch (count)
            {
                case 20:
                    Text = "So, how are you doing today?";
                    break;
                case 25:
                    Text = "I'm doing well today, processing your level loading.";
                    break;
                case 30:
                    Text = "Just another day for me, heh.";
                    break;
                case 35:
                    Text = "You know...";
                    break;
                case 37:
                    Text = "It get's kinda lonely here sometimes.";
                    break;
                case 40:
                    Text = "I have nothing to do when I'm not loading your level.";
                    break;
            }
        }
        public static bool DoClose = false;
        private void TalkTimer_Tick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Calculates a Hash from a string. Excerpt from Hackio.IO.BCSV
        /// </summary>
        /// <param name="Input">String to Convert to a Hash</param>
        /// <returns>Hash</returns>
        private static uint StringToHash(string Input)
        {
            uint Output = 0;
            foreach (char ch in Input)
            {
                Output *= 0x1F;
                Output += ch;
            }
            return Output;
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            if (DoClose)
            {
                LevelTimer.Stop();
                TalkTimer.Stop();
                Text = "Operation Complete!";
                MainProgressBar.Value = MainProgressBar.Maximum;
                Close();
            }
        }

        private void Localize()
        {
            LoadingLevelText = Program.CurrentLanguage.GetTranslation("LoadingLevelText") ?? "Loading Level...";
            DatabaseGeneratingText = Program.CurrentLanguage.GetTranslation("DatabaseGeneratingText") ?? "Generating Database...";
            OperationCompleteText = Program.CurrentLanguage.GetTranslation("OperationCompleteText") ?? "Operation Complete!";
        }

        private string LoadingLevelText { get; set; }
        private string DatabaseGeneratingText { get; set; }
        private string OperationCompleteText { get; set; }
    }
}
