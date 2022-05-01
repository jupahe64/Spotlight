using BYAML;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Spotlight.Level;
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
using GL_EditorFramework;
using Spotlight.GUI;

namespace Spotlight.EditorDrawables
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
            MainZone = zone;

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

        private const string LINKS_LIST_NAME = "Links";
        private CameraStateSave?[] camStateSaves = new CameraStateSave?[10];

        protected void SaveCam(int index)
        {
            camStateSaves[index] = new CameraStateSave(control);
        }

        protected void LoadCam(int index)
        {
            camStateSaves[index]?.ApplyTo(control);
        }


        protected override void OnUndo(IRevertable revertable)
        {

        }

        protected override void OnRedo(RedoEntry redoEntry)
        {
            
        }

        protected override void OnSubmitUndoable(IRevertable revertable)
        {
            
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
        public event EventHandler LayersChanged;

        public event EventHandler ObjectPlaced;

        public T ConvertToOtherSceneType<T>() where T : SM3DWorldScene, new() => new T
        {
            MainZone = MainZone,
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

        public void SignalLayersChanged()
        {
            LayersChanged?.Invoke(this, EventArgs.Empty);
        }

        public override uint MouseClick(MouseEventArgs e, GL_ControlBase control)
        {
            if (ObjectPlaceDelegate != null && e.Button == MouseButtons.Left)
            {
                var placements = ObjectPlaceDelegate.Invoke((new Vector4(-control.CoordFor(e.X, e.Y, Math.Min(100, control.PickingDepth)), 1) * EditZoneTransform.PositionTransform.Inverted()).Xyz, EditZone);

                Dictionary<ObjectList, List<I3dWorldObject>> objsByLists = new Dictionary<ObjectList, List<I3dWorldObject>>();

                SelectedObjects.Clear();

                for (int i = 0; i < placements.Length; i++)
                {
                    if (!objsByLists.ContainsKey(placements[i].objList))
                        objsByLists[placements[i].objList] = new List<I3dWorldObject>();

                    placements[i].obj.Layer = EditZone.CommonLayer;

                    objsByLists[placements[i].objList].Add(placements[i].obj);

                    placements[i].objList.Add(placements[i].obj);
                    placements[i].obj.SelectDefault(control);
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

                UpdateSelection(REDRAW_PICKING);

                return 0;
            }
            else
                return base.MouseClick(e, control);
        }

        public IEnumerable<I3dWorldObject> Objects => Get3DWObjects();

        public IReadOnlyCollection<IRevertable> UndoStack => undoStack;

        public IReadOnlyCollection<RedoEntry> RedoStack => redoStack;

        public override string ToString()
        {
            return MainZone.StageName;
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

#if ODYSSEY
                    foreach (var points in scene.koopaRacePoints)
                    {
                        int randomColor = control.RNG.Next();
                        GL.VertexAttrib4(1, new Vector4(
                            (((randomColor >> 16) & 0xFF) / 255f) * 0.5f + 0.25f,
                            (((randomColor >> 8) & 0xFF) / 255f) * 0.5f + 0.25f,
                            ((randomColor & 0xFF) / 255f) * 0.5f + 0.25f,
                            1f
                            ));
                        
                        GL.Begin(PrimitiveType.LineStrip);
                        foreach (var point in points)
                        {
                            GL.Vertex3(point.Postion*0.01f);
                        }
                        GL.End();
                    }
#endif
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
                var bak = SceneDrawState.EnabledLayers;

                SceneDrawState.EnabledLayers = scene.mainZone.GetVisibleLayers(SuperSet<Layer>.Instance);

                if (scene.editZoneIndex != 0) //zonePlacements can't be edited
                    foreach (var zonePlacement in scene.ZonePlacements)
                        if(zonePlacement.Visible)
                            zonePlacement.Draw(control, pass, scene);
                
                SceneDrawState.EnabledLayers = bak;
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

#if ODYSSEY
        struct KoopaRacePoint
        {
            public Vector3 Postion { get; set; }
        }

        List<List<KoopaRacePoint>> koopaRacePoints = new List<List<KoopaRacePoint>>();
#endif

        public int EditZoneIndex
        {
            get => editZoneIndex;
            set
            {
                if (editZoneIndex == value)
                    return;

#if ODYSSEY
                EditZone?.UpdateRenderBatch(EditZone.CurrentScenario);
#else
                EditZone?.UpdateRenderBatch();
#endif

                if (value == 0)
                {
                    EditZone = MainZone;

#if ODYSSEY
                    koopaRacePoints.Clear();

                    foreach (var entry in MainZone.ExtraFiles[0].Where(x => x.Key.Contains("_")))
                    {
                        var data = entry.Value.RootNode;

                        var actionNames = ((List<object>)data["ActionName"]).Select(x => (string)x).ToList();

                        var capActionNames = new List<string>() { "-" };
                        if (data.ContainsKey("ActionNameCap") && data["ActionNameCap"] != null)
                            capActionNames.AddRange(((List<object>)data["ActionNameCap"]).Select(x => (string)x));

                        var hackNames = new List<string>() { "-" };
                        if (data.ContainsKey("HackName") && data["HackName"] != null)
                            hackNames.AddRange(((List<object>)data["HackName"]).Select(x => (string)x));

                        var materialCodes = ((List<object>)data["MaterialCode"]).Select(x => (string)x).ToList();

                        //listView1.Columns.Add("Index");
                        //listView1.Columns.Add("PosX");
                        //listView1.Columns.Add("PosY");
                        //listView1.Columns.Add("PosZ");
                        //listView1.Columns.Add("RotX");
                        //listView1.Columns.Add("RotY");
                        //listView1.Columns.Add("RotZ");
                        //listView1.Columns.Add("Action");
                        //listView1.Columns.Add("AnimFrame");
                        //listView1.Columns.Add("Flags");
                        //listView1.Columns.Add("MaterialCode");

                        List<KoopaRacePoint> points = new List<KoopaRacePoint>();

                        foreach (var item in data["DataArray"])
                        {
                            points.Add(new KoopaRacePoint
                            {
                                Postion = new Vector3(item[0], item[1], item[2])
                            });

                            //listViewItem.SubItems.Add(item[0].ToString());
                            //listViewItem.SubItems.Add(item[1].ToString());
                            //listViewItem.SubItems.Add(item[2].ToString());
                            //listViewItem.SubItems.Add(item[3].ToString());
                            //listViewItem.SubItems.Add(item[4].ToString());
                            //listViewItem.SubItems.Add(item[5].ToString());
                            //listViewItem.SubItems.Add(actionNames[item[6]]);
                            //listViewItem.SubItems.Add(item[7].ToString());
                            //listViewItem.SubItems.Add(Convert.ToString(item[8], 2).PadLeft(16, '0'));
                            //listViewItem.SubItems.Add(materialCodes[item[9]]);
                        }

                        koopaRacePoints.Add(points);
                    }
#endif

                    EditZoneTransform = ZoneTransform.Identity;

                    ZonePlacements = MainZone.ZonePlacements;
                }
                else
                {
                    foreach (var placement in ZonePlacements)
                        placement.DeselectAll(control);

                    ZonePlacements = new List<ZonePlacement>();

                    EditZone = MainZone.ZonePlacements[value - 1].Zone;

                    EditZoneTransform = MainZone.ZonePlacements[value - 1].GetTransform();

                    for (int i = 0; i < MainZone.ZonePlacements.Count; i++)
                    {
                        if (i == value - 1)
                            continue;

                        ZonePlacement zonePlacement = MainZone.ZonePlacements[i];
                        ZonePlacements.Add(zonePlacement);
                    }

                    ZonePlacements.Add(mainZonePlacement);
                }

                SetZoneDrawState();

                undoStack = EditZone.undoStack;
                redoStack = EditZone.redoStack;
                LastSavedUndo = EditZone.LastSavedUndo;

                editZoneIndex = value;
            }
        }

        public override void Connect(GL_ControlBase control)
        {
            SetZoneDrawState();

            base.Connect(control);
        }

        private void SetZoneDrawState()
        {
            SceneDrawState.ZoneTransform = EditZoneTransform;

            SceneDrawState.EnabledLayers = EditZone.GetVisibleLayers(SuperSet<Layer>.Instance);
        }

#if ODYSSEY
        public void SetScenario(int scenario)
        {
            foreach (var zone in GetZones())
            {
                zone.SetScenario(scenario);

                if (zone!=EditZone)
                    zone.UpdateRenderBatch(scenario);
            }

            SceneDrawState.EnabledLayers = EditZone.GetVisibleLayers(SuperSet<Layer>.Instance);
        }
#endif

        protected ZoneTransform EditZoneTransform { get; private set; }

        public SM3DWorldZone MainZone { 
            get => mainZone;
            private set
            {
                mainZone = value;
                mainZonePlacement = new ZonePlacement(Vector3.Zero, Vector3.Zero, null, value);
            }
        }

        private ZonePlacement mainZonePlacement;

        public IEnumerable<SM3DWorldZone> GetZones()
        {
            yield return MainZone;

            foreach (ZonePlacement zonePlacement in MainZone.ZonePlacements)
            {
                yield return zonePlacement.Zone;
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        public override bool IsSaved
        {
            get => GetZones().All(x => x.IsSaved);
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

            if (editZoneIndex == 0) //zonePlacements can be edited
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
                    obj.UpdateLinkDestinations_Clear();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.UpdateLinkDestinations_Clear();

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.UpdateLinkDestinations_Populate();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.UpdateLinkDestinations_Populate();
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

                    if (_objsToDelete.Count > 0)
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
                        newPlacements.Add(new ZonePlacement(placement.Position, placement.Rotation, placement.Layer, placement.Zone));
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
                {
                    obj.DuplicateSelected(duplicates, EditZone);

                    obj.DeselectAll(control);

                    if (duplicates.TryGetValue(obj, out var copy))
                        copy.SelectDefault(control);
                }

                objList.AddRange(duplicates.Values);

                foreach (var (orignal, copy) in duplicates) totalDuplicates.Add(orignal, copy);

                if (duplicates.Count > 0)
                    objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(objList, duplicates.Values.ToArray()));

                duplicates.Clear();
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
            {
                obj.DuplicateSelected(duplicates, EditZone);

                obj.DeselectAll(control);

                if (duplicates.TryGetValue(obj, out var copy))
                    copy.SelectDefault(control);
            }

            foreach (var (orignal, copy) in duplicates) totalDuplicates.Add(orignal, copy);

            if (duplicates.Count > 0)
                objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(EditZone.LinkedObjects, duplicates.Values.ToArray()));

            EditZone.LinkedObjects.AddRange(duplicates.Values);

            //Rebuild links
            DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in EditZone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.LinkDuplicates(duplicationInfo, true);
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                obj.LinkDuplicates(duplicationInfo, true);

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

            UpdateLinkDestinations();

            UpdateSelection(REDRAW_PICKING);
        }

        static List<ZonePlacement> copiedZonePlacements = null;

        static List<(string listName, Dictionary<I3dWorldObject, I3dWorldObject> copies)> copiedObjects = null;

        static SM3DWorldZone copySrcZone = null;

        static ZoneTransform copySrcZoneTransform;
        private SM3DWorldZone mainZone;

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
                        copiedZonePlacements.Add(new ZonePlacement(placement.Position, placement.Rotation, placement.Layer, placement.Zone));
                    }
                }
                //the rest will be handled at the end of the function
            }

            //Duplicate Selected Objects
            copiedObjects = new List<(string listName, Dictionary<I3dWorldObject, I3dWorldObject> copies)>();

            SceneObjectIterState.InLinks = false;
            foreach (var (listName, objList) in EditZone.ObjLists)
            {
                Dictionary<I3dWorldObject, I3dWorldObject> copies = new Dictionary<I3dWorldObject, I3dWorldObject>();

                foreach (I3dWorldObject obj in objList)
                {
                    obj.DuplicateSelected(copies, null);

                    if(copies.TryGetValue(obj, out var copy))
                        copy.SelectAll(control);
                }

                if (copies.Count > 0)
                    copiedObjects.Add((listName, copies));
            }
            SceneObjectIterState.InLinks = true;

            {
                Dictionary<I3dWorldObject, I3dWorldObject> copies = new Dictionary<I3dWorldObject, I3dWorldObject>();

                foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                {
                    obj.DuplicateSelected(copies, null);

                    if (copies.TryGetValue(obj, out var copy))
                        copy.SelectAll(control);
                }

                if (copies.Count > 0)
                    copiedObjects.Add((LINKS_LIST_NAME, copies));
            }

            if (copiedZonePlacements.Count > 0 || copiedObjects.Count > 0)
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
                    if (!EditZone.ObjLists.TryGetValue(listName, out ObjectList objectList))
                    {
#if ODYSSEY
                        if(listName != LINKS_LIST_NAME)
                        {
                            var newList = new ObjectList();
                            EditZone.ObjLists.Add(listName, newList);
                            objectList = newList;
                        }
                        else
#endif
                            objectList = EditZone.LinkedObjects;
                    }

                    Dictionary<I3dWorldObject, I3dWorldObject> duplicates = new Dictionary<I3dWorldObject, I3dWorldObject>();

                    foreach (I3dWorldObject copy in copies.Values)
                    {
                        copy.DuplicateSelected(duplicates, EditZone, zoneToZoneTransform); //yep we need to duplicate the copies

                        if (duplicates.TryGetValue(copy, out var pasted))
                            pasted.SelectDefault(control);
                    }

                    objectList.AddRange(duplicates.Values);

                    foreach (var (original, copy) in copies) totalDuplicates.Add(original, duplicates[copy]);

                    if (duplicates.Count > 0)
                        objListInfos.Add(new Revertable3DWorldObjAddition.ObjListInfo(objectList, duplicates.Values.ToArray()));
                }

                //Rebuild links
                DuplicationInfo duplicationInfo = new DuplicationInfo(totalDuplicates);

                SceneObjectIterState.InLinks = false;
                foreach (ObjectList objects in EditZone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.LinkDuplicates(duplicationInfo, isSameZone);
                }
                SceneObjectIterState.InLinks = true;
                foreach (I3dWorldObject obj in EditZone.LinkedObjects)
                    obj.LinkDuplicates(duplicationInfo, isSameZone);
            }

            BeginUndoCollection();
            //Add to undo
            if (objListInfos.Count > 0)
                AddToUndo(new Revertable3DWorldObjAddition(objListInfos.ToArray()));

            List<ZonePlacement> totalDuplicatesOfZonePlacements = new List<ZonePlacement>();

            if (editZoneIndex == 0 && copiedZonePlacements?.Count > 0)
            {
                foreach (var placement in copiedZonePlacements)
                {
                    var newPlacement = new ZonePlacement(placement.Position, placement.Rotation, placement.Layer, placement.Zone);

                    newPlacement.SelectDefault(control);
                    ZonePlacements.Add(newPlacement);

                    totalDuplicatesOfZonePlacements.Add(newPlacement);
                }
                ZonePlacementsChanged?.Invoke(this, null);

                RevertableAddition.AddInListInfo addInListInfo = new RevertableAddition.AddInListInfo(totalDuplicatesOfZonePlacements.ToArray(), ZonePlacements);

                AddToUndo(new RevertableAddition(new RevertableAddition.AddInListInfo[] { addInListInfo }, Array.Empty<RevertableAddition.SingleAddInListInfo>()));
            }

            EndUndoCollection();

            UpdateLinkDestinations();

            UpdateSelection(REDRAW_PICKING);
        }

        public void GrowSelection()
        {
            List<I3dWorldObject> objectsToSelect = new List<I3dWorldObject>();

            foreach (var obj in Get3DWObjects())
            {
                if (obj.Links != null && obj.IsSelected())
                {
                    foreach (var (linkName, link) in obj.Links)
                    {
                        foreach (I3dWorldObject _obj in link)
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

            UpdateSelection(REDRAW);
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
                            foreach (var (linkName, link) in obj.Links)
                            {
                                foreach (I3dWorldObject _obj in link)
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

            UpdateSelection(REDRAW);
        }

        public void InvertSelection()
        {
            foreach (var obj in Get3DWObjects())
            {
                if (obj.IsSelected())
                    obj.DeselectAll(control);
                else
                    obj.SelectDefault(control);
            }

            UpdateSelection(REDRAW);
        }

        public class DuplicationInfo
        {
            Dictionary<I3dWorldObject, I3dWorldObject> duplicatesByOriginal;
            HashSet<I3dWorldObject> duplicates = new HashSet<I3dWorldObject>();

            public DuplicationInfo(Dictionary<I3dWorldObject, I3dWorldObject> duplicates)
            {
                duplicatesByOriginal = duplicates;

                foreach (I3dWorldObject obj in duplicates.Values)
                    this.duplicates.Add(obj);
            }

            public bool IsDuplicate(I3dWorldObject obj) => duplicates.Contains(obj);

            public bool HasDuplicate(I3dWorldObject obj) => duplicatesByOriginal.ContainsKey(obj) || duplicates.Contains(obj);

            public bool TryGetDuplicate(I3dWorldObject obj, out I3dWorldObject duplicate) => duplicatesByOriginal.TryGetValue(obj, out duplicate);
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
            bool showPromt = false;
            string overwrites = "";

            var zones = ZonePlacements.Select(x=>x.Zone).Where(y=>!y.IsSaved).Prepend(MainZone).ToArray();

            foreach (var zone in zones)
            {
                if (!string.IsNullOrEmpty(Program.ProjectPath) && zone.Directory == Program.BaseStageDataPath)
                    showPromt = true;

                foreach (var fileName in zone.GetSaveFileNames(zone.StageInfo))
                {
                    if (File.Exists(System.IO.Path.Combine(Program.ProjectStageDataPath, fileName)))
                        overwrites += fileName + '\n';
                }
            }
            bool changeDirectory = false;

            if (showPromt)
            {
                string part1 = Program.CurrentLanguage.GetTranslation("SaveInProjectTextPart1") ?? "Should the level files be saved to {0} so the BaseGame is preserved?";
                string part2 = Program.CurrentLanguage.GetTranslation("SaveInProjectTextPart2") ?? "Following files will be overwritten:";

                string message = string.Format(part1, Program.ProjectStageDataPath);

                if (!string.IsNullOrEmpty(overwrites))
                    message += $"\n\n{part2}\n" + overwrites.Trim('\n');

                DialogResult result = MessageBox.Show(
                        message,
                        Program.CurrentLanguage.GetTranslation("SaveInProjectHeader") ?? "Save file in ProjectPath?", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Cancel)
                    return false;

                changeDirectory = result == DialogResult.Yes;
            }

            foreach (var zone in zones)
            {
                StageInfo newStageInfo = zone.StageInfo;

                if (changeDirectory)
                    newStageInfo.Directory = Program.ProjectStageDataPath;

                zone.Save(newStageInfo);
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
            List<SM3DWorldZone> additionalZones = new List<SM3DWorldZone>();

            foreach (var zonePlacement in MainZone.ZonePlacements)
            {
                if (!additionalZones.Contains(zonePlacement.Zone))
                    additionalZones.Add(zonePlacement.Zone);
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter =
                    $"Split Level Files|*{SM3DWorldZone.MAP_SUFFIX}|" +
                    $"Combined Level Files|*{SM3DWorldZone.COMBINED_SUFFIX}",
                InitialDirectory = MainZone.Directory,
                FileName = MainZone.StageInfo.StageName,
                FilterIndex = (int)MainZone.StageInfo.StageArcType + 1
            };

            ZoneSaveOptionsDialog optionsDialog = null;

            StageInfo? stageInfo = null;

            sfd.FileOk += (s, e) =>
            {
                if (SM3DWorldZone.TryGetStageInfo(sfd.FileName, out stageInfo))
                {
                    optionsDialog = new ZoneSaveOptionsDialog(stageInfo.Value, MainZone.ByteOrder, additionalZones);

                    if (optionsDialog.ShowDialog() != DialogResult.OK)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                    e.Cancel = true;
            };


            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            var _stageInfo = stageInfo.Value;

            _stageInfo.StageName = optionsDialog.StageName;
            _stageInfo.StageArcType = optionsDialog.StageArcType;

            MainZone.Save(_stageInfo, optionsDialog.ByteOrder);

            for (int i = 0; i < additionalZones.Count; i++)
            {
                if (!optionsDialog.AdditionalZoneEntries[i].ShouldSave)
                    continue;

                _stageInfo.StageName = optionsDialog.AdditionalZoneEntries[i].NewName;

                additionalZones[i].Save(_stageInfo, optionsDialog.ByteOrder);
                foreach (ZonePlacement placement in mainZone.ZonePlacements) 
                {
                    if (placement.Zone == additionalZones[i])
                        placement.ZoneLookupName = _stageInfo.StageName;
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
