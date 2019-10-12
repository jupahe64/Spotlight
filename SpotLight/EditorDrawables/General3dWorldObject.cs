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
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using WinInput = System.Windows.Input;

namespace SpotLight.EditorDrawables
{
    /// <summary>
    /// General object for SM3DW
    /// </summary>
    class General3dWorldObject : TransformableObject, I3dWorldObject
    {
        public new static Vector4 selectColor = new Vector4(EditableObject.selectColor.Xyz, 0.5f);
        public new static Vector4 hoverColor = new Vector4(EditableObject.hoverColor.Xyz, 0.125f);

        protected static Vector4 LinkColor = new Vector4(1f, 0.25f, 0f, 1f);

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

        public static Dictionary<string, dynamic> CreateUnitConfig(General3dWorldObject obj) => new Dictionary<string, dynamic>
        {
            ["DisplayName"]         = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
            ["DisplayRotate"]       = Vector3ToDict(obj.displayRotation),
            ["DisplayScale"]        = Vector3ToDict(obj.displayScale),
            ["DisplayTranslate"]    = Vector3ToDict(obj.displayTranslation, 100f),
            ["GenerateCategory"]    = "",
            ["ParameterConfigName"] = obj.ClassName,
            ["PlacementTargetFile"] = "Map"
        };

        /// <summary>
        /// Id of this object
        /// </summary>
        readonly string ID;
        /// <summary>
        /// Base Object name. Can be used as the Model Name, if the ModelName is not overriding this.
        /// </summary>
        public string ObjectName;
        /// <summary>
        /// Overridden model to be used by this object
        /// </summary>
        public string ModelName;
        /// <summary>
        /// Internal name of this object
        /// </summary>
        public string ClassName;

        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public List<(string, I3dWorldObject)> LinkDestinations { get; } = new List<(string, I3dWorldObject)>();

        Vector3 displayTranslation;
        Vector3 displayRotation;
        Vector3 displayScale;

        public Dictionary<string, List<I3dWorldObject>> links = null;
        public Dictionary<string, dynamic> properties;

        private static readonly Dictionary<string, List<I3dWorldObject>> EMPTY_LINKS = new Dictionary<string, List<I3dWorldObject>>();
        /// <summary>
        /// Gets the Object Name
        /// </summary>
        /// <returns>The name of the Object</returns>
        public override string ToString() => ObjectName;

        public void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, bool isLinkDest = false)
        {
            objNode.AddDynamicValue("Comment", null);
            objNode.AddDynamicValue("Id", ID);
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

            objNode.AddDynamicValue("ModelName", ModelName);
            objNode.AddDynamicValue("Rotate", Vector3ToDict(rotation), true);
            objNode.AddDynamicValue("Scale", Vector3ToDict(scale), true);
            objNode.AddDynamicValue("Translate", Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig(this), true);

            objNode.AddDynamicValue("UnitConfigName", ObjectName);

            if (properties != null)
            {
                foreach (KeyValuePair<string, dynamic> keyValuePair in properties)
                    objNode.AddDynamicValue(keyValuePair.Key, keyValuePair.Value, true);
            }
        }
        /// <summary>
        /// Create a General SM3DW Object
        /// </summary>
        /// <param name="objectEntry">Unknown</param>
        /// <param name="linkedObjs">List of objects that are linked with this object</param>
        /// <param name="objectsByReference">Unknown</param>
        public General3dWorldObject(ByamlIterator.ArrayEntry objectEntry, List<I3dWorldObject> linkedObjs, Dictionary<long, I3dWorldObject> objectsByReference) : base(Vector3.Zero, Vector3.Zero, Vector3.One)
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
                        ID = entry.Parse();
                        break;
                    case "Links":
                        links = new Dictionary<string, List<I3dWorldObject>>();
                        foreach (ByamlIterator.DictionaryEntry link in entry.IterDictionary())
                        {
                            links.Add(link.Key, new List<I3dWorldObject>());
                            foreach (ByamlIterator.ArrayEntry linked in link.IterArray())
                            {
                                if (objectsByReference.ContainsKey(linked.Position))
                                {
                                    links[link.Key].Add(objectsByReference[linked.Position]);
                                    objectsByReference[linked.Position].LinkDestinations.Add((link.Key, this));
                                }
                                else
                                {
                                    I3dWorldObject obj = new General3dWorldObject(linked, linkedObjs, objectsByReference);
                                    obj.LinkDestinations.Add((link.Key, this));
                                    links[link.Key].Add(obj);
                                    linkedObjs.Add(obj);
                                }
                            }
                        }
                        break;
                    case "ModelName":
                        ModelName = entry.Parse();
                        break;
                    case "Rotate":
                        dynamic _data = entry.Parse();
                        rotation = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
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
                        ObjectName = entry.Parse();
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
                        ClassName = _data["ParameterConfigName"];
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
        /// <summary>
        /// Deletes this object from the Scene
        /// </summary>
        /// <param name="manager">Unknown</param>
        /// <param name="list">Unknown</param>
        /// <param name="currentList">Unknown</param>
        public override void DeleteSelected(EditorSceneBase.DeletionManager manager, IList list, IList currentList)
        {
            base.DeleteSelected(manager, list, currentList);

            //if (links != null)
            //{
            //    foreach (List<I3dWorldObject> link in links.Values)
            //    {
            //        foreach (I3dWorldObject obj in link)
            //            obj.DeleteSelected(manager, link, currentList);
            //    }
            //}
        }
        /// <summary>
        /// Prepares to draw this Object
        /// </summary>
        /// <param name="control">The GL_Control to draw to</param>
        public override void Prepare(GL_ControlModern control)
        {
            string mdlName = ModelName ?? ObjectName;
            if(File.Exists(Program.ObjectDataPath + mdlName + ".szs"))
            {
                SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Program.ObjectDataPath + mdlName + ".szs"));

                if(objArc.Files.ContainsKey(mdlName + ".bfres"))
                {
                    if(objArc.Files.ContainsKey("InitModel.byml"))
                    {
                        dynamic initModel = ByamlFile.FastLoadN(new MemoryStream(objArc.Files["InitModel.byml"]), false, Syroot.BinaryData.Endian.Big).RootNode;

                        if(initModel is Dictionary<string, dynamic>)
                        {
                            BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), control,
                            initModel.TryGetValue("TextureArc", out dynamic texArc) ? texArc : null);
                            base.Prepare(control);
                            return;
                        }
                    }
                    BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), control, null);
                }
            }
            base.Prepare(control);
        }
        /// <summary>
        /// Draws the model to the given GL_Control
        /// </summary>
        /// <param name="control">The GL_Control to draw to</param>
        /// <param name="pass">The current pass of drawing</param>
        /// <param name="editorScene">The current Editor Scene</param>
        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            if (!editorScene.ShouldBeDrawn(this))
                return;

            bool hovered = editorScene.Hovered == this;

            Matrix3 rotMtx = Framework.Mat3FromEulerAnglesDeg(rotation);

            if (BfresModelCache.Contains(ModelName ?? ObjectName))
            {
                control.UpdateModelMatrix(
                    Matrix4.CreateScale(displayScale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(displayRotation)) *
                    Matrix4.CreateTranslation(displayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale)) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));

                Vector4 highlightColor;

                if (Selected)
                    highlightColor = selectColor;
                else if(hovered)
                    highlightColor = hoverColor;
                else 
                    highlightColor = Vector4.Zero;

                BfresModelCache.TryDraw(ModelName ?? ObjectName, control, pass, highlightColor);
                
                goto RENDER_LINKS;
            }
            else
            {
                control.UpdateModelMatrix(
                    Matrix4.CreateScale(displayScale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(displayRotation)) *
                    Matrix4.CreateTranslation(displayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(scale) : scale) * 0.5f) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));
            }

            Vector4 blockColor;
            Vector4 lineColor;
            Vector4 col = LinkDestinations.Count > 0 ? LinkColor : Color;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = col;

            if (hovered && Selected)
                blockColor = col * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = col * 0.5f + selectColor * 0.5f;
            else
                blockColor = col;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());

            RENDER_LINKS:
            control.ResetModelMatrix();

            if (links != null && pass == Pass.OPAQUE)
            {
                control.CurrentShader = Renderers.ColorBlockRenderer.SolidColorShaderProgram;
                control.CurrentShader.SetVector4("color", Vector4.One);

                GL.LineWidth(1);
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
                GL.LineWidth(2);
            }
        }

        public Vector3 GetLinkingPoint(I3dWorldObject other)
        {
            return Position;
        }

        public override IObjectUIProvider GetPropertyProvider(EditorSceneBase scene) => new PropertyProvider(this, scene);

        public new class PropertyProvider : IObjectUIProvider
        {
            Vector3 prevPos;
            Vector3 prevRot;
            Vector3 prevScale;

            TransformableObject obj;
            EditorSceneBase scene;
            public PropertyProvider(TransformableObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.Position = control.Vector3Input(obj.Position, "Position", 1, 16);
                else
                    obj.Position = control.Vector3Input(obj.Position, "Position", 0.125f, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.rotation = control.Vector3Input(obj.rotation, "Rotation", 45, 18);
                else
                    obj.rotation = control.Vector3Input(obj.rotation, "Rotation", 5, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.scale = control.Vector3Input(obj.scale, "Scale", 1, 16);
                else
                    obj.scale = control.Vector3Input(obj.scale, "Scale", 0.125f, 2);
            }

            public void OnValueChangeStart()
            {
                prevPos = obj.Position;
                prevRot = obj.rotation;
                prevScale = obj.scale;
            }

            public void OnValueChanged()
            {
                scene.Refresh();
            }

            public void OnValueSet()
            {
                if (prevPos != obj.Position)
                    scene.AddToUndo(new RevertableFieldChange(SingleObject.FI_Position, obj, prevPos));
                if (prevRot != obj.rotation)
                    scene.AddToUndo(new RevertableFieldChange(TransformableObject.FI_Rotation, obj, prevRot));
                if (prevScale != obj.scale)
                    scene.AddToUndo(new RevertableFieldChange(TransformableObject.FI_Scale, obj, prevScale));

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }
    }
}
