using BYAML;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;

namespace Spotlight.EditorDrawables
{
    /// <summary>
    /// Interface object for SM3DW Objects
    /// </summary>
    public interface I3dWorldObject : IEditableObject
    {
        bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList);

        List<(string, I3dWorldObject)> LinkDestinations { get; }
        void UpdateLinkDestinations_Clear();
        void UpdateLinkDestinations_Populate();
        void AddLinkDestination(string linkName, I3dWorldObject linkingObject);

        void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldZone destZone, ZoneTransform? zoneToZoneTransform = null);

        ///implemented via <see cref="ObjectUtils.LinkDuplicates"/> or not at all
        void LinkDuplicates(SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowLinkCopyToOrignal);

        Dictionary<string, List<I3dWorldObject>> Links { get; set; }
        void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, HashSet<string> layers, bool isLinkDest);
        Vector3 GetLinkingPoint(SM3DWorldScene editorScene);

        void AddToZoneBatch(ZoneRenderBatch zoneBatch);

        string Layer { get; set; }

        public string ID { get; }
    }
}
