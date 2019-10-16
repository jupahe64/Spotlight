using Syroot.BinaryData;
using Syroot.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BYAML.ByamlFile;

namespace BYAML
{
    public class ByamlReader
    {
        internal ushort _version;
        protected bool _supportPaths;
        internal Endian _byteOrder;
        
        protected List<string> _nameArray;
        protected List<string> _stringArray;
        protected List<List<ByamlPathPoint>> _pathArray;
        protected bool _fastLoad = false;
        protected BinaryStream _binaryStream;

        //Node references are disabled unless fastLoad is active since it leads to multiple objects sharing the same values for different fields (eg. a position node can be shared between multiple objects)
        protected Dictionary<uint, dynamic> _alreadyReadNodes = null; //Offset in the file, reference to node

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ByamlReader(Stream stream, bool supportPaths, Endian byteOrder, ushort version, bool fastLoad = false)
        {
            _version = version;
            _supportPaths = supportPaths;
            _byteOrder = byteOrder;
            _fastLoad = fastLoad;
            if (fastLoad)
            {
                _alreadyReadNodes = new Dictionary<uint, dynamic>();
            }

            _binaryStream = new BinaryStream(stream, ByteConverter.GetConverter(_byteOrder), Encoding.GetEncoding(932), leaveOpen: true);
        }

        protected bool TryStartLoading()
        {
            // Load the header, specifying magic bytes ("BY"), version and main node offsets.
            if (_binaryStream.ReadUInt16() != BYAML_MAGIC)
            {
                _byteOrder = _byteOrder == Endian.Little ? Endian.Big : Endian.Little;
                _binaryStream.ByteConverter = ByteConverter.GetConverter(_byteOrder);
                _binaryStream.Position = 0;
                if (_binaryStream.ReadUInt16() != BYAML_MAGIC) throw new Exception("Header mismatch");
            }
            _version = _binaryStream.ReadUInt16();
            uint nameArrayOffset = _binaryStream.ReadUInt32();
            uint stringArrayOffset = _binaryStream.ReadUInt32();

            if (_binaryStream.Length == 16)
            {
                _supportPaths = false;
            }
            else
            {
                using (_binaryStream.TemporarySeek())
                {
                    // Paths are supported if the third offset is a path array (or unknown) and the fourth a root.
                    ByamlNodeType thirdNodeType = PeekNodeType(_binaryStream);
                    _binaryStream.Seek(sizeof(uint));
                    ByamlNodeType fourthNodeType = PeekNodeType(_binaryStream);

                    _supportPaths = (thirdNodeType == ByamlNodeType.Unknown || thirdNodeType == ByamlNodeType.PathArray)
                         && (fourthNodeType == ByamlNodeType.Array || fourthNodeType == ByamlNodeType.Dictionary);

                }
            }


            uint pathArrayOffset = 0;
            if (_supportPaths)
            {
                pathArrayOffset = _binaryStream.ReadUInt32();
            }
            uint rootNodeOffset = _binaryStream.ReadUInt32();

            if (rootNodeOffset == 0) //empty byml
            {
                return false;
            }

            // Read the name array, holding strings referenced by index for the names of other nodes.
            if (nameArrayOffset != 0)
            {
                _binaryStream.Seek(nameArrayOffset, SeekOrigin.Begin);
                _nameArray = ReadCollectionNode(_binaryStream.ReadUInt32());
            }

            // Read the optional string array, holding strings referenced by index in string nodes.
            if (stringArrayOffset != 0)
            {
                _binaryStream.Seek(stringArrayOffset, SeekOrigin.Begin);
                _stringArray = ReadCollectionNode(_binaryStream.ReadUInt32());
            }

            // Read the optional path array, holding paths referenced by index in path nodes.
            if (_supportPaths && pathArrayOffset != 0)
            {
                // The third offset is the root node, so just read that and we're done.
                _binaryStream.Seek(pathArrayOffset, SeekOrigin.Begin);
                _pathArray = ReadCollectionNode(_binaryStream.ReadUInt32());
            }

            // Read the root node.
            _binaryStream.Seek(rootNodeOffset, SeekOrigin.Begin);

            return true;



            dynamic ReadCollectionNode(uint lengthAndType)
            {
                int length = (int)Get3LsbBytes(lengthAndType);
                ByamlNodeType nodeType = (ByamlNodeType)Get1MsbByte(lengthAndType);

                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        return ReadArrayNode(_binaryStream, length);
                    case ByamlNodeType.Dictionary:
                        return ReadDictionaryNode(_binaryStream, length);
                    case ByamlNodeType.StringArray:
                        return ReadStringArrayNode(_binaryStream, length);
                    case ByamlNodeType.PathArray:
                        return ReadPathArrayNode(_binaryStream, length);
                    default:
                        return null; //should never happen
                }
            }
        }

        public dynamic Read()
        {
            // Open a _binaryStream on the given stream.
            using (_binaryStream)
            {
                if (TryStartLoading())
                {
                    uint lengthAndType = _binaryStream.ReadUInt32();

                    int length = (int)Get3LsbBytes(lengthAndType);
                    ByamlNodeType nodeType = (ByamlNodeType)Get1MsbByte(lengthAndType);

                    switch (nodeType)
                    {
                        case ByamlNodeType.Array:
                            return ReadArrayNode(_binaryStream, length);
                        case ByamlNodeType.Dictionary:
                            return ReadDictionaryNode(_binaryStream, length);
                        case ByamlNodeType.StringArray:
                            return ReadStringArrayNode(_binaryStream, length);
                        case ByamlNodeType.PathArray:
                            return ReadPathArrayNode(_binaryStream, length);
                        default:
                            return null; //should never happen
                    }
                }
                else
                    return new Dictionary<string, dynamic>();
            }
        }

        private static ByamlNodeType PeekNodeType(BinaryStream _binaryStream)
        {
            using (_binaryStream.TemporarySeek())
            {
                // If the offset is invalid, the type cannot be determined.
                uint offset = _binaryStream.ReadUInt32();
                if (offset > 0 && offset < _binaryStream.BaseStream.Length)
                {
                    // Seek to the offset and try to read a valid type.
                    _binaryStream.Position = offset;
                    ByamlNodeType nodeType = (ByamlNodeType)_binaryStream.ReadByte();
                    if (Enum.IsDefined(typeof(ByamlNodeType), nodeType))
                        return nodeType;
                }
            }
            return ByamlNodeType.Unknown;

        }

        private dynamic ReadNode(BinaryStream _binaryStream, ByamlNodeType nodeType)
        {
            if (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray)
            {
                uint offset = _binaryStream.ReadUInt32();

                if (_fastLoad && _alreadyReadNodes.ContainsKey(offset))
                {
                    return _alreadyReadNodes[offset];
                }

                long oldPos = _binaryStream.Position;

                //read value at offset
                _binaryStream.Seek(offset, SeekOrigin.Begin);
                dynamic r = ReadNodeValue(_binaryStream, nodeType);

                //go back
                _binaryStream.Seek(oldPos, SeekOrigin.Begin);
                return r;
            }
            else
            {
                return ReadNodeValue(_binaryStream, nodeType);
            }
        }

        protected dynamic ReadNodeValue(BinaryStream _binaryStream, ByamlNodeType nodeType)
        {
            if (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray)
            {
                int length = (int)Get3LsbBytes(_binaryStream.ReadUInt32());


                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        return ReadArrayNode(_binaryStream, length);
                    case ByamlNodeType.Dictionary:
                        return ReadDictionaryNode(_binaryStream, length);
                    case ByamlNodeType.StringArray:
                        return ReadStringArrayNode(_binaryStream, length);
                    case ByamlNodeType.PathArray:
                        return ReadPathArrayNode(_binaryStream, length);
                    default:
                        return null; //should never happen
                }
            }


            // Read the following UInt32 which is representing the value directly.
            switch (nodeType)
            {
                case ByamlNodeType.StringIndex:
                    return _stringArray[_binaryStream.ReadInt32()];
                case ByamlNodeType.PathIndex:
                    return _pathArray[_binaryStream.ReadInt32()];
                case ByamlNodeType.Boolean:
                    return _binaryStream.ReadInt32() != 0;
                case ByamlNodeType.Integer:
                    return _binaryStream.ReadInt32();
                case ByamlNodeType.Float:
                    return _binaryStream.ReadSingle();
                case ByamlNodeType.UInteger:
                    return _binaryStream.ReadUInt32();
                case ByamlNodeType.Long:
                case ByamlNodeType.ULong:
                case ByamlNodeType.Double:
                    var pos = _binaryStream.Position;
                    _binaryStream.Position = _binaryStream.ReadUInt32();
                    dynamic value = readLongValFromOffset(nodeType);
                    _binaryStream.Position = pos + 4;
                    return value;
                case ByamlNodeType.Null:
                    _binaryStream.Seek(0x4);
                    return null;
                default:
                    throw new Exception($"Unknown node type '{nodeType}'.");
            }

            dynamic readLongValFromOffset(ByamlNodeType type)
            {
                switch (type)
                {
                    case ByamlNodeType.Long:
                        return _binaryStream.ReadInt64();
                    case ByamlNodeType.ULong:
                        return _binaryStream.ReadUInt64();
                    case ByamlNodeType.Double:
                        return _binaryStream.ReadDouble();
                }
                throw new Exception($"Unknown node type '{nodeType}'.");
            }
        }

        private List<dynamic> ReadArrayNode(BinaryStream _binaryStream, int length)
        {
            List<dynamic> array = new List<dynamic>(length);

            if (_fastLoad) _alreadyReadNodes.Add((uint)_binaryStream.Position - 4, array);

            // Read the element types of the array.
            byte[] nodeTypes = _binaryStream.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            _binaryStream.Align(4);
            for (int i = 0; i < length; i++)
            {
                array.Add(ReadNode(_binaryStream, (ByamlNodeType)nodeTypes[i]));
            }

            return array;
        }

        private Dictionary<string, dynamic> ReadDictionaryNode(BinaryStream _binaryStream, int length)
        {
            Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();

            if (_fastLoad) _alreadyReadNodes.Add((uint)_binaryStream.Position - 4, dictionary);

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint indexAndType = _binaryStream.ReadUInt32();
                int nodeNameIndex = (int)Get3MsbBytes(indexAndType);
                ByamlNodeType nodeType = (ByamlNodeType)Get1LsbByte(indexAndType);
                string nodeName = _nameArray[nodeNameIndex];
                dictionary.Add(nodeName, ReadNode(_binaryStream, nodeType));
            }

            return dictionary;
        }

        private List<string> ReadStringArrayNode(BinaryStream _binaryStream, int length)
        {
            List<string> stringArray = new List<string>(length);

            // Read the element offsets.
            long nodeOffset = _binaryStream.Position - 4; // String offsets are relative to the start of node.
            uint[] offsets = _binaryStream.ReadUInt32s(length);

            // Read the strings by seeking to their element offset and then back.
            long oldPosition = _binaryStream.Position;
            for (int i = 0; i < length; i++)
            {
                _binaryStream.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                stringArray.Add(_binaryStream.ReadString(StringCoding.ZeroTerminated,_binaryStream.Encoding));
            }
            _binaryStream.Seek(oldPosition, SeekOrigin.Begin);

            return stringArray;
        }

        protected static bool IsReferenceType(ByamlNodeType nodeType) =>
            (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray);
        //Array
        //Dictionary
        //StringArray
        //PathArray

        private List<List<ByamlPathPoint>> ReadPathArrayNode(BinaryStream _binaryStream, int length)
        {
            List<List<ByamlPathPoint>> pathArray = new List<List<ByamlPathPoint>>(length);

            // Read the element offsets.
            long nodeOffset = _binaryStream.Position - 4; // Path offsets are relative to the start of node.
            uint[] offsets = _binaryStream.ReadUInt32s(length + 1);

            // Read the paths by seeking to their element offset and then back.
            long oldPosition = _binaryStream.Position;
            for (int i = 0; i < length; i++)
            {
                _binaryStream.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                int pointCount = (int)((offsets[i + 1] - offsets[i]) / 0x1C);
                pathArray.Add(ReadPath(_binaryStream, pointCount));
            }
            _binaryStream.Seek(oldPosition, SeekOrigin.Begin);

            return pathArray;
        }

        private List<ByamlPathPoint> ReadPath(BinaryStream _binaryStream, int length)
        {
            List<ByamlPathPoint> byamlPath = new List<ByamlPathPoint>();
            for (int j = 0; j < length; j++)
            {
                byamlPath.Add(ReadPathPoint(_binaryStream));
            }
            return byamlPath;
        }

        private ByamlPathPoint ReadPathPoint(BinaryStream _binaryStream)
        {
            ByamlPathPoint point = new ByamlPathPoint
            {
                Position = new Vector3F(_binaryStream.ReadSingle(), _binaryStream.ReadSingle(), _binaryStream.ReadSingle()),
                Normal = new Vector3F(_binaryStream.ReadSingle(), _binaryStream.ReadSingle(), _binaryStream.ReadSingle()),
                Unknown = _binaryStream.ReadUInt32()
            };
            return point;
        }

        protected uint Get1LsbByte(uint value)
        {
            if (_byteOrder == Endian.Big)
            {
                return value & 0x000000FF;
            }
            else
            {
                return value >> 24;
            }
        }

        protected uint Get1MsbByte(uint value)
        {
            if (_byteOrder == Endian.Big)
            {
                return value >> 24;
            }
            else
            {
                return value & 0x000000FF;
            }
        }

        protected uint Get3LsbBytes(uint value)
        {
            if (_byteOrder == Endian.Big)
            {
                return value & 0x00FFFFFF;
            }
            else
            {
                return value >> 8;
            }
        }

        protected uint Get3MsbBytes(uint value)
        {
            if (_byteOrder == Endian.Big)
            {
                return value >> 8;
            }
            else
            {
                return value & 0x00FFFFFF;
            }
        }
    }
}