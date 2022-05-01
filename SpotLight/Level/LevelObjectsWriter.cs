using BYAML;
using Spotlight.EditorDrawables;
using System;
using System.Collections.Generic;
using GL_EditorFramework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;
using System.Diagnostics;

namespace Spotlight.Level
{
    public class LevelObjectsWriter
    {
        private enum LinkState
        {
            NotLinked,
            Linked,
            LinkedNoDelete
        }

        private readonly HashSet<Layer> layers;
        private readonly ByamlNodeWriter nodeWriter;
        private readonly HashSet<I3dWorldObject> linkedObjs;
        private readonly ArrayNode objsNode;
        private Dictionary<I3dWorldObject, DictionaryNode> alreadyWrittenObjs;

        public IReadOnlyCollection<Layer> Layers => layers;

        public LevelObjectsWriter(HashSet<Layer> layers, ByamlNodeWriter nodeWriter, List<I3dWorldObject> linkedObjs, Dictionary<I3dWorldObject, DictionaryNode> alreadyWrittenObjs = null, ArrayNode objsNode = null)
        {
            this.layers = layers;
            this.nodeWriter = nodeWriter;
            this.linkedObjs = linkedObjs.ToHashSet();
            this.alreadyWrittenObjs = alreadyWrittenObjs ?? new Dictionary<I3dWorldObject, DictionaryNode>();
            this.objsNode = objsNode;
        }

        private LinkState EvaluateLinkState(I3dWorldObject obj)
        {
            var linkState = LinkState.NotLinked;

            var travelStack = new Stack<I3dWorldObject>();

            //if(obj.ID=="obj2449")
            //    Debugger.Break();

            foreach (var (linkName,linkingObj) in obj.LinkDestinations)
            {
                if (TryEvaluateIfObjGetsSaved(linkingObj, travelStack, out bool _result) && _result == true)
                {
                    if (linkName.StartsWith("NoDelete_"))
                        return LinkState.LinkedNoDelete;

                    linkState = LinkState.Linked;
                }
            }

            return linkState;
        }

        private Dictionary<I3dWorldObject, bool> objGetsSaved_cache = new Dictionary<I3dWorldObject, bool>();

        private bool TryEvaluateIfObjGetsSaved(I3dWorldObject obj, Stack<I3dWorldObject> travelStack /*for detecting cycles*/, out bool result)
        {
            if (!layers.Contains(obj.Layer))
            {
                result = false; //if the object's layer isn't active it won't get saved no matter what
                return true;
            }

            if (objGetsSaved_cache.TryGetValue(obj, out result))
                return true;

            if (travelStack.Contains(obj))
                return false; //cycles cannot be evaluated

            if (obj.LinkDestinations.Count == 0)
            {
                result = objGetsSaved_cache[obj] = !linkedObjs.Contains(obj); //"orphans" don't get saved but regular objects that just have no links do
                return true;
            }

            travelStack.Push(obj);

            //if it appears in the links of a saved object it will get saved
            foreach (var (_, linkingObj) in obj.LinkDestinations)
            {
                if (!layers.Contains(linkingObj.Layer))
                    continue;

                if (TryEvaluateIfObjGetsSaved(linkingObj, travelStack, out bool _result) && _result == true)
                {
                    result = objGetsSaved_cache[obj] = true;
                    return true;
                }
            }

            travelStack.Pop();

            result = objGetsSaved_cache[obj] = false;
            return true;
        }

        public ArrayNode SaveObjectList(ObjectList objList)
        {
            ArrayNode objListNode = nodeWriter.CreateArrayNode();

            void WriteObjNode(DictionaryNode node)
            {
                objListNode.AddDictionaryNodeRef(node);
                objsNode?.AddDictionaryNodeRef(node);
            }

            foreach (I3dWorldObject obj in objList)
            {
                LinkState linkState = EvaluateLinkState(obj);

                if (linkState == LinkState.Linked)
                    continue; //will get saved in the link so no need to save it here


                if (!layers.Contains(obj.Layer))
                    continue;

                if (alreadyWrittenObjs.TryGetValue(obj, out DictionaryNode objNode))
                {
                    WriteObjNode(objNode);
                    continue;
                }


                objNode = nodeWriter.CreateDictionaryNode();
                alreadyWrittenObjs.Add(obj, objNode);
                obj.Save(this, objNode);
                WriteObjNode(objNode);
            }

            return objListNode;
        }

        public void SaveLinks(IEnumerable<KeyValuePair<string, List<I3dWorldObject>>> links, DictionaryNode objNode = null)
        {
            if (links != null)
            {
                DictionaryNode linksNode = nodeWriter.CreateDictionaryNode();

                foreach (var (linkName, link) in links)
                {
                    if (link.Count == 0)
                        continue;

                    ArrayNode linkNode = nodeWriter.CreateArrayNode();

                    foreach (I3dWorldObject obj in link)
                    {
                        if (!layers.Contains(obj.Layer))
                            continue;

                        if (alreadyWrittenObjs.ContainsKey(obj))
                        {
                            linkNode.AddDictionaryNodeRef(alreadyWrittenObjs[obj]);

                            continue;
                        }


                        DictionaryNode linkedObjNode = nodeWriter.CreateDictionaryNode();

                        alreadyWrittenObjs.Add(obj, linkedObjNode);

                        obj.Save(this, linkedObjNode);
                        linkNode.AddDictionaryNodeRef(linkedObjNode);
                    }

                    if (linkNode.Count != 0)
                        linksNode.AddArrayNodeRef(linkName, linkNode, true);
                }
                if (linksNode.Count != 0)
                    objNode.AddDictionaryNodeRef("Links", linksNode, true);
                else
                    objNode.AddDynamicValue("Links", new Dictionary<string, dynamic>(), true);
            }
            else
            {
                objNode.AddDynamicValue("Links", new Dictionary<string, dynamic>(), true);
            }
        }

        public ArrayNode CreateArrayNode() => nodeWriter.CreateArrayNode();
        public DictionaryNode CreateDictionaryNode() => nodeWriter.CreateDictionaryNode();
    }
}
