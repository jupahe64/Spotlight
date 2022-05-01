using OpenTK;
using Spotlight.Database;
using Spotlight.EditorDrawables;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Spotlight.GUI
{
    public partial class AddObjectForm : Form
    {
        private bool TryGetPlacementHandler(out SM3DWorldScene.ObjectPlacementHandler placementHandler, out string text)
        {
            #region Local Handlers and variables
            ObjectParam objectParam;
            string objectName;
            string modelName;

            (I3dWorldObject, ObjectList)[] PlaceObject(Vector3 pos, SM3DWorldZone zone)
            {
                General3dWorldObject obj = objectParam.ToGeneral3DWorldObject(zone.NextObjID(), zone, pos,
                    objectName,
                    modelName);

                if (objectParam.TryGetObjectList(zone, out ObjectList objList))
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (obj, objList)
                    };
                }
                else
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (obj, zone.LinkedObjects)
                    };
                }
            }

            RailParam railParam;
            PathPoint[] points;
            bool closeRail;

            (I3dWorldObject, ObjectList)[] PlaceRail(Vector3 pos, SM3DWorldZone zone)
            {
                List<RailPoint> pathPoints = points.Select(p=>new RailPoint(p.Position+pos, p.ControlPoint1, p.ControlPoint2)).ToList();

                Dictionary<string, List<I3dWorldObject>> links = new Dictionary<string, List<I3dWorldObject>>();

                var properties = new Dictionary<string, dynamic>();

                ObjectParameterDatabase.AddToProperties(railParam.Properties, properties);
                ObjectParameterDatabase.AddToLinks(railParam.LinkNames, links);

                Rail rail = new Rail(pathPoints, zone.NextObjID(), railParam.ClassName, closeRail, false, links, properties, zone, zone.CommonLayer);


                if (zone.ObjLists.ContainsKey("Map_Rails"))
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (rail, zone.ObjLists["Map_Rails"])
                    };
                }
                else
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (rail, zone.LinkedObjects)
                    };
                }
            }

            AreaParam areaParam;
            string areaShape;

            (I3dWorldObject, ObjectList)[] PlaceArea(Vector3 pos, SM3DWorldZone zone)
            {
                var properties = new Dictionary<string, dynamic>();

                ObjectParameterDatabase.AddToProperties(areaParam.Properties, properties);

                var links = new Dictionary<string, List<I3dWorldObject>>();

                ObjectParameterDatabase.AddToLinks(areaParam.LinkNames, links);

                if (links.Count == 0)
                    links = null;

                AreaObject obj = new AreaObject(pos, Vector3.Zero, Vector3.One, zone.NextObjID(), areaShape, areaParam.ClassName, -1, links, properties, zone, zone.CommonLayer);


                if (areaParam.TryGetObjectList(zone, out ObjectList objList))
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (obj, objList)
                    };
                }
                else
                {
                    return new (I3dWorldObject, ObjectList)[] {
                        (obj, zone.LinkedObjects)
                    };
                }
            }
            #endregion

            placementHandler = null;
            text = null;

            if (ObjectTypeTabControl.SelectedTab == ObjectsTab && selectedParameter is ObjectParam _objectParam)
            {
                objectParam = _objectParam;
                objectName = ObjectNameTextBox.Text;
                modelName = ModelNameTextBox.Text;

                text = objectParam.ClassName + "\n" + objectParam.GetEnglishName() + '\n' + objectName +
                    (string.IsNullOrEmpty(modelName) ? string.Empty : '\n' + "Mdl: " + modelName);

                placementHandler = PlaceObject;
                return true;

            }

            if (ObjectTypeTabControl.SelectedTab == RailsTab && selectedParameter is RailParam _railParam)
            {
                railParam = _railParam;

                closeRail = PathShapeSelector.SelectedShape.Closed;

                points = PathShapeSelector.SelectedShape.PathPoints;

                text = PathShapeSelector.SelectedShape.Name + '\n' + railParam.ClassName + '\n' + (closeRail ? "Closed" : "Open");

                placementHandler = PlaceRail;
                return true;
            }

            if (ObjectTypeTabControl.SelectedTab == AreasTab && selectedParameter is AreaParam _areaParam)
            {
                areaParam = _areaParam;
                areaShape = (string)AreaShapeComboBox.SelectedItem;

                text = areaParam.ClassName + "\n" + areaParam.GetEnglishName() +
                    (string.IsNullOrEmpty(areaShape) ? string.Empty : '\n' + "Shp: " + areaShape);

                placementHandler = PlaceArea;
                return true;

            }

            return false;
        }
    }

    public struct PathPoint
    {
        public readonly Vector3 Position;
        public readonly Vector3 ControlPoint1;
        public readonly Vector3 ControlPoint2;

        public PathPoint(Vector3 position, Vector3 controlPoint1 = new Vector3(), Vector3 controlPoint2 = new Vector3())
        {
            Position = position;
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;
        }
    }
}