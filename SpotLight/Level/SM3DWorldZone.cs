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

namespace SpotLight.Level
{
    public class SM3DWorldZone
    {
        public static Dictionary<string, WeakReference<SM3DWorldZone>> loadedZones = new Dictionary<string, WeakReference<SM3DWorldZone>>();

        /// <summary>
        /// Name of the Level
        /// </summary>
        public readonly string levelName;
        /// <summary>
        /// Category that this level belongs to (Map, Design, Sound)
        /// </summary>
        public readonly string categoryName;
        /// <summary>
        /// Filename (includes the path)
        /// </summary>
        public readonly string fileName;
        /// <summary>
        /// Any extra files that may be inside the map
        /// </summary>
        Dictionary<string, dynamic> extraFiles = new Dictionary<string, dynamic>();

        public Dictionary<string, List<I3dWorldObject>> ObjLists = new Dictionary<string, List<I3dWorldObject>>();

        public List<I3dWorldObject> LinkedObjects = new List<I3dWorldObject>();

        public List<(ZoneTransform, SM3DWorldZone)> SubZones = new List<(ZoneTransform, SM3DWorldZone)>();

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
                zone = null;
                return false;
            }

            if (!File.Exists(fileName))
            {
                zone = null;
                return false;
            }

            zone = new SM3DWorldZone(fileName, levelName, categoryName);

            return true;
        }

        private SM3DWorldZone(string fileName, string levelName, string categoryName)
        {
            this.levelName = levelName;
            this.categoryName = categoryName;
            this.fileName = fileName;

            string directory = Path.GetDirectoryName(fileName);

            SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(fileName));

            foreach (KeyValuePair<string, byte[]> keyValuePair in sarc.Files)
            {
                if (keyValuePair.Key != levelName + categoryName + ".byml")
                {
                    if ((keyValuePair.Value[0] << 8 | keyValuePair.Value[1]) == ByamlFile.BYAML_MAGIC)
                        extraFiles.Add(keyValuePair.Key, ByamlFile.FastLoadN(new MemoryStream(keyValuePair.Value)));
                    else
                        extraFiles.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            Dictionary<long, I3dWorldObject> objectsByReference = new Dictionary<long, I3dWorldObject>();

            ByamlIterator byamlIter = new ByamlIterator(new MemoryStream(sarc.Files[levelName + categoryName + ".byml"]));
            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
            {
                if (entry.Key == "FilePath" || entry.Key == "Objs")
                    continue;

                if (entry.Key == "ZoneList")
                {
                    foreach (ArrayEntry obj in entry.IterArray())
                    {
                        Vector3 position = new Vector3();
                        Vector3 rotation = new Vector3();
                        SM3DWorldZone zone = null;
                        foreach (DictionaryEntry _entry in obj.IterDictionary())
                        {
                            if (_entry.Key == "UnitConfigName")
                                TryOpen($"{directory}\\{_entry.Parse()}{categoryName}1.szs", out zone);
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
                        }

                        if (zone == null)
                            ObjLists[entry.Key].Add(LevelIO.ParseObject(obj, this, objectsByReference));
                        else
                        {
                            Matrix3 rotMat = GL_EditorFramework.Framework.Mat3FromEulerAnglesDeg(rotation);
                            SubZones.Add((new ZoneTransform(
                                new Matrix4(rotMat) * Matrix4.CreateTranslation(position),
                                rotMat), zone));
                        }
                    }

                    continue;
                }

                ObjLists.Add(entry.Key, new List<I3dWorldObject>());

                foreach (ArrayEntry obj in entry.IterArray())
                {
                    ObjLists[entry.Key].Add(LevelIO.ParseObject(obj, this, objectsByReference));
                }
            }
        }

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save() => Save(fileName);

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <param name="fileName">the file name to save the zone as</param>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save(string fileName)
        {
            SarcData sarcData = new SarcData()
            {
                HashOnly = false,
                endianness = Endian.Big,
                Files = new Dictionary<string, byte[]>()
            };

            foreach (KeyValuePair<string, dynamic> keyValuePair in extraFiles)
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

                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in ObjLists)
                {
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
                    rootNode.AddArrayNodeRef(keyValuePair.Key, categoryNode, true);
                }

                rootNode.AddArrayNodeRef("Objs", objsNode);

                ByamlNodeWriter.ArrayNode zonesNode = writer.CreateArrayNode();

                int zoneID = 0;

                foreach ((ZoneTransform transform, SM3DWorldZone zone) in SubZones)
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
                    objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(transform.RotationTransform.ExtractDegreeEulerAngles()), true);
                    objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
                    objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(transform.PositionTransform.Row3.Xyz, 100f), true);

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

                    objNode.AddDynamicValue("UnitConfigName", zone.levelName);

                    zonesNode.AddDictionaryNodeRef(objNode, true);
                }

                rootNode.AddArrayNodeRef("ZoneList", zonesNode);

                writer.Write(rootNode, true);

                sarcData.Files.Add(levelName + categoryName + ".byml", stream.ToArray());
            }

            File.WriteAllBytes(fileName, YAZ0.Compress(SARC.PackN(sarcData).Item2));

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
