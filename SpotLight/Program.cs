using Spotlight.ObjectInformationDatabase;
using SpotLight.ObjectParamDatabase;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            string FullfilePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            string VersionDirectoryfilePath = Path.GetFullPath(Path.Combine(FullfilePath, @"..\..\"+Application.ProductVersion));
            string BasePath = Path.GetFullPath(Path.Combine(FullfilePath, @"..\..\"));
            Properties.Settings.Default.Upgrade();
            if (Directory.Exists(BasePath))
            {
                DirectoryInfo DirInfo = new DirectoryInfo(BasePath);
                DirectoryInfo[] Dirs = DirInfo.GetDirectories();
                for (int i = 0; i < Dirs.Length; i++)
                    if (!Dirs[i].FullName.Equals(VersionDirectoryfilePath))
                        Directory.Delete(Dirs[i].FullName, true);
            }
            if (Directory.Exists(LanguagePath))
                CurrentLanguage = File.Exists(Path.Combine(LanguagePath, Properties.Settings.Default.Language + ".txt")) ? new Language(Properties.Settings.Default.Language, Path.Combine(LanguagePath, Properties.Settings.Default.Language + ".txt")) : new Language();

            Application.Run(new LevelEditorForm());
        }

        /// <summary>
        /// Checks the game path to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool GamePathIsValid() => Directory.Exists(Properties.Settings.Default.GamePath + "\\ObjectData") && Directory.Exists(Properties.Settings.Default.GamePath + "\\StageData");
        /// <summary>
        /// Checks the game path to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool ProjectPathIsValid() => Directory.Exists(Properties.Settings.Default.ProjectPath + "\\ObjectData") && Directory.Exists(Properties.Settings.Default.ProjectPath + "\\StageData");
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
        /// Override Game Files
        /// </summary>
        public static string ProjectPath
        {
            get => Properties.Settings.Default.ProjectPath;
            set
            {
                Properties.Settings.Default.ProjectPath = value;
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Returns the ObjectData path
        /// </summary>
        public static string BaseObjectDataPath => Properties.Settings.Default.GamePath + "\\ObjectData\\";
        /// <summary>
        /// Returns the StageData path
        /// </summary>
        public static string BaseStageDataPath => Properties.Settings.Default.GamePath + "\\StageData\\";

        public static ObjectParameterDatabase ParameterDB;

        public static string SOPDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParameterDatabase.sopd");
        public static string SODDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DescriptionDatabase.sodd");
        public static string LanguagePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");
        public static Language CurrentLanguage { get; set; }

        public static string TryGetPathViaProject(string Folder, string Filename)
        {
            string GamePathFile = Path.Combine(GamePath, Folder, Filename);
            string ProjectPathFile = Path.Combine(ProjectPath, Folder, Filename);

            if (ProjectPath != "" && File.Exists(ProjectPathFile))
            {
                if (!File.Exists(GamePathFile))
                    return ProjectPathFile;
                else if (File.GetLastWriteTime(ProjectPathFile).CompareTo(File.GetLastWriteTime(GamePathFile)) > 0)
                    return ProjectPathFile;
                else
                    return GamePathFile;
            }
            else
                return Path.Combine(GamePath, Folder, Filename);
        }
    }
}
