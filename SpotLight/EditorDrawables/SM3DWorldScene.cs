using BYAML;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SZS;
using WinInput = System.Windows.Input;

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
                public ObjectList objList;
                public DeleteInfo[] deleteInfos;

                public ObjListInfo(ObjectList objList, DeleteInfo[] deleteInfos)
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
                        objListInfo.objList.Insert(info.index, info.obj);
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
                public ObjectList objList;
                public I3dWorldObject[] objects;

                public ObjListInfo(ObjectList objList, I3dWorldObject[] objects)
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
        public SM3DWorldScene(SM3DWorldZone zone)
        {
            mainZone = zone;

            multiSelect = true;

            StaticObjects.Add(new LinkRenderer(this));

            StaticObjects.Add(new ZonePlacementRenderer(this));
        }

        public SM3DWorldScene()
        {
            multiSelect = true;

            StaticObjects.Add(new LinkRenderer(this));

            StaticObjects.Add(new ZonePlacementRenderer(this));
        }

        private CameraStateSave?[] camStateSaves = new CameraStateSave?[10];

        protected void SaveCam(int index)
        {
            camStateSaves[index] = new CameraStateSave(control);
        }

        protected void LoadCam(int index)
        {
            camStateSaves[index]?.ApplyTo(control);
        }

        public override uint KeyDown(KeyEventArgs e, GL_ControlBase control, bool isRepeat)
        {
            int index = e.KeyCode - Keys.D0;

            if (index >= 0 && index <= 9)
            {
                if (e.Modifiers == Keys.Shift)
                    SaveCam(index);
                else if (e.Modifiers == Keys.None)
                    LoadCam(index);
            }

            return base.KeyDown(e, control, isRepeat);
        }

        public event EventHandler ZonePlacementsChanged;

        public event EventHandler ObjectPlaced;

        public T ConvertToOtherSceneType<T>() where T : SM3DWorldScene, new() => new T
        {
            mainZone = mainZone,
            EditZone = EditZone,
            editZoneIndex = editZoneIndex,
            EditZoneTransform = EditZoneTransform,
            ZonePlacements = ZonePlacements,
            undoStack = undoStack,
            redoStack = redoStack,
            IsSaved = IsSaved,
            camStateSaves = camStateSaves
        };

        public delegate (I3dWorldObject obj, ObjectList objList)[] ObjectPlacementHandler(Vector3 position, SM3DWorldZone zone);
        /// <summary>
        /// Returns an Array of I3DWorldObject and a list to place them in
        /// </summary>
        public ObjectPlacementHandler ObjectPlaceDelegate { get; set; }

        public void ResetObjectPlaceDelegate() => ObjectPlaceDelegate = null;

        public override uint MouseClick(MouseEventArgs e, GL_ControlBase control)
        {
            if (ObjectPlaceDelegate != null && e.Button == MouseButtons.Left)
            {
                var placements = ObjectPlaceDelegate.Invoke((new Vector4(-control.CoordFor(e.X, e.Y, control.PickingDepth), 1) * EditZoneTransform.PositionTransform.Inverted()).Xyz, EditZone);

                Dictionary<ObjectList, List<I3dWorldObject>> objsByLists = new Dictionary<ObjectList, List<I3dWorldObject>>();



                for (int i = 0; i < placements.Length; i++)
                {
                    if (!objsByLists.ContainsKey(placements[i].objList))
                        objsByLists[placements[i].objList] = new List<I3dWorldObject>();

                    objsByLists[placements[i].objList].Add(placements[i].obj);

                    placements[i].objList.Add(placements[i].obj);

                }

                List<Revertable3DWorldObjAddition.ObjListInfo> objListInfos = new List<Revertable3DWorldObjAddition.ObjListInfo>(objsByLists.Count);

                foreach (var item in objsByLists)
                {
                    objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(item.Key, item.Value.ToArray()));
                }

                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));

                if (!WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    ObjectPlaceDelegate = null;

                ObjectPlaced?.Invoke(null, null);

                return REDRAW_PICKING;
            }
            else
                return base.MouseClick(e, control);
        }

        public IEnumerable<I3dWorldObject> Objects => Get3DWObjects();

        public IReadOnlyCollection<IRevertable> UndoStack => undoStack;

        public IReadOnlyCollection<RedoEntry> RedoStack => redoStack;

        public override string ToString()
        {
            return mainZone.LevelName;
        }

        
        class LinkRenderer : AbstractGlDrawable
        {
            static bool Initialized = false;

            static ShaderProgram LinksShaderProgram;

            readonly SM3DWorldScene scene;

            public LinkRenderer(SM3DWorldScene scene)
            {
                this.scene = scene;
                
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
                }"));

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
                    foreach (I3dWorldObject _obj in scene.Get3DWObjects())
                    {
                        if (_obj.Links != null)
                        {
                            foreach (KeyValuePair<string, List<I3dWorldObject>> link in _obj.Links)
                            {
                                foreach (I3dWorldObject obj in link.Value)
                                {
                                    if (_obj.IsSelected() || obj.IsSelected())
                                    {
                                        GL.VertexAttrib4(1, new Vector4(1, 1, 1, 1));
                                        GL.Vertex3(_obj.GetLinkingPoint(scene));
                                        GL.VertexAttrib4(1, new Vector4(0, 1, 1, 1));
                                        GL.Vertex3(obj.GetLinkingPoint(scene));
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

            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                throw new NotImplementedException();
            }
        }

        class ZonePlacementRenderer : AbstractGlDrawable
        {
            readonly SM3DWorldScene scene;

            public ZonePlacementRenderer(SM3DWorldScene scene)
            {
                this.scene = scene;
            }

            public override void Draw(GL_ControlModern control, Pass pass)
            {
                if (scene.editZoneIndex != 0) //zonePlacements can't be edited
                    foreach (var zonePlacement in scene.ZonePlacements)
                        zonePlacement.Draw(control, pass, scene);
            }

            public override int GetPickableSpan()
            {
                return 0;
            }

            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                throw new NotImplementedException();
            }
        }

        public I3dWorldObject Hovered3dObject { get; protected set; } = null;

        public List<ZonePlacement> ZonePlacements { get; private set; } = new List<ZonePlacement>();

        public override uint MouseEnter(int inObjectIndex, GL_ControlBase control)
        {
            uint var = base.MouseEnter(inObjectIndex, control);

            if (Hovered is I3dWorldObject)
                Hovered3dObject = (I3dWorldObject)Hovered;
            else
                Hovered3dObject = null;

            return var;
        }

        public override uint MouseLeaveEntirely(GL_ControlBase control)
        {
            uint var = base.MouseLeaveEntirely(control);

            Hovered3dObject = null;

            return var;
        }

        public SM3DWorldZone EditZone { get; private set; }

        private int editZoneIndex = -1;
        public int EditZoneIndex
        {
            get => editZoneIndex;
            set
            {
                if (editZoneIndex == value)
                    return;

                if (value == 0)
                {
                    EditZone = mainZone;

                    EditZoneTransform = ZoneTransform.Identity;

                    ZonePlacements = mainZone.ZonePlacements;
                }
                else
                {
                    foreach (var placement in ZonePlacements)
                        placement.DeselectAll(control);

                    ZonePlacements = new List<ZonePlacement>();

                    EditZone = mainZone.ZonePlacements[value - 1].Zone;

                    EditZoneTransform = mainZone.ZonePlacements[value - 1].GetTransform();

                    for (int i = 0; i < mainZone.ZonePlacements.Count; i++)
                    {
                        if (i == value - 1)
                            continue;

                        ZonePlacement zonePlacement = mainZone.ZonePlacements[i];
                        ZonePlacements.Add(zonePlacement);
                    }

                    ZonePlacements.Add(new ZonePlacement(Vector3.Zero, Vector3.Zero, Vector3.One, mainZone));
                }

                SceneDrawState.ZoneTransform = EditZoneTransform;

                undoStack = EditZone.undoStack;
                redoStack = EditZone.redoStack;
                LastSavedUndo = EditZone.LastSavedUndo;

                editZoneIndex = value;
            }
        }

        protected ZoneTransform EditZoneTransform { get; private set; }

        private SM3DWorldZone mainZone;

        public IEnumerable<SM3DWorldZone> GetZones()
        {
            yield return mainZone;

            foreach (ZonePlacement zonePlacement in mainZone.ZonePlacements)
            {
                yield return zonePlacement.Zone;
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        public override bool IsSaved
        {
            get => GetZones().All(x=>x.IsSaved);
            protected set
            {
                EditZone.IsSaved = value;

                base.IsSaved = value;
            }
        }

        /// <summary>
        /// Gets all the editable objects
        /// </summary>
        /// <returns><see cref="IEnumerable{IEditableObject}"/></returns>
        protected override IEnumerable<IEditableObject> GetObjects()
        {
            foreach (var obj in Get3DWObjects())
                yield return obj;

            if(editZoneIndex==0) //zonePlacements can be edited
                foreach (var zonePlacement in ZonePlacements)
                    yield return zonePlacement;
        }

        protected IEnumerable<I3dWorldObject> Get3DWObjects()
        {
            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    yield return obj;
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                yield return obj;
        }

        public void UpdateLinkDestinations()
        {
            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.ClearLinkDestinations();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.ClearLinkDestinations();

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.AddLinkDestinations();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.AddLinkDestinations();
        }

        /// <summary>
        /// Deletes the selected objects from the level
        /// </summary>
        public override void DeleteSelected()
        {
            DeletionManager manager = new DeletionManager();

            foreach (ObjectList objList in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objList)
                {
                    obj.DeleteSelected(this, manager, objList);
                }
            }

            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
            {
                obj.DeleteSelected(this, manager, EditZone.LinkedObjects);
            }

            if (editZoneIndex == 0) //editing main zone
            {
                foreach (ZonePlacement placement in ZonePlacements)
                {
                    placement.DeleteSelected(this, manager, ZonePlacements);
                }
            }

            if (manager.Dictionary.Count == 0)
                return;

            List<Revertable3DWorldObjAddition.ObjListInfo> objsToDelete = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            List<IEditableObject> objects;

            foreach (ObjectList objList in EditZone.ObjLists.Values)
            {
                if (manager.Dictionary.TryGetValue(objList, out objects))
                {
                    manager.Dictionary.Remove(objList);
                    List<I3dWorldObject> _objsToDelete = new List<I3dWorldObject>();

                    foreach (I3dWorldObject obj in objects)
                    {
                        _objsToDelete.Add(obj);
                    }

                    if(_objsToDelete.Count>0)
                        objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(objList, _objsToDelete.ToArray()));
                }
            }

            List<I3dWorldObject> linkedObjsToDelete = new List<I3dWorldObject>();

            if (manager.Dictionary.TryGetValue(EditZone.LinkedObjects, out objects))
            {
                manager.Dictionary.Remove(EditZone.LinkedObjects);
                foreach (I3dWorldObject obj in objects)
                {
                    linkedObjsToDelete.Add(obj);
                }

                if (linkedObjsToDelete.Count > 0)
                    objsToDelete.Add(new Revertable3DWorldObjAddition.ObjListInfo(EditZone.LinkedObjects, linkedObjsToDelete.ToArray()));
            }

            BeginUndoCollection();
            if (objsToDelete.Count > 0)
                //A little hack: Delete objects by reverting their creation
                AddToUndo(new Revertable3DWorldObjAddition(objsToDelete.ToArray()).Revert(this));

            ExecuteDeletion(manager);
            if (manager.Dictionary.ContainsKey(ZonePlacements))
                ZonePlacementsChanged?.Invoke(this, null);

            EndUndoCollection();

            UpdateLinkDestinations();
        }

        /// <summary>
        /// Duplicates the selected objects and adds links if necessary
        /// </summary>
        public void DuplicateSelectedObjects()
        {
            List<ZonePlacement> newPlacements = new List<ZonePlacement>();
            if (editZoneIndex == 0) //editing main zone
            {
                foreach (ZonePlacement placement in ZonePlacements)
                {
                    if (placement.IsSelectedAll())
                    {
                        placement.DeselectAll(control);
                        newPlacements.Add(new ZonePlacement(placement.Position, placement.Rotation, placement.Scale, placement.Zone));
                    }
                }
                //the rest will be handled at the end of the function
            }

            //Duplicate Selected Objects
            List<Revertable3DWorldObjAddition.ObjListInfo> objListInfos = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            Dictionary<I3dWorldObject, I3dWorldObject> totalDuplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

            Dictionary<I3dWorldObject, I3dWorldObject> duplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objList in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objList)
                    obj.DuplicateSelected(duplicates, this);

                objList.AddRange(duplicates.Values);

                foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

                if (duplicates.Count > 0)
                    objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(objList, duplicates.Values.ToArray()));

                duplicates.Clear();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.DuplicateSelected(duplicates, this);

            foreach (var keyValuePair in duplicates) totalDuplicates.Add(keyValuePair.Key, keyValuePair.Value);

            if (duplicates.Count > 0)
                objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(EditZone.LinkedObjects, duplicates.Values.ToArray()));

            EditZone.LinkedObjects.AddRange(duplicates.Values);

            //Clear LinkDestinations
            SceneObjectIterState.InLinks = false;
            foreach (List<I3dWorldObject> objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.ClearLinkDestinations();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.ClearLinkDestinations();

            //Rebuild links
            DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo, true);
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo, true);

            BeginUndoCollection();
            //Add to undo
            if (objListInfos.Count > 0)
                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));

            if (newPlacements.Count > 0)
            {
                foreach (var placement in newPlacements)
                {
                    placement.SelectDefault(control);
                    ZonePlacements.Add(placement);
                }
                ZonePlacementsChanged?.Invoke(this, null);

                RevertableAddition.AddInListInfo addInListInfo = new RevertableAddition.AddInListInfo(newPlacements.ToArray(), ZonePlacements);

                AddToUndo(new RevertableAddition(new RevertableAddition.AddInListInfo[] { addInListInfo }, Array.Empty<RevertableAddition.SingleAddInListInfo>()));
            }

            EndUndoCollection();
            control.Refresh();
            control.Repick();
        }

        static List<ZonePlacement> copiedZonePlacements = null;

        static List<(string listName, Dictionary<I3dWorldObject, I3dWorldObject> copies)> copiedObjects = null;

        static SM3DWorldZone copySrcZone = null;

        static ZoneTransform copySrcZoneTransform;

        /// <summary>
        /// Copies the selected objects for pasting
        /// </summary>
        public void CopySelectedObjects()
        {
            copiedZonePlacements = new List<ZonePlacement>();
            if (editZoneIndex == 0) //editing main zone
            {
                foreach (ZonePlacement placement in ZonePlacements)
                {
                    if (placement.IsSelectedAll())
                    {
                        copiedZonePlacements.Add(new ZonePlacement(placement.Position, placement.Rotation, placement.Scale, placement.Zone));
                    }
                }
                //the rest will be handled at the end of the function
            }

            //Duplicate Selected Objects
            copiedObjects = new List<(string listName, Dictionary<I3dWorldObject, I3dWorldObject> copies)>();

            SceneObjectIterState.InLinks = false;
            foreach (KeyValuePair<string, ObjectList> keyValuePair in EditZone.ObjLists)
            {
                Dictionary<I3dWorldObject, I3dWorldObject> copies = new Dictionary<I3dWorldObject, I3dWorldObject>();

                foreach (I3dWorldObject obj in keyValuePair.Value)
                    obj.DuplicateSelected(copies, this, null, false);

                if(copies.Count>0)
                    copiedObjects.Add((keyValuePair.Key, copies));
            }
            SceneObjectIterState.InLinks = true;

            {
                Dictionary<I3dWorldObject, I3dWorldObject> copies = new Dictionary<I3dWorldObject, I3dWorldObject>();

                foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                    obj.DuplicateSelected(copies, this);

                if (copies.Count > 0)
                    copiedObjects.Add(("Links", copies));
            }

            if(copiedZonePlacements.Count > 0 || copiedObjects.Count > 0)
            {
                copySrcZone = EditZone;

                copySrcZoneTransform = EditZoneTransform;
            }
            else
            {
                copiedZonePlacements = null;
                copiedObjects = null;
            }
        }

        /// <summary>
        /// Pastes the copied objects and adds links if necessary
        /// </summary>
        public void PasteCopiedObjects()
        {
            //Duplicate Selected Objects
            List<Revertable3DWorldObjAddition.ObjListInfo> objListInfos = new List<Revertable3DWorldObjAddition.ObjListInfo>();

            Dictionary<I3dWorldObject, I3dWorldObject> totalDuplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

            if (copiedObjects != null)
            {
                bool isSameZone = EditZone == copySrcZone;

                ZoneTransform? zoneToZoneTransform = null;

                if (!isSameZone)
                {
                    zoneToZoneTransform = new ZoneTransform(
                        EditZoneTransform.PositionTransform.Inverted() * copySrcZoneTransform.PositionTransform,
                        EditZoneTransform.RotationTransform.Inverted() * copySrcZoneTransform.RotationTransform
                        );

                }

                SelectedObjects.Clear();

                foreach ((string listName, Dictionary<I3dWorldObject, I3dWorldObject> copies) in copiedObjects)
                {
                    if (!EditZone.ObjLists.TryGetValue(listName, out ObjectList list))
                    {
                        list = EditZone.LinkedObjects;
                    }

                    Dictionary<I3dWorldObject, I3dWorldObject> duplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

                    foreach (I3dWorldObject obj in copies.Values)
                        obj.DuplicateSelected(duplicates, this, zoneToZoneTransform, false); //yep we need to duplicate the copies

                    list.AddRange(duplicates.Values);

                    foreach (var keyValuePair in copies) totalDuplicates.Add(keyValuePair.Key, duplicates[keyValuePair.Value]);

                    if (duplicates.Count > 0)
                        objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(list, duplicates.Values.ToArray()));
                }

                //Clear LinkDestinations
                SceneObjectIterState.InLinks = false;
                foreach (List<I3dWorldObject> objects in EditZone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.ClearLinkDestinations();
                }
                SceneObjectIterState.InLinks = true;
                foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                    obj.ClearLinkDestinations();

                //Rebuild links
                DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

                SceneObjectIterState.InLinks = false;
                foreach (ObjectList objects in EditZone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo, isSameZone);
                }
                SceneObjectIterState.InLinks = true;
                foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                    obj.LinkDuplicatesAndAddLinkDestinations(duplicationInfo, isSameZone);
            }

            BeginUndoCollection();
            //Add to undo
            if (objListInfos.Count > 0)
                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));

            List<ZonePlacement> totalDuplicatesOfZonePlacements = new List<ZonePlacement>();

            if (editZoneIndex==0 && copiedZonePlacements?.Count > 0)
            {
                foreach (var placement in copiedZonePlacements)
                {
                    var newPlacement = new ZonePlacement(placement.Position, placement.Rotation, placement.Scale, placement.Zone);

                    newPlacement.SelectDefault(control);
                    ZonePlacements.Add(newPlacement);

                    totalDuplicatesOfZonePlacements.Add(newPlacement);
                }
                ZonePlacementsChanged?.Invoke(this, null);

                RevertableAddition.AddInListInfo addInListInfo = new RevertableAddition.AddInListInfo(totalDuplicatesOfZonePlacements.ToArray(), ZonePlacements);

                AddToUndo(new RevertableAddition(new RevertableAddition.AddInListInfo[] { addInListInfo }, Array.Empty<RevertableAddition.SingleAddInListInfo>()));
            }

            EndUndoCollection();
            control.Refresh();
            control.Repick();
        }

        public void GrowSelection()
        {
            List<I3dWorldObject> objectsToSelect = new List<I3dWorldObject>();

            foreach (var obj in Get3DWObjects())
            {
                if (obj.Links != null && obj.IsSelected())
                {
                    foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in obj.Links)
                    {
                        foreach (I3dWorldObject _obj in keyValuePair.Value)
                        {
                            objectsToSelect.Add(_obj);
                        }
                    }
                }
            }

            foreach (var obj in objectsToSelect)
            {
                obj.SelectDefault(control);
            }

            control.Refresh();
        }

        public void SelectAllLinked()
        {
            List<I3dWorldObject> objectsToSelect = new List<I3dWorldObject>();

            bool done = false;
            while (!done)
            {
                done = true;

                foreach (var obj in Get3DWObjects())
                {
                    if (obj.IsSelected())
                    {
                        foreach ((string name, I3dWorldObject _obj) in obj.LinkDestinations)
                        {
                            if (!_obj.IsSelected())
                            {
                                _obj.SelectDefault(control);
                                done = false;
                            }
                        }
                    }
                }
            }

            done = false;
            while (!done)
            {
                done = true;

                foreach (var obj in Get3DWObjects())
                {
                    if (obj.IsSelected())
                    {
                        if (obj.Links != null)
                        {
                            foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in obj.Links)
                            {
                                foreach (I3dWorldObject _obj in keyValuePair.Value)
                                {
                                    if (!_obj.IsSelected())
                                    {
                                        _obj.SelectDefault(control);
                                        done = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            control.Refresh();
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
            readonly I3dWorldObject source;
            readonly I3dWorldObject dest;
            readonly string name;

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
            readonly I3dWorldObject source;
            readonly I3dWorldObject dest;
            readonly string name;

            public RevertableConnectionDeletion(I3dWorldObject source, I3dWorldObject dest, string name)
            {
                this.source = source;
                this.dest = dest;
                this.name = name;
            }

            public IRevertable Revert(EditorSceneBase scene)
            {
                SM3DWorldScene s = (SM3DWorldScene)scene;
                s.TryAddConnection(source, dest, name);
                s.UpdateLinkDestinations();
                return new RevertableConnectionAddition(source, dest, name);
            }
        }

        public struct RevertableConnectionChange : IRevertable
        {
            readonly I3dWorldObject source;
            readonly I3dWorldObject dest;
            readonly string name;
            readonly I3dWorldObject prevSource;
            readonly I3dWorldObject prevDest;
            readonly string prevName;

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
                s.TryAddConnection(prevSource, prevDest, prevName);
                s.UpdateLinkDestinations();
                return new RevertableConnectionChange(
                    prevSource, prevDest, prevName,
                    source, dest, name);
            }
        }

        public virtual void RemoveConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            source.Links[name].Remove(dest);
        }

        public virtual bool TryAddConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            if (source.Links == null)
                source.Links = new Dictionary<string, List<I3dWorldObject>>();

            if (source.Links == null) //object doesn't support links
                return false;

            if (!source.Links.ContainsKey(name))
                source.Links.Add(name, new List<I3dWorldObject>());

            source.Links[name].Add(dest);

            return true;
        }

        #endregion

        /// <summary>
        /// Saves the level over the original file
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed</returns>
        public bool Save()
        {
            EditZone.Save();

            foreach (var zonePlacement in ZonePlacements)
            {
                zonePlacement.Zone.Save();
            }

            return IsSaved = IsSaved; //seems dumb but it's the only way to make sure the IsSavedChanged event is triggered
        }

        //[DllImport("User32")]
        //public static extern int SetDlgItemText(IntPtr hwnd, int id, string title);

        //public const int FileTitleCntrlID = 0x47c;

        //void SetFileName(IntPtr hdlg, string name)
        //{
        //    SetDlgItemText(hdlg, FileTitleCntrlID, name);
        //}

        /// <summary>
        /// Saves the level to a new file (.szs)
        /// </summary>
        /// <returns>true if the save succeeded, false if it failed or was cancelled</returns>
        public bool SaveAs()
        {
            string currentDirectory = EditZone.Directory;

            HashSet<SM3DWorldZone> zonesToSave = new HashSet<SM3DWorldZone> { EditZone };

            foreach (var zonePlacement in ZonePlacements)
            {
                if (!zonesToSave.Contains(zonePlacement.Zone))
                    zonesToSave.Add(zonePlacement.Zone);
            }

            foreach (SM3DWorldZone _zone in zonesToSave)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter =
                    "Level Files (Map)|*Map1.szs|" +
                    "Level Files (Design)|*Design1.szs|" +
                    "Level Files (Sound)|*Sound1.szs|" +
                    "All Level Files|*.szs",
                    InitialDirectory = currentDirectory,
                    FileName = _zone.LevelFileName
                };

                sfd.FileOk += (s, e) =>
                {
                    if (!_zone.IsValidSaveName(sfd.FileName))
                    {
                        //Type type = typeof(FileDialog);
                        //FieldInfo info = type.GetField("dialogHWnd", BindingFlags.NonPublic
                        //                                           | BindingFlags.Instance);
                        //IntPtr fileDialogHandle = (IntPtr)info.GetValue(sfd);

                        //SetFileName(fileDialogHandle, zone.GetProperSaveName(sfd.FileName));

                        e.Cancel = true;
                    }
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _zone.Save(sfd.FileName);
                    currentDirectory = System.IO.Path.GetDirectoryName(sfd.FileName);
                }
            }

            return IsSaved = IsSaved; //seems dumb but it's the only way to make sure the IsSavedChanged event is triggered
        }

        public void CheckLocalFiles()
        {
            foreach (var zone in GetZones())
            {
                zone.CheckLocalFiles();
            }
        }

        public void FocusOn(IEditableObject obj)
        {
            if (obj is I3dWorldObject)
            {
                foreach (IEditableObject _obj in GetObjects())
                {
                    if (_obj == obj)
                    {
                        control.CameraTarget = _obj.GetFocusPoint();
                        return;
                    }
                }
            }
            else
                control.CameraTarget = obj.GetFocusPoint();
        }
    }
}
