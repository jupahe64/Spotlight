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
        internal ByteOrder _byteOrder;
        
        protected List<string> _nameArray;
        protected List<string> _stringArray;
        protected List<List<ByamlPathPoint>> _pathArray;
        protected bool _fastLoad = false;
        protected BinaryDataReader _reader;

        //Node references are disabled unless fastLoad is active since it leads to multiple objects sharing the same values for different fields (eg. a position node can be shared between multiple objects)
        protected Dictionary<uint, dynamic> _alreadyReadNodes = null; //Offset in the file, reference to node

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public ByamlReader(Stream stream, bool supportPaths, ByteOrder byteOrder, ushort version, bool fastLoad = false)
        {
            _version = version;
            _supportPaths = supportPaths;
            _byteOrder = byteOrder;
            _fastLoad = fastLoad;
            if (fastLoad)
            {
                _alreadyReadNodes = new Dictionary<uint, dynamic>();
            }

            _reader = new BinaryDataReader(stream, ByamlFile.GetEncoding(byteOrder), true)
            {
                ByteOrder = byteOrder
            };
        }

        protected bool TryStartLoading()
        {
            // Load the header, specifying magic bytes ("BY"), version and main node offsets.
            if (_reader.ReadUInt16() != BYAML_MAGIC)
            {
                //switch endian and try again
                _byteOrder = _byteOrder == ByteOrder.LittleEndian ? ByteOrder.BigEndian : ByteOrder.LittleEndian;

                _reader = new BinaryDataReader(_reader.BaseStream, ByamlFile.GetEncoding(_byteOrder), true)
                {
                    ByteOrder = _byteOrder
                };

                _reader.Position = 0;
                if (_reader.ReadUInt16() != BYAML_MAGIC) throw new Exception("Header mismatch");
            }
            _version = _reader.ReadUInt16();
            uint nameArrayOffset = _reader.ReadUInt32();
            uint stringArrayOffset = _reader.ReadUInt32();

            if (_reader.Length == 16)
            {
                _supportPaths = false;
            }
            else
            {
                using (_reader.TemporarySeek())
                {
                    // Paths are supported if the third offset is a path array (or unknown) and the fourth a root.
                    ByamlNodeType thirdNodeType = PeekNodeType(_reader);
                    _reader.Seek(sizeof(uint));
                    ByamlNodeType fourthNodeType = PeekNodeType(_reader);

                    _supportPaths = (thirdNodeType == ByamlNodeType.Unknown || thirdNodeType == ByamlNodeType.PathArray)
                         && (fourthNodeType == ByamlNodeType.Array || fourthNodeType == ByamlNodeType.Dictionary);

                }
            }


            uint pathArrayOffset = 0;
            if (_supportPaths)
            {
                pathArrayOffset = _reader.ReadUInt32();
            }
            uint rootNodeOffset = _reader.ReadUInt32();

            if (rootNodeOffset == 0) //empty byml
            {
                return false;
            }

            // Read the name array, holding strings referenced by index for the names of other nodes.
            if (nameArrayOffset != 0)
            {
                _reader.Seek(nameArrayOffset, SeekOrigin.Begin);
                _nameArray = ReadCollectionNode(_reader.ReadUInt32());
            }

            // Read the optional string array, holding strings referenced by index in string nodes.
            if (stringArrayOffset != 0)
            {
                _reader.Seek(stringArrayOffset, SeekOrigin.Begin);
                _stringArray = ReadCollectionNode(_reader.ReadUInt32());
            }

            // Read the optional path array, holding paths referenced by index in path nodes.
            if (_supportPaths && pathArrayOffset != 0)
            {
                // The third offset is the root node, so just read that and we're done.
                _reader.Seek(pathArrayOffset, SeekOrigin.Begin);
                _pathArray = ReadCollectionNode(_reader.ReadUInt32());
            }

            // Read the root node.
            _reader.Seek(rootNodeOffset, SeekOrigin.Begin);

            return true;



            dynamic ReadCollectionNode(uint lengthAndType)
            {
                int length = (int)Get3LsbBytes(lengthAndType);
                ByamlNodeType nodeType = (ByamlNodeType)Get1MsbByte(lengthAndType);

                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        return ReadArrayNode(_reader, length);
                    case ByamlNodeType.Dictionary:
                        return ReadDictionaryNode(_reader, length);
                    case ByamlNodeType.StringArray:
                        return ReadStringArrayNode(_reader, length);
                    case ByamlNodeType.PathArray:
                        return ReadPathArrayNode(_reader, length);
                    default:
                        return null; //should never happen
                }
            }
        }

        public dynamic Read()
        {
            // Open a _reader on the given stream.
            using (_reader)
            {
                if (TryStartLoading())
                {
                    uint lengthAndType = _reader.ReadUInt32();

                    int length = (int)Get3LsbBytes(lengthAndType);
                    ByamlNodeType nodeType = (ByamlNodeType)Get1MsbByte(lengthAndType);

                    switch (nodeType)
                    {
                        case ByamlNodeType.Array:
                            return ReadArrayNode(_reader, length);
                        case ByamlNodeType.Dictionary:
                            return ReadDictionaryNode(_reader, length);
                        case ByamlNodeType.StringArray:
                            return ReadStringArrayNode(_reader, length);
                        case ByamlNodeType.PathArray:
                            return ReadPathArrayNode(_reader, length);
                        default:
                            return null; //should never happen
                    }
                }
                else
                    return new Dictionary<string, dynamic>();
            }
        }

        private static ByamlNodeType PeekNodeType(BinaryDataReader _reader)
        {
            using (_reader.TemporarySeek())
            {
                // If the offset is invalid, the type cannot be determined.
                uint offset = _reader.ReadUInt32();
                if (offset > 0 && offset < _reader.BaseStream.Length)
                {
                    // Seek to the offset and try to read a valid type.
                    _reader.Position = offset;
                    ByamlNodeType nodeType = (ByamlNodeType)_reader.ReadByte();
                    if (Enum.IsDefined(typeof(ByamlNodeType), nodeType))
                        return nodeType;
                }
            }
            return ByamlNodeType.Unknown;

        }

        private dynamic ReadNode(BinaryDataReader _reader, ByamlNodeType nodeType)
        {
            if (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray)
            {
                uint offset = _reader.ReadUInt32();

                if (_fastLoad && _alreadyReadNodes.ContainsKey(offset))
                {
                    return _alreadyReadNodes[offset];
                }

                long oldPos = _reader.Position;

                //read value at offset
                _reader.Seek(offset, SeekOrigin.Begin);
                dynamic r = ReadNodeValue(_reader, nodeType);

                //go back
                _reader.Seek(oldPos, SeekOrigin.Begin);
                return r;
            }
            else
            {
                return ReadNodeValue(_reader, nodeType);
            }
        }

        protected dynamic ReadNodeValue(BinaryDataReader _reader, ByamlNodeType nodeType)
        {
            if (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray)
            {
                int length = (int)Get3LsbBytes(_reader.ReadUInt32());


                switch (nodeType)
                {
                    case ByamlNodeType.Array:
                        return ReadArrayNode(_reader, length);
                    case ByamlNodeType.Dictionary:
                        return ReadDictionaryNode(_reader, length);
                    case ByamlNodeType.StringArray:
                        return ReadStringArrayNode(_reader, length);
                    case ByamlNodeType.PathArray:
                        return ReadPathArrayNode(_reader, length);
                    default:
                        return null; //should never happen
                }
            }


            // Read the following UInt32 which is representing the value directly.
            switch (nodeType)
            {
                case ByamlNodeType.StringIndex:
                    return _stringArray[_reader.ReadInt32()];
                case ByamlNodeType.PathIndex:
                    return _pathArray[_reader.ReadInt32()];
                case ByamlNodeType.Boolean:
                    return _reader.ReadInt32() != 0;
                case ByamlNodeType.Integer:
                    return _reader.ReadInt32();
                case ByamlNodeType.Float:
                    return _reader.ReadSingle();
                case ByamlNodeType.UInteger:
                    return _reader.ReadUInt32();
                case ByamlNodeType.Long:
                case ByamlNodeType.ULong:
                case ByamlNodeType.Double:
                    var pos = _reader.Position;
                    _reader.Position = _reader.ReadUInt32();
                    dynamic value = readLongValFromOffset(nodeType);
                    _reader.Position = pos + 4;
                    return value;
                case ByamlNodeType.Null:
                    _reader.Seek(0x4);
                    return null;
                default:
                    throw new Exception($"Unknown node type '{nodeType}'.");
            }

            dynamic readLongValFromOffset(ByamlNodeType type)
            {
                switch (type)
                {
                    case ByamlNodeType.Long:
                        return _reader.ReadInt64();
                    case ByamlNodeType.ULong:
                        return _reader.ReadUInt64();
                    case ByamlNodeType.Double:
                        return _reader.ReadDouble();
                }
                throw new Exception($"Unknown node type '{nodeType}'.");
            }
        }

        private List<dynamic> ReadArrayNode(BinaryDataReader _reader, int length)
        {
            List<dynamic> array = new List<dynamic>(length);

            if (_fastLoad) _alreadyReadNodes.Add((uint)_reader.Position - 4, array);

            // Read the element types of the array.
            byte[] nodeTypes = _reader.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            _reader.Align(4);
            for (int i = 0; i < length; i++)
            {
                array.Add(ReadNode(_reader, (ByamlNodeType)nodeTypes[i]));
            }

            return array;
        }

        private Dictionary<string, dynamic> ReadDictionaryNode(BinaryDataReader _reader, int length)
        {
            Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();

            if (_fastLoad) _alreadyReadNodes.Add((uint)_reader.Position - 4, dictionary);

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint indexAndType = _reader.ReadUInt32();
                int nodeNameIndex = (int)Get3MsbBytes(indexAndType);
                ByamlNodeType nodeType = (ByamlNodeType)Get1LsbByte(indexAndType);
                string nodeName = _nameArray[nodeNameIndex];
                dictionary.Add(nodeName, ReadNode(_reader, nodeType));
            }

            return dictionary;
        }

        private List<string> ReadStringArrayNode(BinaryDataReader _reader, int length)
        {
            List<string> stringArray = new List<string>(length);

            // Read the element offsets.
            long nodeOffset = _reader.Position - 4; // String offsets are relative to the start of node.
            uint[] offsets = _reader.ReadUInt32s(length);

            // Read the strings by seeking to their element offset and then back.
            long oldPosition = _reader.Position;
            for (int i = 0; i < length; i++)
            {
                _reader.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                stringArray.Add(_reader.ReadString(BinaryStringFormat.ZeroTerminated));
            }
            _reader.Seek(oldPosition, SeekOrigin.Begin);

            return stringArray;
        }

        protected static bool IsReferenceType(ByamlNodeType nodeType) =>
            (ByamlNodeType.Array <= nodeType && nodeType <= ByamlNodeType.PathArray);
        //Array
        //Dictionary
        //StringArray
        //PathArray

        private List<List<ByamlPathPoint>> ReadPathArrayNode(BinaryDataReader _reader, int length)
        {
            List<List<ByamlPathPoint>> pathArray = new List<List<ByamlPathPoint>>(length);

            // Read the element offsets.
            long nodeOffset = _reader.Position - 4; // Path offsets are relative to the start of node.
            uint[] offsets = _reader.ReadUInt32s(length + 1);

            // Read the paths by seeking to their element offset and then back.
            long oldPosition = _reader.Position;
            for (int i = 0; i < length; i++)
            {
                _reader.Seek(nodeOffset + offsets[i], SeekOrigin.Begin);
                int pointCount = (int)((offsets[i + 1] - offsets[i]) / 0x1C);
                pathArray.Add(ReadPath(_reader, pointCount));
            }
            _reader.Seek(oldPosition, SeekOrigin.Begin);

            return pathArray;
        }

        private List<ByamlPathPoint> ReadPath(BinaryDataReader _reader, int length)
        {
            List<ByamlPathPoint> byamlPath = new List<ByamlPathPoint>();
            for (int j = 0; j < length; j++)
            {
                byamlPath.Add(ReadPathPoint(_reader));
            }
            return byamlPath;
        }

        private ByamlPathPoint ReadPathPoint(BinaryDataReader _reader)
        {
            ByamlPathPoint point = new ByamlPathPoint
            {
                Position = new Vector3F(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle()),
                Normal = new Vector3F(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle()),
                Unknown = _reader.ReadUInt32()
            };
            return point;
        }

        protected uint Get1LsbByte(uint value)
        {
            if (_byteOrder == ByteOrder.BigEndian)
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
            if (_byteOrder == ByteOrder.BigEndian)
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
            if (_byteOrder == ByteOrder.BigEndian)
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
            if (_byteOrder == ByteOrder.BigEndian)
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