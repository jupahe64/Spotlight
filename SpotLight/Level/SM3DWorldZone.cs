using BYAML;
using GL_EditorFramework;
using OpenTK;
using SpotLight.EditorDrawables;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZS;
using static BYAML.ByamlIterator;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace SpotLight.Level
{
    public class ObjectList : List<I3dWorldObject>
    {

    }

    public class SM3DWorldZone
    {
        public static Dictionary<string, WeakReference<SM3DWorldZone>> loadedZones = new Dictionary<string, WeakReference<SM3DWorldZone>>();

        public override string ToString() => LevelName;

        public readonly Stack<IRevertable> undoStack;
        public readonly Stack<IRevertable> redoStack;

        /// <summary>
        /// Name of the Level
        /// </summary>
        public string LevelName { get; private set; }
        /// <summary>
        /// The Directory this File is stored in
        /// </summary>
        public string Directory { get; private set; }
        /// <summary>
        /// Name of the Level File
        /// </summary>
        public string LevelFileName { get; private set; }
        /// <summary>
        /// Any extra files that may be inside the map
        /// </summary>
        Dictionary<string, dynamic>[] extraFiles = new Dictionary<string, dynamic>[]
        {
            new Dictionary<string, dynamic>(),
            new Dictionary<string, dynamic>(),
            new Dictionary<string, dynamic>()
        };

        public Dictionary<string, ObjectList> ObjLists = new Dictionary<string, ObjectList>();

        public ObjectList LinkedObjects = new ObjectList();

        public readonly List<ZonePlacement> ZonePlacements = new List<ZonePlacement>();

        private ulong highestObjID = 0;

        public void SubmitID(string id)
        {
            if (id.StartsWith("obj") && ulong.TryParse(id.Substring(3), out ulong objID))
            {
                if (objID > highestObjID)
                    highestObjID = objID;
            }
        }

        private ulong highestRailID = 0;

        public bool IsPrepared { get; internal set; }

        public void SubmitRailID(string id)
        {
            if (id.StartsWith("rail") && ulong.TryParse(id.Substring(4), out ulong objID))
            {
                if (objID > highestRailID)
                    highestRailID = objID;
            }
        }

        public string NextObjID() => "obj" + (++highestObjID);

        public string NextRailID() => "rail" + (++highestRailID);

        public static bool TryOpen(string fileName, out SM3DWorldZone zone)
        {
            if (loadedZones.TryGetValue(fileName, out var reference))
            {
                if (reference.TryGetTarget(out zone))
                    return true;
            }

            string levelName;
            string categoryName;

            string fileNameWithoutPath = Path.GetFileName(fileName);

            if (fileNameWithoutPath.EndsWith(MAP_SUFFIX))
            {
                levelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - MAP_SUFFIX.Length);
                categoryName = "Map";
            }
            else if (fileNameWithoutPath.EndsWith(DESIGN_SUFFIX))
            {
                levelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - DESIGN_SUFFIX.Length);
                categoryName = "Design";
            }
            else if (fileNameWithoutPath.EndsWith(SOUND_SUFFIX))
            {
                levelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - DESIGN_SUFFIX.Length);
                categoryName = "Sound";
            }
            else
            {
                zone = null;
                return false;
            }

            if (!File.Exists(fileName))
            {
                zone = null;
                return false;
            }
            
            zone = new SM3DWorldZone(Path.GetDirectoryName(fileName), levelName, categoryName, fileNameWithoutPath);

            loadedZones.Add(fileName, new WeakReference<SM3DWorldZone>(zone));

            return true;
        }

        public const string MAP_SUFFIX = "Map1.szs";
        public const string DESIGN_SUFFIX = "Design1.szs";
        public const string SOUND_SUFFIX = "Sound1.szs";

        static readonly string[] extensionsToReplace = new string[]
        {
            MAP_SUFFIX,
            DESIGN_SUFFIX,
            SOUND_SUFFIX,
            "Map.szs",
            "Design.szs",
            "Sound.szs",
            ".szs"
        };

        public const string MAP_PREFIX = "Map_";
        public const string DESIGN_PREFIX = "Design_";
        public const string SOUND_PREFIX = "Sound_";

        public bool HasCategoryMap { get; private set; }
        public bool HasCategoryDesign { get; private set; }
        public bool HasCategorySound { get; private set; }

        public string GetPreferredSuffix() => HasCategoryMap ? "Map1.szs" : (HasCategoryDesign ? "Design1.szs" : "Sound1.szs");

        public bool IsValidSaveName(string name) =>
            (HasCategoryMap && name.EndsWith(MAP_SUFFIX)) ||
            (HasCategoryDesign && name.EndsWith(DESIGN_SUFFIX)) ||
            (HasCategorySound && name.EndsWith(SOUND_SUFFIX));

        public string GetProperSaveName(string name)
        {
            for (int i = 0; i < extensionsToReplace.Length; i++)
            {
                if (name.EndsWith(extensionsToReplace[i]))
                {
                    name = name.Remove(name.Length - extensionsToReplace[i].Length);
                    break;
                }
            }

            return name + GetPreferredSuffix();
        }

        private SM3DWorldZone(string directory, string levelName, string categoryName, string levelFileName)
        {
            undoStack = new Stack<IRevertable>();
            redoStack = new Stack<IRevertable>();

            LevelName = levelName;
            Directory = directory;
            LevelFileName = levelFileName;

            if (categoryName == "Map")
            {
                HasCategoryMap = true;
                LoadCategory(MAP_PREFIX, "Map", 0);
                HasCategoryDesign = LoadCategory(DESIGN_PREFIX, "Design", 1);
                HasCategorySound = LoadCategory(SOUND_PREFIX, "Sound", 2);
            }
            else if (categoryName == "Design")
            {
                HasCategoryDesign = true;
                LoadCategory(DESIGN_PREFIX, "Design", 1);
            }
            else //if (categoryName == "Sound")
            {
                HasCategorySound = true;
                LoadCategory(SOUND_PREFIX, "Sound", 2);
            }
        }

        private bool LoadCategory(string prefix, string categoryName, int extraFilesIndex)
        {
            string fileName = $"{Directory}\\{LevelName}{categoryName}1.szs";

            if (!File.Exists(fileName))
                return false;

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            string stageFileName = LevelName + categoryName + ".byml";

            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
            {
                if (keyValuePair.Key == stageFileName)
                {
                    Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

                    ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[LevelName + categoryName + ".byml"]));
                    foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
                    {
                        if (entry.Key == "FilePath" || entry.Key == "Objs")
                            continue;

                        if (entry.Key == "ZoneList")
                        {
                            foreach (ArrayEntry obj in entry.IterArray())
                            {
                                Vector3 position = Vector3.Zero;
                                Vector3 rotation = Vector3.Zero;
                                Vector3 scale =    Vector3.One;
                                SM3DWorldZone zone = null;
                                foreach (DictionaryEntry _entry in obj.IterDictionary())
                                {
                                    if (_entry.Key == "UnitConfigName")
                                        TryOpen($"{Directory}\\{_entry.Parse()}Map1.szs", out zone);
                                    else if (_entry.Key == "Translate")
                                    {
                                        dynamic data = _entry.Parse();
                                        position = new Vector3(
                                            data["X"] / 100f,
                                            data["Y"] / 100f,
                                            data["Z"] / 100f
                                        );
                                    }
                                    else if (_entry.Key == "Rotate")
                                    {
                                        dynamic data = _entry.Parse();
                                        rotation = new Vector3(
                                            data["X"],
                                            data["Y"],
                                            data["Z"]
                                        );
                                    }
                                    else if (_entry.Key == "Scale")
                                    {
                                        dynamic data = _entry.Parse();
                                        scale = new Vector3(
                                            data["X"],
                                            data["Y"],
                                            data["Z"]
                                        );
                                    }
                                }

                                if (zone == null)
                                    ObjLists[entry.Key].Add(LevelIO.ParseObject(obj, this, objectsByReference));
                                else
                                {
                                    ZonePlacements.Add(new ZonePlacement(position, rotation, scale, zone));
                                }
                            }

                            continue;
                        }

                        ObjLists.Add(prefix + entry.Key, new ObjectList());

                        foreach (ArrayEntry obj in entry.IterArray())
                        {
                            ObjLists[prefix + entry.Key].Add(LevelIO.ParseObject(obj, this, objectsByReference));
                        }
                    }
                }
                else
                {
                    if ((keyValuePair.Value[0] << 8 | keyValuePair.Value[1]) == ByamlFile.BYAML_MAGIC)
                        extraFiles[extraFilesIndex].Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
                    else
                        extraFiles[extraFilesIndex].Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return true;
        }
        
        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <param name="fileName">the file name to save the zone as</param>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save(string fileName)
        {
            string fileNameWithoutPath = Path.GetFileName(fileName);

            if (fileNameWithoutPath.EndsWith(MAP_SUFFIX))
            {
                LevelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - MAP_SUFFIX.Length);
            }
            else if (fileNameWithoutPath.EndsWith(DESIGN_SUFFIX))
            {
                LevelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - DESIGN_SUFFIX.Length);
            }
            else if (fileNameWithoutPath.EndsWith(SOUND_SUFFIX))
            {
                LevelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - DESIGN_SUFFIX.Length);
            }

            LevelFileName = fileNameWithoutPath;

            Directory = Path.GetDirectoryName(fileName);

            return Save();
        }

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save()
        {
            if (HasCategoryMap && !SaveCategory(Directory, MAP_PREFIX, "Map", 0, true))
                return false;
            if (HasCategoryDesign && !SaveCategory(Directory, DESIGN_PREFIX, "Design", 1))
                return false;
            if (HasCategorySound && !SaveCategory(Directory, SOUND_PREFIX, "Sound", 2))
                return false;

            return true;
        }
        
        private bool SaveCategory(string directory, string prefix, string categoryName, int extraFilesIndex, bool saveZonePlacements = false)
        {
            SarcData sarcData = new SarcData()
            {
                HashOnly = false,
                endianness = Endian.Big,
                Files = new Dictionary<string, byte[]>()
            };

            foreach (KeyValuePair<string, dynamic> keyValuePair in extraFiles[extraFilesIndex])
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    if (keyValuePair.Value is BymlFileData)
                    {
                        ByamlFile.FastSaveN(stream, keyValuePair.Value);
                        sarcData.Files.Add(keyValuePair.Key, stream.ToArray());
                    }
                    else if (keyValuePair.Value is byte[])
                    {
                        sarcData.Files.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    else
                        throw new Exception("The extra file " + keyValuePair.Key + "has no way to save");
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                ByamlNodeWriter writer = new ByamlNodeWriter(stream, false, Endian.Big, 1);

                ByamlNodeWriter.DictionaryNode rootNode = writer.CreateDictionaryNode(ObjLists);

                ByamlNodeWriter.ArrayNode objsNode = writer.CreateArrayNode();

                HashSet<I3dWorldObject> alreadyWrittenObjs = new HashSet<I3dWorldObject>();

                rootNode.AddDynamicValue("FilePath", "N/A");

                foreach (KeyValuePair<string, ObjectList> keyValuePair in ObjLists)
                {
                    if (!keyValuePair.Key.StartsWith(prefix)) //ObjList is not part of the Category
                        continue;

                    ByamlNodeWriter.ArrayNode categoryNode = writer.CreateArrayNode(keyValuePair.Value);

                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        if (!alreadyWrittenObjs.Contains(obj))
                        {
                            ByamlNodeWriter.DictionaryNode objNode = writer.CreateDictionaryNode(obj);
                            obj.Save(alreadyWrittenObjs, writer, objNode, false);
                            categoryNode.AddDictionaryNodeRef(objNode);
                            objsNode.AddDictionaryNodeRef(objNode);
                        }
                        else
                        {
                            categoryNode.AddDictionaryRef(obj);
                            objsNode.AddDictionaryRef(obj);
                        }
                    }
                    rootNode.AddArrayNodeRef(keyValuePair.Key.Substring(prefix.Length), categoryNode, true);
                }

                rootNode.AddArrayNodeRef("Objs", objsNode);

                ByamlNodeWriter.ArrayNode zonesNode = writer.CreateArrayNode();

                if (saveZonePlacements)
                {
                    int zoneID = 0;

                    foreach (var zonePlacement in ZonePlacements)
                    {
                        ByamlNodeWriter.DictionaryNode objNode = writer.CreateDictionaryNode();

                        objNode.AddDynamicValue("Comment", null);
                        objNode.AddDynamicValue("Id", "zone" + zoneID++);
                        objNode.AddDynamicValue("IsLinkDest", false);
                        objNode.AddDynamicValue("LayerConfigName", "Common");

                        {
                            objNode.AddDynamicValue("Links", new Dictionary<string, dynamic>(), true);
                        }

                        objNode.AddDynamicValue("ModelName", null);
                        objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(zonePlacement.Rotation), true);
                        objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(zonePlacement.Scale), true);
                        objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(zonePlacement.Position, 100f), true);

                        objNode.AddDynamicValue("UnitConfig", new Dictionary<string, dynamic>
                        {
                            ["DisplayName"] = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
                            ["DisplayRotate"] = LevelIO.Vector3ToDict(Vector3.Zero),
                            ["DisplayScale"] = LevelIO.Vector3ToDict(Vector3.One),
                            ["DisplayTranslate"] = LevelIO.Vector3ToDict(Vector3.Zero),
                            ["GenerateCategory"] = "",
                            ["ParameterConfigName"] = "Zone",
                            ["PlacementTargetFile"] = "Map"
                        }, true);

                        objNode.AddDynamicValue("UnitConfigName", zonePlacement.Zone.LevelName);

                        zonesNode.AddDictionaryNodeRef(objNode, true);
                    }
                }

                rootNode.AddArrayNodeRef("ZoneList", zonesNode);

                writer.Write(rootNode, true);

                sarcData.Files.Add(LevelName + categoryName + ".byml", stream.ToArray());
            }

            File.WriteAllBytes($"{directory}\\{LevelName}{categoryName}1.szs", YAZ0.Compress(SARC.PackN(sarcData).Item2));

            return true;
        }
    }

    public struct ZoneTransform
    {
        public ZoneTransform(Matrix4 positionTransform, Matrix3 rotationTransform)
        {
            PositionTransform = positionTransform;
            RotationTransform = rotationTransform;
        }

        public static ZoneTransform Identity = new ZoneTransform(Matrix4.Identity, Matrix3.Identity);

        public Matrix4 PositionTransform { get; set; }
        public Matrix3 RotationTransform { get; set; }
    }
}
