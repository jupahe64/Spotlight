using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using OpenTK;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;
using static BYAML.ByamlIterator;

namespace SpotLight.Level
{
    public class LevelIO
    {
        public static bool TryOpenLevel(string fileName, LevelEditorForm levelEditorForm, out SM3DWorldScene scene)
        {
            scene = new SM3DWorldScene();

            if (SM3DWorldZone.TryOpen(fileName, out SM3DWorldZone zone))
            {
                scene.EditZone = zone;

                scene.EditZoneTransform = ZoneTransform.Identity;

                foreach (var zonePlacement in zone.ZonePlacements)
                {
                    scene.ZonePlacements.Add(zonePlacement);
                }

                levelEditorForm.MainSceneListView.SelectedItems = scene.SelectedObjects;
                levelEditorForm.MainSceneListView.Refresh();

                levelEditorForm.LevelZoneTreeView.BeginUpdate();
                levelEditorForm.LevelZoneTreeView.Nodes.Clear();
                TreeNode toplevelZoneNode = levelEditorForm.LevelZoneTreeView.Nodes.Add(zone.LevelName);
                toplevelZoneNode.Tag = zone;

                foreach (var zonePlacement in zone.ZonePlacements)
                {
                    toplevelZoneNode.Nodes.Add(zonePlacement.Zone.LevelName).Tag = zonePlacement.Zone;
                }

                levelEditorForm.LevelZoneTreeView.EndUpdate();
                levelEditorForm.LevelZoneTreeView.SelectedNode = toplevelZoneNode;

                levelEditorForm.LevelZoneTreeView_AfterSelect(levelEditorForm.LevelZoneTreeView, new TreeViewEventArgs(toplevelZoneNode));

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Parses a 3d World from an ArrayEntry
        /// </summary>
        /// <param name="objectEntry"></param>
        /// <param name="zone"></param>
        /// <param name="objectsByReference"></param>
        /// <returns></returns>
        public static I3dWorldObject ParseObject(ArrayEntry objectEntry, SM3DWorldZone zone, Dictionary<long, I3dWorldObject> objectsByReference)
        {
            ObjectInfo info = GetObjectInfo(ref objectEntry, zone);

            I3dWorldObject obj;
            bool loadLinks;

            if (info.ClassName == "Rail")
                obj = new Rail(info, zone, out loadLinks);
            else
                obj = new General3dWorldObject(info, zone, out loadLinks);

            if (!objectsByReference.ContainsKey(objectEntry.Position))
                objectsByReference.Add(objectEntry.Position, obj);

            if (loadLinks)
            {
                obj.Links = new Dictionary<string, List<I3dWorldObject>>();
                foreach (DictionaryEntry link in info.LinkEntries.Values)
                {
                    obj.Links.Add(link.Key, new List<I3dWorldObject>());
                    foreach (ArrayEntry linked in link.IterArray())
                    {
                        if (objectsByReference.ContainsKey(linked.Position))
                        {
                            obj.Links[link.Key].Add(objectsByReference[linked.Position]);
                            objectsByReference[linked.Position].AddLinkDestination(link.Key, obj);
                        }
                        else
                        {
                            I3dWorldObject _obj = ParseObject(linked, zone, objectsByReference);
                            _obj.AddLinkDestination(link.Key, obj);
                            obj.Links[link.Key].Add(_obj);
                            if (zone != null)
                                zone.LinkedObjects.Add(_obj);
                        }
                    }
                }
                if (obj.Links.Count == 0)
                    obj.Links = null;
            }

            return obj;
        }

        public static void GetObjectInfos(string fileName, Dictionary<string, List<ObjectInfo>> infosByListName)
        {
            string levelName;
            string categoryName;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            if (fileNameWithoutExt.EndsWith("Map1"))
            {
                levelName = fileNameWithoutExt.Remove(fileNameWithoutExt.Length - 4);
                categoryName = "Map";
            }
            else if (fileNameWithoutExt.EndsWith("Design1"))
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
                return;
            }

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            Dictionary<long, ObjectInfo> objectInfosByReference = new Dictionary<long, ObjectInfo>();

            ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[levelName + categoryName + ".byml"]));
            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
            {
                if (!infosByListName.ContainsKey(entry.Key))
                    continue;

                List<ObjectInfo> objectInfos = new List<ObjectInfo>();

                foreach (ArrayEntry obj in entry.IterArray())
                {
                    objectInfos.Add(ParseObjectInfo(obj, objectInfosByReference, infosByListName, entry.Key));
                }
            }
        }

        const string str_Links = "Links";

        public static ObjectInfo ParseObjectInfo(
            ArrayEntry objectEntry, Dictionary<long, ObjectInfo> objectInfosByReference, 
            Dictionary<string, List<ObjectInfo>> infosByListName, string objListName)
        {
            ObjectInfo info = GetObjectInfo(ref objectEntry, null);

            infosByListName[objListName].Add(info);

            if (!objectInfosByReference.ContainsKey(objectEntry.Position))
                objectInfosByReference.Add(objectEntry.Position, info);
            
            foreach (DictionaryEntry link in info.LinkEntries.Values)
            {
                foreach (ArrayEntry linked in link.IterArray())
                {
                    if (!objectInfosByReference.ContainsKey(linked.Position))
                    {
                        ParseObjectInfo(linked, objectInfosByReference, infosByListName, str_Links);
                    }
                }
            }

            return info;
        }

        private static ObjectInfo GetObjectInfo(ref ArrayEntry objectEntry, SM3DWorldZone zone)
        {
            Dictionary<string, DictionaryEntry> properties = new Dictionary<string, DictionaryEntry>();
            Dictionary<string, DictionaryEntry> links = new Dictionary<string, DictionaryEntry>();

            ObjectInfo info = new ObjectInfo();

            foreach (DictionaryEntry entry in objectEntry.IterDictionary())
            {
                switch (entry.Key)
                {
                    case "Comment":
                    case "IsLinkDest":
                    case "LayerConfigName":
                        break; //ignore these
                    case "Id":
                        info.ID = entry.Parse();
                        zone?.SubmitID(info.ID);
                        break;
                    case "Links":
                        foreach (DictionaryEntry linkEntry in entry.IterDictionary())
                        {
                            links.Add(linkEntry.Key, linkEntry);
                        }
                        break;
                    case "ModelName":
                        info.ModelName = entry.Parse() ?? "";
                        break;
                    case "Rotate":
                        dynamic _data = entry.Parse();
                        info.Rotation = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Scale":
                        _data = entry.Parse();
                        info.Scale = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Translate":
                        _data = entry.Parse();
                        info.Position = new Vector3(
                            _data["X"] / 100f,
                            _data["Y"] / 100f,
                            _data["Z"] / 100f
                        );
                        break;
                    case "UnitConfigName":
                        info.ObjectName = entry.Parse();
                        break;
                    case "UnitConfig":
                        _data = entry.Parse();

                        info.DisplayTranslation = new Vector3(
                            _data["DisplayTranslate"]["X"] / 100f,
                            _data["DisplayTranslate"]["Y"] / 100f,
                            _data["DisplayTranslate"]["Z"] / 100f
                            );
                        info.DisplayRotation = new Vector3(
                            _data["DisplayRotate"]["X"],
                            _data["DisplayRotate"]["Y"],
                            _data["DisplayRotate"]["Z"]
                            );
                        info.DisplayScale = new Vector3(
                            _data["DisplayScale"]["X"],
                            _data["DisplayScale"]["Y"],
                            _data["DisplayScale"]["Z"]
                            );
                        info.ClassName = _data["ParameterConfigName"];
                        break;
                    default:
                        properties.Add(entry.Key, entry);
                        break;
                }
            }

            info.PropertyEntries = properties;
            info.LinkEntries = links;
            return info;
        }

        public struct ObjectInfo
        {
            public string ID { get; set; }
            public Dictionary<string, DictionaryEntry> LinkEntries { get; set; }
            public Dictionary<string, DictionaryEntry> PropertyEntries { get; set; }

            public string ObjectName { get; set; }
            public string ClassName { get; set; }
            public string ModelName { get; set; }

            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Scale { get; set; }

            public Vector3 DisplayTranslation { get; set; }
            public Vector3 DisplayRotation { get; set; }
            public Vector3 DisplayScale { get; set; }
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="Dictionary"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector) => new Dictionary<string, dynamic>
        {
            ["X"] = vector.X,
            ["Y"] = vector.Y,
            ["Z"] = vector.Z
        };

        /// <summary>
        /// Converts a Vector3 to a Dictionary
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector, float scaleFactor)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            vector *= scaleFactor;

            data["X"] = vector.X;
            data["Y"] = vector.Y;
            data["Z"] = vector.Z;

            return data;
        }
    }
}
