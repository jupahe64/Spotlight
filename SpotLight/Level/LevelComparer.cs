using BYAML;
using EveryFileExplorer;
using SARCExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlIterator;

namespace Spotlight.Level
{
    internal static class LevelComparer
    {
        class Level
        {
            public Dictionary<string, (ArrayEntry data, Dictionary<string, List<long>> linkInfos, Dictionary<string, DictionaryEntry> propertyInfos)[]> objectsPerId = new();

            public Dictionary<long, List<string>> usagesPerReference = new Dictionary<long, List<string>>();

            public Level(ByamlIterator iterator, int scenarioIndex = 0)
            {
                foreach (var scenario in iterator.IterRootArray())
                {
                    if (scenario.Index != scenarioIndex)
                        continue;

                    var objectsLeft = new Queue<(ArrayEntry obj, string listName)>();

                    foreach (var list in scenario.IterDictionary())
                    {
                        if (list.Key == "ZoneList") //TODO
                            continue;


                        foreach (var obj in list.IterArray())
                        {
                            objectsLeft.Enqueue((obj, list.Key));
                        }
                    }

                    string currentRailName = "";
                    int currentPointIndex = 0;

                    while (objectsLeft.Count>0)
                    {
                        var (obj, listName) = objectsLeft.Dequeue();

                        if (usagesPerReference.TryGetValue(obj.Position, out List<string> usages))
                        {
                            usages.Add(listName);
                            continue;
                        }
                        else
                        {
                            usagesPerReference.Add(obj.Position, new List<string> { listName });
                        }

                        string id = null;

                        Dictionary<string, List<long>> linkInfos = new Dictionary<string, List<long>>();
                        Dictionary<string, DictionaryEntry> propertyInfos = new Dictionary<string, DictionaryEntry>();

                        foreach (var entry in obj.IterDictionary())
                        {
                            if (entry.Key == "Links")
                            {
                                foreach (var list in entry.IterDictionary())
                                {
                                    var link = new List<long>();

                                    foreach (var linkedObj in list.IterArray())
                                    {
                                        objectsLeft.Enqueue((linkedObj, list.Key));

                                        link.Add(linkedObj.Position);
                                    }

                                    linkInfos.Add(list.Key, link);
                                }
                            }
                            else if (entry.Key == "RailPoints")
                            {
                                foreach (var point in entry.IterArray())
                                {
                                    objectsLeft.Enqueue((point, "RailPoints"));
                                }
                            }
                            else if(entry.Key == "Id")
                            {
                                id = entry.Parse();
                            }
                            else if (entry.Key == "UnitConfig")
                            {
                                foreach (var ucEntry in entry.IterDictionary())
                                    propertyInfos.Add("UnitConfig:"+ucEntry.Key, ucEntry);
                            }
                            else
                            {
                                propertyInfos.Add(entry.Key, entry);
                            }
                        }

                        if (id.Contains("/"))
                        {
                            var _railname = id.Split('/')[0];

                            if(currentRailName == _railname)
                                currentPointIndex++;
                            else
                            {
                                currentRailName = _railname;
                                currentPointIndex = 0;
                            }

                            id = currentRailName+"/"+currentPointIndex;
                        }



                        var newEntry = (obj, linkInfos, propertyInfos);

                        if (objectsPerId.TryGetValue(id, out (ArrayEntry data, Dictionary<string, List<long>> linkInfos, Dictionary<string, DictionaryEntry> propertyInfos)[] entries))
                        {
                            var tmp = entries;

                            Array.Resize(ref tmp, tmp.Length + 1);

                            tmp[tmp.Length - 1] = newEntry;

                            objectsPerId[id] = tmp;
                        }
                        else
                        {
                            objectsPerId[id] = new (ArrayEntry data, Dictionary<string, List<long>> linkInfos, Dictionary<string, DictionaryEntry> propertyInfos)[]
                            {
                                newEntry
                            };
                        }
                    }
                }
            }
        }

        public static void Compare(string levelFilePathA, string levelFilePathB)
        {
            Level Load(string path, int scenarioIndex = 0)
            {
                SarcData sarc = SARC.UnpackRamN(YAZ0.Decompress(File.ReadAllBytes(path)));

                return new Level(
                    new ByamlIterator(new MemoryStream(sarc.Files[Path.GetFileNameWithoutExtension(path) + ".byml"])), scenarioIndex);
            }

            for (int i = 0; i < 15; i++)
            {
                var levelA = Load(levelFilePathA, i);
                var levelB = Load(levelFilePathB, i);

                foreach (var entry in levelA.objectsPerId)
                {
                    var objsA = entry.Value;

                    bool found = levelB.objectsPerId.TryGetValue(entry.Key, out (ArrayEntry data, Dictionary<string, List<long>> linkInfos, Dictionary<string, DictionaryEntry> propertyInfos)[] objsB);

                    if (!found)
                    {
                        Debugger.Break();
                        continue;
                    }

                    if (objsA.Length != objsB.Length)
                        Debugger.Break();

                    if (objsA.Length == 1)
                    {
                        var objA = objsA[0];
                        var objB = objsB[0];

                        var usagesA = levelA.usagesPerReference[objA.data.Position];
                        var usagesB = levelB.usagesPerReference[objB.data.Position];

                        usagesA.Sort();
                        usagesB.Sort();

                        if (!usagesA.SequenceEqual(usagesB))
                            Debugger.Break();

                        //if (objA.data.Index != objB.data.Index)
                        //    Debugger.Break();

                        if (objA.linkInfos.Count != objB.linkInfos.Count)
                            Debugger.Break();

                        foreach (var linkInfo in objA.linkInfos)
                        {
                            found = objB.linkInfos.TryGetValue(linkInfo.Key, out List<long> linkedObjs);

                            if (!found)
                                Debugger.Break();

                            if (linkInfo.Value.Count != linkedObjs.Count)
                                Debugger.Break();
                        }


                        string className = objA.propertyInfos["UnitConfig:ParameterConfigName"].Parse();

                        foreach (var propertyInfo in objB.propertyInfos)
                        {
                            if (propertyInfo.Key == "UnitConfig:GenerateCategory" || 
                                propertyInfo.Key == "UnitConfig:PlacementTargetFile" || 
                               (propertyInfo.Key == "UnitConfig:ParameterConfigName" && className.EndsWith("Area")) || 
                               (propertyInfo.Key == "Priority" && className.Contains("Area")) || 
                               (propertyInfo.Key == "UnitConfig:DisplayName" && (className.Contains("Area") || className.StartsWith("Rail") || className.StartsWith("Point"))))
                                continue; //those can be skipped

                            found = objA.propertyInfos.TryGetValue(propertyInfo.Key, out var entryA);

                            if (!found)
                            {
                                Debugger.Break();
                                continue;
                            }

                            var entryB = propertyInfo.Value;

                            Compare(entryA, entryB);
                        }
                    }
                    else
                    {
                        Debugger.Break(); //TODO
                    }
                }

                if (levelA.objectsPerId.Count != levelB.objectsPerId.Count)
                    Debugger.Break(); //just in case an object was saved that wasn't supposed to save
            }

            

            ;
        }

        public static void Compare(DictionaryEntry entryA, DictionaryEntry entryB)
        {
            if (entryA.NodeType != entryB.NodeType)
            {
                Debugger.Break();
                return;
            }

            switch (entryA.NodeType)
            {
                case ByamlFile.ByamlNodeType.StringIndex:
                case ByamlFile.ByamlNodeType.Boolean:
                case ByamlFile.ByamlNodeType.Integer:
                case ByamlFile.ByamlNodeType.UInteger:
                case ByamlFile.ByamlNodeType.Long:
                case ByamlFile.ByamlNodeType.ULong:
                    if (entryA.Parse() != entryB.Parse())
                        Debugger.Break();

                    break;
                case ByamlFile.ByamlNodeType.Float:
                case ByamlFile.ByamlNodeType.Double:
                    if (Math.Abs(entryA.Parse() - entryB.Parse()) > 0.01)
                        Debugger.Break();

                    break;

                case ByamlFile.ByamlNodeType.Array:

                    var arrayA = entryA.IterArray().ToArray();

                    foreach (var subEntryB in entryB.IterArray())
                    {
                        Compare(arrayA[subEntryB.Index], subEntryB);

                    }


                    break;
                case ByamlFile.ByamlNodeType.Dictionary:

                    var dictA = entryA.IterDictionary().ToDictionary(x=>x.Key);

                    foreach (var subEntryB in entryB.IterDictionary())
                    {
                        bool found = dictA.TryGetValue(subEntryB.Key, out var subEntryA);

                        Compare(subEntryA, subEntryB);
                    }

                    break;
                default:
                    break;
            }
        }

        public static void Compare(ArrayEntry entryA, ArrayEntry entryB)
        {
            if (entryA.NodeType != entryB.NodeType)
                Debugger.Break();

            switch (entryA.NodeType)
            {
                case ByamlFile.ByamlNodeType.StringIndex:
                case ByamlFile.ByamlNodeType.Boolean:
                case ByamlFile.ByamlNodeType.Integer:
                case ByamlFile.ByamlNodeType.Float:
                case ByamlFile.ByamlNodeType.UInteger:
                case ByamlFile.ByamlNodeType.Long:
                case ByamlFile.ByamlNodeType.ULong:
                case ByamlFile.ByamlNodeType.Double:
                    if (entryA.Parse() != entryB.Parse())
                        Debugger.Break();

                    break;

                case ByamlFile.ByamlNodeType.Array:

                    var arrayA = entryA.IterArray().ToArray();

                    foreach (var subEntryB in entryB.IterArray())
                    {
                        Compare(arrayA[subEntryB.Index], subEntryB);

                    }


                    break;
                case ByamlFile.ByamlNodeType.Dictionary:

                    var dictA = entryA.IterDictionary().ToDictionary(x => x.Key);

                    foreach (var subEntryA in entryB.IterDictionary())
                    {
                        bool found = dictA.TryGetValue(subEntryA.Key, out var subEntryB);

                        Compare(subEntryA, subEntryB);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
