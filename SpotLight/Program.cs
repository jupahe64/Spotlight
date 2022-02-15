using Spotlight.Database;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spotlight.GUI;
using OpenTK;
using System.Drawing;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Runtime.InteropServices;

namespace Spotlight
{
    static class Program
    {
        internal static DiscordRpcClient Client;

        internal static RichPresence Default_Presence => new RichPresence
        {
            Assets = new Assets
            {
                LargeImageKey = "spotlighticon",
                LargeImageText = "A Super Mario 3D World Level Editor using the GL_EditorFramework"
            },
            State = "Idle",
            Timestamps = Timestamps.Now,
            Details = "Currently doing nothing."
        };

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);

            string platform;

            if (IntPtr.Size * 8 == 64)
                platform = "x64";
            else
                platform = "x86";

            var nativelib = Path.Combine(AppContext.BaseDirectory, "nativelib");

            foreach (var dll in Directory.EnumerateFiles(Path.Combine(nativelib,platform)))
            {
                File.Copy(dll, Path.Combine(nativelib, Path.GetFileName(dll)), true);
            }

            Client = new DiscordRpcClient("875108365385220146")
            {
                Logger = new ConsoleLogger(LogLevel.Error)
            };
            Client.Initialize();
            Client.SetPresence(Default_Presence);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Task SplashTask = Task.Run(() =>
            {
                SplashForm SF = new SplashForm(10);
                SF.ShowDialog();
                SF.Dispose();
            });

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
            else
                CurrentLanguage = new Language();


            Form host = new Form();

            host.Controls.Add(new GLControl());

            host.Load += (x, y) =>
            {
                StartUpForm = new LevelEditorForm();

                Rectangle? bounds = null;

                while (StartUpForm != null)
                {
                    var f = StartUpForm;
                    StartUpForm = null;

                    f.Load += (x2, y2) =>
                    {
                        if(bounds!=null)
                            f.SetBounds(bounds.Value.X, bounds.Value.Y, bounds.Value.Width, bounds.Value.Height);
                    };

                    f.Shown += (x2,y2) => f.TopLevel = true;

                    f.ShowDialog(host);
                    bounds = f.DesktopBounds;
                }

                host.Close();
            };


            Application.Run(host);
        }

        static Form StartUpForm; 

        public static void SetRestartForm(Form form)
        {
            StartUpForm = form;
        }


        static string base_localization = "";

        static void AddLocalizationLine(string id, string text) => base_localization += id + "|" + text.Replace("|", "</>").Replace(@"
", "<N>").Replace("\n", "<N>") + "\n";

        public static void Localize(this Control control, params object[] controls)
        {
            if(control is Form)
            {
                string id = control.Name + "Header";

                AddLocalizationLine(id,control.Text);

                string localized = CurrentLanguage.GetTranslation(id);

                if (!string.IsNullOrEmpty(localized))
                    control.Text = localized;
            }


            Type type = control.GetType();

            foreach (var item in controls)
            {
                if(item is Control _control)
                {
                    string id = type.Name + "." + _control.Name;

                    AddLocalizationLine(id, _control.Text);

                    string localized = CurrentLanguage.GetTranslation(id);

                    if (!string.IsNullOrEmpty(localized))
                        _control.Text = localized;
                }
                else if (item is ToolStripItem _item)
                {
                    string id = type.Name + "." + _item.Name;

                    AddLocalizationLine(id, _item.Text);

                    string localized = CurrentLanguage.GetTranslation(id);

                    if (!string.IsNullOrEmpty(localized))
                        _item.Text = localized;
                }
                else if (item is ColumnHeader _header)
                {
                    string id = type.Name + "." + _header.Name;

                    AddLocalizationLine(id, _header.Text);

                    string localized = CurrentLanguage.GetTranslation(id);

                    if (!string.IsNullOrEmpty(localized))
                        _header.Text = localized;
                }
            }

            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!field.CustomAttributes.Any((x) => x.AttributeType == typeof(LocalizedAttribute)))
                    continue;

                string id = field.Name;

                AddLocalizationLine(id, (string)field.GetValue(control));

                string localized = CurrentLanguage.GetTranslation(id);

                if (!string.IsNullOrEmpty(localized))
                    field.SetValue(control, localized);
            }
        }

        public class LocalizedAttribute : Attribute
        {

        }

        /// <summary>
        /// Checks the current GamePath to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool GamePathIsValid() => IsGamePathValid(Properties.Settings.Default.GamePath);
        /// <summary>
        /// Checks the current ProgramPath to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool ProjectPathIsValid() => IsGamePathValid(Properties.Settings.Default.ProjectPath);
        /// <summary>
        /// Checks the given path to see if it is a valid SM3DW path
        /// </summary>
        /// <returns>true if the path is valid, false if it's invalid</returns>
        public static bool IsGamePathValid(string path) => Directory.Exists(path + "\\ObjectData") && Directory.Exists(path + "\\StageData");
        public static void MakeGamePathValid(string path)
        {
            Directory.CreateDirectory(path + "\\ObjectData");
            Directory.CreateDirectory(path + "\\StageData");
        }

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
        public static string BaseObjectDataPath => Properties.Settings.Default.GamePath + "\\ObjectData";
        /// <summary>
        /// Returns the StageData path
        /// </summary>
        public static string BaseStageDataPath => Properties.Settings.Default.GamePath + "\\StageData";

        /// <summary>
        /// Returns the ObjectData path in the project directory
        /// </summary>
        public static string ProjectObjectDataPath => Properties.Settings.Default.ProjectPath + "\\ObjectData";
        /// <summary>
        /// Returns the StageData path in the project directory
        /// </summary>
        public static string ProjectStageDataPath => Properties.Settings.Default.ProjectPath + "\\StageData";

        public static ObjectParameterDatabase ParameterDB;
        public static ObjectInformationDatabase InformationDB;

        public static string SOPDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParameterDatabase.sopd");
        public static string SODDPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DescriptionDatabase.sodd");
        public static string LanguagePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");
        public static string SplashPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Splash");
        public static Language CurrentLanguage { get; set; }
        public static bool IsProgramReady { get; set; } = false;

        public static string TryGetPathViaProject(string Folder, string Filename)
        {
            string GamePathFile = Path.Combine(GamePath, Folder, Filename);
            string ProjectPathFile = Path.Combine(ProjectPath, Folder, Filename);

            if (ProjectPath != "" && File.Exists(ProjectPathFile))
            {
                if (!File.Exists(GamePathFile))
                    return ProjectPathFile;
                else if (File.GetLastWriteTime(ProjectPathFile).CompareTo(File.GetLastWriteTime(GamePathFile)) >= 0)
                    return ProjectPathFile;
                else
                    return GamePathFile;
            }
            else
                return Path.Combine(GamePath, Folder, Filename);
        }

        public static Information GetInfo(this IParameter param)
        {
            return InformationDB.GetInformation(param.ClassName);
        }

        public static string GetEnglishName(this IParameter param)
        {
            return InformationDB.GetInformation(param.ClassName).EnglishName ?? param.ClassName;
        }
    }
}
