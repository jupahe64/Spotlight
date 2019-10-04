using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotLight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LevelEditorForm());
        }

        public static bool GamePathIsValid()
        {
            return Directory.Exists(Properties.Settings.Default.GamePath + "\\ObjectData") && Directory.Exists(Properties.Settings.Default.GamePath + "\\StageData");
        }

        public static string GamePath
        {
            get => Properties.Settings.Default.GamePath;
            set
            {
                Properties.Settings.Default.GamePath = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string ObjectDataPath
        {
            get => Properties.Settings.Default.GamePath + "\\ObjectData\\";
        }

        public static string StageDataPath
        {
            get => Properties.Settings.Default.GamePath + "\\StageData\\";
        }
    }
}
