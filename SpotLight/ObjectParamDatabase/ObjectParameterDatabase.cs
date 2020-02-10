using SpotLight.EditorDrawables;
using SpotLight.Level;
using SpotLight.ObjectParamDatabase;
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

namespace SpotLight.ObjectParamDatabase
{
    /// <summary>
    /// Database of all Object Parameters
    /// </summary>
    public class ObjectParameterDatabase
    {
        public Version Version = LatestVersion;
        public List<Parameter> ObjectParameters = new List<Parameter>();
        public static Version LatestVersion { get; } = new Version(1, 5);

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
                ObjectParameters.Add(param);
            }

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
            for (int i = 0; i < ObjectParameters.Count; i++)
                ObjectParameters[i].Write(FS); 
            FS.Close();
        }
        /// <summary>
        /// Create from Reading the games files
        /// </summary>
        public void Create(string StageDataPath)
        {
            string[] Zones = Directory.GetFiles(StageDataPath,"*Map1.szs");
            string[] Designs = Directory.GetFiles(StageDataPath, "*Design1.szs");
            string[] Sounds = Directory.GetFiles(StageDataPath, "*Sound1.szs");

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

            byte ListID = 0;
            foreach (KeyValuePair<string, List<ObjectInfo>> keyValuePair in MAPinfosByListName)
            {
                for (int j = 0; j < keyValuePair.Value.Count; j++)
                {
                    if (keyValuePair.Value[j].ClassName == "Rail")
                    {
                        continue;
                    }

                    ObjectInfo Tmp = keyValuePair.Value[j];
                    if (ObjectParameters.Any(O => O.ClassName == Tmp.ClassName))
                    {
                        int ParamID = 0;
                        for (int y = 0; y < ObjectParameters.Count; y++)
                        {
                            if (ObjectParameters[y].ClassName == Tmp.ClassName)
                            {
                                ParamID = y;
                                break;
                            }
                        }
                        if (Tmp.ObjectName != "" && !ObjectParameters[ParamID].ObjectNames.Any(O => O == Tmp.ObjectName))
                            ObjectParameters[ParamID].ObjectNames.Add(Tmp.ObjectName);

                        if (Tmp.ModelName != "" && !ObjectParameters[ParamID].ModelNames.Any(O => O == Tmp.ModelName))
                            ObjectParameters[ParamID].ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            if (!ObjectParameters[ParamID].Properties.Any(O => O.Key == propertyEntry.Key))
                            {
                                ByamlNodeType type = propertyEntry.Value.NodeType;
                                ObjectParameters[ParamID].Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.ToString()));
                            }
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                        {
                            if (!ObjectParameters[ParamID].LinkNames.Any(O => O == key))
                                ObjectParameters[ParamID].LinkNames.Add(key);
                        }
                    }
                    else
                    {
                        ObjectParameter OP = new ObjectParameter() { ClassName = Tmp.ClassName };
                        if (Tmp.ObjectName != "")
                            OP.ObjectNames.Add(Tmp.ObjectName);
                        if (Tmp.ModelName != "" && Tmp.ModelName != null)
                            OP.ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            ByamlNodeType type = propertyEntry.Value.NodeType;
                            OP.Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.ToString()));
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                            OP.LinkNames.Add(key);

                        OP.CategoryID = ListID;
                        ObjectParameters.Add(OP);
                    }
                }
                ListID++;
            }
            ListID = 0;
            foreach (KeyValuePair<string, List<ObjectInfo>> keyValuePair in DESIGNinfosByListName)
            {
                for (int j = 0; j < keyValuePair.Value.Count; j++)
                {
                    if (keyValuePair.Value[j].ClassName == "Rail")
                    {
                        continue;
                    }

                    ObjectInfo Tmp = keyValuePair.Value[j];
                    if (ObjectParameters.Any(O => O.ClassName == Tmp.ClassName))
                    {
                        int ParamID = 0;
                        for (int y = 0; y < ObjectParameters.Count; y++)
                        {
                            if (ObjectParameters[y].ClassName == Tmp.ClassName)
                            {
                                ParamID = y;
                                break;
                            }
                        }
                        if (Tmp.ObjectName != "" && !ObjectParameters[ParamID].ObjectNames.Any(O => O == Tmp.ObjectName))
                            ObjectParameters[ParamID].ObjectNames.Add(Tmp.ObjectName);

                        if (Tmp.ModelName != "" && !ObjectParameters[ParamID].ModelNames.Any(O => O == Tmp.ModelName))
                            ObjectParameters[ParamID].ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            if (!ObjectParameters[ParamID].Properties.Any(O => O.Key == propertyEntry.Key))
                            {
                                Type type = propertyEntry.Value.GetType();
                                ObjectParameters[ParamID].Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.Name));
                            }
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                        {
                            if (!ObjectParameters[ParamID].LinkNames.Any(O => O == key))
                                ObjectParameters[ParamID].LinkNames.Add(key);
                        }
                    }
                    else
                    {
                        DesignParameter OP = new DesignParameter() { ClassName = Tmp.ClassName };
                        if (Tmp.ObjectName != "")
                            OP.ObjectNames.Add(Tmp.ObjectName);
                        if (Tmp.ModelName != "" && Tmp.ModelName != null)
                            OP.ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            Type type = propertyEntry.Value.GetType();
                            OP.Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.Name));
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                            OP.LinkNames.Add(key);

                        OP.CategoryID = ListID;
                        ObjectParameters.Add(OP);
                    }
                }
                ListID++;
            }
            ListID = 0;
            foreach (KeyValuePair<string, List<ObjectInfo>> keyValuePair in SOUNDinfosByListName)
            {
                for (int j = 0; j < keyValuePair.Value.Count; j++)
                {
                    if (keyValuePair.Value[j].ClassName == "Rail")
                    {
                        continue;
                    }

                    ObjectInfo Tmp = keyValuePair.Value[j];
                    if (ObjectParameters.Any(O => O.ClassName == Tmp.ClassName))
                    {
                        int ParamID = 0;
                        for (int y = 0; y < ObjectParameters.Count; y++)
                        {
                            if (ObjectParameters[y].ClassName == Tmp.ClassName)
                            {
                                ParamID = y;
                                break;
                            }
                        }
                        if (Tmp.ObjectName != "" && !ObjectParameters[ParamID].ObjectNames.Any(O => O == Tmp.ObjectName))
                            ObjectParameters[ParamID].ObjectNames.Add(Tmp.ObjectName);

                        if (Tmp.ModelName != "" && !ObjectParameters[ParamID].ModelNames.Any(O => O == Tmp.ModelName))
                            ObjectParameters[ParamID].ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            if (!ObjectParameters[ParamID].Properties.Any(O => O.Key == propertyEntry.Key))
                            {
                                Type type = propertyEntry.Value.GetType();
                                ObjectParameters[ParamID].Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.Name));
                            }
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                        {
                            if (!ObjectParameters[ParamID].LinkNames.Any(O => O == key))
                                ObjectParameters[ParamID].LinkNames.Add(key);
                        }
                    }
                    else
                    {
                        SoundFXParameter OP = new SoundFXParameter() { ClassName = Tmp.ClassName };
                        if (Tmp.ObjectName != "")
                            OP.ObjectNames.Add(Tmp.ObjectName);
                        if (Tmp.ModelName != "" && Tmp.ModelName != null)
                            OP.ModelNames.Add(Tmp.ModelName);

                        foreach (var propertyEntry in Tmp.PropertyEntries)
                        {
                            Type type = propertyEntry.Value.GetType();
                            OP.Properties.Add(new KeyValuePair<string, string>(propertyEntry.Key, type.Name));
                        }

                        foreach (string key in Tmp.LinkEntries.Keys)
                            OP.LinkNames.Add(key);

                        OP.CategoryID = ListID;
                        ObjectParameters.Add(OP);
                    }
                }
                ListID++;
            }
        }
        /// <summary>
        /// Finds an Object Parameter based on a string. This will return the ID of the first instance found.
        /// </summary>
        /// <param name="Target">The string to look for based on the <paramref name="Mode"/></param>
        /// <param name="Mode">Value Details =><para/>0x00 = Look for a Class<para/>0x01 - Look for an Object Name<para/>0x02 - Look for a Model Name<para/>0x03 - Look for a Parameter<para/>0x04 - Look for a Link Name<para/>0x05+ returns -1</param>
        /// <returns>ID of the Item. -1 if the Item is not found</returns>
        public int FindByID(string Target, byte Mode)
        {
            for (int i = 0; i < ObjectParameters.Count; i++)
                if (Mode == 0x00 ? (Target == ObjectParameters[i].ClassName) : (Mode == 0x01 ? ObjectParameters[i].ObjectNames.Any(O => O == Target) : (Mode == 0x02 ? ObjectParameters[i].ModelNames.Any(O => O == Target) : (Mode == 0x03 ? ObjectParameters[i].Properties.Any(O => O.Key == Target) : (Mode == 0x04 ? ObjectParameters[i].LinkNames.Any(O => O == Target) : false)))))
                    return i;

            return -1;
        }

        public override string ToString() => $"Object Database Version {Version.Major}.{Version.Minor} [{ObjectParameters.Count} Parameters]";
    }

    /// <summary>
    /// Base class for Object Parameters
    /// </summary>
    public abstract class Parameter
    {
        abstract internal string Identifier { get; set; }
        public virtual string ClassName { get; set; }
        public virtual byte CategoryID { get; set; }
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
            CategoryID = (byte)FS.ReadByte();
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
            ByteList.Add(CategoryID);
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
        public virtual General3dWorldObject ToGeneral3DWorldObject(string ID, OpenTK.Vector3 Position, int ObjectNameID = 0, int ModelNameID = -1)
        {
            Dictionary<string, dynamic> Params = new Dictionary<string, dynamic>();
            for (int i = 0; i < Properties.Count; i++)
            {
                switch (Properties[i].Value)
                {
                    case "Integer":
                        Params.Add(Properties[i].Key, 0);
                        break;
                    case "Float":
                        Params.Add(Properties[i].Key, 0.0f);
                        break;
                    case "Boolean":
                        Params.Add(Properties[i].Key, false);
                        break;
                    case "Null":
                    case "String":
                        Params.Add(Properties[i].Key, "");
                        break;
                }
            }

            Dictionary<string, List<I3dWorldObject>> Links = new Dictionary<string, List<I3dWorldObject>>();
            for (int i = 0; i < LinkNames.Count; i++)
                Links.Add(LinkNames[i], new List<I3dWorldObject>());


            return new General3dWorldObject(Position, new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), ID, ObjectNames[ObjectNameID], ModelNameID == -1 ? "" : ModelNames[ModelNameID], ClassName, new OpenTK.Vector3(0f), new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), Links, Params);
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
            if (!(obj is RailParameter))
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
            else
                return base.Equals(obj);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
    /// <summary>
    /// Object Parameter for objects that go in the MAP Archive
    /// </summary>
    public class ObjectParameter : Parameter
    {
        internal override string Identifier { get => "OBJ"; set { } }

        public override string ToString() => $"Map Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
    }
    /// <summary>
    /// Object Parameter for objects that go in the DESIGN Archive
    /// </summary>
    public class DesignParameter : Parameter
    {
        internal override string Identifier { get => "DSN"; set { } }

        public override string ToString() => $"Design Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
    }
    /// <summary>
    /// Object Parameter for objects that go in the SOUND Archive
    /// </summary>
    public class SoundFXParameter : Parameter
    {
        internal override string Identifier { get => "SND"; set { } }

        public override string ToString() => $"Sound Object:{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
    }

    /// <summary>
    /// Base class for Rails
    /// </summary>
    public abstract class RailParameter
    {

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
 * repeat ^
 */
namespace Spotlight.ObjectDescDatabase
{
    public class ObjectDescriptionDatabase
    {
        public Version Version = LatestVersion;
        private List<Description> ObjectDescriptions = new List<Description>();
        public static Version LatestVersion { get; } = new Version(1, 0);

        public ObjectDescriptionDatabase()
        {
            //nothing lol
        }
        public ObjectDescriptionDatabase(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Open);
            byte[] Read = new byte[4];
            FS.Read(Read, 0, 4);
            if (Encoding.ASCII.GetString(Read) != "SODD")
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

            while (FS.Position < FS.Length)
                ObjectDescriptions.Add(new Description() { ObjectName = FS.ReadString(), Text = FS.ReadString() });
        }

        public void Save(string Filename)
        {
            FileStream FS = new FileStream(Filename, FileMode.Create);
            FS.Write(new byte[8] { (byte)'S', (byte)'O', (byte)'D', (byte)'D', (byte)Version.Major, (byte)Version.Minor, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major, (byte)System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor }, 0, 8);
            for (int i = 0; i < ObjectDescriptions.Count; i++)
            {
                byte[] write = Encoding.GetEncoding(932).GetBytes(ObjectDescriptions[i].ObjectName);
                FS.Write(write,0,write.Length);
                FS.WriteByte(0x00);

                write = Encoding.GetEncoding(932).GetBytes(ObjectDescriptions[i].Text);
                FS.Write(write, 0, write.Length);
                FS.WriteByte(0x00);
            }
            FS.Close();
        }

        public Description GetDescription(string ObjectName)
        {
            List<Description> found = ObjectDescriptions.Where(p => string.Equals(p.ObjectName, ObjectName)).ToList();
            if (found.Count == 0)
                return new Description() { ObjectName = ObjectName, Text = "No Description Found" };
            return found[0];
        }

        public void SetDescription(string ObjectName, string Description)
        {
            List<Description> found = ObjectDescriptions.Where(p => string.Equals(p.ObjectName, ObjectName)).ToList();
            if (found.Count == 0)
                ObjectDescriptions.Add(new Description() { ObjectName = ObjectName, Text = Description });
            else
            {
                if (Description.Length == 0)
                    ObjectDescriptions.Remove(found[0]);
                else
                    found[0].Text = Description;
            }
        }
        /// <summary>
        /// Clears all the Object descriptions from the database. Doesn't check to make sure the user actually wanted this though
        /// </summary>
        public void Clear() => ObjectDescriptions.Clear();
    }

    public class Description
    {
        public string ObjectName;
        public string Text;
    }
}