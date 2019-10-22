using BYAML;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;

namespace SpotLight.EditorDrawables
{
    class SM3DWorldScene : EditorSceneBase
    {
        public struct Revertable3DWorldObjDeletion : IRevertable
        {
            public struct DeleteInfo
            {
                public int index;
                public I3dWorldObject obj;

                public DeleteInfo(int index, I3dWorldObject obj)
                {
                    this.index = index;
                    this.obj = obj;
                }
            }

            public struct ObjListInfo
            {
                public List<I3dWorldObject> objList;
                public DeleteInfo[] deleteInfos;

                public ObjListInfo(List<I3dWorldObject> objList, DeleteInfo[] deleteInfos)
                {
                    this.objList = objList;
                    this.deleteInfos = deleteInfos;
                }
            }

            ObjListInfo[] objListInfos;
            int[] linkIndices;

            public Revertable3DWorldObjDeletion(ObjListInfo[] objListInfos, int[] linkIndices)
            {
                this.objListInfos = objListInfos;
                this.linkIndices = linkIndices;
            }

            public IRevertable Revert(EditorSceneBase scene) //Insert deleted objs back in
            {
                Revertable3DWorldObjAddition.ObjListInfo[] newObjListInfos = new Revertable3DWorldObjAddition.ObjListInfo[objListInfos.Length];
                int i_newObjListInfos = 0;

                int i_index = linkIndices.Length;

                foreach (ObjListInfo objListInfo in objListInfos.Reverse())
                {
                    newObjListInfos[i_newObjListInfos].objList = objListInfo.objList;

                    I3dWorldObject[] newObjs = newObjListInfos[i_newObjListInfos++].objects = new I3dWorldObject[objListInfo.deleteInfos.Length];
                    int i_newObjs = 0;
                    foreach (DeleteInfo info in objListInfo.deleteInfos.Reverse())
                    {
                        //Insert obj into the list
                        objListInfo.objList.Insert(info.index,info.obj);
                        newObjs[i_newObjs++] = info.obj;

                        //Insert obj into all links linking to it
                        for (int i = info.obj.LinkDestinations.Count - 1; i >= 0; i--)
                        {
                            (string, I3dWorldObject) dest = info.obj.LinkDestinations[i];
                            dest.Item2.Links[dest.Item1].Insert(linkIndices[--i_index], info.obj);
                        }
                    }
                }

                (scene as SM3DWorldScene)?.UpdateLinkDestinations();

                return new Revertable3DWorldObjAddition(newObjListInfos);
            }
        }

        public struct Revertable3DWorldObjAddition : IRevertable
        {
            public struct ObjListInfo
            {
                public List<I3dWorldObject> objList;
                public I3dWorldObject[] objects;

                public ObjListInfo(List<I3dWorldObject> objList, I3dWorldObject[] objects)
                {
                    this.objList = objList;
                    this.objects = objects;
                }
            }

            ObjListInfo[] objListInfos;

            public Revertable3DWorldObjAddition(ObjListInfo[] objListInfos)
            {
                this.objListInfos = objListInfos;
            }

            public IRevertable Revert(EditorSceneBase scene)
            {
                Revertable3DWorldObjDeletion.ObjListInfo[] newObjListInfos = new Revertable3DWorldObjDeletion.ObjListInfo[objListInfos.Length];
                int i_newObjListInfos = 0;

                List<int> newLinkIndices = new List<int>();

                foreach (ObjListInfo objListInfo in objListInfos)
                {
                    newObjListInfos[i_newObjListInfos].objList = objListInfo.objList;

                    Revertable3DWorldObjDeletion.DeleteInfo[] newObjs = 
                        newObjListInfos[i_newObjListInfos++].deleteInfos = new Revertable3DWorldObjDeletion.DeleteInfo[objListInfo.objects.Length];
                    int i_newObjs = 0;

                    foreach (I3dWorldObject obj in objListInfo.objects)
                    {
                        //Remove obj from the list
                        newObjs[i_newObjs++] = new Revertable3DWorldObjDeletion.DeleteInfo(objListInfo.objList.IndexOf(obj), obj);
                        objListInfo.objList.Remove(obj);

                        //remove obj from all links linking to it
                        foreach ((string, I3dWorldObject) dest in obj.LinkDestinations)
                        {
                            newLinkIndices.Add(dest.Item2.Links[dest.Item1].IndexOf(obj));
                            dest.Item2.Links[dest.Item1].Remove(obj);
                        }
                    }
                }

                (scene as SM3DWorldScene)?.UpdateLinkDestinations();

                return new Revertable3DWorldObjDeletion(newObjListInfos, newLinkIndices.ToArray());
            }
        }

        /// <summary>
        /// Creates a blank SM3DW Scene
        /// </summary>
        public SM3DWorldScene()
        {
            multiSelect = true;
        }

        /// <summary>
        /// Prepares to draw models
        /// </summary>
        /// <param name="control">The GL_Control that's currently in use</param>
        public override void Prepare(GL_ControlModern control)
        {
            BfresModelCache.Initialize(control);
            
            base.Prepare(control);
        }

        public Dictionary<string, List<I3dWorldObject>> objLists = new Dictionary<string, List<I3dWorldObject>>();

        public List<I3dWorldObject> linkedObjects = new List<I3dWorldObject>();

        private ulong highestObjID = 0;

        public void SubmitID(string id)
        {
            if (id.StartsWith("obj") && ulong.TryParse(id.Substring(3), out ulong objID))
            {
                if (objID > highestObjID)
                    highestObjID = objID;
            }
        }

        public string NextObjID() => "obj" + (++highestObjID);

        public static bool IteratesThroughLinks;

        /// <summary>
        /// Gets all the editable objects
        /// </summary>
        /// <returns><see cref="IEnumerable{IEditableObject}"/></returns>
        protected override IEnumerable<IEditableObject> GetObjects()
        {
            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (IEditableObject obj in objects)
                    yield return obj;
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                yield return obj;
        }

        public void UpdateLinkDestinations()
        {
            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.ClearLinkDestinations();
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                obj.ClearLinkDestinations();

            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.AddLinkDestinations();
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                obj.AddLinkDestinations();
        }

        /// <summary>
        /// Deletes the selected object from the level
        /// </summary>
        public override void DeleteSelected()
        {
            DeletionManager manager = new DeletionManager();

            List<Revertable3DWorldObjAddition.ObjListInfo> objsToDelete = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                List<I3dWorldObject> _objsToDelete = new List<I3dWorldObject>();

                foreach (I3dWorldObject obj in objects)
                {
                    obj.DeleteSelected(manager, objects, CurrentList);
                    obj.DeleteSelected3DWorldObject(_objsToDelete);
                }
                objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(objects, _objsToDelete.ToArray()));
            }

            List<I3dWorldObject> linkedObjsToDelete = new List<I3dWorldObject>();

            foreach (I3dWorldObject obj in linkedObjects)
            {
                obj.DeleteSelected(manager, linkedObjects, CurrentList);
                obj.DeleteSelected3DWorldObject(linkedObjsToDelete);
            }

            objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(linkedObjects, linkedObjsToDelete.ToArray()));

            //A little hack: Delete objects by reverting their creation
            AddToUndo(new Revertable3DWorldObjAddition(objsToDelete.ToArray()).Revert(this));

            _ExecuteDeletion(manager);

            UpdateLinkDestinations();
        }
        /// <summary>
        /// Saves the scene to a BYAML for saving
        /// </summary>
        /// <param name="writer">The current BYAML Node Writer</param>
        /// <param name="levelName">The name of the Level</param>
        /// <param name="categoryName">Category to save in</param>
        public void Save(ByamlNodeWriter writer, string levelName, string categoryName)
        {
            ByamlNodeWriter.DictionaryNode rootNode = writer.CreateDictionaryNode(objLists);

            ByamlNodeWriter.ArrayNode objsNode = writer.CreateArrayNode();

            HashSet<I3dWorldObject> alreadyWrittenObjs = new HashSet<I3dWorldObject>();
            
            rootNode.AddDynamicValue("FilePath", $"D:/home/TokyoProject/RedCarpet/Asset/StageData/{levelName}/Map/{levelName}{categoryName}.muunt");

            foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in objLists)
            {
                ByamlNodeWriter.ArrayNode categoryNode = writer.CreateArrayNode(keyValuePair.Value);

                foreach (I3dWorldObject obj in keyValuePair.Value)
                {
                    if (!alreadyWrittenObjs.Contains(obj))
                    {
                        ByamlNodeWriter.DictionaryNode objNode = writer.CreateDictionaryNode(obj);
                        obj.Save(alreadyWrittenObjs, writer, objNode, false);
                        categoryNode.AddDictionaryNodeRef(objNode);
                        objsNode.AddDictionaryNodeRef(objNode);
                    }
                    else
                    {
                        categoryNode.AddDictionaryRef(obj);
                        objsNode.AddDictionaryRef(obj);
                    }
                }
                rootNode.AddArrayNodeRef(keyValuePair.Key, categoryNode, true);
            }

            rootNode.AddArrayNodeRef("Objs", objsNode);

            writer.Write(rootNode, true);
        }

        public override uint KeyDown(KeyEventArgs e, GL_ControlBase control)
        {
            if(e.Control && e.KeyCode == Keys.D)
            {
                if (SelectedObjects.Count == 0)
                    return 0;
                else
                {
                    DuplicateSelectedObjects();

                    return REDRAW_PICKING;
                }
            }
            else
                return base.KeyDown(e, control);
        }

        public void DuplicateSelectedObjects()
        {
            //Duplicate Selected Objects
            List<Revertable3DWorldObjAddition.ObjListInfo> objListInfos = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            Dictionary<I3dWorldObject, I3dWorldObject> totalDuplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();
            List<I3dWorldObject> newLinkedObjects = new List<I3dWorldObject>();

            Dictionary<I3dWorldObject, I3dWorldObject> duplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.DuplicateSelected(duplicates, this);

                objects.AddRange(duplicates.Values);

                foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

                if (duplicates.Count > 0)
                    objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(objects, duplicates.Values.ToArray()));

                duplicates.Clear();
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                obj.DuplicateSelected(duplicates, this);

            foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

            if (duplicates.Count > 0)
                objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(linkedObjects, duplicates.Values.ToArray()));

            linkedObjects.AddRange(duplicates.Values);


            //Clear LinkDestinations
            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.ClearLinkDestinations();
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                obj.ClearLinkDestinations();


            //Rebuild links
            DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

            IteratesThroughLinks = false;
            foreach (List<I3dWorldObject> objects in objLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo);
            }
            IteratesThroughLinks = true;
            foreach (I3dWorldObject obj in linkedObjects)
                obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo);


            //Add to undo
            if (objListInfos.Count > 0)
                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));
        }

        public class DuplicationInfo
        {
            Dictionary<I3dWorldObject, I3dWorldObject> duplicatedObjects;
            HashSet<I3dWorldObject> duplicates = new HashSet<I3dWorldObject>();

            public DuplicationInfo(Dictionary<I3dWorldObject, I3dWorldObject> duplicates)
            {
                duplicatedObjects = duplicates;

                foreach (I3dWorldObject obj in duplicates.Values)
                    this.duplicates.Add(obj);
            }

            public bool IsDuplicate(I3dWorldObject obj) => duplicates.Contains(obj);

            public bool HasDuplicate(I3dWorldObject obj) => duplicatedObjects.ContainsKey(obj) || duplicates.Contains(obj);

            public bool TryGetDuplicate(I3dWorldObject obj, out I3dWorldObject duplicate) => duplicatedObjects.TryGetValue(obj, out duplicate);
        }
    }
}
