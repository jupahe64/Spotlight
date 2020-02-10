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
    public partial class LoadLevelForm : Form
    {
        public LoadLevelForm(string levelname)
        {
            InitializeComponent();
            CenterToParent();
            LevelName = levelname;
            LevelTimer.Start();
            TalkTimer.Start();
            rand = new Random((int)StringToHash(LevelName));
        }
        string LevelName;
        string[] Messages = new string[]
        {
            "Loading Level...",
            "Still Loading Level...",
            "Level is still Loading...",
            "The Worldmap takes a while to load don't worry.",
            "This level is taking it's sweet time..."
        };
        int counter = 0;
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

            LevelTimer.Interval = rand.Next(1,3)*1000;
            MainProgressBar.Step = rand.Next(1,10);
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

        private void TalkTimer_Tick(object sender, EventArgs e)
        {
            if (counter == 5)
                Text = Messages[1];

            if (counter == 10)
                Text = Messages[2];

            if (LevelName == "CourseSelectStage" && counter == 15)
                Text = Messages[3];
            else if (counter == 15)
                Text = Messages[4];

            if (counter >= 20)
                Converse(counter);

            counter++;
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
    }
}
