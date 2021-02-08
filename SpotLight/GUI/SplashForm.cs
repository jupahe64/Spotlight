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
    public partial class SplashForm : Form
    {
        public SplashForm(int L, bool loop = false)
        {
            InitializeComponent();
            Size = Properties.Settings.Default.SplashSize;
            CenterToScreen();
            timer = new Timer();
            LoadTimer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = L * 1000;
            timer.Start();
            LoadTimer.Interval = 10;
            LoadTimer.Tick += LoadTimer_Tick;
            LoadTimer.Start();
            Loop = loop;
        }

        private void Timer_Tick(object sender, EventArgs e) => Invalidate();
        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            if (!Loop && Program.IsProgramReady)
                Close();
        }


        Timer timer;
        Timer LoadTimer;
        bool Loop = false;

        private void SplashForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            string[] Splashes = Directory.Exists(Program.SplashPath) ? Directory.GetFiles(Program.SplashPath, "*.png", SearchOption.AllDirectories) : new string[0];
            if (Splashes.Length == 0)
            {
                Bitmap Default = new Bitmap(Properties.Resources.BootTvTex);
                Default = ResizeBitmap(Default, Width, Height);
                g.DrawImage(Default, 0, 0);
                return;
            }
            if (Splashes.Length > 1)
            {
                List<string> temp = Splashes.ToList();
                temp.Remove(Properties.Settings.Default.PreviousSplash);
                Splashes = temp.ToArray();
            }

            int ran = new Random().Next(0, Splashes.Length);
            Properties.Settings.Default.PreviousSplash = Splashes[ran];
            Properties.Settings.Default.Save();
            Bitmap ChosenImage = new Bitmap(Splashes[ran]);
            Bitmap b = ResizeBitmap(ChosenImage, Width, Height);
            ChosenImage.Dispose();
            g.DrawImage(b, 0, 0);
            b.Dispose();

            Bitmap OriginalLogo = new Bitmap(Properties.Resources.SpotlightCanvas);
            Bitmap Logo = ResizeBitmap(OriginalLogo, Width, Height);
            OriginalLogo.Dispose();
            g.DrawImage(Logo, 0, 0);
            Logo.Dispose();
            GC.Collect();
        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
                g.Dispose();
            }
            return result;
        }
    }
}
