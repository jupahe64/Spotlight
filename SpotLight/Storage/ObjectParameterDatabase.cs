using BYAML;
using Spotlight.EditorDrawables;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BYAML.ByamlFile;
using static BYAML.ByamlIterator;
using static Spotlight.Level.LevelIO;
using static Spotlight.ObjectParameterForm;
using GL_EditorFramework;

/* File Format .sopd
 * ----------------------------------------------
 * Header - Spotlight Object Parameter Database
 * Magic - SOPD (0x04)
 * Version Major (0x01)
 * Version Minor (0x01)
 * Spotlight Version Major (0x01)
 * Spotlight Version Minor (0x01)
 * ---------------------------------------------- End of Header (0x08)
 * Dictionary - Contains Entries
 * Magic - DICT (0x04)
 * Entry Count (0x04)
 * ---------------------------------------------- 
 * Entry - Contains the Parameter Data
 * Magic - OPP (0x03)
 * Category - (0x01)
 * ClassName (0x??)
 * ObjectName Count (0x02)
 * ObjectName List (0x?? * ObjectNameCount)
 * Padding (0x00-0x03)
 * ModelName Count (0x02)
 * Modelname List (0x?? * ModelNameCount)
 * Padding (0x00-0x03)
 * Proprty Count (0x02)
 * Property List (0x?? * Property Length) [See the Property section below]
 * Padding (0x00-0x03)
 * LinkName Count (0x02)
 * LinkName List (0x?? * ModelNameCount)
 * Padding (0x00-0x03)
 * ---------------------------------------------- End of Entry
 * Properties - Property Data
 * Property Name (0x??)
 * Property Type (0x??)
 * ----------------------------------------------End of Dictionary
*/

namespace Spotlight.Database
{
    /// <summary>
    /// Database of all Object Parameters
    /// </summary>
    public class ObjectParameterDatabase
    {
        /// <summary>
        /// The version of this Object Parameter Database
        /// </summary>
        public Version Version = LatestVersion;
        /// <summary>
        /// Listing of all the object parameters inside this database
        /// </summary>
        public Dictionary<string, ObjectParam> ObjectParameters = new Dictionary<string, ObjectParam>();
        /// <summary>
        /// Listing of all the rail parameters inside this database
        /// </summary>
        public Dictionary<string, RailParam> RailParameters = new Dictionary<string, RailParam>();
        /// <summary>
        /// Listing of all the area parameters inside this database
        /// </summary>
        public Dictionary<string, AreaParam> AreaParameters = new Dictionary<string, AreaParam>();
        /// <summary>
        /// The Latest version of this database
        /// </summary>
        public static Version LatestVersion { get; } = new Version(1, 11);

        /// <summary>
        /// Create an empty Object Parameters File
        /// </summary>
        public ObjectParameterDatabase()
        {

        }
        /// <summary>
        /// Open an existing Object Parameters File
        /// </summary>
        /// <param name="Filename">Object Parameters File</param>
        public ObjectParameterDatabase(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Open);
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "SOPD")
                throw new Exception("Invalid Database File");

            Version Check = new Version(FS.ReadByte(), FS.ReadByte());

            if (Check < Version)
            {
                FS.Close();
                Version = Check;
                return;
            }
            FS.ReadByte();
            FS.ReadByte();

            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "DICT")
                throw new Exception("Invalid Database File");

            FS.Read(Read, 0, 4);
            int ParamCount = BitConverter.ToInt32(Read, 0);

            for (int i = 0; i < ParamCount; i++)
            {
                ObjectParam param = new ObjectParam();

                param.Read(FS);
                ObjectParameters.Add(param.ClassName, param);
            }

            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "DICT")
                goto NO_RAILS;

            FS.Read(Read, 0, 4);
            ParamCount = BitConverter.ToInt32(Read, 0);

            for (int i = 0; i < ParamCount; i++)
            {
                RailParam param = new RailParam();

                param.Read(FS);
                RailParameters.Add(param.ClassName, param);
            }


            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "DICT")
                goto NO_AREAS;
            
        NO_RAILS:

            FS.Read(Read, 0, 4);
            ParamCount = BitConverter.ToInt32(Read, 0);

            for (int i = 0; i < ParamCount; i++)
            {
                AreaParam param = new AreaParam();

                param.Read(FS);
                AreaParameters.Add(param.ClassName, param);
            }

        NO_AREAS:

            FS.Close();
        }
        /// <summary>
        /// Save the Object Parameters File
        /// </summary>
        /// <param name="Filename">Object Parameters File</param>
        public void Save(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Create);
            FS.Write(new byte[8] { (byte)'S', (byte)'O', (byte)'P', (byte)'D', (byte)Version.Major, (byte)Version.Minor, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor }, 0, 8);
            FS.Write(new byte[4] { (byte)'D', (byte)'I', (byte)'C', (byte)'T' }, 0, 4);
            FS.Write(BitConverter.GetBytes(ObjectParameters.Count), 0, 4);
            foreach (ObjectParam parameter in ObjectParameters.Values)
                parameter.Write(FS);

            FS.Write(new byte[4] { (byte)'D', (byte)'I', (byte)'C', (byte)'T' }, 0, 4);
            FS.Write(BitConverter.GetBytes(RailParameters.Count), 0, 4);
            foreach (RailParam railParam in RailParameters.Values)
                railParam.Write(FS);

            FS.Write(new byte[4] { (byte)'D', (byte)'I', (byte)'C', (byte)'T' }, 0, 4);
            FS.Write(BitConverter.GetBytes(AreaParameters.Count), 0, 4);
            foreach (AreaParam areaParam in AreaParameters.Values)
                areaParam.Write(FS);

            FS.Close();
        }
        /// <summary>
        /// Create from Reading the games files
        /// </summary>
        public void Create(string StageDataPath)
        {
            string[] Zones = Directory.GetFiles(StageDataPath, $"*{SM3DWorldZone.MAP_SUFFIX}");
            string[] Designs = Directory.GetFiles(StageDataPath, $"*{SM3DWorldZone.DESIGN_SUFFIX}");
            string[] Sounds = Directory.GetFiles(StageDataPath, $"*{SM3DWorldZone.SOUND_SUFFIX}");

            string[] Combined = Directory.GetFiles(StageDataPath, $"*{SM3DWorldZone.COMBINED_SUFFIX}");

            Dictionary<string, List<ObjectInfo>> MAPinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                [nameof(ObjList.AreaList)] = new List<ObjectInfo>(),
                [nameof(ObjList.CheckPointList)] = new List<ObjectInfo>(),
                [nameof(ObjList.DemoList)] = new List<ObjectInfo>(),
                [nameof(ObjList.GoalList)] = new List<ObjectInfo>(),
                [nameof(ObjList.Links)] = new List<ObjectInfo>(),
                [nameof(ObjList.ObjectList)] = new List<ObjectInfo>(),
                [nameof(ObjList.PlayerList)] = new List<ObjectInfo>(),
                [nameof(ObjList.RailList)] = new List<ObjectInfo>(),
                [nameof(ObjList.SkyList)] = new List<ObjectInfo>(),

                //Bowsers fury
                [nameof(ObjList.BowserSpawnList)] = new List<ObjectInfo>(),
                [nameof(ObjList.DemoObjList)] = new List<ObjectInfo>(),
                [nameof(ObjList.GoalItemList)] = new List<ObjectInfo>(),
                [nameof(ObjList.IslandFlagList)] = new List<ObjectInfo>(),
                [nameof(ObjList.IslandList)] = new List<ObjectInfo>(),
                [nameof(ObjList.IslandStartList)] = new List<ObjectInfo>(),
                [nameof(ObjList.OceanList)] = new List<ObjectInfo>(),
                [nameof(ObjList.RaidonSpawnList)] = new List<ObjectInfo>(),
                [nameof(ObjList.ZoneHolderList)] = new List<ObjectInfo>(),

#if ODYSSEY
                [nameof(ObjList.DemoObjList)] = new List<ObjectInfo>(),
                [nameof(ObjList.NatureList)] = new List<ObjectInfo>(),
                [nameof(ObjList.PlayerStartInfoList)] = new List<ObjectInfo>(),
                [nameof(ObjList.CapMessageList)] = new List<ObjectInfo>(),
                [nameof(ObjList.MapIconList)] = new List<ObjectInfo>(),
                [nameof(ObjList.SceneWatchObjList)] = new List<ObjectInfo>(),
                [nameof(ObjList.ScenarioStartCameraList)] = new List<ObjectInfo>(),
                [nameof(ObjList.PlayerAffectObjList)] = new List<ObjectInfo>(),
                [nameof(ObjList.RaceList)] = new List<ObjectInfo>(),
#endif
            };
            Dictionary<string, List<ObjectInfo>> DESIGNinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                [nameof(ObjList.AreaList)] = new List<ObjectInfo>(),
                [nameof(ObjList.Links)] = new List<ObjectInfo>(),
                [nameof(ObjList.ObjectList)] = new List<ObjectInfo>(),
            };
            Dictionary<string, List<ObjectInfo>> SOUNDinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                [nameof(ObjList.AreaList)] = new List<ObjectInfo>(),
                [nameof(ObjList.Links)] = new List<ObjectInfo>(),
                [nameof(ObjList.ObjectList)] = new List<ObjectInfo>(),
            };

            for (int i = 0; i < Zones.Length; i++)
                GetObjectInfos(Zones[i], MAPinfosByListName);
            for (int i = 0; i < Designs.Length; i++)
                GetObjectInfos(Designs[i], DESIGNinfosByListName);
            for (int i = 0; i < Sounds.Length; i++)
                GetObjectInfos(Sounds[i], SOUNDinfosByListName);

            for (int i = 0; i < Combined.Length; i++)
            {
                GetObjectInfosCombined(Combined[i],
                MAPinfosByListName,
                DESIGNinfosByListName,
                SOUNDinfosByListName);
            }


            void GetParameters(Dictionary<string, List<ObjectInfo>> infosByListName, Category category)
            {
                byte ListID = 0;



                foreach (var (listName, infos) in infosByListName)
                {
                    ObjList objList = (ObjList)Enum.Parse(typeof(ObjList), listName);

                    for (int j = 0; j < infos.Count; j++)
                    {
                        ObjectInfo info = infos[j];

                        if (info.ID.StartsWith("rail"))
                        {
                            CollectRailParameter(ref info);
                        }

                        else if ((info.ClassName=="Area")||info.ObjectName.Contains("Area") && AreaModelNames.Contains(info.ModelName))
                        {
                            CollectAreaParameter(ref info, category);
                        }

                        else
                        {
                            CollectObjectParameter(ref info, objList, category);
                        }
                    }
                    ListID++;
                }
            }

            GetParameters(MAPinfosByListName, Category.Map);
            GetParameters(DESIGNinfosByListName, Category.Design);
            GetParameters(SOUNDinfosByListName, Category.Sound);
        }

        private void CollectObjectParameter(ref ObjectInfo info, ObjList objList, Category category)
        {
            ObjectParam param;

            if (ObjectParameters.TryGetValue(info.ClassName, out param))
            {
                if (!string.IsNullOrEmpty(info.ObjectName) && !param.ObjectNames.Contains(info.ObjectName))
                    param.ObjectNames.Add(info.ObjectName);

                if (!string.IsNullOrEmpty(info.ModelName) && !param.ModelNames.Contains(info.ModelName))
                    param.ModelNames.Add(info.ModelName);

                foreach (var propertyEntry in info.PropertyEntries)
                {
                    if (!param.Properties.Any(O => O.Name == propertyEntry.Key))
                    {
                        if (TypeDef.TryGetFromNodeType(propertyEntry.Value.NodeType, out TypeDef typeDef))
                            param.Properties.Add(new PropertyDef(propertyEntry.Key, typeDef));
                    }
                }

                foreach (string key in info.LinkEntries.Keys)
                {
                    if (!param.LinkNames.Contains(key))
                        param.LinkNames.Add(key);
                }

                if(param.ObjList == ObjList.Links)
                    param.ObjList = objList;
            }
            else
            {
                param = new ObjectParam() { ClassName = info.ClassName, Category = category};
                param.ObjectNames.Add(info.ObjectName);

                if (!string.IsNullOrEmpty(info.ModelName))
                    param.ModelNames.Add(info.ModelName);

                foreach (var propertyEntry in info.PropertyEntries)
                {
                    if (TypeDef.TryGetFromNodeType(propertyEntry.Value.NodeType, out TypeDef typeDef))
                        param.Properties.Add(new PropertyDef(propertyEntry.Key, typeDef));
                }

                foreach (string key in info.LinkEntries.Keys)
                    param.LinkNames.Add(key);

                param.ObjList = objList;
                ObjectParameters.Add(info.ClassName, param);
            }
        }


        private void CollectRailParameter(ref ObjectInfo info)
        {
            //we assume that all Rails of the same class have the exact same properties
            //so we save time by skipping over the ones we already know

            if (!RailParameters.ContainsKey(info.ClassName))
            {
                RailParameters[info.ClassName] = new RailParam() { ClassName = info.ClassName };

                foreach (var propertyEntry in info.PropertyEntries)
                {
                    switch (propertyEntry.Key)
                    {
#if ODYSSEY
                        case "comment":
#else
                        case "Comment":
#endif
                        case "IsLadder":
                        case "IsClosed":
                        case "RailType":
                            continue;
                        case "RailPoints":
                            //extract all infos from the first RailPoint
                            var iter = propertyEntry.Value.IterArray().GetEnumerator();
                            iter.MoveNext();
                            foreach (var entry in iter.Current.IterDictionary())
                            {
                                switch (entry.Key)
                                {
#if ODYSSEY
                                    case "comment":
#else
                                    case "Comment":
#endif
                                    case "Id":
                                    case "IsLinkDest":
                                    case "LayerConfigName":
                                    case "Links":
                                    case "ModelName":
                                    case "Rotate":
                                    case "Scale":
                                    case "UnitConfig":
                                    case "UnitConfigName":
                                    case "Translate":
                                    case "ControlPoints":
                                        continue;
                                    default:
                                        if (TypeDef.TryGetFromNodeType(entry.NodeType, out TypeDef _typeDef))
                                            RailParameters[info.ClassName].PointProperties.Add(new PropertyDef(entry.Key, _typeDef));
                                        continue;

                                }
                            }
                            continue;
                        default:
                            if (TypeDef.TryGetFromNodeType(propertyEntry.Value.NodeType, out TypeDef typeDef))
                                RailParameters[info.ClassName].Properties.Add(new PropertyDef(propertyEntry.Key, typeDef));

                            continue;
                    }
                }

                foreach (var linkEntry in info.LinkEntries)
                    RailParameters[info.ClassName].LinkNames.Add(linkEntry.Key);
            }
        }

        private void CollectAreaParameter(ref ObjectInfo info, Category category)
        {
            if (AreaParameters.TryGetValue(info.ObjectName, out AreaParam param))
            {
                foreach (var propertyEntry in info.PropertyEntries)
                {
                    if (propertyEntry.Key !="Priority" && !param.Properties.Any(O => O.Name == propertyEntry.Key))
                    {
                        if (TypeDef.TryGetFromNodeType(propertyEntry.Value.NodeType, out TypeDef typeDef))
                            param.Properties.Add(new PropertyDef(propertyEntry.Key, typeDef));
                    }
                }

                foreach (string key in info.LinkEntries.Keys)
                {
                    if (!param.LinkNames.Contains(key))
                        param.LinkNames.Add(key);
                }
            }
            else
            {
                AreaParam OP = new AreaParam { ClassName = info.ObjectName, Category = category };

                foreach (var propertyEntry in info.PropertyEntries)
                {
                    if (propertyEntry.Key != "Priority")
                    {
                        if (TypeDef.TryGetFromNodeType(propertyEntry.Value.NodeType, out TypeDef typeDef))
                            OP.Properties.Add(new PropertyDef(propertyEntry.Key, typeDef));
                    }
                }

                foreach (string key in info.LinkEntries.Keys)
                    OP.LinkNames.Add(key);

                AreaParameters.Add(info.ObjectName, OP);
            }
        }


        public override string ToString() => $"Object Database Version {Version.Major}.{Version.Minor} [{ObjectParameters.Count} Parameters]";

        /// <summary>
        /// Adds new Properties to the <paramref name="propertyDict"/> as specified in the <paramref name="propertiesFromDB"/>
        /// </summary>
        public static void AddToProperties(List<PropertyDef> propertiesFromDB, Dictionary<string, dynamic> propertyDict)
        {
            for (int i = 0; i < propertiesFromDB.Count; i++)
                propertyDict.Add(propertiesFromDB[i].Name, propertiesFromDB[i].TypeDef.DefaultValue);
        }

        public static void AddToLinks(List<string> linkNames, Dictionary<string, List<I3dWorldObject>> links)
        {
            for (int i = 0; i < linkNames.Count; i++)
                links.Add(linkNames[i], new List<I3dWorldObject>());
        }
    }

    public struct PropertyDef
    {
        public PropertyDef(string name, TypeDef typeDef)
        {
            Name = name;
            TypeDef = typeDef;
        }

        public string Name { get; }
        public TypeDef TypeDef { get; }
    }

    public interface IParameter
    {
        string ClassName { get; }
        ObjList ObjList { get; }
    }

    /// <summary>
    /// Base class for Object Parameters
    /// </summary>
    public class ObjectParam : IParameter
    {
        public Category Category { get; set; }
        public ObjList ObjList { get; set; }
        public string ClassName { get; set; }
        public List<string> ObjectNames { get; set; }
        public List<string> ModelNames { get; set; }
        public List<PropertyDef> Properties { get; set; }
        public List<string> LinkNames { get; set; }

        public ObjectParam() => InitLists();

        /// <summary>
        /// Read a parameter from a file
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Read(Stream FS)
        {
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);
            Category = (Category)Read[2];
            ObjList = (ObjList)Read[3];

            ClassName = FS.ReadString();

            FS.Read(Read, 0, 2);
            int ObjectNameCount = BitConverter.ToInt16(Read, 0);
            for (int i = 0; i < ObjectNameCount; i++)
                ObjectNames.Add(FS.ReadString());

            while (FS.Position % 4 != 0)
                FS.Position++;
            FS.Read(Read, 0, 2);
            int ModelNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < ModelNameCount; i++)
                ModelNames.Add(FS.ReadString());

            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int ParamNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < ParamNameCount; i++)
                Properties.Add(new PropertyDef(FS.ReadString(), TypeDef.FromTypeID((byte)FS.ReadByte())));
            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int LinkNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < LinkNameCount; i++)
                LinkNames.Add(FS.ReadString());

            while (FS.Position % 4 != 0)
                FS.Position++;
        }
        /// <summary>
        /// Write this parameter
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Write(FileStream FS)
        {
            List<byte> ByteList = new List<byte>() { (byte)'O', (byte)'B' };
            ByteList.Add((byte)Category);
            ByteList.Add((byte)ObjList);
            ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ClassName));
            ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)ObjectNames.Count));
            for (int i = 0; i < ObjectNames.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ObjectNames[i]));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)ModelNames.Count));
            for (int i = 0; i < ModelNames.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ModelNames[i]));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)Properties.Count));
            for (int i = 0; i < Properties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Name));
                ByteList.Add(0x00);
                ByteList.Add(Properties[i].TypeDef.TypeID);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)LinkNames.Count));
            for (int i = 0; i < LinkNames.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(LinkNames[i]));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            FS.Write(ByteList.ToArray(), 0, ByteList.Count);
        }

        /// <summary>
        /// Cast an Object Parameter to a new General 3DW Object with mostly empty values
        /// </summary>
        /// <param name="ID">The ID to give the object.<para/>Can also use <see cref="SM3DWorldScene.NextObjID()"/></param>
        /// <param name="ObjectNameID">For Classes that have multiple Object Names. default is 0</param>
        /// <param name="ModelNameID">For Classes that have multiple Model Names. Default is -1 (No Model Name)</param>
        /// <returns>new General 3DW Object</returns>
        public virtual General3dWorldObject ToGeneral3DWorldObject(string ID, SM3DWorldZone zone, OpenTK.Vector3 Position, string ObjectName, string ModelName = "")
        {
            Dictionary<string, dynamic> Params = new Dictionary<string, dynamic>();

            ObjectParameterDatabase.AddToProperties(Properties, Params);

            Dictionary<string, List<I3dWorldObject>> Links = new Dictionary<string, List<I3dWorldObject>>();
            for (int i = 0; i < LinkNames.Count; i++)
                Links.Add(LinkNames[i], new List<I3dWorldObject>());


            return new General3dWorldObject(Position, new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), ID, ObjectName, ModelName, ClassName, new OpenTK.Vector3(0f), "None", Links, Params, zone);
        }

        static string[] CategoryPrefixes = new string[]
        {
            SM3DWorldZone.MAP_PREFIX,
            SM3DWorldZone.DESIGN_PREFIX,
            SM3DWorldZone.SOUND_PREFIX
        };

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            if (ObjList == ObjList.Links)
            {
                objList = zone.LinkedObjects;
                return true;
            }
            else
            {
                string listName = CategoryPrefixes[(int)Category] + ObjList.ToString();

                if (zone.ObjLists.ContainsKey(listName))
                {
                    objList = zone.ObjLists[listName];
                    return true;
                }
#if ODYSSEY
                else
                {
                    var newList = new ObjectList();
                    zone.ObjLists.Add(listName, newList);
                    objList = newList;
                    return true;
                }
#else
                else
                {
                    objList = null;
                    return false;
                }
#endif
            }
        }

        internal void InitLists()
        {
            ObjectNames = new List<string>();
            ModelNames = new List<string>();
            LinkNames = new List<string>();
            Properties = new List<PropertyDef>();
        }

        public override string ToString() => $"{Category} Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
        /// <summary>
        /// Compare 2 Object Parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ClassName != ((ObjectParam)obj).ClassName || ObjectNames.Count != ((ObjectParam)obj).ObjectNames.Count || ModelNames.Count != ((ObjectParam)obj).ModelNames.Count || Properties.Count != ((ObjectParam)obj).Properties.Count || LinkNames.Count != ((ObjectParam)obj).LinkNames.Count)
                return false;

            for (int i = 0; i < ObjectNames.Count; i++)
                if (ObjectNames[i] != ((ObjectParam)obj).ObjectNames[i])
                    return false;

            for (int i = 0; i < ModelNames.Count; i++)
                if (ModelNames[i] != ((ObjectParam)obj).ModelNames[i])
                    return false;

            for (int i = 0; i < Properties.Count; i++)
                if (Properties[i].Name != ((ObjectParam)obj).Properties[i].Name || Properties[i].TypeDef != ((ObjectParam)obj).Properties[i].TypeDef)
                    return false;

            for (int i = 0; i < LinkNames.Count; i++)
                if (LinkNames[i] != ((ObjectParam)obj).LinkNames[i])
                    return false;

            return true;
        }
        public override int GetHashCode() => base.GetHashCode();
    }

    public class RailParam : IParameter
    {
        public ObjList ObjList => ObjList.RailList;

        public virtual string ClassName { get; set; }
        public virtual List<PropertyDef> Properties { get; set; }
        public virtual List<PropertyDef> PointProperties { get; set; }
        public List<string> LinkNames { get; set; }

        public RailParam() => InitLists();

        /// <summary>
        /// Read a parameter from a file
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Read(Stream FS)
        {
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);
            ClassName = FS.ReadString();

            FS.Read(Read, 0, 2);
            int ParamNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < ParamNameCount; i++)
                Properties.Add(new PropertyDef(FS.ReadString(), TypeDef.FromTypeID((byte)FS.ReadByte())));
            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int PointParamNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < PointParamNameCount; i++)
                PointProperties.Add(new PropertyDef(FS.ReadString(), TypeDef.FromTypeID((byte)FS.ReadByte())));
            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int LinkNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < LinkNameCount; i++)
                LinkNames.Add(FS.ReadString());

            while (FS.Position % 4 != 0)
                FS.Position++;
        }
        /// <summary>
        /// Write this parameter
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Write(FileStream FS)
        {
            List<byte> ByteList = new List<byte>() { (byte)'R', (byte)'L' };
            ByteList.Add(0x00);
            ByteList.Add((byte)ObjList);
            ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ClassName));
            ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)Properties.Count));
            for (int i = 0; i < Properties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Name));
                ByteList.Add(0x00);
                ByteList.Add(Properties[i].TypeDef.TypeID);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)PointProperties.Count));
            for (int i = 0; i < PointProperties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(PointProperties[i].Name));
                ByteList.Add(0x00);
                ByteList.Add(PointProperties[i].TypeDef.TypeID);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)LinkNames.Count));
            for (int i = 0; i < LinkNames.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(LinkNames[i]));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            FS.Write(ByteList.ToArray(), 0, ByteList.Count);
        }

        internal void InitLists()
        {
            Properties = new List<PropertyDef>();
            PointProperties = new List<PropertyDef>();
            LinkNames = new List<string>();
        }

        public override string ToString() => $"Rail Type:{ClassName} | {Properties.Count} Properties | {PointProperties.Count} Point Properties";
    }


    public class AreaParam : IParameter
    {
        public ObjList ObjList => ObjList.AreaList;

        public Category Category { get; set; }
        public virtual string ClassName { get; set; }
        public virtual List<PropertyDef> Properties { get; set; }
        public virtual List<string> LinkNames { get; set; }

        public AreaParam() => InitLists();

        /// <summary>
        /// Read a parameter from a file
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Read(Stream FS)
        {
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);

            Category = (Category)Read[2];

            ClassName = FS.ReadString();

            FS.Read(Read, 0, 2);
            int ParamNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < ParamNameCount; i++)
                Properties.Add(new PropertyDef(FS.ReadString(), TypeDef.FromTypeID((byte)FS.ReadByte())));
            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int LinkNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < LinkNameCount; i++)
                LinkNames.Add(FS.ReadString());


            while (FS.Position % 4 != 0)
                FS.Position++;
        }
        /// <summary>
        /// Write this parameter
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Write(FileStream FS)
        {
            List<byte> ByteList = new List<byte>() { (byte)'A', (byte)'R'};
            ByteList.Add((byte)Category);
            ByteList.Add((byte)ObjList);
            ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ClassName));
            ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)Properties.Count));
            for (int i = 0; i < Properties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Name));
                ByteList.Add(0x00);
                ByteList.Add(Properties[i].TypeDef.TypeID);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)LinkNames.Count));
            for (int i = 0; i < LinkNames.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(LinkNames[i]));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            FS.Write(ByteList.ToArray(), 0, ByteList.Count);
        }

        static string[] CategoryPrefixes = new string[]
        {
            SM3DWorldZone.MAP_PREFIX,
            SM3DWorldZone.DESIGN_PREFIX,
            SM3DWorldZone.SOUND_PREFIX
        };

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            string listName = CategoryPrefixes[(int)Category] + ObjList.ToString();

            if (zone.ObjLists.ContainsKey(listName))
            {
                objList = zone.ObjLists[listName];
                return true;
            }
            else
            {
                objList = null;
                return false;
            }
        }

        internal void InitLists()
        {
            Properties = new List<PropertyDef>();
            LinkNames = new List<string>();
        }

        public override string ToString() => $"Rail Type:{ClassName} | {Properties.Count} Properties | {LinkNames.Count} Links";
    }

    /// <summary>
    /// Enum of the object Lists in SM3DW
    /// </summary>
    public enum ObjList : byte
    {
        /// <summary>
        /// Areas
        /// </summary>
        AreaList,
        /// <summary>
        /// Checkpoints
        /// </summary>
        CheckPointList,
        /// <summary>
        /// Cutscenes
        /// </summary>
        DemoList,
        /// <summary>
        /// Goals (End of Level)
        /// </summary>
        GoalList,
        /// <summary>
        /// Objects
        /// </summary>
        ObjectList,
        /// <summary>
        /// Starting points
        /// </summary>
        PlayerList,
        /// <summary>
        /// Skyboxes
        /// </summary>
        SkyList,
        /// <summary>
        /// Objects that are connected to other objects
        /// </summary>
        Links,
        /// <summary>
        /// Rails
        /// </summary>
        RailList,

        //Odyssey exclusives
        DemoObjList,
        NatureList,
        PlayerStartInfoList,
        CapMessageList,
        MapIconList,
        SceneWatchObjList,
        ScenarioStartCameraList,
        PlayerAffectObjList,
        RaceList,

        //Bowsers Fury
        BowserSpawnList,
        GoalItemList,
        IslandFlagList,
        IslandList,
        IslandStartList,
        OceanList,
        RaidonSpawnList,
        ZoneHolderList
    }

    public enum Category : byte
    {
        Map,
        Design,
        Sound
    }


    /// <summary>
    /// An exerpt from the Hackio.IO Library
    /// </summary>
    public static class HackioIOExcerpt
    {
        /// <summary>
        /// Excerpt from Hackio.IO. Reads a string in SHIFT-JIS. 0x00 terminated
        /// </summary>
        /// <param name="FS"></param>
        /// <returns>string</returns>
        public static string ReadString(this FileStream FS)
        {
            List<byte> bytes = new List<byte>();
            int strCount = 0;
            while (FS.ReadByte() != 0)
            {
                FS.Position -= 1;
                bytes.Add((byte)FS.ReadByte());

                strCount++;
                if (strCount > FS.Length)
                    throw new IOException("An error has occurred while reading the string");
            }
            return Encoding.GetEncoding("Shift-JIS").GetString(bytes.ToArray(), 0, bytes.ToArray().Length);
        }
        /// <summary>
        /// Excerpt from Hackio.IO Reads a string in SHIFT-JIS. 0x00 terminated
        /// </summary>
        /// <param name="FS"></param>
        /// <returns>string</returns>
        public static string ReadString(this Stream FS)
        {
            List<byte> bytes = new List<byte>();
            int strCount = 0;
            while (FS.ReadByte() != 0)
            {
                FS.Position -= 1;
                bytes.Add((byte)FS.ReadByte());

                strCount++;
                if (strCount > FS.Length)
                    throw new IOException("An error has occurred while reading the string");
            }
            return Encoding.GetEncoding("Shift-JIS").GetString(bytes.ToArray(), 0, bytes.ToArray().Length);
        }
        /// <summary>
        /// Excerpt from Hackio.IO Reads a string in SHIFT-JIS of a specified length
        /// </summary>
        /// <param name="FS"></param>
        /// <returns>string</returns>
        public static string ReadString(this Stream FS, int length)
        {
                byte[] bytes = new byte[length];
                FS.Read(bytes, 0, length);
                return Encoding.GetEncoding("Shift-JIS").GetString(bytes, 0, bytes.Length);
        }
    }
}

/* File Format .sodd
 * ----------------------------------------------
 * Header - Spotlight Object Description Database
 * Magic - SODD (0x04)
 * Version Major (0x01)
 * Version Minor (0x01)
 * Spotlight Version Major (0x01)
 * Spotlight Version Minor (0x01)
 * ---------------------------------------------- End of Header (0x08)
 * Strings - Contains descriptions
 * ==Pattern==
 * Object Name
 * Description
 * <Properties>
 * repeat ^
 * -----------
 * PropertyCount - 0x02 (ushort)
 * Property Name
 * Property Description
 * -----------
 */
namespace Spotlight.Database
{
    public class ObjectInformationDatabase
    {
        /// <summary>
        /// The version of this Object Description Database
        /// </summary>
        public Version Version = LatestVersion;
        /// <summary>
        /// List of Informations on certain objects
        /// </summary>
        private Dictionary<string, Information> ObjectInformations = new Dictionary<string, Information>();
        /// <summary>
        /// The Latest version of this database
        /// </summary>
        public static Version LatestVersion { get; } = new Version(1, 3);
        /// <summary>
        /// Create an Empry Database
        /// </summary>
        public ObjectInformationDatabase()
        {
            //nothing lol
        }
        /// <summary>
        /// Load a database from a file
        /// </summary>
        /// <param name="Filename"></param>
        public ObjectInformationDatabase(string Filename)
            : this(new FileStream(Filename, FileMode.Open))
        {
        }

        public ObjectInformationDatabase(Stream stream)
        {
            byte[] Read = new byte[4];
            stream.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "SODD")
                throw new Exception("Invalid Database File");



            Dictionary<string, string> ReadProperties()
            {
                ushort PropCount = BitConverter.ToUInt16(Read, 0);
                if (PropCount == 0)
                    return null;

                Dictionary<string, string> properties = new Dictionary<string, string>();

                for (int i = 0; i < PropCount; i++)
                    properties.Add(stream.ReadString(), stream.ReadString());

                return properties;
            }



            Version Check = new Version(stream.ReadByte(), stream.ReadByte());

            stream.ReadByte();
            stream.ReadByte();

            if (Check < Version)
            {
                //Version 1.0 backwards compatability
                if (Check.Equals(new Version(1, 0)))
                {
                    while (stream.Position < stream.Length)
                    {
                        var info = new Information(stream.ReadString(), string.Empty, stream.ReadString());
                        ObjectInformations.Add(info.ClassName, info);
                    }
                }
                //Version 1.1 backwards compatability
                else if (Check.Equals(new Version(1, 1)))
                {
                    while (stream.Position < stream.Length)
                    {
                        string className = stream.ReadString();
                        string description = stream.ReadString();

                        Read = new byte[2];
                        stream.Read(Read, 0, 2);

                        var properties = ReadProperties();

                        ObjectInformations.Add(className, new Information(className, null, description, properties));
                    }
                }
                //Version 1.2 backwards compatability
                else if (Check.Equals(new Version(1, 2)))
                {
                    while (stream.Position < stream.Length)
                    {
                        string className = stream.ReadString();
                        string englishName = stream.ReadString();
                        string description = stream.ReadString();

                        Read = new byte[2];
                        stream.Read(Read, 0, 2);

                        var properties = ReadProperties();

                        ObjectInformations.Add(className, new Information(className, englishName, description, properties));
                    }
                }
                else
                    System.Diagnostics.Debugger.Break();

                stream.Close();
                return;
            }
            while (stream.Position < stream.Length)
            {
                string className = stream.ReadString();
                string englishName = stream.ReadString();
                string description = stream.ReadString();

                Read = new byte[2];
                stream.Read(Read, 0, 2);

                var properties = ReadProperties();

                stream.Read(Read, 0, 2);
                var pathPointProperties = ReadProperties();

                ObjectInformations.Add(className, new Information(className, englishName, description, properties, pathPointProperties));
            }
            stream.Close();
        }
        /// <summary>
        /// Save the database to a file
        /// </summary>
        /// <param name="Filename"></param>
        public void Save(string Filename)
        {
            Save(new FileStream(Filename, FileMode.Create));
        }

        public void Save(Stream stream)
        {
            stream.Write(new byte[8] { (byte)'S', (byte)'O', (byte)'D', (byte)'D', (byte)Version.Major, (byte)Version.Minor, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor }, 0, 8);
            foreach (var info in ObjectInformations.Values)
            {
                byte[] write = Encoding.GetEncoding(932).GetBytes(info.ClassName);
                stream.Write(write,0,write.Length);
                stream.WriteByte(0x00);

                write = Encoding.GetEncoding(932).GetBytes(info.EnglishName ?? info.ClassName);
                stream.Write(write, 0, write.Length);
                stream.WriteByte(0x00);

                write = Encoding.GetEncoding(932).GetBytes(info.Description);
                stream.Write(write, 0, write.Length);
                stream.WriteByte(0x00);

                stream.Write(BitConverter.GetBytes((ushort)info.Properties.Count), 0, 2);
                for (int j = 0; j < info.Properties.Count; j++)
                {
                    write = Encoding.GetEncoding(932).GetBytes(info.Properties.ElementAt(j).Key);
                    stream.Write(write, 0, write.Length);
                    stream.WriteByte(0x00);

                    write = Encoding.GetEncoding(932).GetBytes(info.Properties.ElementAt(j).Value);
                    stream.Write(write, 0, write.Length);
                    stream.WriteByte(0x00);
                }

                stream.Write(BitConverter.GetBytes((ushort)info.PathPointProperties.Count), 0, 2);
                for (int j = 0; j < info.PathPointProperties.Count; j++)
                {
                    write = Encoding.GetEncoding(932).GetBytes(info.PathPointProperties.ElementAt(j).Key);
                    stream.Write(write, 0, write.Length);
                    stream.WriteByte(0x00);

                    write = Encoding.GetEncoding(932).GetBytes(info.PathPointProperties.ElementAt(j).Value);
                    stream.Write(write, 0, write.Length);
                    stream.WriteByte(0x00);
                }
            }
            stream.Close();
        }

        public EditableInformation GetEditableInformation(string className)
        {
            if (ObjectInformations.TryGetValue(className, out Information info))
                return new EditableInformation(info);
            else
            {
                return new EditableInformation(className, null, null);
            }
        }

        Information empty = new Information(null, null, null);

        public Information GetInformation(string className)
        {
            if (ObjectInformations.TryGetValue(className, out Information info))
                return info;
            else
                return empty;
        }

        public void SetInformation(EditableInformation info)
        {
            var ClassName = info.ClassName;
            var EnglishName = info.EnglishName == info.ClassName ? null : info.EnglishName;
            var Description = info.Description ?? string.Empty;

            var Properties = info.Properties.Count==0 ? null : info.Properties;
            var PathPointProperties = info.PathPointProperties.Count==0 ? null : info.PathPointProperties;


            if (info.EnglishName == info.ClassName &&
                info.Description == string.Empty &&
                info.Properties == null &&
                info.PathPointProperties == null)
                return;


            ObjectInformations[info.ClassName] = new Information(ClassName, EnglishName, Description, Properties, PathPointProperties);
        }

        /// <summary>
        /// Clears all the Object descriptions from the database. Doesn't check to make sure the user actually wanted this though
        /// </summary>
        public void Clear() => ObjectInformations.Clear();

        public override string ToString() => $"Description Database Version {Version.Major}.{Version.Minor} [{ObjectInformations.Count} Objects Documented]";
    }

    public sealed class EditableInformation
    {
        public string ClassName { get; private set; }

        public string EnglishName { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> Properties { get; private set; }
        public Dictionary<string, string> PathPointProperties { get; private set; }

        public EditableInformation(string className, string englishName, string description)
        {
            ClassName = className;
            EnglishName = englishName ?? ClassName;
            Description = description ?? string.Empty;

            Properties =          new Dictionary<string, string>();
            PathPointProperties = new Dictionary<string, string>();
        }

        public EditableInformation(Information information)
        {
            ClassName = information.ClassName;
            EnglishName = information.EnglishName ?? ClassName;
            Description = information.Description ?? string.Empty;

            Properties =          (information.Properties          as Dictionary<string, string>) ?? new Dictionary<string, string>();
            PathPointProperties = (information.PathPointProperties as Dictionary<string, string>) ?? new Dictionary<string, string>();
        }
    }

    public sealed class Information
    {
        private static IReadOnlyDictionary<string, string> EmptyDictionary = new Dictionary<string, string>();

        public Information(string className, string englishName, string description, IReadOnlyDictionary<string, string> properties = null, IReadOnlyDictionary<string, string> pathPointProperties = null)
        {
            ClassName = className;
            EnglishName = englishName;
            Description = description ?? string.Empty;
            Properties = properties ?? EmptyDictionary;
            PathPointProperties = pathPointProperties ?? EmptyDictionary;
        }

        public string ClassName { get; private set; }
        
        public string EnglishName { get; set; }
        public string Description { get; set; }

        public IReadOnlyDictionary<string, string> Properties { get; private set; }
        public IReadOnlyDictionary<string, string> PathPointProperties { get; private set; }


        public override string ToString() => $"{ClassName} | {EnglishName}{(Properties.Count > 0 ? $" | {Properties.Count} Propert{(Properties.Count > 1 ? "ies" : "y")}" : "")}";
    }
}