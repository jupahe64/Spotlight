using BYAML;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
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
        IReadOnlyList<(string, I3dWorldObject)> LinkDestinations { get; }
        void ClearLinkDestinations();
        void AddLinkDestinations();
        void AddLinkDestination(string linkName, I3dWorldObject linkingObject);

        void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldScene scene);
        void LinkDuplicatesAndAddLinkDestinations(SM3DWorldScene.DuplicationInfo duplicationInfo);

        Dictionary<string, List<I3dWorldObject>> Links { get; set; }
        void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest);
        Vector3 GetLinkingPoint();
    }
}
