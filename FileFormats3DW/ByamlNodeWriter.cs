using Syroot.BinaryData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static BYAML.ByamlFile;

namespace BYAML
{
    public class ByamlNodeWriter : ByamlWriter
    {
        public ByamlNodeWriter(Stream stream, bool supportPaths, ByteOrder byteOrder, ushort version) 
            : base(stream, supportPaths, byteOrder, version)
        {
            
        }

        public DictionaryNode CreateDictionaryNode(object valueRepresentation = null) => new DictionaryNode(this, valueRepresentation);

        public struct DictionaryNode
        {
            List<(string, Entry)> _entries;
            ByamlNodeWriter _byamlNodeWriter;
            object _valueRepresentation;

            /// <summary>
            /// The number of entries in this DictionaryNode
            /// </summary>
            public int Count => _entries.Count;

            /// <summary>
            /// Creates a new <see cref="DictionaryNode"/> which needs to be Submited by calling <see cref="Submit"/>
            /// </summary>
            /// <param name="byamlNodeWriter"></param>
            internal DictionaryNode(ByamlNodeWriter byamlNodeWriter, object valueRepresentation)
            {
                _byamlNodeWriter = byamlNodeWriter;
                _entries = new List<(string, Entry)>();
                _valueRepresentation = valueRepresentation;

                if (_valueRepresentation == null)
                    _valueRepresentation = this;
            }

            /// <summary>
            /// Adds a new entry to this <see cref="DictionaryNode"/>. The value is converted to a subnode.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddDynamicValue(string key, dynamic value, bool fastWrite = false)
            {
                if (!_byamlNodeWriter._nameSet.Contains(key))
                    _byamlNodeWriter._nameSet.Add(key);

                _entries.Add((key, _byamlNodeWriter.ProcessValue(value, fastWrite)));
            }

            /// <summary>
            /// Adds a new entry to this <see cref="DictionaryNode"/>.
            /// The entry is later linked by the <paramref name="referenceKey"/> to another submited <see cref="DictionaryNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddDictionaryRef(string key, object referenceKey)
            {
                if (!_byamlNodeWriter._nameSet.Contains(key))
                    _byamlNodeWriter._nameSet.Add(key);

                _entries.Add((key, new Entry(ByamlNodeType.Dictionary, referenceKey)));
            }

            /// <summary>
            /// Adds the <paramref name="node"/> as a subnode to this <see cref="DictionaryNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddDictionaryNodeRef(string key, DictionaryNode node, bool fastWrite = false)
            {
                if (!_byamlNodeWriter._nameSet.Contains(key))
                    _byamlNodeWriter._nameSet.Add(key);

                _entries.Add((key, new Entry(ByamlNodeType.Dictionary, node.Submit(fastWrite) )));
            }

            /// <summary>
            /// Adds the <paramref name="node"/> as a subnode to this <see cref="DictionaryNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddArrayRef(string key, object referenceKey)
            {
                if (!_byamlNodeWriter._nameSet.Contains(key))
                    _byamlNodeWriter._nameSet.Add(key);

                _entries.Add((key, new Entry(ByamlNodeType.Array, referenceKey)));
            }

            /// <summary>
            /// Adds a new entry to this <see cref="ArrayNode"/>.
            /// The entry is later linked by the <paramref name="referenceKey"/> to another submited <see cref="DictionaryNode"/>.
            /// </summary>
            /// <param name="node"></param>
            public void AddArrayNodeRef(string key, ArrayNode node, bool fastWrite = false)
            {
                if (!_byamlNodeWriter._nameSet.Contains(key))
                    _byamlNodeWriter._nameSet.Add(key);

                _entries.Add((key, new Entry(ByamlNodeType.Array, node.Submit(fastWrite) )));
            }


            internal object Submit(bool fastWrite)
            {
                if (_byamlNodeWriter._dictionaries.ContainsKey(_valueRepresentation))
                    return _valueRepresentation;

                (string, Entry)[] entries = _entries.OrderBy(x => x.Item1, StringComparer.Ordinal).ToArray();

                if (!fastWrite)
                {
                    foreach (var keyValuePair in _byamlNodeWriter._dictionaries)
                    {
                        if (entries.Length == keyValuePair.Value.entries.Length)
                        {
                            for (int i = 0; i < entries.Length; i++)
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
                            _valueRepresentation = keyValuePair.Key;
                            return keyValuePair.Key;
                        }
                    not_equal:
                        ;
                    }
                    //No duplicates found
                }

                _byamlNodeWriter._dictionaries.Add(_valueRepresentation, new ByamlDict(_byamlNodeWriter._valStackPointer, entries));
                _byamlNodeWriter._valStackPointer += 4 + entries.Length * 8;

                return _valueRepresentation;
            }
        }

        public ArrayNode CreateArrayNode(object valueRepresentation = null) => new ArrayNode(this, valueRepresentation);

        public struct ArrayNode
        {
            List<Entry> _entries;
            ByamlNodeWriter _byamlNodeWriter;
            object _valueRepresentation;


            /// <summary>
            /// The number of entries in this ArrayNode
            /// </summary>
            public int Count => _entries.Count;

            /// <summary>
            /// Creates a new <see cref="ArrayNode"/> which needs to be Submited by calling <see cref="Submit"/>
            /// </summary>
            /// <param name="byamlNodeWriter"></param>
            internal ArrayNode(ByamlNodeWriter byamlNodeWriter, object valueRepresentation)
            {
                _byamlNodeWriter = byamlNodeWriter;
                _entries = new List<Entry>();
                _valueRepresentation = valueRepresentation;

                if (_valueRepresentation == null)
                    _valueRepresentation = this;
            }

            /// <summary>
            /// Adds a new entry to this <see cref="ArrayNode"/>. The value is converted to a subnode.
            /// </summary>
            /// <param name="value"></param>
            public void AddDynamicValue(dynamic value, bool fastWrite = false)
            {
                _entries.Add(_byamlNodeWriter.ProcessValue(value, fastWrite));
            }

            /// <summary>
            /// Adds a new entry to this <see cref="ArrayNode"/>.
            /// The entry is later linked by the <paramref name="referenceKey"/> to another submited <see cref="DictionaryNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddDictionaryRef(object referenceKey)
            {
                _entries.Add(new Entry(ByamlNodeType.Dictionary, referenceKey));
            }

            /// <summary>
            /// Adds the <paramref name="node"/> as a subnode to this <see cref="ArrayNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddDictionaryNodeRef(DictionaryNode node, bool fastWrite = false)
            {
                _entries.Add(new Entry(ByamlNodeType.Dictionary, node.Submit(fastWrite) ));
            }

            /// <summary>
            /// Adds the <paramref name="node"/> as a subnode to this <see cref="ArrayNode"/>.
            /// </summary>
            /// <param name="node"></param>
            /// 
            public void AddArrayRef(object referenceKey)
            {
                _entries.Add(new Entry(ByamlNodeType.Array, referenceKey));
            }

            /// <summary>
            /// Adds a new entry to this <see cref="ArrayNode"/>.
            /// The entry is later linked by the <paramref name="referenceKey"/> to another submited <see cref="ArrayNode"/>.
            /// </summary>
            /// <param name="node"></param>
            public void AddArrayNodeRef(ArrayNode node, bool fastWrite = false)
            {
                _entries.Add(new Entry(ByamlNodeType.Array, node.Submit(fastWrite) ));
            }


            internal object Submit(bool fastWrite)
            {
                if (_byamlNodeWriter._arrays.ContainsKey(_valueRepresentation))
                    return _valueRepresentation;

                Entry[] entries = _entries.ToArray();

                if (!fastWrite)
                {
                    foreach (var keyValuePair in _byamlNodeWriter._arrays)
                    {
                        if (entries.Length == keyValuePair.Value.entries.Length)
                        {
                            for (int i = 0; i < keyValuePair.Value.entries.Length; i++)
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
                            _valueRepresentation = keyValuePair.Key;
                            return keyValuePair.Key;
                        }
                    not_equal:
                        ;
                    }
                    //No duplicates found
                }

                _byamlNodeWriter._arrays.Add(_valueRepresentation, new ByamlArr(_byamlNodeWriter._valStackPointer, entries));
                _byamlNodeWriter._valStackPointer += (int)(4 + (Math.Ceiling(entries.Length / 4f) + entries.Length) * 4);

                return _valueRepresentation;
            }
        }

        public new void Write(object root, bool fastWrite = false)
        {
            throw new NotImplementedException();
        }

        public void Write(ArrayNode rootNode, bool fastWrite = false)
        {
            _nameArray = _nameSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            _stringArray = _stringSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();

            WriteContent(rootNode.Submit(fastWrite));
        }

        public void Write(DictionaryNode rootNode, bool fastWrite = false)
        {
            _nameArray = _nameSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            _stringArray = _stringSet.OrderBy(x => x, StringComparer.Ordinal).ToArray();

            WriteContent(rootNode.Submit(fastWrite));
        }
    }
}
