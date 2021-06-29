using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.BinaryData;
using System.Diagnostics;
using Syroot.Maths;

namespace BYAML
{
    public class BymlFileData
    {
        public ByteOrder byteOrder;
        public ushort Version;
        public bool SupportPaths;
        public dynamic RootNode;
    }

    /// <summary>
    /// Represents a point in a BYAML path.
    /// </summary>
    public class ByamlPathPoint : IEquatable<ByamlPathPoint>
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// The size of a single point in bytes when serialized as BYAML data.
        /// </summary>
        internal const int SizeInBytes = 28;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ByamlPathPoint"/> class.
        /// </summary>
        public ByamlPathPoint()
        {
            Normal = new Vector3F( 0, 1, 0 );
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Vector3F Position { get; set; }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        public Vector3F Normal { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        public uint Unknown { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ByamlPathPoint other)
        {
            return Position == other.Position && Normal == other.Normal && Unknown == other.Unknown;
        }

        public override string ToString()
        {
            return $"ByamlPathPoint Pos:{Position} Norm:{Normal} Unk:{Unknown}";
        }
    }

    /// <summary>
    /// Represents the loading and saving logic of BYAML files and returns the resulting file structure in dynamic
    /// objects.
    /// </summary>
    /// 
    public class ByamlFile
    {
        public enum ByamlNodeType : byte
        {
            Unknown     = 0x00, //for all NodeTypes that are unkown
            StringIndex = 0xA0,
            PathIndex   = 0xA1,
            Array       = 0xC0,
            Dictionary  = 0xC1,
            StringArray = 0xC2,
            PathArray   = 0xC3,
            Boolean     = 0xD0,
            Integer     = 0xD1,
            Float       = 0xD2,
            UInteger    = 0xD3,
            Long        = 0xD4,
            ULong       = 0xD5,
            Double      = 0xD6,
            Null        = 0xFF
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        public const ushort BYAML_MAGIC = 0x4259; // "BY"
                                                   // ---- MEMBERS ------------------------------------------------------------------------------------------------
        

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Deserializes and returns the dynamic value of the BYAML node read from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to read the data from.</param>
        /// <param name="supportPaths">Whether to expect a path array offset. This must be enabled for Mario Kart 8
        /// files.</param>
        /// <param name="byteOrder">The <see cref="Endian"/> to read data in.</param>
        public static BymlFileData LoadN(string fileName, bool supportPaths = false, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadN(stream, supportPaths, byteOrder);
            }
        }

        /// <summary>
        /// Deserializes and returns the dynamic value of the BYAML node read from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        /// <param name="supportPaths">Whether to expect a path array offset. This must be enabled for Mario Kart 8
        /// files.</param>
        /// <param name="byteOrder">The <see cref="Endian"/> to read data in.</param>
        public static BymlFileData LoadN(Stream stream, bool supportPaths = false, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            ByamlReader reader = new ByamlReader(stream, supportPaths, byteOrder, 3);
            var r = reader.Read();
            return new BymlFileData() { byteOrder = reader._byteOrder, RootNode = r, Version = reader._version, SupportPaths = supportPaths };
        }

        /// <summary>
        /// Deserializes and returns the dynamic value of the BYAML node read from the specified stream keeping the references, do not use this if you intend to edit the byml.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        /// <param name="supportPaths">Whether to expect a path array offset. This must be enabled for Mario Kart 8
        /// files.</param>
        /// <param name="byteOrder">The <see cref="Endian"/> to read data in.</param>
        public static BymlFileData FastLoadN(Stream stream, bool supportPaths = false, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            ByamlReader reader = new ByamlReader(stream, supportPaths, byteOrder, 3, true);
            var r = reader.Read();
            return new BymlFileData() { byteOrder = reader._byteOrder, RootNode = r, Version = reader._version, SupportPaths = supportPaths };
        }

        /// <summary>
        /// Serializes the given dynamic value which requires to be an array or dictionary of BYAML compatible values
        /// and stores it in the given file.
        /// </summary>
        public static void SaveN(string fileName, BymlFileData file)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                SaveN(stream, file);
            }
        }

        public static byte[] SaveN(BymlFileData file)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SaveN(stream, file);
                return stream.ToArray();
            }
        }

        public static Encoding GetEncoding(ByteOrder byteOrder)
        {
            return byteOrder == ByteOrder.BigEndian ? Encoding.GetEncoding("shift_jis") : Encoding.UTF8;
        }

        /// <summary>
        /// Serializes the given dynamic value which requires to be an array or dictionary of BYAML compatible values
        /// and stores it in the specified stream.
        /// </summary>
        public static void SaveN(Stream stream, BymlFileData file)
        {
            new ByamlWriter(stream, file.SupportPaths, file.byteOrder, file.Version).Write(file.RootNode);
        }

        /// <summary>
        /// Serializes the given dynamic value which requires to be an array or dictionary of BYAML compatible values
        /// and stores it in the specified stream.
        /// No duplication checks are done using this method.
        /// </summary>
        public static void FastSaveN(Stream stream, BymlFileData file)
        {
            new ByamlWriter(stream, file.SupportPaths, file.byteOrder, file.Version).Write(file.RootNode, true);
        }

        // ---- Helper methods ----

        /// <summary>
        /// Tries to retrieve the value of the element with the specified <paramref name="key"/> stored in the given
        /// dictionary <paramref name="node"/>. If the key does not exist, <c>null</c> is returned.
        /// </summary>
        /// <param name="node">The dictionary BYAML node to retrieve the value from.</param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <returns>The value stored under the given key or <c>null</c> if the key is not present.</returns>
        public static dynamic GetValue(IDictionary<string, dynamic> node, string key)
        {
            return node.TryGetValue(key, out dynamic value) ? value : null;
        }

        /// <summary>
        /// Sets the given <paramref name="value"/> in the provided dictionary <paramref name="node"/> under the
        /// specified <paramref name="key"/>. If the value is <c>null</c>, the key is removed from the dictionary node.
        /// </summary>
        /// <param name="node">The dictionary node to store the value under.</param>
        /// <param name="key">The key under which the value will be stored or which will be removed.</param>
        /// <param name="value">The value to store under the key or <c>null</c> to remove the key.</param>
        public static void SetValue(IDictionary<string, dynamic> node, string key, dynamic value)
        {
            if (value == null)
            {
                node.Remove(key);
            }
            else
            {
                node[key] = value;
            }
        }

        /// <summary>
        /// Casts all elements of the given array <paramref name="node"/> into the provided type
        /// <typeparamref name="T"/>. If the node is <c>null</c>, <c>null</c> is returned.
        /// </summary>
        /// <typeparam name="T">The type to cast each element to.</typeparam>
        /// <param name="node">The array node which elements will be casted.</param>
        /// <returns>The list of type <typeparamref name="T"/> or <c>null</c> if the node is <c>null</c>.</returns>
        public static List<T> GetList<T>(IEnumerable<dynamic> node)
        {
            return node?.Cast<T>().ToList();
        }
    }

    public static class IEnumerableCompare
    {
        public static bool TypeNotEqual(Type a, Type b)
        {
            return !(a.IsAssignableFrom(b) || b.IsAssignableFrom(a)); // without this LinksNode wouldn't be equal to IDictionary<string,dynamic>
        }

        public static bool IsEqual(IEnumerable a, IEnumerable b)
        {
            if (TypeNotEqual(a.GetType(), b.GetType())) return false;

            List<dynamic> stackA = new List<dynamic>();
            List<dynamic> stackB = new List<dynamic>();


            if (a is IDictionary) return IDictionaryIsEqual((IDictionary)a, (IDictionary)b);
            else if (a is IList<ByamlPathPoint>) return ((IList<ByamlPathPoint>)a).SequenceEqual((IList<ByamlPathPoint>)b);
            else return IListIsEqual((IList)a, (IList)b);


            bool IDictionaryIsEqual(IDictionary _a, IDictionary _b)
            {
                if (_a == _b)
                    return true;

                if (_a.Count != _b.Count)
                    return false;

                stackA.Add(_a);
                stackB.Add(_b);

                foreach (string key in _a.Keys)
                {
                    if (!_b.Contains(key))
                    { stackA.Remove(_a); stackA.Remove(_b); return false; }

                    if ((_a[key] == null) != (_b[key] == null))
                    {
                        stackA.Remove(_a); stackA.Remove(_b); return false;
                    }
                    else if (_a[key] == null && _b[key] == null)
                        continue;

                    if (TypeNotEqual(_a[key].GetType(), _b[key].GetType()))
                    { stackA.Remove(_a); stackA.Remove(_b); return false; }

                    if (_a[key] is IList || _a[key] is IDictionary)
                    {
                        int indexA = stackA.IndexOf(_a[key]);
                        if (indexA != -1) //self reference
                        {
                            if (indexA == stackB.IndexOf(_a[key]))
                                continue;
                            else
                            { stackA.Remove(_a); stackA.Remove(_b); return false; }
                        }

                        if (_a[key] is IList && IListIsEqual((IList)_a[key], (IList)_b[key]))
                            continue;
                        else if (_a[key] is IDictionary && IDictionaryIsEqual((IDictionary)_a[key], (IDictionary)_b[key]))
                            continue;
                    }
                    else if (_a[key] == _b[key])
                        continue;

                    stackA.Remove(_a); stackA.Remove(_b); return false;
                }
                stackA.Remove(_a); stackA.Remove(_b); return true;
            }

            bool IListIsEqual(IList _a, IList _b)
            {
                if (_a == _b)
                    return true;
                

                if (_a.Count != _b.Count)
                    return false;

                stackA.Add(_a);
                stackB.Add(_b);

                for (int i = 0; i < _a.Count; i++)
                {
                    if ((_a[i] == null && _b[i] != null) || (_a[i] != null && _b[i] == null))
                    {
                        stackA.Remove(_a); stackA.Remove(_b); return false;
                    }
                    else if (_a[i] == null && _b[i] == null)
                        continue;

                    if (TypeNotEqual(_a[i].GetType(), _b[i].GetType()))
                    { stackA.Remove(_a); stackA.Remove(_b); return false; }

                    if (_a[i] is IList<dynamic> || _a[i] is IDictionary<string, dynamic>)
                    {
                        int indexA = stackA.IndexOf(_a[i]);
                        if (indexA != -1) //self reference
                        {
                            if (indexA == stackB.IndexOf(_a[i]))
                                continue;
                            else
                            { stackA.Remove(_a); stackA.Remove(_b); return false; }
                        }

                        if (_a[i] is IList<dynamic> && IListIsEqual((IList)_a[i], (IList)_b[i]))
                            continue;
                        else if (_a[i] is IDictionary && IDictionaryIsEqual((IDictionary)_a[i], (IDictionary)_b[i]))
                            continue;
                    }
                    else if (_a[i] == _b[i])
                        continue;

                    stackA.Remove(_a); stackA.Remove(_b); return false;
                }
                stackA.Remove(_a); stackA.Remove(_b); return true;
            }
        }
    }
}