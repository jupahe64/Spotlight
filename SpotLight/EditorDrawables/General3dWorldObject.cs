using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Spotlight.Level;
using Spotlight.Database;
using Syroot.BinaryData;
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
using Spotlight.ObjectRenderers;
using System.Diagnostics;

namespace Spotlight.EditorDrawables
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

        [PropertyCapture.Undoable]
        public Layer Layer { get; set; }

        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public List<(string, I3dWorldObject)> LinkDestinations { get; } = new List<(string, I3dWorldObject)>();

        [PropertyCapture.Undoable]
        public Vector3 DisplayTranslation { get; set; }
        [PropertyCapture.Undoable]
        public Vector3 DisplayScale { get; set; }
        [PropertyCapture.Undoable]
        public Vector3 DisplayRotation { get; set; }
        [PropertyCapture.Undoable]
        public string DisplayName { get; set; }

        public Dictionary<string, List<I3dWorldObject>> Links { get; set; } = null;
        public Dictionary<string, dynamic> Properties { get; set; } = new Dictionary<string, dynamic>();

        readonly string comment = null;

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
        public General3dWorldObject(in LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks) 
            : base(info.Position, info.Rotation, info.Scale)
        {
            ID = info.ID;
            ObjectName = info.ObjectName;
            ModelName = info.ModelName ?? string.Empty;
            ClassName = info.ClassName;
            Layer = zone.GetOrCreateLayer(info.LayerName);
            DisplayTranslation = info.DisplayTranslation;
            DisplayRotation = info.DisplayRotation;
            DisplayScale = info.DisplayScale;
            DisplayName = info.DisplayName;

            comment = info.Comment;

            if (info.PropertyEntries.Count > 0)
            {
                foreach (var entry in info.PropertyEntries.Values)
                {
                    Properties.Add(entry.Key, entry.Parse()??"");
                }
            }

            DoModelLoad();
            
            zone?.SubmitID(ID);


            loadLinks = true;
        }

        public General3dWorldObject(
            Vector3 pos, Vector3 rot, Vector3 scale, 
            string iD, string objectName, string modelName, string className, 
            Vector3 displayTranslation, string displayName,
            Dictionary<string, List<I3dWorldObject>> links, Dictionary<string, dynamic> properties, SM3DWorldZone zone, Layer layer)
            : base(pos,rot,scale)
        {
            ID = iD;
            ObjectName = objectName;
            ModelName = modelName;
            ClassName = className;
            DisplayTranslation = displayTranslation;
            DisplayName = displayName;
            Links = links;
            Properties = properties;
            Layer = layer;

            DoModelLoad();

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
        public void Save(LevelObjectsWriter writer, DictionaryNode objNode)
        {
#if ODYSSEY
            objNode.AddDynamicValue("comment", comment);
#else
            objNode.AddDynamicValue("Comment", comment);
#endif
            objNode.AddDynamicValue("Id", ID);
            objNode.AddDynamicValue("IsLinkDest", LinkDestinations.Count > 0);
            objNode.AddDynamicValue("LayerConfigName", Layer.Name);

            writer.SaveLinks(Links, objNode);

            objNode.AddDynamicValue("ModelName", string.IsNullOrEmpty(ModelName) ? null : ModelName);
            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Rotation), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Scale), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", ObjectUtils.CreateUnitConfig(this), true);

            objNode.AddDynamicValue("UnitConfigName", ObjectName);

            if (Properties.Count != 0)
            {
                foreach (KeyValuePair<string, dynamic> property in Properties)
                {
                    if (property.Value is string && property.Value == "")
                        objNode.AddDynamicValue(property.Key, null, true);
                    else
                        objNode.AddDynamicValue(property.Key, property.Value, true);
                }
            }
        }

        public bool Equals(I3dWorldObject obj)
        {
            return obj is General3dWorldObject @object &&
                   Position.Equals(@object.Position) &&
                   Rotation.Equals(@object.Rotation) &&
                   Scale.Equals(@object.Scale) &&
                   ObjectName == @object.ObjectName &&
                   ModelName == @object.ModelName &&
                   ClassName == @object.ClassName &&
                   Layer == @object.Layer &&
                   DisplayTranslation.Equals(@object.DisplayTranslation) &&
                   ObjectUtils.EqualProperties(Properties, @object.Properties);
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

        public void UpdateLinkDestinations_Clear()
        {
            LinkDestinations.Clear();
        }

        public void UpdateLinkDestinations_Populate()
        {
            if (Links != null)
            {
                foreach (var (linkName, link) in Links)
                {
                    foreach (I3dWorldObject obj in link)
                    {
                        obj.AddLinkDestination(linkName, this);
                    }
                }
            }
        }

        public void AddLinkDestination(string linkName, I3dWorldObject linkingObject)
        {
            LinkDestinations.Add((linkName, linkingObject));
        }

        public void DuplicateSelected(Dictionary<I3dWorldObject, I3dWorldObject> duplicates, SM3DWorldZone destZone, ZoneTransform? zoneToZoneTransform = null)
        {
            if (!Selected)
                return;


            duplicates[this] = new General3dWorldObject(
                ObjectUtils.TransformedPosition(Position, zoneToZoneTransform),
                ObjectUtils.TransformedRotation(Rotation, zoneToZoneTransform),

                Scale, destZone?.NextObjID(), ObjectName, ModelName, ClassName, DisplayTranslation, DisplayName,

                ObjectUtils.DuplicateLinks(Links),
                ObjectUtils.DuplicateProperties(Properties),
                destZone, Layer);
        }

        public void LinkDuplicates(SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate)
            => ObjectUtils.LinkDuplicates(this, duplicationInfo, allowKeepLinksOfDuplicate);

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            return Program.ParameterDB.ObjectParameters[ClassName].TryGetObjectList(zone, out objList);
        }

        public void AddToZoneBatch(ZoneRenderBatch zoneBatch)
        {
            General3dWorldObjectBatch renderer = (General3dWorldObjectBatch)zoneBatch.GetBatchRenderer(typeof(General3dWorldObjectBatch));

            BfresModelRenderer.TryGetModel(string.IsNullOrEmpty(ModelName) ? ObjectName : ModelName, out BfresModelRenderer.CachedModel cachedModel);

            if (cachedModel != null)
            {
                renderer.cachedModels.Add((cachedModel,
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale(Scale) *
                    new Matrix4(Framework.Mat3FromEulerAnglesDeg(Rotation)) *
                    Matrix4.CreateTranslation(Position)
                ));
            }
        }

#endregion

        

        /// <summary>
        /// Draws the model to the given GL_Control
        /// </summary>
        /// <param name="control">The GL_Control to draw to</param>
        /// <param name="pass">The current pass of drawing</param>
        /// <param name="editorScene">The current Editor Scene</param>
        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!Selected)
            {
                if (!Spotlight.Properties.Settings.Default.DrawSkyBoxes && ClassName == "SkyProjection")
                {
                    control.SkipPickingColors(1);
                    return;
                }

                if (!Spotlight.Properties.Settings.Default.DrawTransparentWalls && ObjectName.Contains("TransparentWall"))
                {
                    control.SkipPickingColors(1);
                    return;
                }

                if (!SceneDrawState.EnabledLayers.Contains(Layer))
                {
                    control.SkipPickingColors(1);
                    return;
                }
            }

            bool hovered = editorScene.Hovered == this;

            Matrix3 rotMtx = GlobalRotation;

            Vector3 transformedGloablPos = Selected ? editorScene.SelectionTransformAction.NewPos(GlobalPosition) : GlobalPosition;

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

            if (SceneObjectIterState.InLinks && LinkDestinations.Count == 0)
                highlightColor = new Vector4(1, 0, 0, 1) * 0.5f + highlightColor * 0.5f;


            //Gizmos don't need the object transforms
            //the method constructs a matrix from GlobalPosition and the controls camera
            if (GizmoRenderer.TryDraw(ClassName, control, pass, transformedGloablPos, highlightColor))
                return;

            control.UpdateModelMatrix(
                    Matrix4.CreateTranslation(DisplayTranslation) *
                    Matrix4.CreateScale((Selected ? editorScene.SelectionTransformAction.NewScale(GlobalScale, rotMtx) : GlobalScale)) *
                    new Matrix4(Selected ? editorScene.SelectionTransformAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(transformedGloablPos));


            string renderMdlName = string.IsNullOrEmpty(ModelName) ? ObjectName : ModelName;

            if (BfresModelRenderer.TryDraw(renderMdlName, control, pass, highlightColor))
                return;
            else if (ExtraModelRenderer.TryDraw(renderMdlName, control, pass, highlightColor))
                return;
            else
            {
                if (pass == Pass.TRANSPARENT)
                    return;



                Vector4 blockColor = Color * (1 - highlightColor.W) + highlightColor * highlightColor.W;
                Vector4 lineColor;

                if (highlightColor.W != 0)
                    lineColor = highlightColor;
                else
                    lineColor = Color;

                lineColor.W = 1;

                Renderers.ColorCubeRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());
            }
        }

        public override void GetSelectionBox(ref BoundingBox boundingBox)
        {
            if(Selected)
                boundingBox.Include(GlobalPosition + Vector3.Transform(Framework.Mat3FromEulerAnglesDeg(Rotation), DisplayTranslation));
        }

        public void DoModelLoad()
        {
            string mdlName = string.IsNullOrEmpty(ModelName) ? ObjectName : ModelName;
            if (BfresModelRenderer.Contains(mdlName))
                return;
            string Result = Program.TryGetPathViaProject("ObjectData", mdlName + ".szs");
            if (File.Exists(Result))
            {
                SARCExt.SarcData objArc = SARCExt.SARC.UnpackRamN(YAZ0.Decompress(Result));

                if (objArc.Files.ContainsKey(mdlName + ".bfres"))
                {
                    if (objArc.Files.ContainsKey("InitModel.byml"))
                    {
                        dynamic initModel = ByamlFile.FastLoadN(new MemoryStream(objArc.Files["InitModel.byml"]), false, ByteOrder.BigEndian).RootNode;

                        if (initModel is Dictionary<string, dynamic>)
                        {
                            BfresModelRenderer.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]),
                            initModel.TryGetValue("TextureArc", out dynamic texArc) ? texArc : null);
                            return;
                        }
                    }
                    BfresModelRenderer.Submit(mdlName, new MemoryStream(objArc.Files[mdlName + ".bfres"]), null);
                }
            }
        }

        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            if (!Selected)
                return false;
            objectUIControl.AddObjectUIContainer(new BasicPropertyUIContainer(this, scene), "General");

            if (Properties.Count > 0)
            {
                var info = Program.InformationDB.GetInformation(ClassName);
                objectUIControl.AddObjectUIContainer(new ExtraPropertiesUIContainer(Properties, scene, info), "Properties");
            }

            if (Links != null)
                objectUIControl.AddObjectUIContainer(new LinksUIContainer(this, scene), "Links");

            if (LinkDestinations.Count > 0)
                objectUIControl.AddObjectUIContainer(new LinkDestinationsUIContainer(this, (SM3DWorldScene)scene), "Link Destinations");

            return true;
        }

        protected class General3dWorldObjectBatch : IBatchRenderer
        {
            public Dictionary<Vector4, List<Matrix4>> colorCubes = new Dictionary<Vector4, List<Matrix4>>();

            public List<(BfresModelRenderer.CachedModel model, Matrix4 transform)> cachedModels = new List<(BfresModelRenderer.CachedModel, Matrix4)>();

            public void Draw(GL_ControlModern control, Pass pass, Vector4 highlightColor, Matrix4 zoneTransform, Vector4 pickingColor)
            {
                bool drawHighlight = highlightColor.W != 0;

                if (pass == Pass.OPAQUE)
                {
                    control.CurrentShader = BfresModelRenderer.BfresShaderProgram;

                    control.CurrentShader.SetVector4("highlight_color", highlightColor);

                    GL.ActiveTexture(TextureUnit.Texture0);

                    if (drawHighlight)
                    {
                        GL.Enable(EnableCap.StencilTest);
                        GL.Clear(ClearBufferMask.StencilBufferBit);
                        GL.ClearStencil(0);
                        GL.StencilFunc(StencilFunction.Always, 0x1, 0x1);
                        GL.StencilOp(StencilOp.Keep, StencilOp.Replace, StencilOp.Replace);
                    }

                    for (int i = 0; i < cachedModels.Count; i++)
                    {
                        control.UpdateModelMatrix(cachedModels[i].transform * zoneTransform);
                        cachedModels[i].model.BatchDrawOpaque(control, drawHighlight);
                    }

                    if (drawHighlight)
                    {
                        control.CurrentShader = Framework.SolidColorShaderProgram;
                        control.CurrentShader.SetVector4("color", new Vector4(highlightColor.Xyz, 1));

                        GL.LineWidth(3.0f);
                        GL.StencilFunc(StencilFunction.Equal, 0x0, 0x1);
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

                        GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

                        for (int i = 0; i < cachedModels.Count; i++)
                        {
                            control.UpdateModelMatrix(cachedModels[i].transform * zoneTransform);
                            cachedModels[i].model.BatchDrawSolidColor(control);
                        }

                        GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

                        GL.Disable(EnableCap.StencilTest);
                        GL.LineWidth(2);
                    }
                }
                else if (pass == Pass.TRANSPARENT)
                {
                    control.CurrentShader = BfresModelRenderer.BfresShaderProgram;

                    control.CurrentShader.SetVector4("highlight_color", highlightColor);

                    GL.ActiveTexture(TextureUnit.Texture0);

                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Blend);

                    for (int i = 0; i < cachedModels.Count; i++)
                    {
                        control.UpdateModelMatrix(cachedModels[i].transform * zoneTransform);
                        cachedModels[i].model.BatchDrawTranparent(control);
                    }

                    GL.Disable(EnableCap.Blend);
                }
                else
                {
                    control.CurrentShader = Framework.SolidColorShaderProgram;
                    control.CurrentShader.SetVector4("color", pickingColor);

                    for (int i = 0; i < cachedModels.Count; i++)
                    {
                        control.UpdateModelMatrix(cachedModels[i].transform * zoneTransform);
                        cachedModels[i].model.BatchDrawSolidColor(control);
                    }
                }
            }
        }

        public class BasicPropertyUIContainer : IObjectUIContainer
        {
            PropertyCapture? capture = null;

            General3dWorldObject obj;
            EditorSceneBase scene;

            string[] DB_objectNames;
            string[] DB_modelNames;

            string[] DB_classNames;

            string classNameAlias;
            bool showClassNameAlias;
            string classNameInfo;
            bool showClassNameInfo;
            private LayerUIField layerUIField;

            public BasicPropertyUIContainer(General3dWorldObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;

                DB_classNames = Program.ParameterDB.ObjectParameters.Keys.ToArray();

                UpdateClassNameInfo();

                layerUIField = new LayerUIField((SM3DWorldScene)scene, obj);
            }

            void UpdateClassNameInfo()
            {
                var information = Program.InformationDB.GetInformation(obj.ClassName);

                showClassNameAlias = information.EnglishName != null;
                classNameAlias = "aka " + information.EnglishName;

                showClassNameInfo = information.Description != string.Empty;
                classNameInfo = information.Description;

                if (Program.ParameterDB.ObjectParameters.TryGetValue(obj.ClassName, out ObjectParam entry))
                {
                    DB_objectNames = entry.ObjectNames.ToArray();
                    DB_modelNames = entry.ModelNames.ToArray();
                }
                else
                {
                    DB_objectNames = Array.Empty<string>();
                    DB_modelNames = Array.Empty<string>();
                }
            }

            public void DoUI(IObjectUIControl control)
            {
                if (Spotlight.Properties.Settings.Default.AllowIDEdits)
                    obj.ID = control.TextInput(obj.ID, "Object ID");
                else
                    control.TextInput(obj.ID, "Object ID");

                if(obj.comment!=null)
                    control.TextInput(obj.comment, "Comment");

                layerUIField.DoUI(control);
                obj.ObjectName = control.DropDownTextInput("Object Name", obj.ObjectName, DB_objectNames);

                if(showClassNameInfo)
                    control.SetTooltip(classNameInfo);
                obj.ClassName = control.DropDownTextInput("Class Name", obj.ClassName, DB_classNames);
                if (showClassNameAlias)
                    control.PlainText(classNameAlias);
                control.SetTooltip(null);

                obj.ModelName = control.DropDownTextInput("Model Name", obj.ModelName, DB_modelNames);

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

                //TODO
                obj.DisplayName = control.DropDownTextInput("Display Name", obj.DisplayName, Array.Empty<string>());
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

                obj.DoModelLoad();

                UpdateClassNameInfo();

                layerUIField.OnValueSet();

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

            IReadOnlyDictionary<string, string> propertyInfos;

            public ExtraPropertiesUIContainer(Dictionary<string, dynamic> propertyDict, EditorSceneBase scene, Information information = null)
            {
                this.propertyDict = propertyDict;
                this.scene = scene;
                propertyDictKeys = propertyDict.Keys.ToArray();

                if (information != null)
                    propertyInfos = information.Properties;
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

                    if(propertyInfos != null && propertyInfos.TryGetValue(key, out string desc))
                        control.SetTooltip(desc);
                    else
                        control.SetTooltip("No info for " + key);


                    if (propertyDict[key] is int)
                        propertyDict[key] = (int)control.NumberInput(propertyDict[key], key);
                    else if (propertyDict[key] is float)
                        propertyDict[key] = control.NumberInput(propertyDict[key], key);
                    else if (propertyDict[key] is string)
                        propertyDict[key] = control.TextInput(propertyDict[key], key);
                    else if (propertyDict[key] is bool)
                        propertyDict[key] = control.CheckBox(key, propertyDict[key]);
                }
                
                control.SetTooltip(null);


                if (control.Button("Edit"))
                {
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
                        foreach (var property in propertyDict)
                        {
                            if (!newParamNames.Contains(property.Key))
                                deleteInfos.Add(new RevertableDictDeletion.DeleteInfo(property.Value, property.Key));
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

                        foreach (var parameter in newParameters)
                            propertyDict.Add(parameter.Key, parameter.Value);

                        foreach (var parameter in otherParameters)
                            propertyDict.Add(parameter.Key, parameter.Value);
                        
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

                foreach (var capturedProperty in capture)
                {
                    if(capturedProperty.Value!= propertyDict[capturedProperty.Key])
                    {
                        scene.AddToUndo(new RevertableDictEntryChange(capturedProperty.Key, propertyDict, capturedProperty.Value));
                    }

                }

                capture = null;

                scene.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }

        public class LinksUIContainer : IObjectUIContainer
        {
            I3dWorldObject obj;
            EditorSceneBase scene;

            public LinksUIContainer(I3dWorldObject obj, EditorSceneBase scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                foreach (var (linkName, link) in obj.Links)
                {
                    if (control.Link(linkName))
                        scene.EnterList(link);
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

        public class LinkDestinationsUIContainer : IObjectUIContainer
        {
            I3dWorldObject obj;
            SM3DWorldScene scene;

            public LinkDestinationsUIContainer(I3dWorldObject obj, SM3DWorldScene scene)
            {
                this.obj = obj;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                foreach ((string name, I3dWorldObject _obj) in obj.LinkDestinations)
                {
                    if (control.Link($"{_obj.ToString()} ({name})"))
                        scene.FocusOn(_obj);
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

        public class LayerUIField
        {
            private readonly SM3DWorldScene scene;
            private readonly I3dWorldObject obj;

            string[] layerNames;
            readonly List<Layer> layerList = new List<Layer>();

            public LayerUIField(SM3DWorldScene scene, I3dWorldObject obj)
            {
                layerList = scene.EditZone.availibleLayers;

                layerNames = layerList.Select(x=>x.Name).ToArray();
                this.scene = scene;
                this.obj = obj;
            }

            public void DoUI(IObjectUIControl control)
            {
                bool layerNamesInvalid = false;

                if (layerList.Count != layerNames.Length)
                    layerNamesInvalid = true;
                else
                {
                    for (int i = 0; i < layerNames.Length; i++)
                    {
                        if (layerNames[i] != layerList[i].Name)
                        {
                            layerNamesInvalid = true;
                            break;
                        }
                    }
                }

                if (layerNamesInvalid)
                    layerNames = layerList.Select(x=>x.Name).ToArray();


                obj.Layer = scene.EditZone.GetOrCreateLayer(control.DropDownTextInput("Layer", obj.Layer.Name, layerNames, false));
            }

            public void OnValueSet()
            {
                if (!layerList.Contains(obj.Layer))
                {
                    layerList.Add(obj.Layer);
                    scene.SignalLayersChanged();
                }
            }
        }
    }
}
