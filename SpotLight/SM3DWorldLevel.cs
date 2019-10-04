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
        readonly string levelName;
        readonly string categoryName;
        readonly string fileName;

        Dictionary<string, BymlFileData> extraFiles = new Dictionary<string, BymlFileData>();

        public SM3DWorldScene scene;

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

        public bool SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
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
    }
}
