using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Version Version = new Version(1, 1);
        public List<ObjectParameter> ObjectParameters = new List<ObjectParameter>();
        

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

            FS.Read(Read,0,4);
            int ParamCount = BitConverter.ToInt32(Read, 0);

            for (int i = 0; i < ParamCount; i++)
                ObjectParameters.Add(new ObjectParameter(FS));

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

            for (int i = 0; i < Zones.Length; i++)
            {
                SM3DWorldLevel Test = new SM3DWorldLevel(Zones[i],Zones[i].Replace(StageDataPath,""), "Map");
                for (int Item = 0; Item < Test.ObjectBaseReference.Count; Item++)
                {
                    if (Test.ObjectBaseReference.ElementAt(Item).Value is Rail)
                        continue;

                    General3dWorldObject Tmp = (General3dWorldObject)Test.ObjectBaseReference.ElementAt(Item).Value;
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

                        if (Tmp.Properties != null)
                            for (int y = 0; y < Tmp.Properties.Count; y++)
                            {
                                if (!ObjectParameters[ParamID].Properties.Any(O => O.Key == Tmp.Properties.ElementAt(y).Key))
                                {
                                    dynamic Prevalue = Tmp.Properties.ElementAt(y).Value.GetType();
                                    ObjectParameters[ParamID].Properties.Add(new KeyValuePair<string, string>(Tmp.Properties.ElementAt(y).Key, Prevalue.Name));
                                }
                            }
                        if (Tmp.Links != null)
                            for (int y = 0; y < Tmp.Links.Count; y++)
                            {
                                if (!ObjectParameters[ParamID].LinkNames.Any(O => O == Tmp.Links.ElementAt(y).Key))
                                    ObjectParameters[ParamID].LinkNames.Add(Tmp.Links.ElementAt(y).Key);
                            }
                    }
                    else
                    {
                        ObjectParameter OP = new ObjectParameter() { ClassName = Tmp.ClassName };
                        if (Tmp.ObjectName != "")
                            OP.ObjectNames.Add(Tmp.ObjectName);
                        if (Tmp.ModelName != "" && Tmp.ModelName != null)
                            OP.ModelNames.Add(Tmp.ModelName);
                        if (Tmp.Properties != null)
                            for (int x = 0; x < Tmp.Properties.Count; x++)
                            {
                                dynamic Prevalue = Tmp.Properties.ElementAt(x).Value.GetType();
                                OP.Properties.Add(new KeyValuePair<string, string>(Tmp.Properties.ElementAt(x).Key, Prevalue.Name));
                            }
                        if (Tmp.Links != null)
                            for (int x = 0; x < Tmp.Links.Count; x++)
                                OP.LinkNames.Add(Tmp.Links.ElementAt(x).Key);

                        ObjectParameters.Add(OP);
                    }
                }
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
    public class ObjectParameter
    {
        public string ClassName { get; set; }
        public byte CategoryID { get; set; }
        public List<string> ObjectNames { get; set; } = new List<string>();
        public List<string> ModelNames { get; set; } = new List<string>();
        public List<KeyValuePair<string, string>> Properties { get; set; } = new List<KeyValuePair<string, string>>();
        public List<string> LinkNames { get; set; } = new List<string>();

        /// <summary>
        /// Create an empty Object Parameter
        /// </summary>
        public ObjectParameter()
        {

        }
        /// <summary>
        /// Read one from a file
        /// </summary>
        /// <param name="FS"></param>
        public ObjectParameter(FileStream FS)
        {
            byte[] Read = new byte[3];
            FS.Read(Read, 0, 3);
            if (Encoding.ASCII.GetString(Read) != "OPP")
                throw new Exception("Invalid Database File");
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
                Properties.Add(new KeyValuePair<string, string>(FS.ReadString(),FS.ReadString()));
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
        public void Write(FileStream FS)
        {
            List<byte> ByteList = new List<byte>() { (byte)'O', (byte)'P', (byte)'P', 0x02 };
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

            FS.Write(ByteList.ToArray(),0,ByteList.Count);
        }
        /// <summary>
        /// Cast an Object Parameter to a new General 3DW Object with mostly empty values
        /// </summary>
        /// <param name="ID">The ID to give the object. Default is "New_" + The name of the class.<para/>Can also use <see cref="SM3DWorldScene.NextObjID()"/></param>
        /// <param name="ObjectNameID">For Classes that have multiple Object Names. default is 0</param>
        /// <param name="ModelNameID">For Classes that have multiple Model Names. Default is -1 (No Model Name)</param>
        /// <returns>new General 3DW Object</returns>
        public General3dWorldObject ToGeneral3DWorldObject(string ID = "", int ObjectNameID = 0, int ModelNameID = -1)
        {
            Dictionary<string, dynamic> Params = new Dictionary<string, dynamic>();
            for (int i = 0; i < Properties.Count; i++)
            {
                switch (Properties[i].Value)
                {
                    case "UInt32":
                    case "Int32":
                    case "Int16":
                    case "UInt16":
                    case "SByte":
                    case "Byte":
                        Params.Add(Properties[i].Key, 0);
                        break;
                    case "Boolean":
                        Params.Add(Properties[i].Key, false);
                        break;
                    case "String":
                        Params.Add(Properties[i].Key, "");
                        break;
                }
            }

            Dictionary<string, List<I3dWorldObject>> Links = new Dictionary<string, List<I3dWorldObject>>();
            for (int i = 0; i < LinkNames.Count; i++)
                Links.Add(LinkNames[i],new List<I3dWorldObject>());


            return new General3dWorldObject(new OpenTK.Vector3(0f), new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), $"New_{ClassName}", ObjectNames[ObjectNameID], ModelNameID == -1 ? "" : ModelNames[ModelNameID], ClassName, new OpenTK.Vector3(0f), new OpenTK.Vector3(0f), new OpenTK.Vector3(1f), Links, Params);
        }

        public override string ToString() => $"{ClassName} | {ObjectNames.Count} Object Names | {ModelNames.Count} Model Names | {Properties.Count} Properties | {LinkNames.Count} Link Names";
        /// <summary>
        /// Compare 2 Object Parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ObjectParameter)
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

    public static class FileStreamExExcerpt
    {
        /// <summary>
        /// Excerpt from my FileStreamEx Extensions. Reads a string in SHIFT-JIS. 0x00 terminated
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
    }
}
