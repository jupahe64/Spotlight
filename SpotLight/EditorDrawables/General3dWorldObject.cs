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
        public new static Vector4 hoverSelectColor = new Vector4(EditableObject.hoverSelectColor.Xyz, 0.5f);
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
            get => Vector4.Transform(new Vector4(Position, 1), SceneDrawState.ZoneTransform.PositionTransform).Xyz;
            set => Position = Vector4.Transform(new Vector4(value, 1), SceneDrawState.ZoneTransform.PositionTransform.Inverted()).Xyz;
        }

        public override Matrix3 GlobalRotation
        {
            get => Framework.Mat3FromEulerAnglesDeg(Rotation) * SceneDrawState.ZoneTransform.RotationTransform;
            set => Rotation = (value * SceneDrawState.ZoneTransform.RotationTransform.Inverted())
                    .ExtractDegreeEulerAngles();
        }

        /// <summary>
        /// Id of this object
        /// </summary>
        public string ID { get; set; }
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
        public Dictionary<string, dynamic> Properties { get; set; } = new Dictionary<string, dynamic>();

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
                foreach (var entry in info.PropertyEntries.Values)
                {
                    Properties.Add(entry.Key, entry.Parse()??"");
                }
            }

            zone?.SubmitID(ID);

            loadLinks = true;
        }

        public General3dWorldObject(
            Vector3 pos, Vector3 rot, Vector3 scale, 
            string iD, string objectName, string modelName, string className, 
            Vector3 displayTranslation, Vector3 displayRotation, Vector3 displayScale, 
            Dictionary<string, List<I3dWorldObject>> links, Dictionary<string, dynamic> properties, SM3DWorldZone zone)
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

            zone?.SubmitID(ID);
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
                    if (keyValuePair.Value.Count == 0)
                        continue;

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

            if (Properties.Count!=0)
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
            return Selected ? editorScene.SelectionTransformAction.NewPos(GlobalPosition) : GlobalPosition + Vector3.Transform(GlobalRotation, DisplayTranslation);
        }

        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            if (!Selected)
                return false;
            objectUIControl.AddObjectUIContainer(new BasicPropertyUIContainer(this, scene), "General");

            if (Properties != null)
                objectUIControl.AddObjectUIContainer(new ExtraPropertiesUIContainer(Properties, scene), "Properties");

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
                newLinks, newProperties, zone);

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
            DoModelLoad(control);

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
            if (!ObjectRenderState.ShouldBeDrawn(this))
                return;

            if (!Selected)
            {
                if ((!SpotLight.Properties.Settings.Default.DrawAreas && ClassName.Contains("Area")) ||
                    (!SpotLight.Properties.Settings.Default.DrawSkyBoxes && ClassName == "SkyProjection"))
                {
                    control.SkipPickingColors(1);
                    return;
                }
            }

            bool hovered = editorScene.Hovered == this;

            Matrix3 rotMtx = GlobalRotation;

            Vector4 highlightColor;

            if (SceneDrawState.HighlightColorOverride.HasValue)
                highlightColor = SceneDrawState.HighlightColorOverride.Value;
            else if (Selected && hovered)
                highlightColor = hoverSelectColor;
            else if (Selected)
                highlightColor = selectColor;
            else if (hovered)
                highlightColor = hoverColor;
            else
                highlightColor = Vector4.Zero;

            if (SceneObjectIterState.InLinks && linkDestinations.Count == 0)
                highlightColor = new Vector4(1, 0, 0, 1) * 0.5f + highlightColor * 0.5f;

            if (BfresModelCache.Contains(ModelName == "" ? ObjectName : ModelName))
            {


                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.SelectionTransformAction.NewScale(GlobalScale, rotMtx) : GlobalScale)) *
                    new Matrix4(Selected ? editorScene.SelectionTransformAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.SelectionTransformAction.NewPos(GlobalPosition) : GlobalPosition));

                BfresModelCache.TryDraw(string.IsNullOrEmpty(ModelName) ? ObjectName : ModelName, control, pass, highlightColor);
            }
            else
            {
                if (pass == Pass.TRANSPARENT)
                    return;



                control.UpdateModelMatrix(
                    Matrix4.CreateScale(DisplayScale * 0.5f) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(DisplayRotation)) *
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.SelectionTransformAction.NewScale(GlobalScale, rotMtx) : GlobalScale)) *
                    new Matrix4(Selected ? editorScene.SelectionTransformAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(Selected ? editorScene.SelectionTransformAction.NewPos(GlobalPosition) : GlobalPosition));

                Vector4 blockColor;
                Vector4 lineColor;

                blockColor = Color * (1 - highlightColor.W) + highlightColor * highlightColor.W;

                if (highlightColor.W != 0)
                    lineColor = highlightColor;
                else
                    lineColor = Color;

                lineColor.W = 1;

                Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());
            }
        }

        public override void GetSelectionBox(ref BoundingBox boundingBox)
        {
            if(Selected)
                boundingBox.Include(GlobalPosition + Vector3.Transform(Framework.Mat3FromEulerAnglesDeg(Rotation), DisplayTranslation));
        }

        public void DoModelLoad(GL_ControlModern control)
        {
            string mdlName = ModelName == "" ? ObjectName : ModelName;
            if (BfresModelCache.Contains(mdlName))
                return;
            string Result = Program.TryGetPathViaProject("ObjectData", mdlName + ".szs");
            if (File.Exists(Result))
            {
                SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Result));

                if (objArc.Files.ContainsKey(mdlName + ".bfres"))
                {
                    if (objArc.Files.ContainsKey("InitModel.byml"))
                    {
                        dynamic initModel = ByamlFile.FastLoadN(new MemoryStream(objArc.Files["InitModel.byml"]), false, Syroot.BinaryData.Endian.Big).RootNode;

                        if (initModel is Dictionary<string, dynamic>)
                        {
                            BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), control,
                            initModel.TryGetValue("TextureArc", out dynamic texArc) ? texArc : null);
                            return;
                        }
                    }
                    BfresModelCache.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), control, null);
                }
            }
        }

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            return Program.ParameterDB.ObjectParameters[ClassName].TryGetObjectList(zone, out objList);
        }

        public class BasicPropertyUIContainer : IObjectUIContainer
        {
            PropertyCapture? capture = null;

            General3dWorldObject obj;
            EditorSceneBase scene;

            public BasicPropertyUIContainer(General3dWorldObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                if (SpotLight.Properties.Settings.Default.AllowIDEdits)
                    obj.ID = control.TextInput(obj.ID, "Object ID");
                else
                    control.TextInput(obj.ID, "Object ID");

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

                obj.DoModelLoad((GL_ControlModern)scene.GL_Control);

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }

        public class ExtraPropertiesUIContainer : IObjectUIContainer
        {
            readonly Dictionary<string, dynamic> propertyDict;
            EditorSceneBase scene;

            string[] propertyDictKeys;

            List<KeyValuePair<string, dynamic>> capture = null;

            public ExtraPropertiesUIContainer(Dictionary<string, dynamic> propertyDict, EditorSceneBase scene)
            {
                this.propertyDict = propertyDict;
                this.scene = scene;
                propertyDictKeys = propertyDict.Keys.ToArray();
            }

            public void UpdateKeys()
            {
                propertyDictKeys = propertyDict.Keys.ToArray();
                capture = null;
            }

            public void DoUI(IObjectUIControl control)
            {
                for (int i = 0; i < propertyDictKeys.Length; i++)
                {
                    string key = propertyDictKeys[i];
                    if (propertyDict[key] is int)
                        propertyDict[key] = (int)control.NumberInput(propertyDict[key], key);
                    else if (propertyDict[key] is float)
                        propertyDict[key] = control.NumberInput(propertyDict[key], key);
                    else if (propertyDict[key] is string)
                        propertyDict[key] = control.TextInput(propertyDict[key], key);
                    else if (propertyDict[key] is bool)
                        propertyDict[key] = control.CheckBox(key, propertyDict[key]);
                }

                if (control.Button("Edit"))
                {
                    ObjectParameterForm.LocalizeTypeDefs();
                    List<(ObjectParameterForm.TypeDef typeDef, string name)> parameterInfos = new List<(ObjectParameterForm.TypeDef typeDef, string name)>();

                    List<KeyValuePair<string, dynamic>> otherParameters = new List<KeyValuePair<string, dynamic>>();

                    //get parameterInfos from propertyDict
                    foreach (var item in propertyDict)
                    {
                        if (item.Value is int)
                            parameterInfos.Add((ObjectParameterForm.TypeDef.IntDef, item.Key));
                        else if (item.Value is float)
                            parameterInfos.Add((ObjectParameterForm.TypeDef.FloatDef, item.Key));
                        else if (item.Value is string)
                            parameterInfos.Add((ObjectParameterForm.TypeDef.StringDef, item.Key));
                        else if (item.Value is bool)
                            parameterInfos.Add((ObjectParameterForm.TypeDef.BoolDef, item.Key));
                        else
                            otherParameters.Add(item); //arrays and dictionaries are not supported
                    }

                    var parameterForm = new ObjectParameterForm(parameterInfos);
                    
                    if (parameterForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        List<RevertableDictAddition.AddInfo> addInfos = new List<RevertableDictAddition.AddInfo>();
                        List<RevertableDictDeletion.DeleteInfo> deleteInfos = new List<RevertableDictDeletion.DeleteInfo>();
                        List<RevertableDictEntryChange> changeInfos = new List<RevertableDictEntryChange>();

                        HashSet<string> newParamNames = new HashSet<string>();

                        List<KeyValuePair<string, dynamic>> newParameters = new List<KeyValuePair<string, dynamic>>();
                        foreach ((ObjectParameterForm.TypeDef typeDef, string parameterName) in parameterForm.EditedParameterInfos)
                        {
                            //check if name stayed the same
                            if (propertyDict.ContainsKey(parameterName))
                            {
                                if (propertyDict[parameterName].GetType() == typeDef.Type)
                                {
                                    //keep value if name and type stayed the same
                                    newParameters.Add(new KeyValuePair<string, dynamic>(parameterName, propertyDict[parameterName]));
                                }
                                else
                                {
                                    //set value to the default value of the new type
                                    newParameters.Add(new KeyValuePair<string, dynamic>(parameterName, typeDef.DefaultValue));
                                    //and add the value change to changeInfos
                                    changeInfos.Add(new RevertableDictEntryChange(parameterName, propertyDict, propertyDict[parameterName]));
                                }
                            }
                            else
                            {
                                //add added paramter to addInfos
                                newParameters.Add(new KeyValuePair<string, dynamic>(parameterName, typeDef.DefaultValue));
                                addInfos.Add(new RevertableDictAddition.AddInfo(typeDef.DefaultValue, parameterName));
                            }
                            newParamNames.Add(parameterName);
                        }

                        //add removed parameters to deleteInfos
                        foreach (var keyValuePair in propertyDict)
                        {
                            if (!newParamNames.Contains(keyValuePair.Key))
                                deleteInfos.Add(new RevertableDictDeletion.DeleteInfo(keyValuePair.Value, keyValuePair.Key));
                        }

                        //add everything to undo
                        scene.BeginUndoCollection();
                        scene.AddToUndo(new RevertableDictAddition(new RevertableDictAddition.AddInDictInfo[]
                        {
                            new RevertableDictAddition.AddInDictInfo(addInfos.ToArray(), propertyDict)
                        },
                        Array.Empty<RevertableDictAddition.SingleAddInDictInfo>()));

                        scene.AddToUndo(new RevertableDictDeletion(new RevertableDictDeletion.DeleteInDictInfo[]
                        {
                            new RevertableDictDeletion.DeleteInDictInfo(deleteInfos.ToArray(), propertyDict)
                        },
                        Array.Empty<RevertableDictDeletion.SingleDeleteInDictInfo>()));

                        
                        foreach (var changeInfo in changeInfos)
                        {
                            scene.AddToUndo(changeInfo);
                        }
                        scene.EndUndoCollection();

                        //regenerate propertyDict by merging the newParameters and the oterParameters
                        propertyDict.Clear();

                        foreach (var keyValuePair in newParameters)
                            propertyDict.Add(keyValuePair.Key, keyValuePair.Value);

                        foreach (var keyValuePair in otherParameters)
                            propertyDict.Add(keyValuePair.Key, keyValuePair.Value);
                        
                        //update keys
                        propertyDictKeys = propertyDict.Keys.ToArray();
                    }
                }
            }

            public void OnValueChangeStart()
            {
                capture = propertyDict.ToList();
            }

            public void OnValueChanged()
            {
                scene.Refresh();
            }

            public void OnValueSet()
            {
                if (capture == null)
                    return;

                foreach (var keyValuePair in capture)
                {
                    if(keyValuePair.Value!= propertyDict[keyValuePair.Key])
                    {
                        scene.AddToUndo(new RevertableDictEntryChange(keyValuePair.Key, propertyDict, keyValuePair.Value));
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
