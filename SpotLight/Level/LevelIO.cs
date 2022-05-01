using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using OpenTK;
using Spotlight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;
using static BYAML.ByamlIterator;

namespace Spotlight.Level
{
    public class LevelIO
    {
        public static HashSet<string> AreaModelNames { get; private set; } = new HashSet<string>
        {
                "AreaCubeBase",
                "AreaCubeCenter",
#if ODYSSEY
                "AreaCubeTop",
#endif
                "AreaCylinder",
#if ODYSSEY
                "AreaCylinderTop",
                "AreaCylinderCenter",

                "AreaPrismBase",
                "AreaPrismTop",
                "AreaPrismCenter",
                "AreaInfinite",
#endif
                "AreaSphere",
        };

        public static void GetObjectInfosCombined(string fileName, 
            Dictionary<string, List<ObjectInfo>> MAPinfosByListName,
            Dictionary<string, List<ObjectInfo>> DESIGNinfosByListName,
            Dictionary<string, List<ObjectInfo>> SOUNDinfosByListName)
        {
            string levelName;

            string levelNameWithSuffix = Path.GetFileName(fileName);

            levelName = levelNameWithSuffix.Remove(levelNameWithSuffix.Length - SM3DWorldZone.COMBINED_SUFFIX.Length);


            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            GetObjectInfos(MAPinfosByListName, levelName, "Map", sarc);
            GetObjectInfos(DESIGNinfosByListName, levelName, "Design", sarc);
            GetObjectInfos(SOUNDinfosByListName, levelName, "Sound", sarc);
        }

        public static void GetObjectInfos(string fileName, Dictionary<string, List<ObjectInfo>> infosByListName)
        {
            string levelName;
            string categoryName;

            string levelNameWithSuffix = Path.GetFileName(fileName);

            if (fileName.EndsWith(SM3DWorldZone.MAP_SUFFIX))
            {
                levelName = levelNameWithSuffix.Remove(levelNameWithSuffix.Length - SM3DWorldZone.MAP_SUFFIX.Length);
                categoryName = "Map";
            }
            else if (fileName.EndsWith(SM3DWorldZone.DESIGN_SUFFIX))
            {
                levelName = levelNameWithSuffix.Remove(levelNameWithSuffix.Length - SM3DWorldZone.DESIGN_SUFFIX.Length);
                categoryName = "Design";
            }
            else if (fileName.EndsWith(SM3DWorldZone.SOUND_SUFFIX))
            {
                levelName = levelNameWithSuffix.Remove(levelNameWithSuffix.Length - SM3DWorldZone.SOUND_SUFFIX.Length);
                categoryName = "Sound";
            }
            else
            {
                return;
            }

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));
            GetObjectInfos(infosByListName, levelName, categoryName, sarc);
        }

        private static void GetObjectInfos(Dictionary<string, List<ObjectInfo>> infosByListName, string levelName, string categoryName, SarcData sarc)
        {
            Dictionary<long, ObjectInfo> objectInfosByReference = new Dictionary<long, ObjectInfo>();

            if(sarc.Files.TryGetValue(levelName + categoryName + ".byml", out var data))
            {
                ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(data));
#if ODYSSEY
            foreach (ArrayEntry scenario in byamlIter.IterRootArray())
                foreach (DictionaryEntry entry in scenario.IterDictionary())
                {
                    if (!infosByListName.ContainsKey(entry.Key))
                        continue;

                    List<ObjectInfo> objectInfos = new List<ObjectInfo>();

                    foreach (ArrayEntry obj in entry.IterArray())
                    {
                        objectInfos.Add(ParseObjectInfo(obj, objectInfosByReference, infosByListName, entry.Key));
                    }
                }
#else
                foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
                {
                    if (!infosByListName.ContainsKey(entry.Key))
                        continue;

                    List<ObjectInfo> objectInfos = new List<ObjectInfo>();

                    foreach (ArrayEntry obj in entry.IterArray())
                    {
                        var info = ParseObjectInfo(obj, objectInfosByReference, infosByListName, entry.Key);

                        objectInfos.Add(info);

                        if (
                            info.LayerName != "Common" &&
                            info.LayerName != "CommonSub" &&
                            info.LayerName != "Disaster" &&
                            !info.LayerName.StartsWith("Scenario") &&
                            !info.LayerName.StartsWith("NoScenario") &&
                            !info.LayerName.StartsWith("Phase")
                            )
                            Debugger.Break();
                    }
                }
#endif
            }
        }

        const string str_Links = "Links";

        public static ObjectInfo ParseObjectInfo(
            ArrayEntry objectEntry, Dictionary<long, ObjectInfo> objectInfosByReference, 
            Dictionary<string, List<ObjectInfo>> infosByListName, string objListName)
        {
            ObjectInfo info = GetObjectInfo(objectEntry);

            infosByListName[objListName].Add(info);

            if (!objectInfosByReference.ContainsKey(objectEntry.Position))
                objectInfosByReference.Add(objectEntry.Position, info);
            
            if(infosByListName.ContainsKey(str_Links))
            {
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
            }

            return info;
        }

        internal static ObjectInfo GetObjectInfo(in ArrayEntry objectEntry)
        {
            Dictionary<string, DictionaryEntry> properties = new Dictionary<string, DictionaryEntry>();
            Dictionary<string, DictionaryEntry> links = new Dictionary<string, DictionaryEntry>();

            ObjectInfo info = new ObjectInfo();

            foreach (DictionaryEntry entry in objectEntry.IterDictionary())
            {
                switch (entry.Key)
                {
                    case "comment":
                    case "Comment":
                        info.Comment = entry.Parse();
                        break;

                    case "IsLinkDest":
#if ODYSSEY
                    case "SrcUnitLayerList":
                    case "PlacementFileName":
#endif
                        break; //ignore these
                    case "Id":
                        info.ID = entry.Parse();
                        break;
                    case "LayerConfigName":
                        info.LayerName = entry.Parse();
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

                        info.DisplayName = _data["DisplayName"];
                        info.ClassName = _data["ParameterConfigName"];

                        info.ObjListName = _data["GenerateCategory"];
                        info.CategoryName = _data["PlacementTargetFile"];
                        break;
                    default:
                        if(!properties.ContainsKey(entry.Key))
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

            public string Comment { get; set; }
            public string ObjectName { get; set; }
            public string LayerName { get; set; }
            public string ClassName { get; set; }
            public string ModelName { get; set; }

            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Scale { get; set; }

            public Vector3 DisplayTranslation { get; set; }
            public Vector3 DisplayRotation { get; set; }
            public Vector3 DisplayScale { get; set; }
            public string DisplayName { get; set; }
            public dynamic ObjListName { get; internal set; }
            public dynamic CategoryName { get; internal set; }
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
