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
using System.Reflection;
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

        protected static Vector4 LinkColor = new Vector4(0f, 1f, 1f, 1f);

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
            ["DisplayRotate"]       = Vector3ToDict(obj.DisplayRotation),
            ["DisplayScale"]        = Vector3ToDict(obj.DisplayScale),
            ["DisplayTranslate"]    = Vector3ToDict(obj.DisplayTranslation, 100f),
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
        [PropertyCapture.Undoable]
        public string ObjectName { get; set; }
        /// <summary>
        /// Overridden model to be used by this object
        /// </summary>
        [PropertyCapture.Undoable]
        public string ModelName { get; set; }
        /// <summary>
        /// Internal name of this object
        /// </summary>
        [PropertyCapture.Undoable]
        public string ClassName { get; set; }

        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public IReadOnlyList<(string, I3dWorldObject)> LinkDestinations { get => linkDestinations; }


        List<(string, I3dWorldObject)> linkDestinations = new List<(string, I3dWorldObject)>();

        [PropertyCapture.Undoable]
        public Vector3 DisplayTranslation { get; set; }
        [PropertyCapture.Undoable]
        public Vector3 DisplayRotation { get; set; }
        [PropertyCapture.Undoable]
        public Vector3 DisplayScale { get; set; }

        public Dictionary<string, List<I3dWorldObject>> Links { get; } = null;
        public Dictionary<string, dynamic> Properties { get; }

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

            if (Links != null)
            {
                DictionaryNode linksNode = writer.CreateDictionaryNode(Links);

                foreach (KeyValuePair<string,List<I3dWorldObject>> keyValuePair in Links)
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

            objNode.AddDynamicValue("ModelName", (ModelName=="")?null:ModelName);
            objNode.AddDynamicValue("Rotate", Vector3ToDict(Rotation), true);
            objNode.AddDynamicValue("Scale", Vector3ToDict(Scale), true);
            objNode.AddDynamicValue("Translate", Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig(this), true);

            objNode.AddDynamicValue("UnitConfigName", ObjectName);

            if (Properties != null)
            {
                foreach (KeyValuePair<string, dynamic> keyValuePair in Properties)
                    objNode.AddDynamicValue(keyValuePair.Key, keyValuePair.Value, true);
            }
        }
        /// <summary>
        /// Create a General SM3DW Object
        /// </summary>
        /// <param name="objectEntry">Unknown</param>
        /// <param name="linkedObjs">List of objects that are linked with this object</param>
        /// <param name="objectsByReference">Unknown</param>
        public General3dWorldObject(ByamlIterator.ArrayEntry objectEntry, SM3DWorldScene scene, Dictionary<long, I3dWorldObject> objectsByReference) : base(Vector3.Zero, Vector3.Zero, Vector3.One)
        {
            Properties = new Dictionary<string, dynamic>();
            
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
                        scene.SubmitID(ID);
                        break;
                    case "Links":
                        Links = new Dictionary<string, List<I3dWorldObject>>();
                        foreach (ByamlIterator.DictionaryEntry link in entry.IterDictionary())
                        {
                            Links.Add(link.Key, new List<I3dWorldObject>());
                            foreach (ByamlIterator.ArrayEntry linked in link.IterArray())
                            {
                                if (objectsByReference.ContainsKey(linked.Position))
                                {
                                    Links[link.Key].Add(objectsByReference[linked.Position]);
                                    objectsByReference[linked.Position].AddLinkDestination(link.Key, this);
                                }
                                else
                                {
                                    I3dWorldObject obj = new General3dWorldObject(linked, scene, objectsByReference);
                                    obj.AddLinkDestination(link.Key, this);
                                    Links[link.Key].Add(obj);
                                    scene.linkedObjects.Add(obj);
                                }
                            }
                        }
                        break;
                    case "ModelName":
                        ModelName = entry.Parse()??"";
                        break;
                    case "Rotate":
                        dynamic _data = entry.Parse();
                        Rotation = new Vector3(
                            _data["X"],
                            _data["Y"],
                            _data["Z"]
                        );
                        break;
                    case "Scale":
                        _data = entry.Parse();
                        Scale = new Vector3(
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

                        DisplayTranslation = new Vector3(
                            _data["DisplayTranslate"]["X"] / 100f,
                            _data["DisplayTranslate"]["Y"] / 100f,
                            _data["DisplayTranslate"]["Z"] / 100f
                            );
                        DisplayRotation = new Vector3(
                            _data["DisplayRotate"]["X"],
                            _data["DisplayRotate"]["Y"],
                            _data["DisplayRotate"]["Z"]
                            );
                        DisplayScale = new Vector3(
                            _data["DisplayScale"]["X"],
                            _data["DisplayScale"]["Y"],
                            _data["DisplayScale"]["Z"]
                            );
                        ClassName = _data["ParameterConfigName"];
                        break;
                    default:
                        Properties.Add(entry.Key, entry.Parse());
                        break;
                }
            }

            if (Properties.Count == 0)
                Properties = null;

            if (Links.Count == 0)
                Links = null;
        }

        public General3dWorldObject(
            Vector3 pos, Vector3 rot, Vector3 scale, 
            string iD, string objectName, string modelName, string className, 
            Vector3 displayTranslation, Vector3 displayRotation, Vector3 displayScale, 
            Dictionary<string, List<I3dWorldObject>> links, Dictionary<string, dynamic> properties)
            : base(pos,rot,scale)
        {
            ID = iD;
            ObjectName = objectName;
            ModelName = modelName;
            ClassName = className;
            DisplayTranslation = displayTranslation;
            DisplayRotation = displayRotation;
            DisplayScale = displayScale;
            Links = links;
            this.Properties = properties;
        }

        /// <summary>
        /// Deletes this object from the Scene
        /// </summary>
        /// <param name="manager">The deletion manager that executes the deletion afterwards</param>
        /// <param name="list">The main list this object is referenced in</param>
        /// <param name="currentList">The list selected as the currentList in the EditorScene</param>
        public override void DeleteSelected(EditorSceneBase.DeletionManager manager, IList list, IList currentList)
        {
            //Deletion is handled by DeleteSelected3DWorldObject
        }

        public void DeleteSelected3DWorldObject(List<I3dWorldObject> objectsToDelete)
        {
            if (Selected)
                objectsToDelete.Add(this);
        }
        /// <summary>
        /// Prepares to draw this Object
        /// </summary>
        /// <param name="control">The GL_Control to draw to</param>
        public override void Prepare(GL_ControlModern control)
        {
            string mdlName = ModelName == "" ? ObjectName : ModelName;
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

            Matrix3 rotMtx = Framework.Mat3FromEulerAnglesDeg(Rotation);

            if (BfresModelCache.Contains(ModelName == "" ? ObjectName : ModelName))
            {
                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(Scale) : Scale)) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));

                Vector4 highlightColor;

                if (Selected)
                    highlightColor = selectColor;
                else if (hovered)
                    highlightColor = hoverColor;
                else if (SM3DWorldScene.IteratesThroughLinks && linkDestinations.Count == 0)
                    highlightColor = new Vector4(1, 0, 0, 0.5f);
                else
                    highlightColor = Vector4.Zero;

                BfresModelCache.TryDraw(ModelName=="" ? ObjectName : ModelName, control, pass, highlightColor);
                
                goto RENDER_LINKS;
            }
            else
            {
                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale * 0.5f) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(Scale) : Scale)) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position));
            }

            Vector4 blockColor;
            Vector4 lineColor;
            Vector4 col = (SM3DWorldScene.IteratesThroughLinks && linkDestinations.Count > 0) ? LinkColor : Color;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else if (SM3DWorldScene.IteratesThroughLinks && linkDestinations.Count == 0)
                lineColor = new Vector4(1, 0, 0, 1);
            else
                lineColor = col;

            if (hovered && Selected)
                blockColor = col * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = col * 0.5f + selectColor * 0.5f;
            else if (SM3DWorldScene.IteratesThroughLinks && linkDestinations.Count == 0)
                blockColor = col * 0.5f + new Vector4(1, 0, 0, 1) * 0.5f;
            else
                blockColor = col;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());

            RENDER_LINKS:

            if (Links != null && pass == Pass.OPAQUE)
            {
                control.ResetModelMatrix();

                control.CurrentShader = Renderers.ColorBlockRenderer.SolidColorShaderProgram;
                control.CurrentShader.SetVector4("color", Vector4.One);

                GL.LineWidth(1);
                GL.Begin(PrimitiveType.Lines);
                foreach (List<I3dWorldObject> link in Links.Values)
                {
                    foreach (I3dWorldObject obj in link)
                    {
                        GL.Vertex3(GetLinkingPoint());
                        GL.Vertex3(obj.GetLinkingPoint());
                    }
                }
                GL.End();
                GL.LineWidth(2);
            }
        }

        public virtual Vector3 GetLinkingPoint()
        {
            return Position+Vector3.Transform(Framework.Mat3FromEulerAnglesDeg(Rotation), DisplayTranslation);
        }

        public override void GetSelectionBox(ref BoundingBox boundingBox)
        {
            boundingBox.Include(Position + Vector3.Transform(Framework.Mat3FromEulerAnglesDeg(Rotation), DisplayTranslation));
        }

        public override IObjectUIProvider GetPropertyProvider(EditorSceneBase scene) => new PropertyProvider(this, scene);

        public void ClearLinkDestinations()
        {
            linkDestinations.Clear();
        }

        public void AddLinkDestinations()
        {
            if (Links != null)
            {
                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
                {
                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        obj.AddLinkDestination(keyValuePair.Key, this);
                    }
                }
            }
        }

        public void AddLinkDestination(string linkName, I3dWorldObject linkingObject)
        {
            linkDestinations.Add((linkName, linkingObject));
        }

        public void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldScene scene)
        {
            if (!Selected)
                return;

            Selected = false;
            scene.SelectedObjects.Remove(this);

            //copy links
            Dictionary<string, List<I3dWorldObject>> newLinks;
            if (Links != null)
            {
                newLinks = new Dictionary<string, List<I3dWorldObject>>();
                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
                {
                    newLinks[keyValuePair.Key] = new List<I3dWorldObject>();
                    foreach (I3dWorldObject obj in keyValuePair.Value)
                    {
                        newLinks[keyValuePair.Key].Add(obj);
                    }
                }
            }
            else
                newLinks = null;

            //copy properties
            Dictionary<string, dynamic> newProperties;
            if (Properties != null)
            {
                newProperties = new Dictionary<string, dynamic>();
                foreach (KeyValuePair<string, dynamic> keyValuePair in Properties)
                    newProperties[keyValuePair.Key] = keyValuePair.Value;
            }
            else
                newProperties = null;
            
            duplicates[this] = new General3dWorldObject(Position, Rotation, Scale, scene.NextObjID(), ObjectName, ModelName, ClassName, DisplayTranslation, DisplayRotation, DisplayScale,
                newLinks, newProperties);

            duplicates[this].SelectDefault(scene.GL_Control,scene.SelectedObjects);
        }

        public void LinkDuplicatesAndAddLinkDestinations(SM3DWorldScene.DuplicationInfo duplicationInfo)
        {
            if (Links != null)
            {
                bool isDuplicate = duplicationInfo.IsDuplicate(this);

                bool hasDuplicate = duplicationInfo.HasDuplicate(this);

                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
                {
                    I3dWorldObject[] oldLink = keyValuePair.Value.ToArray();

                    //Clear Link
                    keyValuePair.Value.Clear();

                    //Populate Link
                    foreach (I3dWorldObject obj in oldLink)
                    {
                        bool objHasDuplicate = duplicationInfo.TryGetDuplicate(obj, out I3dWorldObject duplicate);

                        if (!(isDuplicate && objHasDuplicate))
                        {
                            //Link to original
                            keyValuePair.Value.Add(obj);
                            obj.AddLinkDestination(keyValuePair.Key, this);
                        }

                        if(objHasDuplicate && (hasDuplicate==isDuplicate))
                        {
                            //Link to duplicate
                            keyValuePair.Value.Add(duplicate);
                            duplicate.AddLinkDestination(keyValuePair.Key, this);
                        }
                    }
                }
            }
        }



        public new class PropertyProvider : IObjectUIProvider
        {
            PropertyCapture? capture = null;

            General3dWorldObject obj;
            EditorSceneBase scene;

            public PropertyProvider(General3dWorldObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                obj.ObjectName = control.TextInput(obj.ObjectName, "Object Name");
                obj.ClassName = control.TextInput(obj.ClassName, "Class Name");
                obj.ModelName = control.TextInput(obj.ModelName, "Model Name");

                control.VerticalSeperator();

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.Position = control.Vector3Input(obj.Position, "Position", 1, 16);
                else
                    obj.Position = control.Vector3Input(obj.Position, "Position", 0.125f, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.Rotation = control.Vector3Input(obj.Rotation, "Rotation", 45, 18);
                else
                    obj.Rotation = control.Vector3Input(obj.Rotation, "Rotation", 5, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.Scale = control.Vector3Input(obj.Scale, "Scale", 1, 16);
                else
                    obj.Scale = control.Vector3Input(obj.Scale, "Scale", 0.125f, 2);

                control.VerticalSeperator();

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.DisplayTranslation = control.Vector3Input(obj.DisplayTranslation, "Display Position", 1, 16);
                else
                    obj.DisplayTranslation = control.Vector3Input(obj.DisplayTranslation, "Display Position", 0.125f, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.DisplayRotation = control.Vector3Input(obj.DisplayRotation, "Display Rotation", 45, 18);
                else
                    obj.DisplayRotation = control.Vector3Input(obj.DisplayRotation, "Display Rotation", 5, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    obj.DisplayScale = control.Vector3Input(obj.DisplayScale, "Display Scale", 1, 16);
                else
                    obj.DisplayScale = control.Vector3Input(obj.DisplayScale, "Display Scale", 0.125f, 2);
            }

            public void OnValueChangeStart()
            {
                capture = new PropertyCapture(obj);
            }

            public void OnValueChanged()
            {
                scene.Refresh();
            }

            public void OnValueSet()
            {
                capture?.HandleUndo(scene);
                capture = null;
                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }
    }
}
