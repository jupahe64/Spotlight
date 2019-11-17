//using GL_EditorFramework;
//using GL_EditorFramework.GL_Core;
//using SZS;
//using BYAML;
//using SpotLight.EditorDrawables;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using static BYAML.ByamlIterator;
//using System.Diagnostics;
//using Syroot.NintenTools.Bfres;
//using Syroot.BinaryData;
//using OpenTK.Graphics.OpenGL;
//using Syroot.NintenTools.Bfres.Helpers;

//namespace SpotLight
//{
//    public class SM3DWorldLevel
//    {
//        public readonly Dictionary<long, I3dWorldObject> ObjectBaseReference = new Dictionary<long, I3dWorldObject>();

//        public SM3DWorldScene scene;

//        /// <summary>
//        /// Creates a new SM3DW level from a file.
//        /// </summary>
//        /// <param name="fileName">Filepath to the .szs</param>
//        /// <param name="levelName">Name of the Level</param>
//        /// <param name="categoryName">Category that this file belongs to</param>
//        /// <param name="scene">The current SM3DW Scene</param>
//        /// <param name="sceneListView">The SceneListView to put the objects on</param>
//        private SM3DWorldLevel(string fileName, string levelName, string categoryName, SM3DWorldScene scene, SceneListView sceneListView)
//        {
//            this.levelName = levelName;
//            this.categoryName = categoryName;
//            this.fileName = fileName;
//            this.scene = scene;

//            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

//            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
//            {
//                if (keyValuePair.Key != levelName + categoryName + ".byml")
//                    extraFiles.Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
//            }

//            Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

//            sceneListView.RootLists.Clear();

//            ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[levelName + categoryName + ".byml"]));
//            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
//            {
//                if (entry.Key == "FilePath" || entry.Key == "Objs")
//                    continue;
//                scene.ObjLists.Add(entry.Key, new List<I3dWorldObject>());
                
//                foreach (ArrayEntry obj in entry.IterArray())
//                {
//                    scene.ObjLists[entry.Key].Add(LevelIO.ParseObject(obj, scene, objectsByReference));
//                }
//                sceneListView.RootLists.Add(entry.Key, scene.ObjLists[entry.Key]);

//                sceneListView.UpdateComboBoxItems();
//            }
//        }
//        public SM3DWorldLevel(string Filename, string Levelname, string CATEGORY)
//        {
//            levelName = Levelname;
//            fileName = Filename;
//            categoryName = CATEGORY;
//            scene = new SM3DWorldScene();

//            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));
//            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
//            {
//                if (keyValuePair.Key != levelName.Replace("Map1.szs", "") + categoryName + ".byml")
//                    extraFiles.Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
//            }
//            Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

//            ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[levelName.Replace("Map1.szs","") + CATEGORY + ".byml"]));
//            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
//            {
//                if (entry.Key == "FilePath" || entry.Key == "Objs")
//                    continue;
//                scene.ObjLists.Add(entry.Key, new List<I3dWorldObject>());
//                foreach (ArrayEntry obj in entry.IterArray())
//                {
//                    scene.ObjLists[entry.Key].Add(LevelIO.ParseObject(obj, scene, objectsByReference));
//                }
//            }
//            ObjectBaseReference = objectsByReference;
//        }

//        /// <summary>
//        /// Attempts to open a SM3DW level (.szs)
//        /// </summary>
//        /// <param name="fileName">Filepath to the .szs</param>
//        /// <param name="control">The OpenGL Control currently in use</param>
//        /// <param name="sceneListView">The SceneListView to put the objects on</param>
//        /// <param name="level">A Complete SM3DW level</param>
//        /// <returns>true if the load succeeded, false if it failed</returns>
//        public static bool TryOpen(string fileName, GL_ControlBase control, SceneListView sceneListView, out SM3DWorldLevel level)
//        {
//            SM3DWorldScene scene;
//            string levelName;
//            string categoryName;

//            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

//            if (fileNameWithoutExt.EndsWith("Map1"))
//            {
//                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 4);
//                categoryName = "Map";
//            }
//            else if(fileNameWithoutExt.EndsWith("Design1"))
//            {
//                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 7);
//                categoryName = "Design";
//            }
//            else if (fileNameWithoutExt.EndsWith("Sound1"))
//            {
//                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 6);
//                categoryName = "Sound";
//            }
//            else
//            {
//                scene = null;
//                level = null;
//                return false;
//            }

//            scene = new SM3DWorldScene();

//            level = new SM3DWorldLevel(fileName, levelName, categoryName, scene, sceneListView);

//            sceneListView.SelectedItems = scene.SelectedObjects;
//            sceneListView.Refresh();

//            control.MainDrawable = scene;

//            if (scene.ObjLists.ContainsKey("PlayerList") && scene.ObjLists["PlayerList"].Count > 0)
//            {
//                scene.GL_Control.CamRotX = 0;
//                scene.GL_Control.CamRotY = Framework.HALF_PI/4;
//                scene.GL_Control.CameraTarget = scene.ObjLists["PlayerList"][0].GetFocusPoint();
//            }

//            return true;
//        }

//        

//        /// <summary>
//        /// Returns the name of the level
//        /// </summary>
//        /// <returns></returns>
//        public override string ToString() => $"{levelName}";
//    }
//}
