using BYAML;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZS;

namespace SpotLight.EditorDrawables
{
    class SM3DWorldScene : CategorizedScene
    {
        /// <summary>
        /// Creates a blank SM3DW Scene
        /// </summary>
        public SM3DWorldScene()
        {
            
        }
        /// <summary>
        /// Prepares to draw models
        /// </summary>
        /// <param name="control">The GL_Control that's currently in use</param>
        public override void Prepare(GL_ControlModern control)
        {
            BfresModelCache.Initialize(control);
            
            base.Prepare(control);
        }

        public List<I3dWorldObject> linkedObjects = new List<I3dWorldObject>();
        /// <summary>
        /// Gets all the editable objects
        /// </summary>
        /// <returns><see cref="IEnumerable{IEditableObject}"/></returns>
        protected override IEnumerable<IEditableObject> GetObjects()
        {
            foreach (List<IEditableObject> objects in categories.Values)
            {
                foreach (IEditableObject obj in objects)
                    yield return obj;
            }

            foreach (I3dWorldObject obj in linkedObjects)
                yield return obj;
        }
        /// <summary>
        /// Deletes the selected object from the level
        /// </summary>
        public override void DeleteSelected()
        {
            DeletionManager manager = new DeletionManager();

            foreach (List<IEditableObject> objects in categories.Values)
            {
                foreach (IEditableObject obj in objects)
                    obj.DeleteSelected(manager, objects, CurrentList);
            }

            foreach (I3dWorldObject obj in linkedObjects)
                obj.DeleteSelected(manager, linkedObjects, CurrentList);

            _ExecuteDeletion(manager);
        }
        /// <summary>
        /// Saves the scene to a BYAML for saving
        /// </summary>
        /// <param name="writer">The current BYAML Node Writer</param>
        /// <param name="levelName">The name of the Level</param>
        /// <param name="categoryName">Category to save in</param>
        public void Save(ByamlNodeWriter writer, string levelName, string categoryName)
        {
            ByamlNodeWriter.DictionaryNode rootNode = writer.CreateDictionaryNode(categories);

            ByamlNodeWriter.ArrayNode objsNode = writer.CreateArrayNode();

            HashSet<I3dWorldObject> alreadyWrittenObjs = new HashSet<I3dWorldObject>();
            
            rootNode.AddDynamicValue("FilePath", $"D:/home/TokyoProject/RedCarpet/Asset/StageData/{levelName}/Map/{levelName}{categoryName}.muunt");

            foreach (KeyValuePair<string, List<IEditableObject>> keyValuePair in categories)
            {
                ByamlNodeWriter.ArrayNode categoryNode = writer.CreateArrayNode(keyValuePair.Value);

                foreach (I3dWorldObject obj in keyValuePair.Value)
                {
                    if (!alreadyWrittenObjs.Contains(obj))
                    {
                        ByamlNodeWriter.DictionaryNode objNode = writer.CreateDictionaryNode(obj);
                        obj.Save(alreadyWrittenObjs, writer, objNode, false);
                        categoryNode.AddDictionaryNodeRef(objNode);
                        objsNode.AddDictionaryNodeRef(objNode);
                    }
                    else
                    {
                        categoryNode.AddDictionaryRef(obj);
                        objsNode.AddDictionaryRef(obj);
                    }
                }
                rootNode.AddArrayNodeRef(keyValuePair.Key, categoryNode, true);
            }

            rootNode.AddArrayNodeRef("Objs", objsNode);

            writer.Write(rootNode, true);
        }
    }
}
