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
using System.Windows.Forms;
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
        #region Constants
        public const string MAP_SUFFIX = "Map1.szs";
        public const string DESIGN_SUFFIX = "Design1.szs";
        public const string SOUND_SUFFIX = "Sound1.szs";

        public const string COMBINED_SUFFIX = ".szs";

        public const string COMMON_SUFFIX = "1.szs";

        private const string CATEGORY_MAP = "Map";
        private const string CATEGORY_DESIGN = "Design";
        private const string CATEGORY_SOUND = "Sound";

        static readonly string[] extensionsToReplace = new string[]
        {
            MAP_SUFFIX,
            DESIGN_SUFFIX,
            SOUND_SUFFIX,
            "Map.szs",
            "Design.szs",
            "Sound.szs",
            COMBINED_SUFFIX
        };

        public const string MAP_PREFIX = "Map_";
        public const string DESIGN_PREFIX = "Design_";
        public const string SOUND_PREFIX = "Sound_";
        #endregion

        public ByteOrder byteOrder;

        public static Dictionary<string, SM3DWorldZone> loadedZones = new Dictionary<string, SM3DWorldZone>();

        public override string ToString() => LevelName;

        public readonly Stack<IRevertable> undoStack;
        public readonly Stack<RedoEntry> redoStack;

        public virtual bool IsSaved
        {
            get => isSaved;
            set
            {
                if (isSaved != value)
                {
                    isSaved = value;

                    if (value)
                    {
                        if (undoStack.Count == 0)
                            LastSavedUndo = null;
                        else
                            LastSavedUndo = undoStack.Peek();
                    }
                }
            }
        }

        private DateTime lastSaveTime;
        bool isSaved = true;

        public IRevertable LastSavedUndo { get; private set; }

        /// <summary>
        /// Name of the Level
        /// </summary>
        public string LevelName
        {
            get => levelName;
            private set
            {
                levelName = value;
                if(LevelFileName != null && Directory != null)
                    loadedZones.Remove(Path.Combine(Directory, LevelFileName));

                LevelFileName = levelName + fileSuffix;

                if (Directory != null)
                    loadedZones.Add(Path.Combine(Directory, LevelFileName), this);
            }
        }
        /// <summary>
        /// The Directory this File is stored in
        /// </summary>
        public string Directory
        {
            get => directory;
            internal set
            {
                if (LevelFileName != null && Directory != null)
                    loadedZones.Remove(Path.Combine(Directory, LevelFileName));
                directory = value;
                if (LevelFileName != null)
                    loadedZones.Add(Path.Combine(Directory, LevelFileName), this);
            }
        }

        private readonly string fileSuffix;

        public bool HasCategoryMap { get; private set; }
        public bool HasCategoryDesign { get; private set; }
        public bool HasCategorySound { get; private set; }

        /// <summary>
        /// Name of the Level File
        /// </summary>
        public string LevelFileName { get; private set; }

        /// <summary>
        /// Any extra files that may be inside the map
        /// </summary>
        readonly Dictionary<string, dynamic>[] extraFiles = new Dictionary<string, dynamic>[]
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
            if (loadedZones.TryGetValue(fileName, out zone))
            {
                return true;
            }

            string levelFileName = Path.GetFileName(fileName);

            if (!File.Exists(fileName) && !File.Exists(Path.Combine(Program.BaseStageDataPath, levelFileName)))
            {
                zone = null;
                return false;
            }

            string levelName;
            string suffix;

            if (levelFileName.EndsWith(MAP_SUFFIX))
            {
                levelName = levelFileName.Remove(levelFileName.Length - MAP_SUFFIX.Length);
                suffix = MAP_SUFFIX;
            }
            else if (levelFileName.EndsWith(DESIGN_SUFFIX))
            {
                levelName = levelFileName.Remove(levelFileName.Length - DESIGN_SUFFIX.Length);
                suffix = DESIGN_SUFFIX;
            }
            else if (levelFileName.EndsWith(SOUND_SUFFIX))
            {
                levelName = levelFileName.Remove(levelFileName.Length - SOUND_SUFFIX.Length);
                suffix = SOUND_SUFFIX;
            }
            else if (levelFileName.EndsWith(COMBINED_SUFFIX))
            {
                levelName = levelFileName.Remove(levelFileName.Length - COMBINED_SUFFIX.Length);
                suffix = COMBINED_SUFFIX;
            }
            else
            {
                zone = null;
                return false;
            }

            zone = new SM3DWorldZone(Path.GetDirectoryName(fileName), levelName, suffix);

            return true;
        }

        public string GetPreferredSuffix() => IsCombined ? COMBINED_SUFFIX : (HasCategoryMap ? MAP_SUFFIX : (HasCategoryDesign ? DESIGN_SUFFIX : SOUND_SUFFIX));

        public bool IsValidSaveName(string name) =>
            (IsCombined && name.EndsWith(COMBINED_SUFFIX)) ||
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

        /// <summary>
        /// Create a new Super Mario 3D World Zone
        /// </summary>
        /// <param name="directory">StageData folder path</param>
        /// <param name="levelName">Internal name of the level</param>
        /// <param name="suffix">File Suffix </param>
        /// <param name="levelFileName">Name of the level file (Excludes the path, includes extension)</param>
        private SM3DWorldZone(string directory, string levelName, string suffix)
        {
            undoStack = new Stack<IRevertable>();
            redoStack = new Stack<RedoEntry>();

            fileSuffix = suffix;
            LevelName = levelName;
            Directory = directory;
            //will also add this zone to loadedZones

            Dictionary<string, I3dWorldObject> objectsByID = new Dictionary<string, I3dWorldObject>();

            if (suffix == COMBINED_SUFFIX)
            {
                LoadCombined(objectsByID);
            }
            if (suffix == MAP_SUFFIX)
            {
                HasCategoryMap = true;
                LoadCategory(MAP_PREFIX, CATEGORY_MAP, 0, objectsByID);
                HasCategoryDesign = LoadCategory(DESIGN_PREFIX, CATEGORY_DESIGN, 1, objectsByID);
                HasCategorySound = LoadCategory(SOUND_PREFIX, CATEGORY_SOUND, 2, objectsByID);
            }
            else if (suffix == DESIGN_SUFFIX)
            {
                HasCategoryDesign = true;
                LoadCategory(DESIGN_PREFIX, CATEGORY_DESIGN, 1, objectsByID);
            }
            else //if (suffix == SOUND_SUFFIX)
            {
                HasCategorySound = true;
                LoadCategory(SOUND_PREFIX, CATEGORY_SOUND, 2, objectsByID);
            }

            lastSaveTime = DateTime.Now;
        }

        private bool LoadCategory(string prefix, string categoryName, int extraFilesIndex, Dictionary<string, I3dWorldObject> linkedObjsByID)
        {
            string fileName = Path.Combine(Directory, $"{LevelName}{categoryName}1.szs");

            if (!File.Exists(fileName))
                return false;

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            byteOrder = sarc.byteOrder;

            string stageFileName = LevelName + categoryName + ".byml";

            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
            {
                if (keyValuePair.Key == stageFileName)
                {
                    Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

                    ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(keyValuePair.Value));
                    LoadStageByml(byamlIter, prefix, linkedObjsByID, objectsByReference);
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

        private bool IsCombined = false;
        private string levelName;
        private string directory;

        private bool LoadCombined(Dictionary<string, I3dWorldObject> linkedObjsByID)
        {
            string fileName = $"{Directory}\\{LevelName}.szs";

            if (!File.Exists(fileName))
                return false;

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            byteOrder = sarc.byteOrder;

            string stageFileNameMap = LevelName + "Map.byml";
            string stageFileNameDesign = LevelName + "Design.byml";
            string stageFileNameSound = LevelName + "Sound.byml";

            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
            {
                if (keyValuePair.Key == stageFileNameMap)
                {
                    Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

                    ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(keyValuePair.Value));
                    LoadStageByml(byamlIter, MAP_PREFIX, linkedObjsByID, objectsByReference);
                    HasCategoryMap = true;
                }
                else if (keyValuePair.Key == stageFileNameDesign)
                {
                    Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

                    ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(keyValuePair.Value));
                    LoadStageByml(byamlIter, DESIGN_PREFIX, linkedObjsByID, objectsByReference);
                    HasCategoryDesign = true;
                }
                else if (keyValuePair.Key == stageFileNameSound)
                {
                    Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

                    ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(keyValuePair.Value));
                    LoadStageByml(byamlIter, SOUND_PREFIX, linkedObjsByID, objectsByReference);
                    HasCategorySound = true;
                }
                else
                {
                    int extraFilesIndex;
                    switch (keyValuePair.Key)
                    {
                        case "CameraParam.byml":
                        case "InterpoleParam.byml":
                            extraFilesIndex = 0; //Map
                            break;
                        default:
                            extraFilesIndex = 1; //Design
                            break;
                    }

                    if ((keyValuePair.Value[0] << 8 | keyValuePair.Value[1]) == ByamlFile.BYAML_MAGIC)
                        extraFiles[extraFilesIndex].Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
                    else
                        extraFiles[extraFilesIndex].Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            IsCombined = true;

            return true;
        }

        private void LoadStageByml(ByamlIterator byamlIter, string prefix, Dictionary<string, I3dWorldObject> linkedObjsByID, Dictionary<long, I3dWorldObject> objectsByReference)
        {
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
                        Vector3 scale = Vector3.One;
                        SM3DWorldZone zone = null;
                        foreach (DictionaryEntry _entry in obj.IterDictionary())
                        {
                            if (_entry.Key == "UnitConfigName")
                            {
                                if (!TryOpen($"{Directory}\\{_entry.Parse()}{MAP_SUFFIX}", out zone))
                                    TryOpen($"{Directory}\\{_entry.Parse()}", out zone);
                            }
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

                        if (zone != null)
                        {
                            ZonePlacements.Add(new ZonePlacement(position, rotation, scale, zone));
                        }
                    }

                    continue;
                }

                ObjLists.Add(prefix + entry.Key, new ObjectList());

                foreach (ArrayEntry obj in entry.IterArray())
                {
                    I3dWorldObject _obj = LevelIO.ParseObject(obj, this, objectsByReference, out bool alreadyReferenced, Properties.Settings.Default.UniqueIDs ? linkedObjsByID : null);
                    if (!alreadyReferenced)
                        ObjLists[prefix + entry.Key].Add(_obj);
                }
            }
        }

        /// <summary>
        /// Saves the level as a new file
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
                LevelName = fileNameWithoutPath.Remove(fileNameWithoutPath.Length - SOUND_SUFFIX.Length);
            }

            Directory = Path.GetDirectoryName(fileName);

            return Save();
        }

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save()
        {
            if (IsCombined)
            {
                if (SaveCombined())
                    goto SAVED;
                else
                    return false;
            }

            if (HasCategoryMap && !SaveCategory(MAP_PREFIX, CATEGORY_MAP, 0, true))
                return false;
            if (HasCategoryDesign && !SaveCategory(DESIGN_PREFIX, CATEGORY_DESIGN, 1))
                return false;
            if (HasCategorySound && !SaveCategory(SOUND_PREFIX, CATEGORY_SOUND, 2))
                return false;

            SAVED:
            IsSaved = true;

            lastSaveTime = DateTime.Now;

            return true;
        }

        private bool SaveCategory(string prefix, string categoryName, int extraFilesIndex, bool saveZonePlacements = false)
        {
            SarcData sarcData = new SarcData()
            {
                HashOnly = false,
                byteOrder = byteOrder,
                Files = new Dictionary<string, byte[]>()
            };

            foreach (KeyValuePair<string, dynamic> keyValuePair in extraFiles[extraFilesIndex])
                SaveExtraFile(sarcData, keyValuePair);

            using (MemoryStream stream = new MemoryStream())
            {
                WriteStageByml(stream, prefix, saveZonePlacements);

                sarcData.Files.Add(LevelName + categoryName + ".byml", stream.ToArray());
            }

            File.WriteAllBytes(Path.Combine(Directory, LevelName + categoryName + COMMON_SUFFIX), YAZ0.Compress(SARC.PackN(sarcData).Item2));

            return true;
        }

        private static void SaveExtraFile(SarcData sarcData, KeyValuePair<string, dynamic> keyValuePair)
        {
            if (keyValuePair.Value is BymlFileData)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ByamlFile.FastSaveN(stream, keyValuePair.Value);
                    sarcData.Files.Add(keyValuePair.Key, stream.ToArray());
                }
            }
            else if (keyValuePair.Value is byte[])
            {
                sarcData.Files.Add(keyValuePair.Key, keyValuePair.Value);
            }
            else
                throw new Exception("The extra file " + keyValuePair.Key + "has no way to save");
        }

        private bool SaveCombined()
        {
            SarcData sarcData = new SarcData()
            {
                HashOnly = false,
                byteOrder = byteOrder,
                Files = new Dictionary<string, byte[]>()
            };
            for (int extraFilesIndex = 0; extraFilesIndex < 3; extraFilesIndex++)
            {
                foreach (KeyValuePair<string, dynamic> keyValuePair in extraFiles[extraFilesIndex])
                    SaveExtraFile(sarcData, keyValuePair);
            }

            if (HasCategoryMap)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    WriteStageByml(stream, MAP_PREFIX, true);

                    sarcData.Files.Add(LevelName + "Map.byml", stream.ToArray());
                }
            }

            if (HasCategoryDesign)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    WriteStageByml(stream, DESIGN_PREFIX, true);

                    sarcData.Files.Add(LevelName + "Design.byml", stream.ToArray());
                }
            }

            if (HasCategorySound)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    WriteStageByml(stream, SOUND_PREFIX, true);

                    sarcData.Files.Add(LevelName + "Sound.byml", stream.ToArray());
                }
            }

            File.WriteAllBytes(Path.Combine(Directory, LevelName + COMBINED_SUFFIX), YAZ0.Compress(SARC.PackN(sarcData).Item2));

            return true;
        }

        private void WriteStageByml(MemoryStream stream, string prefix, bool saveZonePlacements)
        {
            ByamlNodeWriter writer = new ByamlNodeWriter(stream, false, byteOrder, 1);

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
                        ["PlacementTargetFile"] = CATEGORY_MAP
                    }, true);

                    objNode.AddDynamicValue("UnitConfigName", zonePlacement.Zone.LevelName);

                    zonesNode.AddDictionaryNodeRef(objNode, true);
                }
            }

            rootNode.AddArrayNodeRef("ZoneList", zonesNode);

            writer.Write(rootNode, true);
        }

        public void Unload()
        {
            loadedZones.Remove(Path.Combine(Directory, LevelFileName));
        }

        public void CheckLocalFiles()
        {
            void CheckCategory(int extraFilesIndex, string categoryName)
            {
                string fileName = Path.Combine(Directory, LevelName + categoryName + COMMON_SUFFIX);

                if (File.GetLastWriteTime(fileName) > lastSaveTime && MessageBox.Show(
                    /*Todo localize*/
                    fileName + " was modified outside of Spotlight. Should all extra files be reloaded?",
                    "File Modified",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    extraFiles[extraFilesIndex].Clear();

                    SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

                    string stageFileName = LevelName + categoryName + ".byml";

                    foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
                    {
                        if (keyValuePair.Key != stageFileName)
                        {
                            if ((keyValuePair.Value[0] << 8 | keyValuePair.Value[1]) == ByamlFile.BYAML_MAGIC)
                                extraFiles[extraFilesIndex].Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
                            else
                                extraFiles[extraFilesIndex].Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                }
            }

            if (HasCategoryMap)
                CheckCategory(0, CATEGORY_MAP);
            if (HasCategoryDesign)
                CheckCategory(1, CATEGORY_DESIGN);
            if (HasCategorySound)
                CheckCategory(2, CATEGORY_SOUND);

            lastSaveTime = DateTime.Now;
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
