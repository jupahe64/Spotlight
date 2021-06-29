using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Spotlight.Database;
using Spotlight.Level;
using Spotlight.ObjectRenderers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BYAML.ByamlNodeWriter;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase.PropertyCapture;
using static Spotlight.EditorDrawables.General3dWorldObject;
using WinInput = System.Windows.Input;

namespace Spotlight.EditorDrawables
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class AreaObject : TransformableObject, I3dWorldObject
    {
        public Dictionary<string, dynamic> Properties { get; private set; } = null;

        public override string ToString()
        {
            return ClassName.ToString();
        }

        public override Vector3 GlobalPosition
        {
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

        [Undoable]
        public int Priority { get; set; }

        [Undoable]
        public string ClassName { get; set; }

        [Undoable]
        public string ModelName { get; set; }

        [Undoable]
        public string Layer { get; set; } = "Common";

        readonly string comment = null;


        public AreaObject(in LevelIO.ObjectInfo info, SM3DWorldZone zone, out bool loadLinks)
            : base(info.Position, info.Rotation, info.Scale)
        {
            ID = info.ID;
            if (zone != null)
                zone.SubmitRailID(ID);

            ModelName = info.ModelName;
            ClassName = info.ObjectName; //...yep

            Layer = info.Layer;

            comment = info.Comment;

            Properties = new Dictionary<string, dynamic>();

            foreach (var propertyEntry in info.PropertyEntries)
            {
                switch (propertyEntry.Key)
                {
                    case "Priority":
                        Priority = propertyEntry.Value.Parse();
                        continue;
                    default:
                        Properties.Add(propertyEntry.Key, propertyEntry.Value.Parse());
                        continue;
                }
            }

            zone?.SubmitRailID(ID);

            loadLinks = true;
        }
        /// <summary>
        /// Creates a new rail for 3DW
        /// </summary>
        /// <param name="pathPoints">List of Path Points to use in this rail</param>
        /// <param name="iD">ID Of the rail</param>
        /// <param name="isClosed">Is the path closed?</param>
        /// <param name="Priority">Unknown</param>
        /// <param name="isReverseCoord">Reverses the order the rails are in</param>
        /// <param name="className"></param>
        public AreaObject(
            Vector3 pos, Vector3 rot, Vector3 scale,
            string iD, string modelName, string className, int priority,
            Dictionary<string, List<I3dWorldObject>> links, Dictionary<string, dynamic> properties, SM3DWorldZone zone)
            : base(pos, rot, scale)
        {
            ID = iD;
            ModelName = modelName;
            ClassName = className;
            Priority = priority;
            Properties = properties;
            Links = links;
            
            zone?.SubmitID(ID);
        }


        #region I3DWorldObject implementation
        /// <summary>
        /// All places where this object is linked to
        /// </summary>
        public List<(string, I3dWorldObject)> LinkDestinations { get; } = new List<(string, I3dWorldObject)>();

        public Dictionary<string, List<I3dWorldObject>> Links { get; set; } = null;

        public void Save(HashSet<I3dWorldObject> alreadyWrittenObjs, ByamlNodeWriter writer, DictionaryNode objNode, HashSet<string> layers, bool isLinkDest = false)
        {
            objNode.AddDynamicValue("Comment", null);
            objNode.AddDynamicValue("Id", ID);
            objNode.AddDynamicValue("Priority", Priority);

            objNode.AddDynamicValue("IsLinkDest", isLinkDest);
            objNode.AddDynamicValue("LayerConfigName", Layer);

            alreadyWrittenObjs.Add(this);

            ObjectUtils.SaveLinks(Links, alreadyWrittenObjs, writer, objNode, layers);

            objNode.AddDynamicValue("ModelName", ModelName);


            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Rotation), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Scale), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", ObjectUtils.CreateUnitConfig(ClassName), true);

            objNode.AddDynamicValue("UnitConfigName", ClassName);

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

        public virtual Vector3 GetLinkingPoint(SM3DWorldScene editorScene)
        {
            return GlobalPosition;
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


            duplicates[this] = new AreaObject(
                ObjectUtils.TransformedPosition(Position, zoneToZoneTransform),
                ObjectUtils.TransformedPosition(Rotation, zoneToZoneTransform),

                Scale, destZone?.NextObjID(), ModelName, ClassName, Priority,

                ObjectUtils.DuplicateLinks(Links),
                ObjectUtils.DuplicateProperties(Properties),
                destZone);
        }

        public void LinkDuplicates(SM3DWorldScene.DuplicationInfo duplicationInfo, bool allowKeepLinksOfDuplicate)
            => ObjectUtils.LinkDuplicates(this, duplicationInfo, allowKeepLinksOfDuplicate);

        public bool TryGetObjectList(SM3DWorldZone zone, out ObjectList objList)
        {
            return Program.ParameterDB.AreaParameters[ClassName].TryGetObjectList(zone, out objList);
        }

        public void AddToZoneBatch(ZoneRenderBatch zoneBatch)
        {
            //not needed
        }

        #endregion

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!Selected)
            {
                if (!Spotlight.Properties.Settings.Default.DrawAreas)
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

            control.UpdateModelMatrix(
                    Matrix4.CreateScale(Selected ? editorScene.SelectionTransformAction.NewScale(GlobalScale, rotMtx) : GlobalScale) *
                    new Matrix4(Selected ? editorScene.SelectionTransformAction.NewRot(rotMtx) : rotMtx) *
                    Matrix4.CreateTranslation(transformedGloablPos));


            if (AreaRenderer.TryDraw(ClassName, ModelName, control, pass, highlightColor))
                return;
            else
            {
                if (pass == Pass.TRANSPARENT)
                    return;

                Vector4 color = new Vector4(1, 0.8f, 0, 1);

                Vector4 blockColor = color * (1 - highlightColor.W) + highlightColor * highlightColor.W;
                Vector4 lineColor;

                if (highlightColor.W != 0)
                    lineColor = highlightColor;
                else
                    lineColor = color;

                lineColor.W = 1;

                Renderers.ColorCubeRenderer.Draw(control, pass, blockColor, lineColor, control.NextPickingColor());
            }
        }

        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            if (!Selected)
                return false;
            objectUIControl.AddObjectUIContainer(new BasicPropertyUIContainer(this, scene), "Area");

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

        public override bool Equals(object obj)
        {
            return obj is AreaObject @object &&
                   Position.Equals(@object.Position) &&
                   Rotation.Equals(@object.Rotation) &&
                   Scale.Equals(@object.Scale) &&
                   ID == @object.ID &&
                   Priority == @object.Priority &&
                   ClassName == @object.ClassName &&
                   ModelName == @object.ModelName &&
                   Layer == @object.Layer &&
                   ObjectUtils.EqualProperties(Properties, @object.Properties);
        }

        public class BasicPropertyUIContainer : IObjectUIContainer
        {
            PropertyCapture? capture = null;

            AreaObject area;
            EditorSceneBase scene;

            string[] shapeNames;
            string[] DB_classNames;

            public BasicPropertyUIContainer(AreaObject area, EditorSceneBase scene)
            {
                this.area = area;
                this.scene = scene;

                shapeNames = LevelIO.AreaModelNames.ToArray();
                DB_classNames = Program.ParameterDB.AreaParameters.Keys.ToArray();
            }

            public void DoUI(IObjectUIControl control)
            {
                if (Spotlight.Properties.Settings.Default.AllowIDEdits)
                    area.ID = control.TextInput(area.ID, "Object ID");
                else
                    control.TextInput(area.ID, "Object ID");

                if (area.comment != null)
                    control.TextInput(area.comment, "Comment");

                area.Layer = control.TextInput(area.Layer, "Layer");

                area.ClassName = control.DropDownTextInput("Class Name", area.ClassName, DB_classNames);
                area.ModelName = control.DropDownTextInput("Shape Name", area.ModelName, shapeNames, false);

                area.Priority = (int)control.NumberInput(area.Priority, "Priority");

                control.VerticalSeperator();

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    area.Position = control.Vector3Input(area.Position, "Position", 1, 16);
                else
                    area.Position = control.Vector3Input(area.Position, "Position", 0.125f, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    area.Rotation = control.Vector3Input(area.Rotation, "Rotation", 45, 18);
                else
                    area.Rotation = control.Vector3Input(area.Rotation, "Rotation", 5, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    area.Scale = control.Vector3Input(area.Scale, "Scale", 1, 16);
                else
                    area.Scale = control.Vector3Input(area.Scale, "Scale", 0.125f, 2);
            }

            public void OnValueChangeStart()
            {
                capture = new PropertyCapture(area);
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






    public static class AreaRenderer
    {
        private static bool Initialized = false;

        public static ShaderProgram DefaultShaderProgram { get; private set; }

        public static void Initialize()
        {
            if (!Initialized)
            {
                ExtraModelRenderer.Initialize();

                var defaultFrag = new FragmentShader(
                    @"#version 330
                    uniform vec4 highlight_color;
                    in vec4 vertex_color;
                
                    void main(){
                        float hc_a   = highlight_color.w;
                        gl_FragColor = vec4( mix(vertex_color.rgb, highlight_color.rgb, hc_a), vertex_color.a);
                    }");
                var defaultVert = new VertexShader(
                    @"#version 330
                    layout(location = 0) in vec4 position;
                    layout(location = 1) in vec4 color;
                    uniform vec4 colorA;
                    uniform vec4 colorB;

                    uniform mat4 mtxMdl;
                    uniform mat4 mtxCam;
                    out vec4 vertex_color;

                    void main(){
                        vertex_color = mix(colorA, colorB, color.g*2) * color.b;
                        gl_Position = mtxCam*mtxMdl*position;
                    }");
                DefaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);


                Initialized = true;
            }
        }

        public static bool TryDraw(string className, string modelName, GL_ControlModern control, Pass pass, Vector4 highlightColor)
        {
            if (pass != Pass.TRANSPARENT)
            {
                Vector4 colorA;
                Vector4 colorB;

                switch (className)
                {
#if ODYSSEY
                    case "CameraArea2D":
#endif
                    case "CameraArea": //red
                        colorA = new Vector4(1, 0, 0, 1);
                        colorB = new Vector4(1, 0, 0, 1);
                        break;

                    case "GraphicsArea": //yellow-orange
                        colorA = new Vector4(1, 0.5f, 0, 1);
                        colorB = new Vector4(1, 1, 0, 1);
                        break;
#if ODYSSEY
                    case "HackerCheckKeepOnArea":
#endif
                    case "SwitchOnArea":
                    case "SwitchKeepOnArea": //light blue
                        colorA = new Vector4(0, 1, 1, 1);
                        colorB = new Vector4(0, 1, 1, 1);
                        break;

                    case "ViewCtrlArea": //grey
                        colorA = new Vector4(0.5f ,0.5f ,0.5f , 1);
                        colorB = new Vector4(0.75f, 0.75f, 0.75f, 1);
                        break;

#if ODYSSEY
                    case "SnapMoveArea":
                    case "2DMoveArea": //green
                        colorA = new Vector4(0, 0.5f, 0, 1);
                        colorB = new Vector4(0, 1, 0, 1);
                        break;
#endif
                    case "DeathArea": //dark red
                        colorA = new Vector4(0.5f, 0, 0, 1);
                        colorB = new Vector4(0.5f, 0, 0, 1);
                        break;

                    default: //blue
                        colorA = new Vector4(0, 0, 1, 1);
                        colorB = new Vector4(0, 0.5f, 1, 1);
                        break;
                }

                ExtraModelRenderer.TryGetModel(modelName, out var extraModel);

                if (extraModel == null)
                    return false;

                if (pass == Pass.PICKING)
                {
                    control.CurrentShader = Framework.SolidColorShaderProgram;

                    GL.LineWidth(4);
                    control.CurrentShader.SetVector4("color", control.NextPickingColor());
                }
                else if (pass == Pass.OPAQUE)
                {
                    control.CurrentShader = DefaultShaderProgram;

                    GL.PointSize(4);

                    DefaultShaderProgram.SetVector4("colorA", colorA);
                    DefaultShaderProgram.SetVector4("colorB", colorB);
                    DefaultShaderProgram.SetVector4("highlight_color", highlightColor);
                }

                extraModel.Vao.Use(control);

                GL.DrawElements(extraModel.PrimitiveType, extraModel.IndexCount, DrawElementsType.UnsignedInt, 0);

                GL.LineWidth(2);

                GL.Disable(EnableCap.Blend);
            }

            return true;

        }
    }
}
