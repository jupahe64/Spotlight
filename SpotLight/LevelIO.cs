using BYAML;
using OpenTK;
using SpotLight.EditorDrawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotLight
{
    public class LevelIO
    {
        /// <summary>
        /// Parses a 3d World from an ArrayEntry
        /// </summary>
        /// <param name="objectEntry"></param>
        /// <param name="scene"></param>
        /// <param name="objectsByReference"></param>
        /// <returns></returns>
        public static I3dWorldObject ParseObject(ByamlIterator.ArrayEntry objectEntry, SM3DWorldScene scene, Dictionary<long, I3dWorldObject> objectsByReference)
        {
            Dictionary<string, ByamlIterator.DictionaryEntry> properties = new Dictionary<string, ByamlIterator.DictionaryEntry>();

            ObjectBaseInfo info = new ObjectBaseInfo();

            foreach (ByamlIterator.DictionaryEntry entry in objectEntry.IterDictionary())
            {
                switch (entry.Key)
                {
                    case "Comment":
                    case "IsLinkDest":
                    case "LayerConfigName":
                        break; //ignore these
                    case "Id":
                        info.ID = entry.Parse();
                        if (scene != null)
                            scene.SubmitID(info.ID);
                        break;
                    case "Links":
                        info.LinksEntry = entry;
                        break;
                    case "ModelName":
                        info.ModelName = entry.Parse() ?? "";
                        break;
                    case "Rotate":
                        dynamic _data = entry.Parse();
                        info.Rotation = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Scale":
                        _data = entry.Parse();
                        info.Scale = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Translate":
                        _data = entry.Parse();
                        info.Position = new Vector3(
                            _data["X"] / 100f,
                            _data["Y"] / 100f,
                            _data["Z"] / 100f
                        );
                        break;
                    case "UnitConfigName":
                        info.ObjectName = entry.Parse();
                        break;
                    case "UnitConfig":
                        _data = entry.Parse();

                        info.DisplayTranslation = new Vector3(
                            _data["DisplayTranslate"]["X"] / 100f,
                            _data["DisplayTranslate"]["Y"] / 100f,
                            _data["DisplayTranslate"]["Z"] / 100f
                            );
                        info.DisplayRotation = new Vector3(
                            _data["DisplayRotate"]["X"],
                            _data["DisplayRotate"]["Y"],
                            _data["DisplayRotate"]["Z"]
                            );
                        info.DisplayScale = new Vector3(
                            _data["DisplayScale"]["X"],
                            _data["DisplayScale"]["Y"],
                            _data["DisplayScale"]["Z"]
                            );
                        info.ClassName = _data["ParameterConfigName"];
                        break;
                    default:
                        properties.Add(entry.Key, entry);
                        break;
                }
            }

            info.PropertyEntries = properties;

            I3dWorldObject obj;
            bool loadLinks;

            if (info.ClassName == "Rail")
                obj = new Rail(info, scene, out loadLinks);
            else
                obj = new General3dWorldObject(info, scene, out loadLinks);
            
            if (!objectsByReference.ContainsKey(objectEntry.Position))
                objectsByReference.Add(objectEntry.Position, obj);

            if (loadLinks)
            {
                obj.Links = new Dictionary<string, List<I3dWorldObject>>();
                foreach (ByamlIterator.DictionaryEntry link in info.LinksEntry.IterDictionary())
                {
                    obj.Links.Add(link.Key, new List<I3dWorldObject>());
                    foreach (ByamlIterator.ArrayEntry linked in link.IterArray())
                    {
                        if (objectsByReference.ContainsKey(linked.Position))
                        {
                            obj.Links[link.Key].Add(objectsByReference[linked.Position]);
                            objectsByReference[linked.Position].AddLinkDestination(link.Key, obj);
                        }
                        else
                        {
                            I3dWorldObject _obj = ParseObject(linked, scene, objectsByReference);
                            _obj.AddLinkDestination(link.Key, obj);
                            obj.Links[link.Key].Add(_obj);
                            if (scene != null)
                                scene.linkedObjects.Add(_obj);
                        }
                    }
                }
                if (obj.Links.Count == 0)
                    obj.Links = null;
            }

            return obj;
        }

        public struct ObjectBaseInfo
        {
            public string ID { get; set; }
            public ByamlIterator.DictionaryEntry LinksEntry { get; set; }
            public Dictionary<string, ByamlIterator.DictionaryEntry> PropertyEntries { get; set; }

            public string ObjectName { get; set; }
            public string ClassName { get; set; }
            public string ModelName { get; set; }

            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Scale { get; set; }

            public Vector3 DisplayTranslation { get; set; }
            public Vector3 DisplayRotation { get; set; }
            public Vector3 DisplayScale { get; set; }
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="Dictionary"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector) => new Dictionary<string, dynamic>
        {
            ["X"] = vector.X,
            ["Y"] = vector.Y,
            ["Z"] = vector.Z
        };

        /// <summary>
        /// Converts a Vector3 to a Dictionary
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector, float scaleFactor)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            vector *= scaleFactor;

            data["X"] = vector.X;
            data["Y"] = vector.Y;
            data["Z"] = vector.Z;

            return data;
        }
    }
}
