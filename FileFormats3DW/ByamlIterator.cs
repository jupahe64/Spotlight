using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BYAML.ByamlFile;

namespace BYAML
{
    public class ByamlIterator : ByamlReader
    {
        public struct DictionaryEntry
        {
            public string Key { get; private set; }

            private ByamlIterator _iterator;
            public long Position { get; private set; }
            public ByamlNodeType NodeType { get; private set; }

            internal DictionaryEntry(ByamlIterator iterator, (string, ByamlNodeType, long) entry)
            {
                Key = entry.Item1;
                _iterator = iterator;
                Position = entry.Item3;
                NodeType = entry.Item2;
            }

            public dynamic Parse()
            {
                if (_iterator._alreadyReadNodes.ContainsKey((uint)Position))
                    return _iterator._alreadyReadNodes[(uint)Position];

                _iterator._reader.Position = Position;
                return _iterator.ReadNodeValue(_iterator._reader, NodeType);
            }

            public IEnumerable<DictionaryEntry> IterDictionary()
            {
                _iterator._reader.Position = Position;
                uint lengthAndType = _iterator._reader.ReadUInt32();

                if (NodeType != ByamlNodeType.Dictionary)
                    throw new Exception("This entry is not a Dictionary");

                foreach ((string, ByamlNodeType, long) entry in _iterator.IterDictionaryNode(
                        _iterator._reader, (int)_iterator.Get3LsbBytes(lengthAndType))
                    )
                    yield return new DictionaryEntry(_iterator, entry);
            }

            public IEnumerable<ArrayEntry> IterArray()
            {
                _iterator._reader.Position = Position;
                uint length = _iterator._reader.ReadUInt32();

                if (NodeType != ByamlNodeType.Array)
                    throw new Exception("This entry is not an Array");

                foreach ((int, ByamlNodeType, long) entry in _iterator.IterArrayNode(
                        _iterator._reader, (int)_iterator.Get3LsbBytes(length))
                    )
                    yield return new ArrayEntry(_iterator, entry);
            }
        }

        public struct ArrayEntry
        {
            public int Index { get; private set; }

            private ByamlIterator _iterator;
            public long Position { get; private set; }
            public ByamlNodeType NodeType { get; private set; }

            internal ArrayEntry(ByamlIterator iterator, (int, ByamlNodeType, long) entry)
            {
                Index = entry.Item1;
                _iterator = iterator;
                Position = entry.Item3;
                NodeType = entry.Item2;
            }

            public dynamic Parse()
            {
                _iterator._reader.Position = Position;
                return _iterator.ReadNodeValue(_iterator._reader, NodeType);
            }

            public IEnumerable<DictionaryEntry> IterDictionary()
            {
                _iterator._reader.Position = Position;
                uint length = _iterator._reader.ReadUInt32();

                if (NodeType != ByamlNodeType.Dictionary)
                    throw new Exception("This entry is not a Dictionary");

                foreach ((string, ByamlNodeType, long) entry in _iterator.IterDictionaryNode(
                        _iterator._reader, (int)_iterator.Get3LsbBytes(length))
                    )
                    yield return new DictionaryEntry(_iterator, entry);
            }

            public IEnumerable<ArrayEntry> IterArray()
            {
                _iterator._reader.Position = Position;
                uint length = _iterator._reader.ReadUInt32();

                if (NodeType != ByamlNodeType.Array)
                    throw new Exception("This entry is not an Array");

                foreach ((int, ByamlNodeType, long) entry in _iterator.IterArrayNode(
                        _iterator._reader, (int)_iterator.Get3LsbBytes(length))
                    )
                    yield return new ArrayEntry(_iterator, entry);
            }
        }
        
        bool _isClosed = false;

        public ByamlIterator(Stream stream, ByteOrder byteOrder = ByteOrder.LittleEndian, ushort version = 3, bool fastLoad = true)
            : base(stream, false, byteOrder, version, fastLoad)
        {
            
        }

        ~ByamlIterator()
        {
            _reader.Dispose();
        }

        public IEnumerable<DictionaryEntry> IterRootDictionary()
        {
            if (_isClosed)
                throw new Exception("Can't iterate more than once");


            if (!TryStartLoading())
                yield break;

            uint lengthAndType = _reader.ReadUInt32();

            if ((ByamlNodeType)Get1MsbByte(lengthAndType) != ByamlNodeType.Dictionary)
                throw new Exception("Root is not a Dictionary");

            foreach ((string, ByamlNodeType, long) entry in IterDictionaryNode(_reader, (int)Get3LsbBytes(lengthAndType)))
                yield return new DictionaryEntry(this, entry);

            _isClosed = true;
        }

        public IEnumerable<ArrayEntry> IterRootArray()
        {
            if (_isClosed)
                throw new Exception("Can't iterate more than once");

            if (!TryStartLoading())
                yield break;

            uint lengthAndType = _reader.ReadUInt32();

            if ((ByamlNodeType)Get1MsbByte(lengthAndType) != ByamlNodeType.Array)
                throw new Exception("Root is not an Array");

            foreach ((int, ByamlNodeType, long) entry in IterArrayNode(_reader, (int)Get3LsbBytes(lengthAndType)))
                yield return new ArrayEntry(this, entry);

            _isClosed = true;
        }


        private IEnumerable<(int, ByamlNodeType, long)> IterArrayNode(BinaryDataReader reader, int length, uint offset = 0)
        {
            // Read the element types of the array.
            byte[] nodeTypes = reader.ReadBytes(length);
            // Read the elements, which begin after a padding to the next 4 bytes.
            reader.Align(4);
            for (int i = 0; i < length; i++)
            {
                long pos = reader.Position;
                ByamlNodeType nodeType = (ByamlNodeType)nodeTypes[i];
                if (IsReferenceType(nodeType))
                    yield return (i, nodeType, reader.ReadUInt32());
                else
                    yield return (i, nodeType, pos);

                reader.Position = pos + 4;
            }
        }

        private IEnumerable<(string, ByamlNodeType, long)> IterDictionaryNode(BinaryDataReader reader, int length, uint offset = 0)
        {
            HashSet<string> readNames = new HashSet<string>();

            // Read the elements of the dictionary.
            for (int i = 0; i < length; i++)
            {
                uint indexAndType = reader.ReadUInt32();
                int nodeNameIndex = (int)Get3MsbBytes(indexAndType);
                ByamlNodeType nodeType = (ByamlNodeType)Get1LsbByte(indexAndType);
                string nodeName = _nameArray[nodeNameIndex];

                if (readNames.Contains(nodeName))
                    Console.WriteLine("Duplicate Key: " + nodeName);

                readNames.Add(nodeName);

                long pos = reader.Position;

                if (IsReferenceType(nodeType))
                    yield return (nodeName, nodeType, reader.ReadUInt32());
                else
                    yield return (nodeName, nodeType, pos);

                reader.Position = pos + 4;
            }
        }
    }
}
