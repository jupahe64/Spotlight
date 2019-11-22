using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpotLight.Level;
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
    public class General3dWorldObject : TransformableObject, I3dWorldObject
    {
        public new static Vector4 selectColor = new Vector4(EditableObject.selectColor.Xyz, 0.5f);
        public new static Vector4 hoverColor = new Vector4(EditableObject.hoverColor.Xyz, 0.125f);

        protected static Vector4 LinkColor = new Vector4(0f, 1f, 1f, 1f);
        
        public static Dictionary<string, dynamic> CreateUnitConfig(General3dWorldObject obj) => new Dictionary<string, dynamic>
        {
            ["DisplayName"]         = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
            ["DisplayRotate"]       = LevelIO.Vector3ToDict(obj.DisplayRotation),
            ["DisplayScale"]        = LevelIO.Vector3ToDict(obj.DisplayScale),
            ["DisplayTranslate"]    = LevelIO.Vector3ToDict(obj.DisplayTranslation, 100f),
            ["GenerateCategory"]    = "",
            ["ParameterConfigName"] = obj.ClassName,
            ["PlacementTargetFile"] = "Map"
        };
        
        public override Vector3 GlobalPosition {
            get => Vector4.Transform(new Vector4(Position, 1), SM3DWorldScene.IteratedZoneTransform.PositionTransform).Xyz;
            set => Position = Vector4.Transform(new Vector4(value, 1), SM3DWorldScene.IteratedZoneTransform.PositionTransform.Inverted()).Xyz;
        }

        public override Matrix3 GlobalRotation
        {
            get => Framework.Mat3FromEulerAnglesDeg(Rotation) * SM3DWorldScene.IteratedZoneTransform.RotationTransform;
            set => Rotation = (value * SM3DWorldScene.IteratedZoneTransform.RotationTransform.Inverted())
                    .ExtractDegreeEulerAngles();
        }

        /// <summary>
        /// Id of this object
        /// </summary>
        public string ID { get; }
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

        public Dictionary<string, List<I3dWorldObject>> Links { get; set; } = null;
        public Dictionary<string, dynamic> Properties { get; set; } = null;

        private static readonly Dictionary<string, List<I3dWorldObject>> EMPTY_LINKS = new Dictionary<string, List<I3dWorldObject>>();
        /// <summary>
        /// Gets the Object Name
        /// </summary>
        /// <returns>The name of the Object</returns>
        public override string ToString() => ObjectName;
        
        /// <summary>
        /// Create a General SM3DW Object
        /// </summary>
        /// <param name="objectEntry">Unknown</param>
        /// <param name="linkedObjs">List of objects that are linked with this object</param>
        /// <param name="objectsByReference">Unknown</param>
        public General3dWorldObject(LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks) 
            : base(info.Position, info.Rotation, info.Scale)
        {
            ID = info.ID;
            ObjectName = info.ObjectName;
            ModelName = info.ModelName;
            ClassName = info.ClassName;
            DisplayTranslation = info.DisplayTranslation;
            DisplayRotation = info.DisplayRotation;
            DisplayScale = info.DisplayScale;

            if (info.PropertyEntries.Count > 0)
            {
                Properties = new Dictionary<string, dynamic>();
                foreach (var entry in info.PropertyEntries.Values)
                {
                    Properties.Add(entry.Key, entry.Parse()??"");
                }
            }

            loadLinks = true;
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
            Properties = properties;
        }

        /// <summary>
        /// Deletes this object from the Scene
        /// </summary>
        /// <param name="manager">The deletion manager that executes the deletion afterwards</param>
        /// <param name="list">The main list this object is referenced in</param>
        /// <param name="currentList">The list selected as the currentList in the EditorScene</param>
        //public override void DeleteSelected(EditorSceneBase.DeletionManager manager, IList list, IList currentList)
        //{
        //    //Deletion is handled by DeleteSelected3DWorldObject
        //}

        #region I3DWorldObject implementation
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

                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in Links)
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

            objNode.AddDynamicValue("ModelName", (ModelName == "") ? null : ModelName);
            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Rotation), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Scale), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", CreateUnitConfig(this), true);

            objNode.AddDynamicValue("UnitConfigName", ObjectName);

            if (Properties != null)
            {
                foreach (KeyValuePair<string, dynamic> keyValuePair in Properties)
                {
                    if(keyValuePair.Value is string && keyValuePair.Value == "")
                        objNode.AddDynamicValue(keyValuePair.Key, null, true);
                    else
                        objNode.AddDynamicValue(keyValuePair.Key, keyValuePair.Value, true);
                }
            }
        }

        public void DeleteSelected3DWorldObject(List<I3dWorldObject> objectsToDelete)
        {
            if (Selected)
                objectsToDelete.Add(this);
        }
        
        public virtual Vector3 GetLinkingPoint(SM3DWorldScene editorScene)
        {
            return Selected ? editorScene.CurrentAction.NewPos(GlobalPosition) : GlobalPosition + Vector3.Transform(GlobalRotation, DisplayTranslation);
        }

        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            if (!Selected)
                return false;
            objectUIControl.AddObjectUIContainer(new BasicPropertyProvider(this, scene), "General");

            if (Properties != null)
                objectUIControl.AddObjectUIContainer(new ExtraPropertiesProvider(Properties, scene), "Properties");

            if (Links != null)
                objectUIControl.AddObjectUIContainer(new LinksProvider(this, scene), "Links");

            return true;
        }

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

        public void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldScene scene, SM3DWorldZone zone)
        {
            if (!Selected)
                return;

            Selected = false;

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
            
            duplicates[this] = new General3dWorldObject(Position, Rotation, Scale, zone.NextObjID(), ObjectName, ModelName, ClassName, DisplayTranslation, DisplayRotation, DisplayScale,
                newLinks, newProperties);

            duplicates[this].SelectDefault(scene.GL_Control);
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
        #endregion

        /// <summary>
        /// Prepares to draw this Object
        /// </summary>
        /// <param name="control">The GL_Control to draw to</param>
        public override void Prepare(GL_ControlModern control)
        {
            string mdlName = ModelName == "" ? ObjectName : ModelName;
            if (File.Exists(Program.ObjectDataPath + mdlName + ".szs"))
            {
                SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Program.ObjectDataPath + mdlName + ".szs"));

                if (objArc.Files.ContainsKey(mdlName + ".bfres"))
                {
                    if (objArc.Files.ContainsKey("InitModel.byml"))
                    {
                        dynamic initModel = ByamlFile.FastLoadN(new MemoryStream(objArc.Files["InitModel.byml"]), false, Syroot.BinaryData.Endian.Big).RootNode;

                        if (initModel is Dictionary<string, dynamic>)
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
            if (!editorScene.ShouldBeDrawn(this))
                return;

            bool hovered = editorScene.Hovered == this;

            Matrix3 rotMtx = GlobalRotation;

            if (BfresModelCache.Contains(ModelName == "" ? ObjectName : ModelName))
            {
                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(GlobalScale, rotMtx) : GlobalScale)) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(GlobalPosition) : GlobalPosition));

                Vector4 highlightColor;

                if (Selected)
                    highlightColor = selectColor;
                else if (hovered)
                    highlightColor = hoverColor;
                else if (SM3DWorldScene.IteratesThroughLinks && linkDestinations.Count == 0)
                    highlightColor = new Vector4(1, 0, 0, 0.5f);
                else
                    highlightColor = Vector4.Zero;

                BfresModelCache.TryDraw(ModelName == "" ? ObjectName : ModelName, control, pass, highlightColor);
                return;
            }
            else
            {
                if (pass == Pass.TRANSPARENT)
                    return;

                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale * 0.5f) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.CurrentAction.NewScale(GlobalScale, rotMtx) : GlobalScale)) *
                    new Matrix4(Selected ? editorScene.CurrentAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(GlobalPosition) : GlobalPosition));
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
        }

        public override void GetSelectionBox(ref BoundingBox boundingBox)
        {
            if(Selected)
                boundingBox.Include(GlobalPosition + Vector3.Transform(Framework.Mat3FromEulerAnglesDeg(Rotation), DisplayTranslation));
        }


        public class BasicPropertyProvider : IObjectUIContainer
        {
            PropertyCapture? capture = null;

            General3dWorldObject obj;
            EditorSceneBase scene;

            public BasicPropertyProvider(General3dWorldObject obj, EditorSceneBase scene)
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

                string mdlName = obj.ModelName == "" ? obj.ObjectName : obj.ModelName;
                if (File.Exists(Program.ObjectDataPath + mdlName + ".szs"))
                {
                    SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Program.ObjectDataPath + mdlName + ".szs"));

                    if (objArc.Files.ContainsKey(mdlName + ".bfres"))
                    {
                        if (objArc.Files.ContainsKey("InitModel.byml"))
                        {
                            dynamic initModel = ByamlFile.FastLoadN(new MemoryStream(objArc.Files["InitModel.byml"]), false, Syroot.BinaryData.Endian.Big).RootNode;

                            if (initModel is Dictionary<string, dynamic>)
                            {
                                BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), (GL_ControlModern)scene.GL_Control,
                                initModel.TryGetValue("TextureArc", out dynamic texArc) ? texArc : null);
                                return;
                            }
                        }
                        BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), (GL_ControlModern)scene.GL_Control, null);
                    }
                }

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }

        public class ExtraPropertiesProvider : IObjectUIContainer
        {
            Dictionary<string, dynamic> dict;
            EditorSceneBase scene;

            string[] keys;

            List<KeyValuePair<string, dynamic>> capture = null;

            public ExtraPropertiesProvider(Dictionary<string, dynamic> dict, EditorSceneBase scene)
            {
                this.dict = dict;
                this.scene = scene;
                keys = dict.Keys.ToArray();
            }

            public void DoUI(IObjectUIControl control)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    if (dict[key] is int)
                        dict[key] = (int)control.NumberInput(dict[key], key);
                    else if (dict[key] is float)
                        dict[key] = control.NumberInput(dict[key], key);
                    else if (dict[key] is string)
                        dict[key] = control.TextInput(dict[key], key);
                    else if (dict[key] is bool)
                        dict[key] = control.CheckBox(key, dict[key]);
                }

                if (control.Button("Edit"))
                {
                    List<(ObjectParameterForm.TypeDef typeDef, string name)> parameters = new List<(ObjectParameterForm.TypeDef typeDef, string name)>();

                    List<KeyValuePair<string, dynamic>> otherParameters = new List<KeyValuePair<string, dynamic>>();

                    foreach (var item in dict)
                    {
                        if (item.Value is int)
                            parameters.Add((ObjectParameterForm.typeDefs[0], item.Key));
                        else if (item.Value is float)
                            parameters.Add((ObjectParameterForm.typeDefs[1], item.Key));
                        else if (item.Value is string)
                            parameters.Add((ObjectParameterForm.typeDefs[2], item.Key));
                        else if (item.Value is bool)
                            parameters.Add((ObjectParameterForm.typeDefs[3], item.Key));
                        else
                            otherParameters.Add(item);
                    }

                    var parameterForm = new ObjectParameterForm(parameters);

                    if(parameterForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        List<KeyValuePair<string, dynamic>> newEntries = new List<KeyValuePair<string, dynamic>>();
                        foreach ((ObjectParameterForm.TypeDef def, string name) in parameterForm.Parameters)
                        {
                            if (dict.ContainsKey(name))
                                newEntries.Add(new KeyValuePair<string, dynamic>(name, dict[name]));
                            else
                                newEntries.Add(new KeyValuePair<string, dynamic>(name, def.DefaultValue));
                        }

                        dict.Clear();

                        foreach (var item in newEntries)
                            dict.Add(item.Key, item.Value);

                        foreach (var item in otherParameters)
                            dict.Add(item.Key, item.Value);
                        

                        keys = dict.Keys.ToArray();
                    }
                }
            }

            public void OnValueChangeStart()
            {
                capture = dict.ToList();
            }

            public void OnValueChanged()
            {
                scene.Refresh();
            }

            public void OnValueSet()
            {

                foreach (var keyValuePair in capture)
                {
                    if(keyValuePair.Value!= dict[keyValuePair.Key])
                    {
                        scene.AddToUndo(new RevertableDictEntryChange(keyValuePair.Key, dict, keyValuePair.Value));
                    }

                }
                capture = null;

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }

        public class LinksProvider : IObjectUIContainer
        {
            General3dWorldObject obj;
            EditorSceneBase scene;

            public LinksProvider(General3dWorldObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                foreach (KeyValuePair<string, List<I3dWorldObject>> keyValuePair in obj.Links)
                {
                    if (control.Link(keyValuePair.Key))
                        scene.EnterList(keyValuePair.Value);
                }
            }

            public void OnValueChanged()
            {
                
            }

            public void OnValueChangeStart()
            {
                
            }

            public void OnValueSet()
            {
                
            }

            public void UpdateProperties()
            {
                
            }
        }
    }
}
