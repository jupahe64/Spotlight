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
    interface I3dWorldObject : IEditableObject
    {
         void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest);
         Vector3 GetLinkingPoint(I3dWorldObject other);
    }
}
