using SpotLight.EditorDrawables;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BYAML.ByamlFile;
using static SpotLight.Level.LevelIO;

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

namespace SpotLight.Database
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
        public Dictionary<string, Parameter> ObjectParameters = new Dictionary<string, Parameter>();
        /// <summary>
        /// Listing of all the object parameters inside this database
        /// </summary>
        public Dictionary<string, RailParam> RailParameters = new Dictionary<string, RailParam>();
        /// <summary>
        /// The Latest version of this database
        /// </summary>
        public static Version LatestVersion { get; } = new Version(1, 7);

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
                string ID = FS.PeekString(3);
                Parameter param;

                if (ID == "OBJ")
                     param = new ObjectParameter();
                else if (ID == "DSN")
                    param = new DesignParameter();
                else if (ID == "SND")
                    param = new SoundFXParameter();
                else
                    param = new ObjectParameter();

                param.Read(FS);
                ObjectParameters.Add(param.ClassName, param);
            }

            while (FS.Position % 4 != 0)
                FS.Position++;

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

            NO_RAILS:

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
            foreach (Parameter parameter in ObjectParameters.Values)
                parameter.Write(FS);

            FS.Write(new byte[4] { (byte)'D', (byte)'I', (byte)'C', (byte)'T' }, 0, 4);
            FS.Write(BitConverter.GetBytes(RailParameters.Count), 0, 4);
            foreach (RailParam railParam in RailParameters.Values)
                railParam.Write(FS);

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

            Dictionary<string, List<ObjectInfo>> MAPinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                ["AreaList"] = new List<ObjectInfo>(),
                ["CheckPointList"] = new List<ObjectInfo>(),
                ["DemoObjList"] = new List<ObjectInfo>(),
                ["GoalList"] = new List<ObjectInfo>(),
                ["ObjectList"] = new List<ObjectInfo>(),
                ["PlayerList"] = new List<ObjectInfo>(),
                ["SkyList"] = new List<ObjectInfo>(),
                ["Links"] = new List<ObjectInfo>()
            };
            Dictionary<string, List<ObjectInfo>> DESIGNinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                ["AreaList"] = new List<ObjectInfo>(),
                ["CheckPointList"] = new List<ObjectInfo>(),
                ["DemoObjList"] = new List<ObjectInfo>(),
                ["GoalList"] = new List<ObjectInfo>(),
                ["ObjectList"] = new List<ObjectInfo>(),
                ["PlayerList"] = new List<ObjectInfo>(),
                ["SkyList"] = new List<ObjectInfo>(),
                ["Links"] = new List<ObjectInfo>()
            };
            Dictionary<string, List<ObjectInfo>> SOUNDinfosByListName = new Dictionary<string, List<ObjectInfo>>()
            {
                ["AreaList"] = new List<ObjectInfo>(),
                ["CheckPointList"] = new List<ObjectInfo>(),
                ["DemoObjList"] = new List<ObjectInfo>(),
                ["GoalList"] = new List<ObjectInfo>(),
                ["ObjectList"] = new List<ObjectInfo>(),
                ["PlayerList"] = new List<ObjectInfo>(),
                ["SkyList"] = new List<ObjectInfo>(),
                ["Links"] = new List<ObjectInfo>()
            };

            for (int i = 0; i < Zones.Length; i++)
                GetObjectInfos(Zones[i], MAPinfosByListName);
            for (int i = 0; i < Designs.Length; i++)
                GetObjectInfos(Designs[i], DESIGNinfosByListName);
            for (int i = 0; i < Sounds.Length; i++)
                GetObjectInfos(Sounds[i], SOUNDinfosByListName);

            void GetParameters<T>(Dictionary<string, List<ObjectInfo>> infosByListName) where T : Parameter, new()
            {
                byte ListID = 0;



                foreach (KeyValuePair<string, List<ObjectInfo>> keyValuePair in infosByListName)
                {
                    for (int j = 0; j < keyValuePair.Value.Count; j++)
                    {
                        ObjectInfo Tmp = keyValuePair.Value[j];
                        
                        if (Tmp.ID.StartsWith("rail") && !RailParameters.ContainsKey(Tmp.ClassName))
                        {
                            RailParameters[Tmp.ClassName] = new RailParam() { ClassName = Tmp.ClassName };

                            foreach (var propertyEntry in Tmp.PropertyEntries)
                            {
                                switch(propertyEntry.Key)
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
                                        foreach(var entry in iter.Current.IterDictionary())
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
                                                    ByamlNodeType _type = entry.NodeType;
                                                    if (_type == ByamlNodeType.Null)
                                                        _type = ByamlNodeType.StringIndex;
                                                    RailParameters[Tmp.ClassName].PointProperties.Add(new KeyValuePair<string, string>(entry.Key, _type == ByamlNodeType.StringIndex ? "String" : _type.ToString()));
                                                    continue;

                                            }
                                        }
                                        continue;
                                    default:
                                        ByamlNodeType type = propertyEntry.Value.NodeType;
                                        if (type == ByamlNodeType.Null)
                                            type = ByamlNodeType.StringIndex;
                                        RailParameters[Tmp.ClassName].Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type == ByamlNodeType.StringIndex ? "String" : type.ToString()));
                                        continue;
                                }
                            }
                        }

                        if (ObjectParameters.ContainsKey(Tmp.ClassName))
                        {
                            if (Tmp.ObjectName != "" && !ObjectParameters[Tmp.ClassName].ObjectNames.Any(O => O == Tmp.ObjectName))
                                ObjectParameters[Tmp.ClassName].ObjectNames.Add(Tmp.ObjectName);

                            if (Tmp.ModelName != "" && !ObjectParameters[Tmp.ClassName].ModelNames.Any(O => O == Tmp.ModelName))
                                ObjectParameters[Tmp.ClassName].ModelNames.Add(Tmp.ModelName);

                            foreach (var propertyEntry in Tmp.PropertyEntries)
                            {
                                if (!ObjectParameters[Tmp.ClassName].Properties.Any(O => O.Key == propertyEntry.Key))
                                {
                                    ByamlNodeType type = propertyEntry.Value.NodeType;
                                    if (type == ByamlNodeType.Null)
                                        type = ByamlNodeType.StringIndex;
                                    ObjectParameters[Tmp.ClassName].Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type == ByamlNodeType.StringIndex ? "String" : type.ToString()));
                                }
                            }

                            foreach (string key in Tmp.LinkEntries.Keys)
                            {
                                if (!ObjectParameters[Tmp.ClassName].LinkNames.Any(O => O == key))
                                    ObjectParameters[Tmp.ClassName].LinkNames.Add(key);
                            }
                        }
                        else
                        {
                            T OP = new T() { ClassName = Tmp.ClassName };
                            if (Tmp.ObjectName != "")
                                OP.ObjectNames.Add(Tmp.ObjectName);
                            if (Tmp.ModelName != "" && Tmp.ModelName != null)
                                OP.ModelNames.Add(Tmp.ModelName);

                            foreach (var propertyEntry in Tmp.PropertyEntries)
                            {
                                ByamlNodeType type = propertyEntry.Value.NodeType;
                                if (type == ByamlNodeType.Null)
                                    type = ByamlNodeType.StringIndex;
                                OP.Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type == ByamlNodeType.StringIndex ? "String" : type.ToString()));
                            }

                            foreach (string key in Tmp.LinkEntries.Keys)
                                OP.LinkNames.Add(key);

                            OP.ObjList = (ObjList)ListID;
                            ObjectParameters.Add(Tmp.ClassName,OP);
                        }
                    }
                    ListID++;
                }
            }

            GetParameters<ObjectParameter>(MAPinfosByListName);
            GetParameters<DesignParameter>(DESIGNinfosByListName);
            GetParameters<SoundFXParameter>(SOUNDinfosByListName);
        }

        public override string ToString() => $"Object Database Version {Version.Major}.{Version.Minor} [{ObjectParameters.Count} Parameters]";

        /// <summary>
        /// Adds new Properties to the <paramref name="propertyDict"/> as specified in the <paramref name="propertiesFromDB"/>
        /// </summary>
        public static void AddToProperties(List<KeyValuePair<string, string>> propertiesFromDB, Dictionary<string, dynamic> propertyDict)
        {
            for (int i = 0; i < propertiesFromDB.Count; i++)
            {
                switch (propertiesFromDB[i].Value)
                {
                    case "Integer":
                        propertyDict.Add(propertiesFromDB[i].Key, 0);
                        break;
                    case "Float":
                        propertyDict.Add(propertiesFromDB[i].Key, 0.0f);
                        break;
                    case "Boolean":
                        propertyDict.Add(propertiesFromDB[i].Key, false);
                        break;
                    case "Null":
                    case "String":
                        propertyDict.Add(propertiesFromDB[i].Key, "");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Base class for Object Parameters
    /// </summary>
    public abstract class Parameter
    {
        abstract internal string Identifier { get; set; }

        //abstract public string ListName { get; }

        public abstract bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList);

        public virtual string ClassName { get; set; }
        public virtual ObjList ObjList { get; set; }
        public virtual List<string> ObjectNames { get; set; }
        public virtual List<string> ModelNames { get; set; }
        public virtual List<KeyValuePair<string, string>> Properties { get; set; }
        public virtual List<string> LinkNames { get; set; }

        public Parameter() => InitLists();

        /// <summary>
        /// Read a parameter from a file
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Read(Stream FS)
        {
            byte[] Read = new byte[3];
            FS.Read(Read, 0, 3);
//            if (Encoding.ASCII.GetString(Read) != Identifier)
//                throw new Exception(
//$@"Database Error at Spotlight.ObjectParameterDatabase.cs in ABSTRACT CLASS: Parameter.
//Database Read Exception => File Offset 0x{FS.Position.ToString("X2").PadLeft(8, '0')}
//Got {Encoding.ASCII.GetString(Read)}, Expected {Identifier}");
            Identifier = Encoding.ASCII.GetString(Read);
            ObjList = (ObjList)FS.ReadByte();
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
                Properties.Add(new KeyValuePair<string, string>(FS.ReadString(), FS.ReadString()));
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
            List<byte> ByteList = new List<byte>() { (byte)Identifier[0], (byte)Identifier[1], (byte)Identifier[2] };
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
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Key));
                ByteList.Add(0x00);
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Value));
                ByteList.Add(0x00);
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
        public virtual General3dWorldObject ToGeneral3DWorldObject(string ID, SM3DWorldZone zone, OpenTK.Vector3 Position, int ObjectNameID = 0, int ModelNameID = -1)
        {
            Dictionary<string, dynamic> Params = new Dictionary<string, dynamic>();

            ObjectParameterDatabase.AddToProperties(Properties, Params);

            Dictionary<string, List<I3dWorldObject>> Links = new Dictionary<string, List<I3dWorldObject>>();
            for (int i = 0; i < LinkNames.Count; i++)
                Links.Add(LinkNames[i], new List<I3dWorldObject>());


            return new General3dWorldObject(Position, new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), ID, ObjectNames[ObjectNameID], ModelNameID == -1 ? "" : ModelNames[ModelNameID], ClassName, new OpenTK.Vector3(0f), new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), Links, Params, zone);
        }

        internal void InitLists()
        {
            ObjectNames = new List<string>();
            ModelNames = new List<string>();
            LinkNames = new List<string>();
            Properties = new List<KeyValuePair<string, string>>();
        }

        public override string ToString() => "Base Parameter Class";
        /// <summary>
        /// Compare 2 Object Parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ClassName != ((ObjectParameter)obj).ClassName || ObjectNames.Count != ((ObjectParameter)obj).ObjectNames.Count || ModelNames.Count != ((ObjectParameter)obj).ModelNames.Count || Properties.Count != ((ObjectParameter)obj).Properties.Count || LinkNames.Count != ((ObjectParameter)obj).LinkNames.Count)
                return false;

            for (int i = 0; i < ObjectNames.Count; i++)
                if (ObjectNames[i] != ((ObjectParameter)obj).ObjectNames[i])
                    return false;

            for (int i = 0; i < ModelNames.Count; i++)
                if (ModelNames[i] != ((ObjectParameter)obj).ModelNames[i])
                    return false;

            for (int i = 0; i < Properties.Count; i++)
                if (Properties[i].Key != ((ObjectParameter)obj).Properties[i].Key || Properties[i].Value != ((ObjectParameter)obj).Properties[i].Value)
                    return false;

            for (int i = 0; i < LinkNames.Count; i++)
                if (LinkNames[i] != ((ObjectParameter)obj).LinkNames[i])
                    return false;

            return true;
        }
        public override int GetHashCode() => base.GetHashCode();
    }

    public class RailParam
    {
        public virtual string ClassName { get; set; }
        public virtual List<KeyValuePair<string, string>> Properties { get; set; }
        public virtual List<KeyValuePair<string, string>> PointProperties { get; set; }

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
                Properties.Add(new KeyValuePair<string, string>(FS.ReadString(), FS.ReadString()));
            while (FS.Position % 4 != 0)
                FS.Position++;

            FS.Read(Read, 0, 2);
            int PointParamNameCount = BitConverter.ToInt16(Read, 0);

            for (int i = 0; i < PointParamNameCount; i++)
                PointProperties.Add(new KeyValuePair<string, string>(FS.ReadString(), FS.ReadString()));
            while (FS.Position % 4 != 0)
                FS.Position++;
        }
        /// <summary>
        /// Write this parameter
        /// </summary>
        /// <param name="FS"></param>
        public virtual void Write(FileStream FS)
        {
            List<byte> ByteList = new List<byte>() { (byte)'R', (byte)'A', (byte)'L' };
            ByteList.Add(0x00);
            ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(ClassName));
            ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)Properties.Count));
            for (int i = 0; i < Properties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Key));
                ByteList.Add(0x00);
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(Properties[i].Value));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            ByteList.AddRange(BitConverter.GetBytes((ushort)PointProperties.Count));
            for (int i = 0; i < PointProperties.Count; i++)
            {
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(PointProperties[i].Key));
                ByteList.Add(0x00);
                ByteList.AddRange(Encoding.GetEncoding(932).GetBytes(PointProperties[i].Value));
                ByteList.Add(0x00);
            }
            while (ByteList.Count % 4 != 0)
                ByteList.Add(0x00);

            FS.Write(ByteList.ToArray(), 0, ByteList.Count);
        }

        internal void InitLists()
        {
            Properties = new List<KeyValuePair<string, string>>();
            PointProperties = new List<KeyValuePair<string, string>>();
        }

        public override string ToString() => $"Rail Type:{ClassName} | {Properties.Count} Properties | {PointProperties.Count} Point Properties";
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
        Linked
    }

    /// <summary>
    /// Object Parameter for objects that go in the MAP Archive
    /// </summary>
    public class ObjectParameter : Parameter
    {
        internal override string Identifier { get => "OBJ"; set { } }

        public override bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            if (ObjList == ObjList.Linked)
            {
                objList = zone.LinkedObjects;
                return true;
            }
            else
            {
                string listName = SM3DWorldZone.MAP_PREFIX + ObjList.ToString();

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
        }

        public override string ToString() => $"Map Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
    }
    /// <summary>
    /// Object Parameter for objects that go in the DESIGN Archive
    /// </summary>
    public class DesignParameter : Parameter
    {
        internal override string Identifier { get => "DSN"; set { } }

        public override bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            if (ObjList == ObjList.Linked)
            {
                objList = zone.LinkedObjects;
                return true;
            }
            else
            {
                string listName = SM3DWorldZone.DESIGN_PREFIX + ObjList.ToString();

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
        }

        public override string ToString() => $"Design Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
    }
    /// <summary>
    /// Object Parameter for objects that go in the SOUND Archive
    /// </summary>
    public class SoundFXParameter : Parameter
    {
        internal override string Identifier { get => "SND"; set { } }

        public override bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            if (ObjList == ObjList.Linked)
            {
                objList = zone.LinkedObjects;
                return true;
            }
            else
            {
                string listName = SM3DWorldZone.SOUND_PREFIX + ObjList.ToString();

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
        }

        public override string ToString() => $"Sound Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
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
        public static string PeekString(this Stream FS, int length)
        {
            string target = FS.ReadString(length);
            FS.Position -= target.Length;
            return target;
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
namespace SpotLight.Database
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
        private List<Information> ObjectInformations = new List<Information>();
        /// <summary>
        /// The Latest version of this database
        /// </summary>
        public static Version LatestVersion { get; } = new Version(1, 2);
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
        {
            FileStream FS = new FileStream(Filename, FileMode.Open);
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "SODD")
                throw new Exception("Invalid Database File");

            Version Check = new Version(FS.ReadByte(), FS.ReadByte());

            FS.ReadByte();
            FS.ReadByte();

            if (Check < Version)
            {
                //Version 1.0 backwards compatability
                if (Check.Equals(new Version(1, 0)))
                {
                    while (FS.Position < FS.Length)
                        ObjectInformations.Add(new Information() { ClassName = FS.ReadString(), Description = FS.ReadString() });
                }
                //Version 1.1 backwards compatability
                if (Check.Equals(new Version(1, 1)))
                {
                    while (FS.Position < FS.Length)
                    {
                        Information NewInfo = new Information() { ClassName = FS.ReadString(), Description = FS.ReadString() };
                        Read = new byte[2];
                        FS.Read(Read, 0, 2);
                        ushort PropCount = BitConverter.ToUInt16(Read, 0);
                        for (int i = 0; i < PropCount; i++)
                            NewInfo.Properties.Add(FS.ReadString(), FS.ReadString());
                        ObjectInformations.Add(NewInfo);
                    }
                }
                return;
            }
            while (FS.Position < FS.Length)
            {
                Information NewInfo = new Information() { ClassName = FS.ReadString(), EnglishName = FS.ReadString(), Description = FS.ReadString() };
                Read = new byte[2];
                FS.Read(Read, 0, 2);
                ushort PropCount = BitConverter.ToUInt16(Read, 0);
                for (int i = 0; i < PropCount; i++)
                    NewInfo.Properties.Add(FS.ReadString(), FS.ReadString());
                ObjectInformations.Add(NewInfo);
            }
            FS.Close();
        }
        /// <summary>
        /// Save the database to a file
        /// </summary>
        /// <param name="Filename"></param>
        public void Save(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Create);
            FS.Write(new byte[8] { (byte)'S', (byte)'O', (byte)'D', (byte)'D', (byte)Version.Major, (byte)Version.Minor, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor }, 0, 8);
            for (int i = 0; i < ObjectInformations.Count; i++)
            {
                byte[] write = Encoding.GetEncoding(932).GetBytes(ObjectInformations[i].ClassName);
                FS.Write(write,0,write.Length);
                FS.WriteByte(0x00);

                write = Encoding.GetEncoding(932).GetBytes(ObjectInformations[i].EnglishName ?? ObjectInformations[i].ClassName);
                FS.Write(write, 0, write.Length);
                FS.WriteByte(0x00);

                write = Encoding.GetEncoding(932).GetBytes(ObjectInformations[i].Description);
                FS.Write(write, 0, write.Length);
                FS.WriteByte(0x00);

                FS.Write(BitConverter.GetBytes((ushort)ObjectInformations[i].Properties.Count), 0, 2);
                for (int j = 0; j < ObjectInformations[i].Properties.Count; j++)
                {
                    write = Encoding.GetEncoding(932).GetBytes(ObjectInformations[i].Properties.ElementAt(j).Key);
                    FS.Write(write, 0, write.Length);
                    FS.WriteByte(0x00);

                    write = Encoding.GetEncoding(932).GetBytes(ObjectInformations[i].Properties.ElementAt(j).Value);
                    FS.Write(write, 0, write.Length);
                    FS.WriteByte(0x00);
                }
            }
            FS.Close();
        }
        /// <summary>
        /// Get a piece of information
        /// </summary>
        /// <param name="TargetClassName">the Class to get information on</param>
        /// <returns></returns>
        public Information GetInformation(string TargetClassName)
        {
            List<Information> found = ObjectInformations.Where(p => string.Equals(p.ClassName, TargetClassName)).ToList();
            if (found.Count == 0)//An area that when entered activates a camera
                return new Information() { ClassName = TargetClassName, Description = "", EnglishName = TargetClassName };
            return found[0];
        }
        /// <summary>
        /// Set a piece of Information. If it already exists, and the input is not an empty piece of information, the data will be saved. If it's empty, the entry will be deleted.
        /// </summary>
        /// <param name="Info">Information to set</param>
        public void SetInformation(Information Info)
        {
            if (Info.EnglishName == null)
                Info.EnglishName = Info.ClassName;

            if (ObjectInformations.Any(p => p.Equals(Info)))
            {
                if (Info.Properties.Count == 0 && Info.Description.Length == 0 && (Info.EnglishName.Equals(Info.ClassName) || Info.EnglishName.Length == 0))
                    ObjectInformations.Remove(Info);
            }
            else
                ObjectInformations.Add(Info);
        }
       
        /// <summary>
        /// Clears all the Object descriptions from the database. Doesn't check to make sure the user actually wanted this though
        /// </summary>
        public void Clear() => ObjectInformations.Clear();

        public override string ToString() => $"Description Database Version {Version.Major}.{Version.Minor} [{ObjectInformations.Count} Objects Documented]";
    }

    public class Information
    {
        public string EnglishName;
        public string ClassName;
        public string Description;
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public string GetNoteForProperty(string PropertyName)
        {
            if (Properties.ContainsKey(PropertyName))
                return Properties[PropertyName];
            return "No Description Found";
        }
        public void SetNoteForProperty(string PropertyName, string PropertyDescription)
        {
            if (Properties.ContainsKey(PropertyName))
            {
                if (PropertyDescription.Length == 0)
                    Properties.Remove(PropertyName);
                else
                    Properties[PropertyName] = PropertyDescription;
            }
            else
                Properties.Add(PropertyName, PropertyDescription);
        }
        public override string ToString() => $"{ClassName} | {EnglishName}{(Properties.Count > 0 ? $" | {Properties.Count} Propert{(Properties.Count > 1 ? "ies" : "y")}" : "")}";
    }
}