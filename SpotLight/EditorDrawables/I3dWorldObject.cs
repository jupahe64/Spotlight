using BYAML;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;

namespace SpotLight.EditorDrawables
{
    /// <summary>
    /// Interface object for SM3DW Objects
    /// </summary>
    public interface I3dWorldObject : IEditableObject
    {
        bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList);

        IReadOnlyList<(string, I3dWorldObject)> LinkDestinations { get; }
        void ClearLinkDestinations();
        void AddLinkDestinations();
        void AddLinkDestination(string linkName, I3dWorldObject linkingObject);

        void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldScene scene, ZoneTransform? zoneToZoneTransform = null, bool deselectOld = true);
        void LinkDuplicatesAndAddLinkDestinations(SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate);

        Dictionary<string, List<I3dWorldObject>> Links { get; set; }
        void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest);
        Vector3 GetLinkingPoint(SM3DWorldScene editorScene);

        void AddToZoneBatch(ZonePlacement zonePlacement);
    }
}
