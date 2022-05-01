using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase.PropertyCapture;

namespace Spotlight.EditorDrawables
{
    public class Rail : Path<RailPoint>, I3dWorldObject
    {
        public override string ToString()
        {
            return ClassName.ToString();
        }

        protected static List<RailPoint> RailPointsFromRailPointsEntry(ByamlIterator.DictionaryEntry railPointsEntry, in LevelIO.ObjectInfo railInfo)
        {
            List<RailPoint> pathPoints = new List<RailPoint>();

            int pointIndex = 0;

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
                        entry.Key == "ModelName" ||
                        entry.Key == "Rotate" ||
                        entry.Key == "Scale" ||
                        entry.Key == "UnitConfig" ||
                        entry.Key == "UnitConfigName"

                        )
                        continue;

                    if (entry.Key == "Links")
                    {
                        foreach (ByamlIterator.DictionaryEntry linkEntry in entry.IterDictionary())
                        {
                            railInfo.LinkEntries.Add(linkEntry.Key + "_FromPoint" + pointIndex, linkEntry);
                        }

                        continue;
                    }

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

                pointIndex++;
            }

            return pathPoints;
        }

        /// <summary>
        /// Id of this object
        /// </summary>
        public string ID { get; set; }

        [Undoable]
        public bool IsLadder { get; set; }

        [Undoable]
        public string ClassName { get; set; }

        public Dictionary<string, dynamic> Properties { get; private set; } = null;

        [Undoable]
        public Layer Layer { get; set; }

        readonly string comment = null;

        public Rail(in LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks)
        {
            pathPoints = RailPointsFromRailPointsEntry(info.PropertyEntries["RailPoints"], in info);

            ID = info.ID;
            if (zone != null)
                zone.SubmitRailID(ID);

            ClassName = info.ClassName;

            Layer = zone.GetOrCreateLayer(info.LayerName);

            comment = info.Comment;

            Properties = new Dictionary<string, dynamic>();

            foreach (var propertyEntry in info.PropertyEntries)
            {
                switch (propertyEntry.Key)
                {
                    case "IsLadder":
                        IsLadder = propertyEntry.Value.Parse();
                        continue;
                    case "IsClosed":
                        Closed = propertyEntry.Value.Parse();
                        continue;
                    case "RailType":
                    case "RailPoints":
                        continue;
                    default:
                        Properties.Add(propertyEntry.Key, propertyEntry.Value.Parse());
                        continue;
                }
            }

            zone?.SubmitRailID(ID);

            loadLinks = true;
        }


        /// <summary>
        /// Creates a new rail for 3DW
        /// </summary>
        /// <param name="pathPoints">List of Path Points to use in this rail</param>
        /// <param name="iD">ID Of the rail</param>
        /// <param name="isClosed">Is the path closed?</param>
        /// <param name="isLadder">Unknown</param>
        /// <param name="isReverseCoord">Reverses the order the rails are in</param>
        /// <param name="className"></param>
        public Rail(List<RailPoint> railPoints, string iD, string className, bool isClosed, bool isLadder, Dictionary<string, List<I3dWorldObject>> links, Dictionary<string, dynamic> properties, SM3DWorldZone zone, Layer layer)
        {
            ID = iD;
            Closed = isClosed;
            IsLadder = isLadder;
            Properties = properties;
            ClassName = className;

            Links = links;

            pathPoints = railPoints;

            Layer = layer;

            foreach (var point in pathPoints)
                SetupPoint(point);

            zone?.SubmitRailID(ID);
        }

        protected override void SetupPoint(RailPoint point)
        {
            if(Program.ParameterDB.RailParameters.TryGetValue(ClassName, out Database.RailParam railParam))
                Database.ObjectParameterDatabase.AddToProperties(railParam.PointProperties, point.Properties);
        }

        #region I3DWorldObject implementation
        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public List<(string, I3dWorldObject)> LinkDestinations { get; } = new List<(string, I3dWorldObject)>();

        public Dictionary<string, List<I3dWorldObject>> Links { get; set; } = null;

        public void Save(LevelObjectsWriter writer, DictionaryNode objNode)
        {
#if ODYSSEY
            objNode.AddDynamicValue("comment", comment);
#else
            objNode.AddDynamicValue("Comment", comment);
#endif
            objNode.AddDynamicValue("Id", ID);
            objNode.AddDynamicValue("IsClosed", Closed);
            objNode.AddDynamicValue("IsLadder", IsLadder);

            objNode.AddDynamicValue("IsLinkDest", LinkDestinations.Count > 0);
            objNode.AddDynamicValue("LayerConfigName", Layer.Name);

            writer.SaveLinks(Links?.Where(x=>!x.Key.Contains("_FromPoint")), objNode);

            objNode.AddDynamicValue("ModelName", null);

            #region Save RailPoints
            ArrayNode railPointsNode = writer.CreateArrayNode();

            var pointClassName = "Point" + ClassName.Substring("Rail".Length);

            int pointIndex = 0;
            foreach (RailPoint point in PathPoints)
            {
                DictionaryNode pointNode = writer.CreateDictionaryNode();

//#if ODYSSEY
//                objNode.AddDynamicValue("comment", comment);
//#else
//                objNode.AddDynamicValue("Comment", comment);
//#endif

                pointNode.AddDynamicValue("ControlPoints", new List<dynamic>()
                {
                    LevelIO.Vector3ToDict(point.ControlPoint1 + point.Position, 100f),
                    LevelIO.Vector3ToDict(point.ControlPoint2 + point.Position, 100f)
                });

                pointNode.AddDynamicValue("Id", $"{ID}/{pointIndex}");
                pointNode.AddDynamicValue("IsLinkDest", false);
                pointNode.AddDynamicValue("LayerConfigName", Layer.Name);

                string pointLinkSuffix = "_FromPoint" + pointIndex;

                writer.SaveLinks(
                    Links?.Where(x => x.Key.EndsWith(pointLinkSuffix)).Select(x=>new KeyValuePair<string, List<I3dWorldObject>>(
                        x.Key.Substring(0,x.Key.Length-pointLinkSuffix.Length), x.Value
                        )), 
                    pointNode);

                pointNode.AddDynamicValue("ModelName", null);

                pointNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Vector3.Zero), true);
                pointNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
                pointNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(point.Position, 100f), true);
                
                pointNode.AddDynamicValue("UnitConfig", ObjectUtils.CreateUnitConfig(pointClassName), true);
                
                pointNode.AddDynamicValue("UnitConfigName", pointClassName);

                if (point.Properties.Count != 0)
                {
                    foreach (var property in point.Properties)
                    {
                        if (property.Value is string && property.Value == "")
                            pointNode.AddDynamicValue(property.Key, null, true);
                        else
                            pointNode.AddDynamicValue(property.Key, property.Value, true);
                    }
                }

                railPointsNode.AddDictionaryNodeRef(pointNode, true);

                pointIndex++;
            }

            objNode.AddArrayNodeRef("RailPoints", railPointsNode);
            #endregion

            objNode.AddDynamicValue("RailType", pathPoints.Exists(x=>x.ControlPoint1!=Vector3.Zero || x.ControlPoint2 != Vector3.Zero) ? "Bezier" : "Linear");

            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Vector3.Zero), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(PathPoints[0].Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", ObjectUtils.CreateUnitConfig(ClassName), true);

            objNode.AddDynamicValue("UnitConfigName", ClassName);

            if (Properties.Count != 0)
            {
                foreach (KeyValuePair<string, dynamic> property in Properties)
                {
                    if (property.Value is string && property.Value == "")
                        objNode.AddDynamicValue(property.Key, null, true);
                    else
                        objNode.AddDynamicValue(property.Key, property.Value, true);
                }
            }
        }

        public bool Equals(I3dWorldObject obj)
        {
            return obj is Rail rail &&
                   EqualRailPoints(pathPoints, rail.pathPoints) &&
                   Closed == rail.Closed &&
                   ID == rail.ID &&
                   IsLadder == rail.IsLadder &&
                   ClassName == rail.ClassName &&
                   Layer == rail.Layer &&
                   ObjectUtils.EqualProperties(Properties, rail.Properties);
        }

        private static bool EqualRailPoints(List<RailPoint> pointsA, List<RailPoint> pointsB)
        {
            if (pointsA.Count != pointsB.Count)
                return false;

            for (int i = 0; i < pointsA.Count; i++)
            {
                var a = pointsA[i];
                var b = pointsB[i];

                if (a.Position != b.Position)
                    return false;

                if (a.ControlPoint1 != b.ControlPoint1)
                    return false;

                if (a.ControlPoint2 != b.ControlPoint2)
                    return false;

                if (!ObjectUtils.EqualProperties(a.Properties, b.Properties))
                    return false;
            }

            return true;
        }

        public virtual Vector3 GetLinkingPoint(SM3DWorldScene editorScene)
        {
            return PathPoints[0]?.GetLinkingPoint(editorScene) ?? Vector3.Zero;
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!IsSelected() && !SceneDrawState.EnabledLayers.Contains(Layer))
            {
                control.SkipPickingColors(1);
                return;
            }

            base.Draw(control, pass, editorScene);
        }

        public override int GetPickableSpan()
        {
            if (SceneDrawState.EnabledLayers.Contains(Layer))
                return base.GetPickableSpan();
            else
                return 0;
        }

        public void UpdateLinkDestinations_Clear()
        {
            LinkDestinations.Clear();
        }

        public void UpdateLinkDestinations_Populate()
        {
            if (Links != null)
            {
                foreach (var (linkName, link) in Links)
                {
                    foreach (I3dWorldObject obj in link)
                    {
                        obj.AddLinkDestination(linkName, this);
                    }
                }
            }
        }

        public void AddLinkDestination(string linkName, I3dWorldObject linkingObject)
        {
            LinkDestinations.Add((linkName, linkingObject));
        }

        public void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldZone destZone, ZoneTransform? zoneToZoneTransform = null)
        {
            LinkDestinations.Clear();

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
                    //copy point properties
                    Dictionary<string, dynamic> newPointProperties = new Dictionary<string, dynamic>();

                    foreach (var property in point.Properties)
                        newPointProperties.Add(property.Key, property.Value);

                    newPoints.Add(new RailPoint(point.Position, point.ControlPoint1, point.ControlPoint2, newPointProperties));
                }
            }

            duplicates[this] = new Rail(newPoints, destZone?.NextRailID(), ClassName, Closed, IsLadder, 
                ObjectUtils.DuplicateLinks(Links), ObjectUtils.DuplicateProperties(Properties), destZone, Layer);
        }

        public void LinkDuplicates(SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate)
        {
            //We don't expect Rails to have Links
        }

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            return zone.ObjLists.TryGetValue("Map_Rails", out objList);
        }

        public void AddToZoneBatch(ZoneRenderBatch zoneBatch)
        {
            //TODO figure out if this is needed or not
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

            General3dWorldObject.ExtraPropertiesUIContainer pathPropertyContainer = new General3dWorldObject.ExtraPropertiesUIContainer(Properties, scene);

            objectUIControl.AddObjectUIContainer(new RailUIContainer(this, scene, pointPropertyContainer, pathPropertyContainer), "Rail");
            
            objectUIControl.AddObjectUIContainer(pathPropertyContainer, "Rail Properties");

            if (points.Count == 1)
            {
                objectUIControl.AddObjectUIContainer(new SinglePathPointUIContainer(points[0], scene), "Rail Point");
                objectUIControl.AddObjectUIContainer(pointPropertyContainer, "Point Properties");
            }

            if (Links != null)
                objectUIControl.AddObjectUIContainer(new General3dWorldObject.LinksUIContainer(this, scene), "Links");

            if (LinkDestinations.Count > 0)
                objectUIControl.AddObjectUIContainer(new General3dWorldObject.LinkDestinationsUIContainer(this, (SM3DWorldScene)scene), "Link Destinations");

            return true;
        }

        public class RailUIContainer : IObjectUIContainer
        {
            PropertyCapture? pathCapture = null;

            Rail rail;
            readonly EditorSceneBase scene;
            string[] DB_classNames;
            private General3dWorldObject.LayerUIField layerUIField;
            General3dWorldObject.ExtraPropertiesUIContainer pathPointPropertyContainer;
            General3dWorldObject.ExtraPropertiesUIContainer pathPropertyContainer;

            public RailUIContainer(Rail rail, EditorSceneBase scene, General3dWorldObject.ExtraPropertiesUIContainer pathPointPropertyContainer, General3dWorldObject.ExtraPropertiesUIContainer pathPropertyContainer)
            {
                this.rail = rail;

                this.scene = scene;

                this.pathPointPropertyContainer = pathPointPropertyContainer;
                this.pathPropertyContainer = pathPropertyContainer;

                DB_classNames = Program.ParameterDB.RailParameters.Keys.ToArray();

                layerUIField = new General3dWorldObject.LayerUIField((SM3DWorldScene) scene, rail);
            }

            public void DoUI(IObjectUIControl control)
            {
                if (Spotlight.Properties.Settings.Default.AllowIDEdits)
                    rail.ID = control.TextInput(rail.ID, "Rail ID");
                else
                    control.TextInput(rail.ID, "Rail ID");

                if (rail.comment != null)
                    control.TextInput(rail.comment, "Comment");

                rail.ClassName = control.DropDownTextInput("Class Name", rail.ClassName, DB_classNames, false);

                rail.IsLadder = control.CheckBox("Is Ladder", rail.IsLadder);

                rail.Closed = control.CheckBox("Closed", rail.Closed);

                layerUIField.DoUI(control);

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
                pathCapture?.HandleUndo(scene);

                pathCapture = null;

                layerUIField.OnValueSet();

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
