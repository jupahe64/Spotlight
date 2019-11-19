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
    public class Rail : Path, I3dWorldObject
    {
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

        protected static List<PathPoint> PathPointsFromRailPointsEntry(ByamlIterator.DictionaryEntry railPointsEntry)
        {
            List<PathPoint> pathPoints = new List<PathPoint>();

            foreach (ByamlIterator.ArrayEntry pointEntry in railPointsEntry.IterArray())
            {
                Vector3 pos = new Vector3();
                Vector3 cp1 = new Vector3();
                Vector3 cp2 = new Vector3();
                foreach (ByamlIterator.DictionaryEntry entry in pointEntry.IterDictionary())
                {
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
                }
                pathPoints.Add(new RailPoint(pos, cp1 - pos, cp2 - pos));
            }

            return pathPoints;
        }

        /// <summary>
        /// Id of this object
        /// </summary>
        public string ID { get; }

        [Undoable]
        public bool IsLadder { get; set; }

        public Rail(LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks)
            : base(PathPointsFromRailPointsEntry(info.PropertyEntries["RailPoints"]))
        {
            ID = info.ID;
            if (zone != null)
            zone.SubmitRailID(ID);

            IsLadder = info.PropertyEntries["IsLadder"].Parse();

            Closed = info.PropertyEntries["IsClosed"].Parse();

            loadLinks = false; //We don't expect Rails to have Links
        }

        public Rail(List<PathPoint> pathPoints, string iD, bool isClosed, bool isLadder)
            : base(pathPoints)
        {
            ID = iD;
            Closed = isClosed;
            IsLadder = isLadder;
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
            objNode.AddDynamicValue("IsLinkDest", isLinkDest);
            objNode.AddDynamicValue("LayerConfigName", "Common");

            alreadyWrittenObjs.Add(this);
            
            objNode.AddDictionaryNodeRef("Links", writer.CreateDictionaryNode(), true); //We don't expect Rails to have Links

            objNode.AddDynamicValue("ModelName", null);

            #region Save RailPoints
            ArrayNode railPointsNode = writer.CreateArrayNode();

            int i = 0;
            foreach (PathPoint point in PathPoints)
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

                railPointsNode.AddDictionaryNodeRef(pointNode, true);

                i++;
            }

            objNode.AddArrayNodeRef("RailPoints", railPointsNode);
            #endregion

            objNode.AddDynamicValue("RailType", "Linear");

            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Vector3.Zero), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(PathPoints[0].Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig("Rail"), true);

            objNode.AddDynamicValue("UnitConfigName", "Rail");
        }

        public virtual Vector3 GetLinkingPoint()
        {
            return PathPoints[0].Position;
        }

        public void ClearLinkDestinations()
        {
            linkDestinations.Clear();
        }

        public void AddLinkDestinations()
        {
            if (Links != null)
            {
                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
                {
                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        obj.AddLinkDestination(keyValuePair.Key, this);
                    }
                }
            }
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

            List<PathPoint> newPoints = new List<PathPoint>();

            foreach (PathPoint point in pathPoints)
            {
                if (point.Selected)
                {
                    newPoints.Add(new PathPoint(point.Position, point.ControlPoint1, point.ControlPoint2));
                }
            }

            duplicates[this] = new Rail(newPoints, zone.NextRailID(), Closed, IsLadder);

            var duplicate = duplicates[this];

            DeselectAll(scene.GL_Control);
            
            duplicates[this].Prepare((GL_EditorFramework.GL_Core.GL_ControlModern)scene.GL_Control);

            duplicates[this].SelectDefault(scene.GL_Control);
        }

        public void LinkDuplicatesAndAddLinkDestinations(SM3DWorldScene.DuplicationInfo duplicationInfo)
        {
            if (Links != null)
            {
                bool isDuplicate = duplicationInfo.IsDuplicate(this);

                bool hasDuplicate = duplicationInfo.HasDuplicate(this);

                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
                {
                    I3dWorldObject[] oldLink = keyValuePair.Value.ToArray();

                    //Clear Link
                    keyValuePair.Value.Clear();

                    //Populate Link
                    foreach (I3dWorldObject obj in oldLink)
                    {
                        bool objHasDuplicate = duplicationInfo.TryGetDuplicate(obj, out I3dWorldObject duplicate);

                        if (!(isDuplicate && objHasDuplicate))
                        {
                            //Link to original
                            keyValuePair.Value.Add(obj);
                            obj.AddLinkDestination(keyValuePair.Key, this);
                        }

                        if (objHasDuplicate && (hasDuplicate == isDuplicate))
                        {
                            //Link to duplicate
                            keyValuePair.Value.Add(duplicate);
                            duplicate.AddLinkDestination(keyValuePair.Key, this);
                        }
                    }
                }
            }
        }
        #endregion


        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            bool any = false;

            foreach (PathPoint point in pathPoints)
                any |= point.Selected;


            if (!any)
                return false;

            objectUIControl.AddObjectUIContainer(new RailUIContainer(this, scene), "Path");

            List<PathPoint> points = new List<PathPoint>();

            foreach (PathPoint point in pathPoints)
            {
                if (point.Selected)
                    points.Add(point);
            }

            if (points.Count == 1)
                objectUIControl.AddObjectUIContainer(new SinglePathPointUIContainer(points[0], scene), "Path Point");

            return true;
        }

        public class RailUIContainer : IObjectUIContainer
        {
            PropertyCapture? pathCapture = null;

            Rail rail;
            readonly EditorSceneBase scene;
            public RailUIContainer(Rail rail, EditorSceneBase scene)
            {
                this.rail = rail;

                List<PathPoint> points = new List<PathPoint>();

                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                rail.IsLadder = control.CheckBox("Is Ladder", rail.IsLadder);

                rail.Closed = control.CheckBox("Closed", rail.Closed);

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

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }
    }

    public class RailPoint : PathPoint
    {
        public override Vector3 GlobalPosition
        {
            get => Vector4.Transform(new Vector4(Position, 1), SM3DWorldScene.IteratedZoneTransform.PositionTransform).Xyz;
            set => Position = Vector4.Transform(new Vector4(value, 1), SM3DWorldScene.IteratedZoneTransform.PositionTransform.Inverted()).Xyz;
        }

        public override Vector3 GlobalCP1
        {
            get => Vector3.Transform(ControlPoint1, SM3DWorldScene.IteratedZoneTransform.RotationTransform);
            set => ControlPoint1 = Vector3.Transform(value, SM3DWorldScene.IteratedZoneTransform.RotationTransform.Inverted());
        }

        public override Vector3 GlobalCP2
        {
            get => Vector3.Transform(ControlPoint2, SM3DWorldScene.IteratedZoneTransform.RotationTransform);
            set => ControlPoint2 = Vector3.Transform(value, SM3DWorldScene.IteratedZoneTransform.RotationTransform.Inverted());
        }

        public RailPoint(Vector3 position, Vector3 controlPoint1, Vector3 controlPoint2)
            : base(position, controlPoint1, controlPoint2)
        {

        }
    }
}
