using GL_EditorFramework;
using OpenTK;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlIterator;

namespace SpotLight.EditorDrawables
{
    public static class ObjectUtils
    {
        public static Dictionary<string, dynamic> CreateUnitConfig(General3dWorldObject obj) => new Dictionary<string, dynamic>
        {
            ["DisplayName"] = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
            ["DisplayRotate"] = LevelIO.Vector3ToDict(obj.DisplayRotation),
            ["DisplayScale"] = LevelIO.Vector3ToDict(obj.DisplayScale),
            ["DisplayTranslate"] = LevelIO.Vector3ToDict(obj.DisplayTranslation, 100f),
            ["GenerateCategory"] = "",
            ["ParameterConfigName"] = obj.ClassName,
            ["PlacementTargetFile"] = "Map"
        };

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


        public static Dictionary<string, List<I3dWorldObject>> DuplicateLinks(Dictionary<string, List<I3dWorldObject>> links)
        {
            Dictionary<string, List<I3dWorldObject>> newLinks;
            if (links != null)
            {
                newLinks = new Dictionary<string, List<I3dWorldObject>>();
                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in links)
                {
                    newLinks[keyValuePair.Key] = new List<I3dWorldObject>();
                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        newLinks[keyValuePair.Key].Add(obj);
                    }
                }
                return newLinks;
            }
            else
                return null;
        }

        public static Dictionary<string, dynamic> DuplicateProperties(Dictionary<string, dynamic> properties)
        {
            Dictionary<string, dynamic> newProperties = new Dictionary<string, dynamic>();

            foreach (KeyValuePair<string, dynamic> keyValuePair in properties)
                newProperties[keyValuePair.Key] = keyValuePair.Value;

            return newProperties;
        }

        public static void LinkDuplicatesAndAddLinkDestinations(I3dWorldObject self, SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate)
        {
            if (self.Links != null)
            {
                bool isDuplicate = duplicationInfo.IsDuplicate(self);

                bool hasDuplicate = duplicationInfo.HasDuplicate(self);

                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in self.Links)
                {
                    I3dWorldObject[] oldLink = keyValuePair.Value.ToArray();

                    //Clear Link
                    keyValuePair.Value.Clear();

                    //Populate Link
                    foreach (I3dWorldObject obj in oldLink)
                    {
                        bool objHasDuplicate = duplicationInfo.TryGetDuplicate(obj, out I3dWorldObject duplicate);

                        if (!(isDuplicate && objHasDuplicate) && !(isDuplicate && !allowKeepLinksOfDuplicate))
                        {
                            //Link to original
                            keyValuePair.Value.Add(obj);
                            obj.AddLinkDestination(keyValuePair.Key, self);
                        }

                        if (objHasDuplicate && (hasDuplicate == isDuplicate))
                        {
                            //Link to duplicate
                            keyValuePair.Value.Add(duplicate);
                            duplicate.AddLinkDestination(keyValuePair.Key, self);
                        }
                    }
                }
            }
        }


        public static Vector3 TransformedPosition(in Vector3 position, ZoneTransform? zoneToZoneTransform)
        {
            if (zoneToZoneTransform.HasValue)
                return (new Vector4(position, 1) * zoneToZoneTransform.Value.PositionTransform).Xyz;
            else
                return position;
        }

        public static Vector3 TransformedRotation(in Vector3 rotation, ZoneTransform? zoneToZoneTransform)
        {
            if (zoneToZoneTransform.HasValue)
                return Framework.ApplyRotation(rotation, zoneToZoneTransform.Value.RotationTransform);
            else
                return rotation;
        }
    }
}
