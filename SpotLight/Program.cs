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

        /// <summary>
        /// Checks the game path to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool GamePathIsValid() => Directory.Exists(Properties.Settings.Default.GamePath + "\\ObjectData") && Directory.Exists(Properties.Settings.Default.GamePath + "\\StageData");
        /// <summary>
        /// The path that the game files are kept at.<para/>This is the path that should contain the StageData and ObjectData folders.
        /// </summary>
        public static string GamePath
        {
            get => Properties.Settings.Default.GamePath;
            set
            {
                Properties.Settings.Default.GamePath = value;
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Returns the ObjectData path
        /// </summary>
        public static string ObjectDataPath => Properties.Settings.Default.GamePath + "\\ObjectData\\";
        /// <summary>
        /// Returns the StageData path
        /// </summary>
        public static string StageDataPath => Properties.Settings.Default.GamePath + "\\StageData\\";


        public static string SOPDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParameterDatabase.sopd");
        public static string SODDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DescriptionDatabase.sodd");
    }
}
