using BYAML;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpotLight.Level;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;

namespace SpotLight.EditorDrawables
{
    public class SM3DWorldScene : EditorSceneBase
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

            StaticObjects.Add(new LinkRenderer(this));
        }

        public IEnumerable<I3dWorldObject> Objects
        {
            get
            {
                foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
                {
                    IteratedZoneOffset = position;

                    foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                    {
                        foreach (I3dWorldObject obj in objects)
                            yield return obj;
                    }
                    foreach (I3dWorldObject obj in zone.LinkedObjects)
                        yield return obj;
                }
            }
        }

        public Stack<IRevertable> UndoStack
        {
            get => undoStack;
            set => undoStack = value;
        }

        public Stack<IRevertable> RedoStack
        {
            get => redoStack;
            set => redoStack = value;
        }

        public override string ToString()
        {
            return Zones[0].Item2.levelName;
        }

        static bool Initialized = false;

        static ShaderProgram LinksShaderProgram;

        /// <summary>
        /// Prepares to draw models
        /// </summary>
        /// <param name="control">The GL_Control that's currently in use</param>
        public override void Prepare(GL_ControlModern control)
        {
            BfresModelCache.Initialize(control);

            if (!Initialized)
            {
                LinksShaderProgram = new ShaderProgram(
                    new FragmentShader(
              @"#version 330
                in vec4 fragColor;
                void main(){
                    gl_FragColor = fragColor;
                }"),
                    new VertexShader(
              @"#version 330
                layout(location = 0) in vec4 position;
                layout(location = 1) in vec4 color;

                out vec4 fragColor;

                uniform mat4 mtxMdl;
                uniform mat4 mtxCam;
                void main(){
                    gl_Position = mtxCam*mtxMdl*position;
                    fragColor = color;
                }"), control);

                Initialized = true;
            }

            base.Prepare(control);
        }

        class LinkRenderer : AbstractGlDrawable
        {
            readonly SM3DWorldScene scene;

            public LinkRenderer(SM3DWorldScene scene)
            {
                this.scene = scene;
            }

            public override void Prepare(GL_ControlModern control)
            {
                if (!Initialized)
                {
                    LinksShaderProgram = new ShaderProgram(
                        new FragmentShader(
                  @"#version 330
                in vec4 fragColor;
                void main(){
                    gl_FragColor = fragColor;
                }"),
                        new VertexShader(
                  @"#version 330
                layout(location = 0) in vec4 position;
                layout(location = 1) in vec4 color;

                out vec4 fragColor;

                uniform mat4 mtxMdl;
                uniform mat4 mtxCam;
                void main(){
                    gl_Position = mtxCam*mtxMdl*position;
                    fragColor = color;
                }"), control);

                    Initialized = true;
                }
            }

            public override void Draw(GL_ControlModern control, Pass pass)
            {
                if (pass == Pass.OPAQUE)
                {
                    control.ResetModelMatrix();

                    control.CurrentShader = LinksShaderProgram;

                    GL.Begin(PrimitiveType.Lines);
                    foreach (I3dWorldObject _obj in scene.GetObjects())
                    {
                        if (_obj.Links != null)
                        {
                            foreach (KeyValuePair<string, List<I3dWorldObject>> link in _obj.Links)
                            {
                                foreach (I3dWorldObject obj in link.Value)
                                {
                                    if (_obj.IsSelected() || obj.IsSelected())
                                    {
                                        GL.VertexAttrib4(1, new OpenTK.Vector4(1, 1, 1, 1));
                                        GL.Vertex3(_obj.GetLinkingPoint());
                                        GL.VertexAttrib4(1, new OpenTK.Vector4(0, 1, 1, 1));
                                        GL.Vertex3(obj.GetLinkingPoint());
                                    }
                                }
                            }
                        }
                    }
                    GL.End();
                }
            }

            public override int GetPickableSpan()
            {
                return 0;
            }

            public override void Prepare(GL_ControlLegacy control)
            {
                throw new NotImplementedException();
            }

            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                throw new NotImplementedException();
            }
        }

        public I3dWorldObject Hovered3dObject { get; protected set; } = null;

        public override uint MouseEnter(int inObjectIndex, GL_ControlBase control)
        {
            uint var = base.MouseEnter(inObjectIndex, control);

            if (Hovered is I3dWorldObject)
                Hovered3dObject = (I3dWorldObject)Hovered;

            return var;
        }

        public List<(Vector3, SM3DWorldZone)> Zones { get; set; }

        public static bool IteratesThroughLinks { get; protected set; }

        public static Vector3 IteratedZoneOffset { get; protected set; }

        /// <summary>
        /// Gets all the editable objects
        /// </summary>
        /// <returns><see cref="IEnumerable{IEditableObject}"/></returns>
        protected override IEnumerable<IEditableObject> GetObjects()
        {
            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (IEditableObject obj in objects)
                        yield return obj;
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    yield return obj;
            }
        }

        public void UpdateLinkDestinations()
        {
            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.ClearLinkDestinations();
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    obj.ClearLinkDestinations();
            }

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.AddLinkDestinations();
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    obj.AddLinkDestinations();
            }
        }

        /// <summary>
        /// Deletes the selected object from the level
        /// </summary>
        public override void DeleteSelected()
        {
            DeletionManager manager = new DeletionManager();

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                foreach (List<I3dWorldObject> objList in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objList)
                    {
                        obj.DeleteSelected(this, manager, objList);
                    }
                }

                foreach (I3dWorldObject obj in zone.LinkedObjects)
                {
                    obj.DeleteSelected(this, manager, zone.LinkedObjects);
                }
            }


            List<Revertable3DWorldObjAddition.ObjListInfo> objsToDelete = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            List<IEditableObject> objects;

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                foreach (List<I3dWorldObject> objList in zone.ObjLists.Values)
                {
                    if (manager.Dictionary.TryGetValue(objList, out objects))
                    {
                        manager.Dictionary.Remove(objList);
                        List<I3dWorldObject> _objsToDelete = new List<I3dWorldObject>();

                        foreach (I3dWorldObject obj in objects)
                        {
                            _objsToDelete.Add(obj);
                        }
                        objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(objList, _objsToDelete.ToArray()));
                    }
                }

                List<I3dWorldObject> linkedObjsToDelete = new List<I3dWorldObject>();

                if (manager.Dictionary.TryGetValue(zone.LinkedObjects, out objects))
                {
                    manager.Dictionary.Remove(zone.LinkedObjects);
                    foreach (I3dWorldObject obj in objects)
                    {
                        linkedObjsToDelete.Add(obj);
                    }

                    objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(zone.LinkedObjects, linkedObjsToDelete.ToArray()));
                }
            }

            BeginUndoCollection();
            //A little hack: Delete objects by reverting their creation
            AddToUndo(new Revertable3DWorldObjAddition(objsToDelete.ToArray()).Revert(this));

            _ExecuteDeletion(manager);
            EndUndoCollection();

            UpdateLinkDestinations();
        }

        public void DuplicateSelectedObjects()
        {
            //Duplicate Selected Objects
            List<Revertable3DWorldObjAddition.ObjListInfo> objListInfos = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            Dictionary<I3dWorldObject, I3dWorldObject> totalDuplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();
            List<I3dWorldObject> newLinkedObjects = new List<I3dWorldObject>();

            Dictionary<I3dWorldObject, I3dWorldObject> duplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.DuplicateSelected(duplicates, this, zone);

                    objects.AddRange(duplicates.Values);

                    foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

                    if (duplicates.Count > 0)
                        objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(objects, duplicates.Values.ToArray()));

                    duplicates.Clear();
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    obj.DuplicateSelected(duplicates, this, zone);

                foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

                if (duplicates.Count > 0)
                    objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(zone.LinkedObjects, duplicates.Values.ToArray()));

                zone.LinkedObjects.AddRange(duplicates.Values);
            }

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                IteratedZoneOffset = position;

                //Clear LinkDestinations
                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.ClearLinkDestinations();
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    obj.ClearLinkDestinations();

                //Rebuild links
                DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

                IteratesThroughLinks = false;
                foreach (List<I3dWorldObject> objects in zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo);
                }
                IteratesThroughLinks = true;
                foreach (I3dWorldObject obj in zone.LinkedObjects)
                    obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo);
            }

            //Add to undo
            if (objListInfos.Count > 0)
                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));

            control.Refresh();
            control.Repick();
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

        #region Link Connection Editing

        public struct RevertableConnectionAddition : IRevertable
        {
            I3dWorldObject source;
            I3dWorldObject dest;
            string name;

            public RevertableConnectionAddition(I3dWorldObject source, I3dWorldObject dest, string name)
            {
                this.source = source;
                this.dest = dest;
                this.name = name;
            }

            public IRevertable Revert(EditorSceneBase scene)
            {
                SM3DWorldScene s = (SM3DWorldScene)scene;
                s.RemoveConnection(source, dest, name);
                s.UpdateLinkDestinations();
                return new RevertableConnectionDeletion(source, dest, name);
            }
        }

        public struct RevertableConnectionDeletion : IRevertable
        {
            I3dWorldObject source;
            I3dWorldObject dest;
            string name;

            public RevertableConnectionDeletion(I3dWorldObject source, I3dWorldObject dest, string name)
            {
                this.source = source;
                this.dest = dest;
                this.name = name;
            }

            public IRevertable Revert(EditorSceneBase scene)
            {
                SM3DWorldScene s = (SM3DWorldScene)scene;
                s.AddConnection(source, dest, name);
                s.UpdateLinkDestinations();
                return new RevertableConnectionAddition(source, dest, name);
            }
        }

        public struct RevertableConnectionChange : IRevertable
        {
            I3dWorldObject source;
            I3dWorldObject dest;
            string name;

            I3dWorldObject prevSource;
            I3dWorldObject prevDest;
            string prevName;

            public RevertableConnectionChange(I3dWorldObject source, I3dWorldObject dest, string name, 
                I3dWorldObject prevSource, I3dWorldObject prevDest, string prevName)
            {
                this.source = source;
                this.dest = dest;
                this.name = name;
                this.prevSource = prevSource;
                this.prevDest = prevDest;
                this.prevName = prevName;
            }

            public IRevertable Revert(EditorSceneBase scene)
            {
                SM3DWorldScene s = (SM3DWorldScene)scene;
                s.RemoveConnection(source, dest, name);
                s.AddConnection(prevSource, prevDest, prevName);
                s.UpdateLinkDestinations();
                return new RevertableConnectionChange(
                    prevSource, prevDest, prevName,
                    source, dest, name);
            }
        }

        public void RemoveConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            source.Links[name].Remove(dest);

            if (source.Links[name].Count == 0)
                source.Links.Remove(name);

            if (source.Links.Count == 0)
                source.Links = null;

            if (this is LinkEdit3DWScene les)
            {
                if (les.SelectedConnection?.Source == source && les.SelectedConnection?.Dest == dest && les.SelectedConnection?.Name == name)
                    les.SelectedConnection = null;

                control.Refresh();
            }
        }

        public void AddConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            if (source.Links == null)
                source.Links = new Dictionary<string, List<I3dWorldObject>>();

            if (!source.Links.ContainsKey(name))
                source.Links.Add(name, new List<I3dWorldObject>());

            source.Links[name].Add(dest);
        }

        #endregion

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save()
        {
            IteratedZoneOffset = Vector3.Zero;

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                zone.Save();
            }

            return true;
        }

        /// <summary>
        /// Saves the level to a new file (.szs)
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed or was cancelled</returns>
        public bool SaveAs()
        {
            IteratedZoneOffset = Vector3.Zero;

            foreach ((Vector3 position, SM3DWorldZone zone) in Zones)
            {
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "3DW Levels|*.szs", InitialDirectory = Program.StageDataPath };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    zone.Save(sfd.FileName);
                }
            }

            return true;
        }
    }
}
