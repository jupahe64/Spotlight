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
                foreach (var (linkName, link) in links)
                {
                    newLinks[linkName] = new List<I3dWorldObject>();
                    foreach (I3dWorldObject obj in link)
                    {
                        newLinks[linkName].Add(obj);
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

            foreach (KeyValuePair<string, dynamic> property in properties)
                newProperties[property.Key] = property.Value;

            return newProperties;
        }

        public static void LinkDuplicatesAndAddLinkDestinations(I3dWorldObject self, SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate)
        {
            if (self.Links != null)
            {
                bool isDuplicate = duplicationInfo.IsDuplicate(self);

                bool hasDuplicate = duplicationInfo.HasDuplicate(self);

                foreach (var (linkName, link) in self.Links)
                {
                    I3dWorldObject[] oldLink = link.ToArray();

                    //Clear Link
                    link.Clear();

                    //Populate Link
                    foreach (I3dWorldObject linkedObj in oldLink)
                    {
                        bool linkedObjHasDuplicate = duplicationInfo.TryGetDuplicate(linkedObj, out I3dWorldObject duplicate);

                        if (!(isDuplicate && linkedObjHasDuplicate) && !(isDuplicate && !allowKeepLinksOfDuplicate))
                        {
                            //Link to original
                            link.Add(linkedObj);
                            linkedObj.AddLinkDestination(linkName, self);
                        }

                        if (linkedObjHasDuplicate && (hasDuplicate == isDuplicate))
                        {
                            //Link to duplicate
                            link.Add(duplicate);
                            duplicate.AddLinkDestination(linkName, self);
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
