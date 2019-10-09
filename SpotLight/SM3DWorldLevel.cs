using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using SZS;
using BYAML;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BYAML.ByamlIterator;
using System.Diagnostics;
using Syroot.NintenTools.Bfres;
using Syroot.BinaryData;
using OpenTK.Graphics.OpenGL;
using Syroot.NintenTools.Bfres.Helpers;

namespace SpotLight
{
    class SM3DWorldLevel
    {
        /// <summary>
        /// Name of the Level
        /// </summary>
        readonly string levelName;
        /// <summary>
        /// Category that this level belongs to (Map, Design, Sound)
        /// </summary>
        readonly string categoryName;
        /// <summary>
        /// Filename (includes the path)
        /// </summary>
        readonly string fileName;
        /// <summary>
        /// Any extra files that may be inside the map
        /// </summary>
        Dictionary<string, BymlFileData> extraFiles = new Dictionary<string, BymlFileData>();

        public SM3DWorldScene scene;

        /// <summary>
        /// Creates a new SM3DW level from a file.
        /// </summary>
        /// <param name="fileName">Filepath to the .szs</param>
        /// <param name="levelName">Name of the Level</param>
        /// <param name="categoryName">Category that this file belongs to</param>
        /// <param name="scene">The current SM3DW Scene</param>
        /// <param name="sceneListView">The SceneListView to put the objects on</param>
        private SM3DWorldLevel(string fileName, string levelName, string categoryName, SM3DWorldScene scene, SceneListView sceneListView)
        {
            this.levelName = levelName;
            this.categoryName = categoryName;
            this.fileName = fileName;
            this.scene = scene;

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
            {
                if (keyValuePair.Key != levelName + categoryName + ".byml")
                    extraFiles.Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
            }

            Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

            sceneListView.RootLists.Clear();

            ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[levelName + categoryName + ".byml"]));
            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
            {
                if (entry.Key == "FilePath" || entry.Key == "Objs")
                    continue;
                scene.categories.Add(entry.Key, new List<GL_EditorFramework.EditorDrawables.IEditableObject>());
                
                foreach (ArrayEntry obj in entry.IterArray())
                {
                    scene.categories[entry.Key].Add(new General3dWorldObject(obj, scene.linkedObjects, objectsByReference));
                }
                sceneListView.RootLists.Add(entry.Key, scene.categories[entry.Key]);
            }
        }

        /// <summary>
        /// Attempts to open a SM3DW level (.szs)
        /// </summary>
        /// <param name="fileName">Filepath to the .szs</param>
        /// <param name="control">The OpenGL Control currently in use</param>
        /// <param name="sceneListView">The SceneListView to put the objects on</param>
        /// <param name="level">A Complete SM3DW level</param>
        /// <returns>true if the load succeeded, false if it failed</returns>
        public static bool TryOpen(string fileName, GL_ControlBase control, SceneListView sceneListView, out SM3DWorldLevel level)
        {
            SM3DWorldScene scene;
            string levelName;
            string categoryName;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            if (fileNameWithoutExt.EndsWith("Map1"))
            {
                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 4);
                categoryName = "Map";
            }
            else if(fileNameWithoutExt.EndsWith("Design1"))
            {
                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 7);
                categoryName = "Design";
            }
            else if (fileNameWithoutExt.EndsWith("Sound1"))
            {
                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 6);
                categoryName = "Sound";
            }
            else
            {
                scene = null;
                level = null;
                return false;
            }

            scene = new SM3DWorldScene();

            level = new SM3DWorldLevel(fileName, levelName, categoryName, scene, sceneListView);

            sceneListView.SelectedItems = scene.SelectedObjects;
            sceneListView.Refresh();

            control.MainDrawable = scene;

            return true;
        }

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save()
        {
                SarcData sarcData = new SarcData()
                {
                    HashOnly = false,
                    endianness = Endian.Big,
                    Files = new Dictionary<string, byte[]>()
                };

                foreach (KeyValuePair<string, BymlFileData> keyValuePair in extraFiles)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ByamlFile.FastSaveN(stream, keyValuePair.Value);
                        sarcData.Files.Add(keyValuePair.Key, stream.ToArray());
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    ByamlNodeWriter writer = new ByamlNodeWriter(stream, false, Endian.Big, 1);

                    scene.Save(writer, levelName, categoryName);

                    sarcData.Files.Add(levelName + categoryName + ".byml", stream.ToArray());
                }

                File.WriteAllBytes(fileName, YAZ0.Compress(SARC.PackN(sarcData).Item2));

                return true;
        }

        /// <summary>
        /// Saves the level to a new file (.szs)
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed or was cancelled</returns>
        public bool SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "3DW Levels|*.szs", InitialDirectory = Program.StageDataPath };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SarcData sarcData = new SarcData()
                {
                    HashOnly = false,
                    endianness = Endian.Big,
                    Files = new Dictionary<string, byte[]>()
                };

                foreach(KeyValuePair<string, BymlFileData> keyValuePair in extraFiles)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ByamlFile.FastSaveN(stream, keyValuePair.Value);
                        sarcData.Files.Add(keyValuePair.Key, stream.ToArray());
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    ByamlNodeWriter writer = new ByamlNodeWriter(stream, false, Endian.Big, 1);

                    scene.Save(writer, levelName, categoryName);

                    sarcData.Files.Add(levelName + categoryName + ".byml", stream.ToArray());
                }

                File.WriteAllBytes(sfd.FileName, YAZ0.Compress(SARC.PackN(sarcData).Item2));

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns the name of the level
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{levelName}";
    }
}
