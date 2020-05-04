using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase.PropertyCapture;

namespace SpotLight.EditorDrawables
{
    public class Rail : Path<RailPoint>, I3dWorldObject
    {
        public enum RailObjType
        {
            Rail,
            RailWithMoveParameter,
            RailWithEffect
        }

        public static Array RailTypes = Enum.GetValues(typeof(RailObjType));

        public static Dictionary<string, dynamic> CreateUnitConfig(string className) => new Dictionary<string, dynamic>
        {
            ["DisplayName"] = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
            ["DisplayRotate"] = LevelIO.Vector3ToDict(Vector3.Zero),
            ["DisplayScale"] = LevelIO.Vector3ToDict(Vector3.One),
            ["DisplayTranslate"] = LevelIO.Vector3ToDict(Vector3.Zero, 100f),
            ["GenerateCategory"] = "",
            ["ParameterConfigName"] = className,
            ["PlacementTargetFile"] = "Map"
        };

        protected static List<RailPoint> RailPointsFromRailPointsEntry(ByamlIterator.DictionaryEntry railPointsEntry)
        {
            List<RailPoint> pathPoints = new List<RailPoint>();

            foreach (ByamlIterator.ArrayEntry pointEntry in railPointsEntry.IterArray())
            {
                Vector3 pos = new Vector3();
                Vector3 cp1 = new Vector3();
                Vector3 cp2 = new Vector3();

                var properties = new Dictionary<string, dynamic>();

                foreach (ByamlIterator.DictionaryEntry entry in pointEntry.IterDictionary())
                {
                    if (entry.Key == "Comment" ||
                        entry.Key == "Id" ||
                        entry.Key == "IsLinkDest" ||
                        entry.Key == "LayerConfigName" ||
                        entry.Key == "Links" ||
                        entry.Key == "ModelName" ||
                        entry.Key == "Rotate" ||
                        entry.Key == "Scale" ||
                        entry.Key == "UnitConfig" ||
                        entry.Key == "UnitConfigName"

                        )
                        continue;

                    dynamic _data = entry.Parse();
                    if (entry.Key == "Translate")
                    {
                        pos = new Vector3(
                            _data["X"] / 100f,
                            _data["Y"] / 100f,
                            _data["Z"] / 100f
                        );
                    }
                    else if (entry.Key == "ControlPoints")
                    {
                        cp1 = new Vector3(
                            _data[0]["X"] / 100f,
                            _data[0]["Y"] / 100f,
                            _data[0]["Z"] / 100f
                        );

                        cp2 = new Vector3(
                            _data[1]["X"] / 100f,
                            _data[1]["Y"] / 100f,
                            _data[1]["Z"] / 100f
                        );
                    }
                    else
                        properties.Add(entry.Key, _data);
                }

                pathPoints.Add(new RailPoint(pos, cp1 - pos, cp2 - pos, properties));
            }

            return pathPoints;
        }

        /// <summary>
        /// Id of this object
        /// </summary>
        public string ID { get; }

        [Undoable]
        public bool IsLadder { get; set; }

        [Undoable]
        public bool IsReverseCoord { get; set; }

        [Undoable]
        public RailObjType ObjType { get; set; }

        public Rail(LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks)
        {
            pathPoints = RailPointsFromRailPointsEntry(info.PropertyEntries["RailPoints"]);

            ID = info.ID;
            if (zone != null)
                zone.SubmitRailID(ID);

            ObjType = (RailObjType)Enum.Parse(typeof(RailObjType), info.ClassName);

            if(ObjType==RailObjType.RailWithMoveParameter)
                IsReverseCoord = info.PropertyEntries["IsReverseCoord"].Parse();


            IsLadder = info.PropertyEntries["IsLadder"].Parse();

            Closed = info.PropertyEntries["IsClosed"].Parse();

            zone?.SubmitRailID(ID);

            loadLinks = false; //We don't expect Rails to have Links
        }
        /// <summary>
        /// Creates a new rail for 3DW
        /// </summary>
        /// <param name="pathPoints">List of Path Points to use in this rail</param>
        /// <param name="iD">ID Of the rail</param>
        /// <param name="isClosed">Is the path closed?</param>
        /// <param name="isLadder">Unknown</param>
        /// <param name="isReverseCoord">Reverses the order the rails are in</param>
        /// <param name="railObjType"><see cref="RailObjType"/></param>
        public Rail(List<RailPoint> railPoints, string iD, bool isClosed, bool isLadder, bool isReverseCoord, RailObjType railObjType, SM3DWorldZone zone)
        {
            ID = iD;
            Closed = isClosed;
            IsLadder = isLadder;
            IsReverseCoord = isReverseCoord;
            ObjType = railObjType;

            pathPoints = railPoints;

            foreach (var point in pathPoints)
                SetupPoint(point);

            zone?.SubmitRailID(ID);
        }

        protected override void SetupPoint(RailPoint point)
        {
            point.Properties.Clear();

            if (ObjType == RailObjType.RailWithMoveParameter)
            {
                point.Properties.Add("AngleV", 0f);
                point.Properties.Add("Distance", 0f);
                point.Properties.Add("OffsetY", 0f);
                point.Properties.Add("Speed", 0f);
                point.Properties.Add("WaitTime", 0);
            }
            else if(ObjType == RailObjType.RailWithEffect)
            {
                point.Properties.Add("ThroughWater", false);
                point.Properties.Add("ThroughWaterBottom", false);
            }
        }

        #region I3DWorldObject implementation
        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public IReadOnlyList<(string, I3dWorldObject)> LinkDestinations { get => linkDestinations; }


        List<(string, I3dWorldObject)> linkDestinations = new List<(string, I3dWorldObject)>();

        public Dictionary<string, List<I3dWorldObject>> Links { get => null; set { } } //We don't expect Rails to have Links

        public void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest = false)
        {
            objNode.AddDynamicValue("Comment", null);
            objNode.AddDynamicValue("Id", ID);
            objNode.AddDynamicValue("IsClosed", Closed);
            objNode.AddDynamicValue("IsLadder", IsLadder);

            if(ObjType==RailObjType.RailWithMoveParameter)
                objNode.AddDynamicValue("IsReverseCoord", IsReverseCoord);

            objNode.AddDynamicValue("IsLinkDest", isLinkDest);
            objNode.AddDynamicValue("LayerConfigName", "Common");

            alreadyWrittenObjs.Add(this);
            
            objNode.AddDictionaryNodeRef("Links", writer.CreateDictionaryNode(), true); //We don't expect Rails to have Links

            objNode.AddDynamicValue("ModelName", null);

            #region Save RailPoints
            ArrayNode railPointsNode = writer.CreateArrayNode();

            int i = 0;
            foreach (RailPoint point in PathPoints)
            {
                DictionaryNode pointNode = writer.CreateDictionaryNode();

                pointNode.AddDynamicValue("Comment", null);

                pointNode.AddDynamicValue("ControlPoints", new List<dynamic>()
                {
                    LevelIO.Vector3ToDict(point.ControlPoint1 + point.Position, 100f),
                    LevelIO.Vector3ToDict(point.ControlPoint2 + point.Position, 100f)
                });

                pointNode.AddDynamicValue("Id", $"{ID}/{i}");
                pointNode.AddDynamicValue("IsLinkDest", isLinkDest);
                pointNode.AddDynamicValue("LayerConfigName", "Common");

                pointNode.AddDictionaryNodeRef("Links", writer.CreateDictionaryNode(), true); //We don't expect Points to have Links either

                pointNode.AddDynamicValue("ModelName", null);

                pointNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Vector3.Zero), true);
                pointNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
                pointNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(point.Position, 100f), true);
                
                pointNode.AddDynamicValue("UnitConfig", CreateUnitConfig("Point"), true);
                
                pointNode.AddDynamicValue("UnitConfigName", "Point");

                if (point.Properties.Count != 0)
                {
                    foreach (KeyValuePair<string, dynamic> keyValuePair in point.Properties)
                    {
                        if (keyValuePair.Value is string && keyValuePair.Value == "")
                            pointNode.AddDynamicValue(keyValuePair.Key, null, true);
                        else
                            pointNode.AddDynamicValue(keyValuePair.Key, keyValuePair.Value, true);
                    }
                }

                railPointsNode.AddDictionaryNodeRef(pointNode, true);

                i++;
            }

            objNode.AddArrayNodeRef("RailPoints", railPointsNode);
            #endregion

            objNode.AddDynamicValue("RailType", "Linear");

            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Vector3.Zero), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(PathPoints[0].Position, 100f), true);

            string objTypeName = Enum.GetName(typeof(RailObjType), ObjType);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig(objTypeName), true);

            objNode.AddDynamicValue("UnitConfigName", objTypeName);
        }

        public virtual Vector3 GetLinkingPoint(SM3DWorldScene editorScene)
        {
            return (PathPoints[0] as RailPoint)?.GetLinkingPoint(editorScene) ?? Vector3.Zero;
        }

        public void ClearLinkDestinations()
        {
            linkDestinations.Clear();
        }

        public void AddLinkDestinations()
        {
            //We don't expect Rails to have Links
        }

        public void AddLinkDestination(string linkName, I3dWorldObject linkingObject)
        {
            linkDestinations.Add((linkName, linkingObject));
        }

        public void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldScene scene, SM3DWorldZone zone)
        {
            bool anyPointsSelected = false;
            foreach (PathPoint point in pathPoints)
            {
                if (point.Selected)
                    anyPointsSelected = true;
            }


            if(!anyPointsSelected)
                return;

            List<RailPoint> newPoints = new List<RailPoint>();

            foreach (RailPoint point in pathPoints)
            {
                if (point.Selected)
                {
                    newPoints.Add(new RailPoint(point.Position, point.ControlPoint1, point.ControlPoint2, point.Properties));
                }
            }

            duplicates[this] = new Rail(newPoints, zone.NextRailID(), Closed, IsLadder, IsReverseCoord, ObjType, zone);

            var duplicate = duplicates[this];

            DeselectAll(scene.GL_Control);
            
            duplicates[this].Prepare((GL_EditorFramework.GL_Core.GL_ControlModern)scene.GL_Control);

            duplicates[this].SelectDefault(scene.GL_Control);
        }

        public void LinkDuplicatesAndAddLinkDestinations(SM3DWorldScene.DuplicationInfo duplicationInfo)
        {
            //We don't expect Rails to have Links
        }
        #endregion


        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            bool any = false;

            foreach (RailPoint point in pathPoints)
                any |= point.Selected;


            if (!any)
                return false;

            

            List<RailPoint> points = new List<RailPoint>();

            foreach (RailPoint point in pathPoints)
            {
                if (point.Selected)
                    points.Add(point);
            }

            General3dWorldObject.ExtraPropertiesUIContainer pointPropertyContainer = null;

            if (points.Count == 1)
                pointPropertyContainer = new General3dWorldObject.ExtraPropertiesUIContainer(points[0].Properties, scene);

            objectUIControl.AddObjectUIContainer(new RailUIContainer(this, scene, pointPropertyContainer), "Rail");

            if (points.Count == 1)
            {
                objectUIControl.AddObjectUIContainer(new SinglePathPointUIContainer(points[0], scene), "Rail Point");
                objectUIControl.AddObjectUIContainer(pointPropertyContainer, "Point Properties");
            }

            return true;
        }

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            return zone.ObjLists.TryGetValue("Map_Rails", out objList);
        }

        public class RailUIContainer : IObjectUIContainer
        {
            PropertyCapture? pathCapture = null;

            Rail rail;
            readonly EditorSceneBase scene;

            RailObjType lastRailType;

            General3dWorldObject.ExtraPropertiesUIContainer pathPointPropertyContainer;

            public RailUIContainer(Rail rail, EditorSceneBase scene, General3dWorldObject.ExtraPropertiesUIContainer pathPointPropertyContainer)
            {
                this.rail = rail;

                List<PathPoint> points = new List<PathPoint>();

                this.scene = scene;

                lastRailType = rail.ObjType;

                this.pathPointPropertyContainer = pathPointPropertyContainer;
            }

            public void DoUI(IObjectUIControl control)
            {
                rail.ObjType = (RailObjType)control.ChoicePicker("Rail Type", rail.ObjType, RailTypes);

                rail.IsLadder = control.CheckBox("Is Ladder", rail.IsLadder);

                rail.Closed = control.CheckBox("Closed", rail.Closed);

                if(rail.ObjType==RailObjType.RailWithMoveParameter)
                    rail.IsReverseCoord = control.CheckBox("Reverse Coord", rail.IsReverseCoord);

                if (scene.CurrentList != rail.pathPoints && control.Button("Edit Pathpoints"))
                    scene.EnterList(rail.pathPoints);
            }

            public void OnValueChangeStart()
            {
                pathCapture = new PropertyCapture(rail);
            }

            public void OnValueChanged()
            {
                scene.Refresh();
            }

            public void OnValueSet()
            {
                scene.BeginUndoCollection();

                pathCapture?.HandleUndo(scene);

                pathCapture = null;

                if (lastRailType != rail.ObjType)
                {
                    foreach (var point in rail.pathPoints)
                    {
                        #region add deletion to undo

                        RevertableDictDeletion.DeleteInfo[] deleteInfos = new RevertableDictDeletion.DeleteInfo[point.Properties.Count];

                        int i = 0;

                        foreach (var keyValuePair in point.Properties)
                            deleteInfos[i++] = new RevertableDictDeletion.DeleteInfo(keyValuePair.Value, keyValuePair.Key);

                        scene.AddToUndo(new RevertableDictDeletion(
                            new RevertableDictDeletion.DeleteInDictInfo[]
                            {
                                new RevertableDictDeletion.DeleteInDictInfo(deleteInfos, point.Properties)
                            },
                            Array.Empty<RevertableDictDeletion.SingleDeleteInDictInfo>()));

                        #endregion

                        rail.SetupPoint(point);

                        #region add addition to undo

                        RevertableDictAddition.AddInfo[] addInfos = new RevertableDictAddition.AddInfo[point.Properties.Count];

                        i = 0;

                        foreach (var keyValuePair in point.Properties)
                            addInfos[i++] = new RevertableDictAddition.AddInfo(keyValuePair.Value, keyValuePair.Key);

                        scene.AddToUndo(new RevertableDictAddition(
                            new RevertableDictAddition.AddInDictInfo[]
                            {
                                new RevertableDictAddition.AddInDictInfo(addInfos, point.Properties)
                            },
                            Array.Empty<RevertableDictAddition.SingleAddInDictInfo>()));

                        #endregion
                    }

                    

                    pathPointPropertyContainer?.UpdateKeys();

                    lastRailType = rail.ObjType;
                }

                scene.EndUndoCollection();

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }
    }

    public class RailPoint : PathPoint
    {
        public override Vector3 GlobalPos
        {
            get => Vector4.Transform(new Vector4(Position, 1), SceneDrawState.ZoneTransform.PositionTransform).Xyz;
            set => Position = Vector4.Transform(new Vector4(value, 1), SceneDrawState.ZoneTransform.PositionTransform.Inverted()).Xyz;
        }

        public override Vector3 GlobalCP1
        {
            get => Vector3.Transform(ControlPoint1, SceneDrawState.ZoneTransform.RotationTransform);
            set => ControlPoint1 = Vector3.Transform(value, SceneDrawState.ZoneTransform.RotationTransform.Inverted());
        }

        public override Vector3 GlobalCP2
        {
            get => Vector3.Transform(ControlPoint2, SceneDrawState.ZoneTransform.RotationTransform);
            set => ControlPoint2 = Vector3.Transform(value, SceneDrawState.ZoneTransform.RotationTransform.Inverted());
        }

        public Dictionary<string, dynamic> Properties { get; private set; } = null;

        public RailPoint()
            : base()
        {
            Properties = new Dictionary<string, dynamic>();
        }

        public RailPoint(Vector3 position, Vector3 controlPoint1, Vector3 controlPoint2, Dictionary<string, dynamic> properties)
            : base(position, controlPoint1, controlPoint2)
        {
            Properties = properties;
        }

        public RailPoint(Vector3 position, Vector3 controlPoint1, Vector3 controlPoint2)
            : base(position, controlPoint1, controlPoint2)
        {
            Properties = new Dictionary<string, dynamic>();
        }

        public virtual Vector3 GetLinkingPoint(SM3DWorldScene editorScene)
        {
            return Selected ? editorScene.SelectionTransformAction.NewPos(GlobalPos) : GlobalPos;
        }
    }
}
