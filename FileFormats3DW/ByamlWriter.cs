using Syroot.BinaryData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BYAML.ByamlFile;

namespace BYAML
{
    public class ByamlWriter
    {
        protected ushort _version;
        protected bool _supportPaths;
        protected ByteOrder _byteOrder;
        
        protected BinaryDataWriter _writer;

        protected string[] _nameArray;
        protected string[] _stringArray;

        protected HashSet<string> _nameSet = new HashSet<string>();
        protected HashSet<string> _stringSet = new HashSet<string>();

        protected List<List<ByamlPathPoint>> _pathArray = new List<List<ByamlPathPoint>>();
        protected Dictionary<object, ByamlArr> _arrays = new Dictionary<object, ByamlArr>();
        protected Dictionary<object, ByamlDict> _dictionaries = new Dictionary<object, ByamlDict>();
        protected Dictionary<dynamic, int> _eightByteValues = new Dictionary<dynamic, int>();
        
        protected int _valStackPointer = 0;

        public ByamlWriter(Stream stream, bool supportPaths, ByteOrder byteOrder, ushort version)
        {
            _version = version;
            _supportPaths = supportPaths;
            _byteOrder = byteOrder;
            _writer = new BinaryDataWriter(stream, ByamlFile.GetEncoding(byteOrder), true)
            {
                ByteOrder = _byteOrder
            };
        }

        public void Write(object root, bool fastWrite = false)
        {
            // Check if the root is of the correct type.
            if (root == null)
            {
                throw new Exception("Root node must not be null.");
            }
            else if (!(root is IDictionary<string, dynamic> || root is IEnumerable))
            {
                throw new Exception($"Type '{root.GetType()}' is not supported as a BYAML root node.");
            }

            ProcessValue(root,fastWrite);

            _nameArray = _nameSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            _stringArray = _stringSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();

            WriteContent(root);
        }

        protected void WriteContent(object rootReferenceKey)
        {
            using (_writer)
            {
                // Write the header, specifying magic bytes, version and main node offsets.
                _writer.Write(BYAML_MAGIC);
                _writer.Write(_version);
                Offset nameArrayOffset = _writer.ReserveOffset();
                Offset stringArrayOffset = _writer.ReserveOffset();
                Offset pathArrayOffset = _supportPaths ? _writer.ReserveOffset() : null;
                Offset rootOffset = _writer.ReserveOffset();

                // Write the main nodes.
                _writer.Align(4);
                nameArrayOffset.Satisfy();
                WriteStringArrayNode(_writer, _nameArray);
                if (_stringArray.Length == 0)
                {
                    stringArrayOffset.Satisfy(0);
                }
                else
                {
                    _writer.Align(4);
                    stringArrayOffset.Satisfy();
                    WriteStringArrayNode(_writer, _stringArray);
                }

                // Include a path array offset if requested.
                if (_supportPaths)
                {
                    if (_pathArray.Count == 0)
                    {
                        pathArrayOffset.Satisfy(0);
                    }
                    else
                    {
                        _writer.Align(4);
                        pathArrayOffset.Satisfy();
                        WritePathArrayNode(_writer, _pathArray);
                    }
                }

                _writer.Align(4);

                //write value stack (Dictionary, Array, long, uint, double)
                int valStackPos = (int)_writer.BaseStream.Position;

                //write all dictionaries
                foreach (KeyValuePair<object, ByamlDict> keyValuePair in _dictionaries)
                {
                    _writer.Seek(valStackPos + keyValuePair.Value.offset, SeekOrigin.Begin);

                    if (keyValuePair.Key == rootReferenceKey)
                        rootOffset.Satisfy();

                    if (_byteOrder == ByteOrder.BigEndian)
                        _writer.Write((uint)ByamlNodeType.Dictionary << 24 | (uint)keyValuePair.Value.entries.Length);
                    else
                        _writer.Write((uint)ByamlNodeType.Dictionary | (uint)keyValuePair.Value.entries.Length << 8);

                    foreach ((string key, Entry entry) in keyValuePair.Value.entries)
                    {
                        if (_byteOrder == ByteOrder.BigEndian)
                            _writer.Write(Array.IndexOf(_nameArray, key) << 8 | (byte)entry.type);
                        else
                            _writer.Write(Array.IndexOf(_nameArray, key) | (byte)entry.type << 24);

                        WriteValue(entry);
                    }
                }

                //write all arrays
                foreach (KeyValuePair<object, ByamlArr> keyValuePair in _arrays)
                {
                    _writer.Seek(valStackPos + keyValuePair.Value.offset, SeekOrigin.Begin);

                    if (keyValuePair.Key == rootReferenceKey)
                        rootOffset.Satisfy();

                    if (_byteOrder == ByteOrder.BigEndian)
                        _writer.Write((uint)ByamlNodeType.Array << 24 | (uint)keyValuePair.Value.entries.Length);
                    else
                        _writer.Write((uint)ByamlNodeType.Array | (uint)keyValuePair.Value.entries.Length << 8);


                    foreach (Entry entry in keyValuePair.Value.entries)
                        _writer.Write((byte)entry.type);

                    _writer.Align(4);

                    foreach (Entry entry in keyValuePair.Value.entries)
                    {
                        WriteValue(entry);
                    }
                }

                //write all 8 byte values
                foreach (var keyValuePair in _eightByteValues)
                {
                    _writer.Seek(valStackPos + keyValuePair.Value, SeekOrigin.Begin);
                    _writer.Write(keyValuePair.Key);
                }

                void WriteValue(Entry entry)
                {
                    // Only write the offset for the complex value contents, write simple values directly.
                    switch (entry.type)
                    {
                        case ByamlNodeType.StringIndex:
                            _writer.Write((uint)Array.IndexOf(_stringArray, entry.value));
                            break;
                        case ByamlNodeType.PathIndex:
                            _writer.Write(_pathArray.IndexOf(entry.value));
                            break;
                        case ByamlNodeType.Dictionary:
                            _writer.Write(valStackPos + _dictionaries[(object)entry.value].offset);
                            break;
                        case ByamlNodeType.Array:
                            _writer.Write(valStackPos + _arrays[(object)entry.value].offset);
                            break;
                        case ByamlNodeType.Boolean:
                            _writer.Write(entry.value ? 1 : 0);
                            break;
                        case ByamlNodeType.Integer:
                        case ByamlNodeType.Float:
                        case ByamlNodeType.UInteger:
                            _writer.Write(entry.value);
                            break;
                        case ByamlNodeType.Double:
                        case ByamlNodeType.ULong:
                        case ByamlNodeType.Long:
                            _writer.Write(valStackPos + _eightByteValues[entry.value]);
                            return;
                        case ByamlNodeType.Null:
                            _writer.Write(0);
                            break;
                    }
                }
            }
        }

        protected struct Entry
        {
            public ByamlNodeType type;
            public dynamic value;

            public override string ToString()
            {
                return ((byte)type).ToString("X") + " " + value.ToString();
            }

            public Entry(ByamlNodeType type, dynamic value)
            {
                this.type = type;
                this.value = value;
            }
        }

        protected struct ByamlDict
        {
            public int offset;
            public (string, Entry)[] entries;

            public ByamlDict(int offset, (string, Entry)[] entries)
            {
                this.offset = offset;
                this.entries = entries;
            }
        }

        protected struct ByamlArr
        {
            public int offset;
            public Entry[] entries;

            public ByamlArr(int offset, Entry[] entries)
            {
                this.offset = offset;
                this.entries = entries;
            }
        }

        protected Entry ProcessValue(dynamic value, bool fastWrite)
        {
            ByamlNodeType type = GetNodeType(value);
            
            if (type == ByamlNodeType.Dictionary)
            {
                Dictionary<string, dynamic> _value = value;

                //check for duplicates by key
                if (_dictionaries.ContainsKey(value))
                    return new Entry(type, value);

                //Process Dictionary
                (string, Entry)[] entries = new (string, Entry)[value.Count];
                
                int i = 0;
                foreach (string key in _value.Keys.OrderBy(x => x, StringComparer.Ordinal))
                {
                    if (!_nameSet.Contains(key))
                        _nameSet.Add(key);

                    entries[i] = (key, ProcessValue(_value[key],fastWrite));
                    i++;
                }
                if (!fastWrite)
                {
                    //check for duplicates
                    foreach (var keyValuePair in _dictionaries)
                    {
                        if (entries.Length == keyValuePair.Value.entries.Length)
                        {
                            for (i = 0; i < entries.Length; i++)
                            {
                                if (entries[i].Item2.type == keyValuePair.Value.entries[i].Item2.type)
                                {
                                    if (entries[i].Item2.type == ByamlNodeType.Array || entries[i].Item2.type == ByamlNodeType.Dictionary)
                                    {
                                        if ((object)entries[i].Item2.value == (object)keyValuePair.Value.entries[i].Item2.value)
                                            continue; //same reference
                                    }
                                    else
                                    {
                                        if (entries[i].Item2.value == keyValuePair.Value.entries[i].Item2.value)
                                            continue; //same value
                                    }
                                }
                                goto not_equal;
                            }
                            //Duplicate was found
                            return new Entry(ByamlNodeType.Dictionary, keyValuePair.Key);
                        }
                    not_equal:
                        ;
                    }
                    //No duplicates found:
                }

                //Submit
                _dictionaries.Add(value, new ByamlDict(_valStackPointer, entries));
                _valStackPointer += 4 + entries.Length * 8;
            }
            else if (type == ByamlNodeType.Array)
            {
                List<dynamic> _value = value;

                //check for duplicates by key
                if (_dictionaries.ContainsKey(value))
                    return new Entry(type, value);

                //Process Array
                Entry[] entries = new Entry[value.Count];
                
                int i = 0;
                foreach (dynamic item in _value)
                {
                    entries[i] = ProcessValue(item,fastWrite);
                    i++;
                }

                if (!fastWrite)
                {
                    //check for duplicates
                    foreach (var keyValuePair in _arrays)
                    {
                        if (entries.Length == keyValuePair.Value.entries.Length)
                        {
                            for (i = 0; i < keyValuePair.Value.entries.Length; i++)
                            {
                                if (entries[i].type == keyValuePair.Value.entries[i].type)
                                {
                                    if (entries[i].type == ByamlNodeType.Array || entries[i].type == ByamlNodeType.Dictionary)
                                    {
                                        if ((object)entries[i].value == (object)keyValuePair.Value.entries[i].value)
                                            continue; //same reference
                                    }
                                    else
                                    {
                                        if (entries[i].value == keyValuePair.Value.entries[i].value)
                                            continue; //same value
                                    }
                                }
                                goto not_equal;
                            }
                            //Duplicate was found
                            return new Entry(ByamlNodeType.Array, keyValuePair.Key);
                        }
                    not_equal:
                        ;
                    }
                    //No duplicates found:
                }

                //Submit
                _arrays.Add(value, new ByamlArr(_valStackPointer, entries));
                _valStackPointer += (int)(4 + (Math.Ceiling(entries.Length / 4f) + entries.Length) * 4);
            }
            else if (type == ByamlNodeType.StringIndex)
            {
                if (!_stringSet.Contains(value))
                    _stringSet.Add(value);
            }
            else if (type == ByamlNodeType.PathIndex)
            {
                _pathArray.Add((List<ByamlPathPoint>)value);
            }
            else if (ByamlNodeType.Long <= type && type <= ByamlNodeType.Double)
            {
                if (!_eightByteValues.ContainsKey(value))
                    _eightByteValues.Add(value, _valStackPointer);

                _valStackPointer += 8;
            }

            return new Entry(type, value);
        }

        private void WriteStringArrayNode(BinaryDataWriter _writer, IEnumerable<string> node)
        {
            uint NodeStartPos = (uint)_writer.BaseStream.Position;

            if (_byteOrder == ByteOrder.BigEndian)
                _writer.Write((uint)ByamlNodeType.StringArray << 24 | (uint)Enumerable.Count(node));
            else
                _writer.Write((uint)ByamlNodeType.StringArray | (uint)Enumerable.Count(node) << 8);

            for (int i = 0; i <= node.Count(); i++) _writer.Write(new byte[4]); //Space for offsets
            List<uint> offsets = new List<uint>();
            foreach (string str in node)
            {
                offsets.Add((uint)_writer.BaseStream.Position - NodeStartPos);
                _writer.Write(str, BinaryStringFormat.ZeroTerminated);
            }
            offsets.Add((uint)_writer.BaseStream.Position - NodeStartPos);
            _writer.Align(4);
            uint backHere = (uint)_writer.BaseStream.Position;
            _writer.BaseStream.Position = NodeStartPos + 4;
            foreach (uint off in offsets) _writer.Write(off);
            _writer.BaseStream.Position = backHere;
        }


        private void WritePathArrayNode(BinaryDataWriter _writer, IEnumerable<List<ByamlPathPoint>> node)
        {
            if (_byteOrder == ByteOrder.BigEndian)
                _writer.Write((uint)ByamlNodeType.StringArray << 24 | (uint)Enumerable.Count(node));
            else
                _writer.Write((uint)ByamlNodeType.StringArray | (uint)Enumerable.Count(node) << 8);

            // Write the offsets to the paths, where the last one points to the end of the last path.
            long offset = 4 + 4 * (node.Count() + 1); // Relative to node start + all uint32 offsets.
            foreach (List<ByamlPathPoint> path in node)
            {
                _writer.Write((uint)offset);
                offset += path.Count * 28; // 28 bytes are required for a single point.
            }
            _writer.Write((uint)offset);

            // Write the paths.
            foreach (List<ByamlPathPoint> path in node)
            {
                WritePathNode(_writer, path);
            }
        }

        private void WritePathNode(BinaryDataWriter _writer, List<ByamlPathPoint> node)
        {
            foreach (ByamlPathPoint point in node)
            {
                WritePathPoint(_writer, point);
            }
        }

        private void WritePathPoint(BinaryDataWriter _writer, ByamlPathPoint point)
        {
            _writer.Write(point.Position[0]);
            _writer.Write(point.Position[1]);
            _writer.Write(point.Position[2]);
            _writer.Write(point.Normal[0]);
            _writer.Write(point.Normal[1]);
            _writer.Write(point.Normal[2]);
            _writer.Write(point.Unknown);
        }

        // ---- Helper methods ----

        static internal ByamlNodeType GetNodeType(dynamic node, bool isInternalNode = false)
        {
            if (isInternalNode)
            {
                if (node is IEnumerable<string>) return ByamlNodeType.StringArray;
                else if (node is IEnumerable<List<ByamlPathPoint>>) return ByamlNodeType.PathArray;
                else throw new Exception($"Type '{node.GetType()}' is not supported as a main BYAML node.");
            }
            else
            {
                if (node is string)
                    return ByamlNodeType.StringIndex;
                else if (node is List<ByamlPathPoint>) return ByamlNodeType.PathIndex;
                else if (node is IDictionary<string, dynamic>) return ByamlNodeType.Dictionary;
                else if (node is IEnumerable) return ByamlNodeType.Array;
                else if (node is bool) return ByamlNodeType.Boolean;
                else if (node is int) return ByamlNodeType.Integer;
                else if (node is float) return ByamlNodeType.Float; /*TODO decimal is float or double ? */
                else if (node is uint) return ByamlNodeType.UInteger;
                else if (node is long) return ByamlNodeType.Long;
                else if (node is ulong) return ByamlNodeType.ULong;
                else if (node is double) return ByamlNodeType.Double;
                else if (node is null) return ByamlNodeType.Null;
                else throw new Exception($"Type '{node.GetType()}' is not supported as a BYAML node.");
            }
        }
    }
}
