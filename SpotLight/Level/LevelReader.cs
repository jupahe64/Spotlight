using BYAML;
using Spotlight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlIterator;
using static Spotlight.Level.LevelIO;
using GL_EditorFramework;
using OpenTK;

namespace Spotlight.Level
{
    class LevelReader
    {
        Dictionary<long, I3dWorldObject> objsByBymlRef = new Dictionary<long, I3dWorldObject>();

        Dictionary<string, (string list, ushort scenarioBits, I3dWorldObject obj)[]> objsInfosByIDs = new Dictionary<string, (string list, ushort scenarioBits, I3dWorldObject obj)[]>();

        Dictionary<string, (ushort scenarioBits, ZonePlacement placement)[]> zonePlacementsInfosByID = new Dictionary<string, (ushort scenarioBits, ZonePlacement placement)[]>();

        public HashSet<string> readLayers = new HashSet<string>();

        ushort scenarioBit = 1;

        int scenarioIndex = 0;

        string listName = null;

        SM3DWorldZone zone;

        public LevelReader(SM3DWorldZone zone)
        {
            this.zone = zone;
        }

        public IEnumerable<(I3dWorldObject obj, ushort scenarioBits, bool isLinked)> GetObjectsWithScenarioBits()
        {
            foreach (var value in objsInfosByIDs.Values)
            {
                foreach (var (list, scenarioBits, obj) in value)
                {
                    yield return (obj, scenarioBits, list==null);
                }
            }
        }

        public IEnumerable<(ZonePlacement placement, ushort scenarioBits)> GetZonePlacementsWithScenarioBits()
        {
            foreach (var value in zonePlacementsInfosByID.Values)
            {
                foreach (var (scenarioBits, placement) in value)
                {
                    yield return (placement, scenarioBits);
                }
            }
        }

        public I3dWorldObject ParseObject(in ArrayEntry objectEntry, out bool alreadyAdded, bool isLinked = false)
        {
            alreadyAdded = false;

            I3dWorldObject obj;
            bool loadLinks;

            //all objects that have the same ID as the object currently parsed
            (string list, ushort scenarioBits, I3dWorldObject obj)[] sameIDObjs;

            if (objsByBymlRef.TryGetValue(objectEntry.Position, out I3dWorldObject refObj))
            {
                obj = refObj;

                alreadyAdded = true;

                objsInfosByIDs.TryGetValue(obj.ID, out sameIDObjs);

                UpdateLinkScenarioBits(objectEntry); //"Mofumofu", "DemoBattleEndPosition", 233028
            }
            else
            {
                ObjectInfo info = GetObjectInfo(objectEntry);

                readLayers.Add(info.Layer);

                objsInfosByIDs.TryGetValue(info.ID, out sameIDObjs);

                #region Create object
                //creates an I3dWorldObject from the given object info

                if ((info.ClassName == "Area") || info.ObjectName.Contains("Area") && AreaModelNames.Contains(info.ModelName))
                    obj = new AreaObject(in info, zone, out loadLinks);
                else if (info.PropertyEntries.TryGetValue("RailPoints", out DictionaryEntry railPointEntry) && railPointEntry.NodeType == ByamlFile.ByamlNodeType.Array) //at this point we can be sure it's a rail
                    obj = new Rail(in info, zone, out loadLinks);
                else
                    obj = new General3dWorldObject(in info, zone, out loadLinks);

                #endregion


                if (sameIDObjs == null)
                {
                    //Add entry for this obj ID
                    objsInfosByIDs[info.ID] = new (string list, ushort scenarioBits, I3dWorldObject obj)[]
                    {
                        (isLinked ? null : listName, scenarioBit, obj)
                    };
                }
                else
                {
                    #region Unify

                    //try to unify with all objects that have the same ID
                    bool successfullyUnified = false;

                    for (int i = 0; i < sameIDObjs.Length; i++)
                    {
                        if (obj.Equals(sameIDObjs[i].obj))
                        {
                            obj = sameIDObjs[i].obj;
                            successfullyUnified = true;
                            break;
                        }
                    }

                    #endregion

                    if (successfullyUnified)
                    {
                        alreadyAdded = true;
                    }
                    else
                    {
                        //No match was found, object can not be unified, so add it to the list of objects with the same ID
                        Array.Resize(ref sameIDObjs, sameIDObjs.Length + 1);
                        sameIDObjs[sameIDObjs.Length - 1] = (isLinked ? null : listName, scenarioBit, obj);

                        objsInfosByIDs[info.ID] = sameIDObjs;
                    }
                }

                objsByBymlRef[objectEntry.Position] = obj;


                #region Add/merge links
                if (info.LinkEntries.Count > 0)
                {
                    if (obj.Links == null)
                        obj.Links = new Dictionary<string, List<I3dWorldObject>>();

                    if (obj.Links != null) //links are supported by the parsed object, otherwise Links would still be null
                    {
                        foreach (var (linkName, linkEntry) in info.LinkEntries)
                        {
                            foreach (var linkedEntry in linkEntry.IterArray())
                            {
                                var linkedObj = ParseObject(linkedEntry, out bool linkedAlreadyAdded, true);

                                if (!obj.Links.TryGetValue(linkName, out var link))
                                {
                                    link = new List<I3dWorldObject>();

                                    obj.Links[linkName] = link;
                                }

                                if (!link.Contains(linkedObj))
                                {
                                    link.Add(linkedObj);
                                    linkedObj.AddLinkDestination(linkName, obj);
                                }

                                if (!linkedAlreadyAdded)
                                    zone.LinkedObjects.Add(linkedObj);
                            }
                        }
                    }
                }
                #endregion
            }

            if (sameIDObjs != null)
            {
                #region Update Scenario, Validate Linked

                //scenarios are ODYSSEY only

                //for an object to be valid in (Common_)Linked it can't appear in any ObjectList,
                //if it does, remove it from LinkedObjects

                for (int i = 0; i < sameIDObjs.Length; i++)
                {
                    if (sameIDObjs[i].obj == obj)
                    {
                        #region Update Scenario
                        sameIDObjs[i].scenarioBits |= scenarioBit;
                        #endregion

                        #region Validate Linked
                        if (!isLinked && sameIDObjs[i].list == null)
                        {
                            sameIDObjs[i].list = listName;
                            zone.LinkedObjects.Remove(obj);
                            alreadyAdded = false;
                        }
                        #endregion

                        break;
                    }
                }

                #endregion
            }

            return obj;
        }

        /// <summary>
        /// go recursivly through all links 
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateLinkScenarioBits(ArrayEntry objectEntry, Stack<long> travelStack = null)
        {
            //return; //For now

            if (travelStack == null)
                travelStack = new Stack<long>();

            travelStack.Push(objectEntry.Position);

            foreach (var entry in objectEntry.IterDictionary())
            {
                if(entry.Key=="Links")
                {
                    foreach (var link in entry.IterDictionary())
                    {
                        foreach (var _objectEntry in link.IterArray())
                        {
                            if (travelStack.Contains(_objectEntry.Position))
                                continue;

                            var linkedObj = objsByBymlRef[_objectEntry.Position];

                            var sameIDObjs = objsInfosByIDs[linkedObj.ID];

                            bool skipThis = false;

                            for (int i = 0; i < sameIDObjs.Length; i++)
                            {
                                if (sameIDObjs[i].obj == linkedObj)
                                {
                                    ushort newBits = (ushort)(sameIDObjs[i].scenarioBits | scenarioBit);

                                    if (newBits == sameIDObjs[i].scenarioBits)
                                        skipThis = true;

                                    sameIDObjs[i].scenarioBits = newBits;
                                    break;
                                }
                            }

                            if(!skipThis)
                                UpdateLinkScenarioBits(_objectEntry, travelStack);
                        }
                    }
                }
            }

            travelStack.Pop();
        }

        public bool TryParseZone(in ArrayEntry arrayEntry, out ZonePlacement zonePlacement)
        {
            var info = GetObjectInfo(arrayEntry);
            string stageName = info.ObjectName;


            SM3DWorldZone refZone;
            if (!SM3DWorldZone.TryOpen(zone.Directory, stageName, out refZone))
                SM3DWorldZone.TryOpen(Program.BaseStageDataPath, stageName, out refZone);

            var zoneId = info.ID;

            (ushort scenarioBits, ZonePlacement placement)[] sameZoneIDPlacements;


            //check for duplicates
            if (zonePlacementsInfosByID.TryGetValue(zoneId, out sameZoneIDPlacements))
            {
                for (int i = 0; i < sameZoneIDPlacements.Length; i++)
                {
                    if(sameZoneIDPlacements[i].placement.Zone==refZone && 
                        sameZoneIDPlacements[i].placement.Position == info.Position &&
                        sameZoneIDPlacements[i].placement.Rotation == info.Rotation &&
                        sameZoneIDPlacements[i].placement.Layer == info.Layer)
                    {

                        sameZoneIDPlacements[i].scenarioBits |= scenarioBit;
                        zonePlacement = null;
                        return false;
                    }
                }
            }


            {
                
                
                if (refZone != null)
                {
                    
                    zonePlacement = new ZonePlacement(info.Position, info.Rotation, info.Layer, refZone);


                    if (sameZoneIDPlacements == null)
                    {
                        //Add entry for this obj ID
                        zonePlacementsInfosByID[info.ID] = new (ushort scenarioBits, ZonePlacement obj)[]
                        {
                            (scenarioBit, zonePlacement)
                        };
                    }
                    else
                    {
                        Array.Resize(ref sameZoneIDPlacements, sameZoneIDPlacements.Length + 1);
                        sameZoneIDPlacements[sameZoneIDPlacements.Length - 1] = (scenarioBit, zonePlacement);

                        zonePlacementsInfosByID[info.ID] = sameZoneIDPlacements;
                    }

                    return true;
                }
                else
                {
                    zonePlacement = null;
                    return false;
                }
            }
        }



        public void LoadStageByml(ByamlIterator byamlIter, string prefix)
        {
            objsByBymlRef.Clear();

#if ODYSSEY
            foreach (var scenario in byamlIter.IterRootArray())
            {
                if (scenario.Index > 14)
                    continue; //for compatibiltiy with levels saved in older versons of Moonlight

                scenarioIndex = scenario.Index;
                scenarioBit = (ushort)(1 << scenarioIndex);

                foreach (DictionaryEntry entry in scenario.IterDictionary())
                {
                    if (entry.Key == "ZoneList")
                    {
                        foreach (ArrayEntry obj in entry.IterArray())
                        {
                            if (TryParseZone(obj, out ZonePlacement zonePlacement))
                                zone.ZonePlacements.Add(zonePlacement);
                        }
                    }
                    else //object list
                    {
                        listName = prefix + entry.Key;

                        if (!zone.ObjLists.TryGetValue(listName, out var list))
                        {
                            list = new ObjectList();

                            zone.ObjLists.Add(listName, list);
                        }

                        foreach (ArrayEntry objEntry in entry.IterArray())
                        {
                            I3dWorldObject obj = ParseObject(objEntry, out bool alreadyAdded);
                            if (!alreadyAdded)
                                list.Add(obj);
                        }
                    }
                }
            }
#else //3D World
            foreach (DictionaryEntry entry in byamlIter.IterRootDictionary())
            {
                if (entry.Key == "FilePath" || entry.Key == "Objs")
                    continue;

                if (entry.Key == "ZoneList")
                {
                    foreach (ArrayEntry obj in entry.IterArray())
                    {
                        if (TryParseZone(obj, out ZonePlacement zonePlacement))
                            zone.ZonePlacements.Add(zonePlacement);
                    }
                }
                else //object list
                {
                    listName = prefix + entry.Key;

                    if (!zone.ObjLists.TryGetValue(listName, out var list))
                    {
                        list = new ObjectList();

                        zone.ObjLists.Add(listName, list);
                    }

                    foreach (ArrayEntry objEntry in entry.IterArray())
                    {
                        I3dWorldObject obj = ParseObject(objEntry, out bool alreadyAdded);
                        if (!alreadyAdded)
                            list.Add(obj);
                    }
                }
            }
#endif
        }
    }
}
