using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZS;
using static BYAML.ByamlNodeWriter;

namespace SpotLight.EditorDrawables
{
    class General3dWorldObject : TransformableObject, I3dWorldObject
    {
        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector) => new Dictionary<string, dynamic>
        {
            ["X"] = vector.X,
            ["Y"] = vector.Y,
            ["Z"] = vector.Z
        };

        public static Dictionary<string, dynamic> Vector3ToDict(Vector3 vector, float scaleFactor)
        {
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            vector *= scaleFactor;

            data["X"] = vector.X;
            data["Y"] = vector.Y;
            data["Z"] = vector.Z;

            return data;
        }

        public static Dictionary<string, dynamic> CreateUnitConfig(General3dWorldObject obj) => new Dictionary<string, dynamic>
        {
            ["DisplayName"]         = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
            ["DisplayRotate"]       = Vector3ToDict(obj.displayRotation),
            ["DisplayScale"]        = Vector3ToDict(obj.displayScale),
            ["DisplayTranslate"]    = Vector3ToDict(obj.displayTranslation, 100f),
            ["GenerateCategory"]    = "",
            ["ParameterConfigName"] = obj.className,
            ["PlacementTargetFile"] = "Map"
        };

        readonly string id;

        public string objectName;
        public string modelName;
        public string className;

        Vector3 displayTranslation;
        Vector3 displayRotation;
        Vector3 displayScale;

        public Dictionary<string, List<I3dWorldObject>> links = null;
        public Dictionary<string, dynamic> properties;

        private static readonly Dictionary<string, List<I3dWorldObject>> EMPTY_LINKS = new Dictionary<string, List<I3dWorldObject>>();

        public override string ToString()
        {
            return objectName;
        }

        public void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest = false)
        {
            objNode.AddDynamicValue("Comment", null);
            objNode.AddDynamicValue("Id", id);
            objNode.AddDynamicValue("IsLinkDest", isLinkDest);
            objNode.AddDynamicValue("LayerConfigName", "Common");
            
            alreadyWrittenObjs.Add(this);

            if (links != null)
            {
                DictionaryNode linksNode = writer.CreateDictionaryNode(links);

                foreach (KeyValuePair<string,List<I3dWorldObject>> keyValuePair in links)
                {
                    ArrayNode linkNode = writer.CreateArrayNode(keyValuePair.Value);
                    
                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        if (!alreadyWrittenObjs.Contains(obj))
                        {
                            DictionaryNode linkedObjNode = writer.CreateDictionaryNode(obj);
                            obj.Save(alreadyWrittenObjs, writer, linkedObjNode, true);
                            linkNode.AddDictionaryNodeRef(linkedObjNode);
                        }
                        else
                            linkNode.AddDictionaryRef(obj);
                    }

                    linksNode.AddArrayNodeRef(keyValuePair.Key, linkNode, true);
                }
                objNode.AddDictionaryNodeRef("Links", linksNode, true);
            }
            else
            {
                objNode.AddDynamicValue("Links", new Dictionary<string, dynamic>(), true);
            }

            objNode.AddDynamicValue("ModelName", modelName);
            objNode.AddDynamicValue("Rotate", Vector3ToDict(rotation.ToEulerAnglesDeg()), true);
            objNode.AddDynamicValue("Scale", Vector3ToDict(scale), true);
            objNode.AddDynamicValue("Translate", Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig(this), true);

            objNode.AddDynamicValue("UnitConfigName", objectName);

            if (properties != null)
            {
                foreach (KeyValuePair<string, dynamic> keyValuePair in properties)
                    objNode.AddDynamicValue(keyValuePair.Key, keyValuePair.Value, true);
            }
        }

        public General3dWorldObject(ByamlIterator.ArrayEntry objectEntry, List<I3dWorldObject> linkedObjs, Dictionary<long, I3dWorldObject> objectsByReference)
          : base(
                Vector3.Zero,
                Quaternion.Identity,
                Vector3.One)
        {
            properties = new Dictionary<string, dynamic>();



            foreach (ByamlIterator.DictionaryEntry entry in objectEntry.IterDictionary())
            {
                if (!objectsByReference.ContainsKey(objectEntry.Position))
                    objectsByReference.Add(objectEntry.Position, this);

                switch (entry.Key)
                {
                    case "Comment":
                    case "IsLinkDest":
                    case "LayerConfigName":
                        break; //ignore these
                    case "Id":
                        id = entry.Parse();
                        break;
                    case "Links":
                        links = new Dictionary<string, List<I3dWorldObject>>();
                        foreach (ByamlIterator.DictionaryEntry link in entry.IterDictionary())
                        {
                            links.Add(link.Key, new List<I3dWorldObject>());
                            foreach (ByamlIterator.ArrayEntry linked in link.IterArray())
                            {
                                if(objectsByReference.ContainsKey(linked.Position))
                                    links[link.Key].Add(objectsByReference[linked.Position]);
                                else
                                {
                                    I3dWorldObject obj = new LinkedObj(linked, linkedObjs, objectsByReference);
                                    links[link.Key].Add(obj);
                                    linkedObjs.Add(obj);
                                }
                            }
                        }
                        break;
                    case "ModelName":
                        modelName = entry.Parse();
                        break;
                    case "Rotate":
                        dynamic _data = entry.Parse();
                        rotation = Framework.QFromEulerAnglesDeg(
                            new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                            )
                        );
                        break;
                    case "Scale":
                        _data = entry.Parse();
                        scale = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Translate":
                        _data = entry.Parse();
                        Position = new Vector3(
                            _data["X"] / 100f,
                            _data["Y"] / 100f,
                            _data["Z"] / 100f
                        );
                        break;
                    case "UnitConfigName":
                        objectName = entry.Parse();
                        break;
                    case "UnitConfig":
                        _data = entry.Parse();

                        displayTranslation = new Vector3(
                            _data["DisplayTranslate"]["X"] / 100f,
                            _data["DisplayTranslate"]["Y"] / 100f,
                            _data["DisplayTranslate"]["Z"] / 100f
                            );
                        displayRotation = new Vector3(
                            _data["DisplayRotate"]["X"],
                            _data["DisplayRotate"]["Y"],
                            _data["DisplayRotate"]["Z"]
                            );
                        displayScale = new Vector3(
                            _data["DisplayScale"]["X"],
                            _data["DisplayScale"]["Y"],
                            _data["DisplayScale"]["Z"]
                            );
                        className = _data["ParameterConfigName"];
                        break;
                    default:
                        properties.Add(entry.Key, entry.Parse());
                        break;
                }
            }

            if (properties.Count == 0)
                properties = null;

            if (links.Count == 0)
                links = null;
        }

        public override void DeleteSelected(EditorSceneBase.DeletionManager manager, IList list, IList currentList)
        {
            base.DeleteSelected(manager, list, currentList);

            if (links != null)
            {
                foreach (List<I3dWorldObject> link in links.Values)
                {
                    foreach (I3dWorldObject obj in link)
                        obj.DeleteSelected(manager, link, currentList);
                }
            }
        }

        public override void Prepare(GL_ControlModern control)
        {
            string mdlName = modelName ?? objectName;
            if(File.Exists(Program.ObjectDataPath + mdlName + ".szs"))
            {
                SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Program.ObjectDataPath + mdlName + ".szs"));

                if(objArc.Files.ContainsKey(mdlName + ".bfres"))
                {
                    BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), control);
                }
            }
            base.Prepare(control);
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            if (!editorScene.ShouldBeDrawn(this))
                return;

            bool hovered = editorScene.Hovered == this;

            if(BfresModelCache.Contains(modelName ?? objectName))
            {
                control.UpdateModelMatrix(
                Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale)) *
                Matrix4.CreateFromQuaternion(Selected ? editorScene.CurrentAction.NewRot(rotation) : rotation) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));
            }
            else
            {
                control.UpdateModelMatrix(
                Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale) * 0.5f) *
                Matrix4.CreateFromQuaternion(Selected ? editorScene.CurrentAction.NewRot(rotation) : rotation) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));
            }

            if (BfresModelCache.TryDraw(modelName ?? objectName, control))
                return;

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());

            control.ResetModelMatrix();

            if (links != null)
            {
                GL.Begin(PrimitiveType.Lines);
                foreach (List<I3dWorldObject> link in links.Values)
                {
                    foreach (I3dWorldObject obj in link)
                    {
                        GL.Vertex3(Position);
                        GL.Vertex3(obj.GetLinkingPoint(this));
                    }
                }
                GL.End();
            }
        }

        public Vector3 GetLinkingPoint(I3dWorldObject other)
        {
            return Position;
        }
    }

    class LinkedObj : General3dWorldObject
    {
        protected new static Vector4 Color = new Vector4(1f, 0.25f, 0f, 1f);

        public LinkedObj(ByamlIterator.ArrayEntry objectEntry, List<I3dWorldObject> linkedObjs, Dictionary<long, I3dWorldObject> objectsByReference)
            : base(objectEntry, linkedObjs, objectsByReference)
        {

        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            if (!editorScene.ShouldBeDrawn(this))
                return;

            bool hovered = editorScene.Hovered == this;

            control.UpdateModelMatrix(
                Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale) * 0.5f) *
                Matrix4.CreateFromQuaternion(Selected ? editorScene.CurrentAction.NewRot(rotation) : rotation) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());

            control.ResetModelMatrix();

            if (links != null)
            {
                GL.Begin(PrimitiveType.Lines);
                foreach (List<I3dWorldObject> link in links.Values)
                {
                    foreach (I3dWorldObject obj in link)
                    {
                        GL.Vertex3(Position);
                        GL.Vertex3(obj.GetLinkingPoint(this));
                    }
                }
                GL.End();
            }
        }
    }
}
