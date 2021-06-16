using BYAML;
using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using Spotlight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BYAML.ByamlNodeWriter;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using WinInput = System.Windows.Input;

namespace Spotlight.EditorDrawables
{
    public struct ZoneTransform
    {
        public ZoneTransform(Matrix4 positionTransform, Matrix3 rotationTransform)
        {
            PositionTransform = positionTransform;
            RotationTransform = rotationTransform;
        }

        public static ZoneTransform Identity = new ZoneTransform(Matrix4.Identity, Matrix3.Identity);

        public Matrix4 PositionTransform { get; set; }
        public Matrix3 RotationTransform { get; set; }
    }

    public class ZonePlacement : SingleObject
    {
        public SM3DWorldZone Zone;

        /// <summary>
        /// the name of the zone this zone placement looks for in StageData, will not change if the Zone's name changes
        /// </summary>
        internal string ZoneLookupName;

        [PropertyCapture.Undoable]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [PropertyCapture.Undoable]
        public string Layer { get; set; }

        public ZonePlacement(Vector3 pos, Vector3 rot, string layer, SM3DWorldZone zone)
            : base(pos)
        {
            Rotation = rot;

            Layer = layer;

            Zone = zone;

            ZoneLookupName = zone.StageName;

            Zone.UpdateRenderBatch();
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!SceneDrawState.EnabledLayers.Contains(Layer))
            {
                control.SkipPickingColors(1);
                return;
            }

            Vector4 hightlightColor;

            if (Selected && editorScene.Hovered == this)
                hightlightColor = General3dWorldObject.hoverSelectColor;
            else if (Selected)
                hightlightColor = General3dWorldObject.selectColor;
            else if (editorScene.Hovered == this)
                hightlightColor = General3dWorldObject.hoverColor;
            else
                hightlightColor = Vector4.Zero;

            Matrix3 rotMat = Framework.Mat3FromEulerAnglesDeg(Rotation);

            rotMat = Selected ? editorScene.SelectionTransformAction.NewRot(rotMat) : rotMat;

            Matrix4 transform = new Matrix4(rotMat) * Matrix4.CreateTranslation(Selected ? editorScene.SelectionTransformAction.NewPos(Position) : Position);

            Vector4 pickingColor = control.NextPickingColor();

            foreach (var renderer in Zone.ZoneBatch.BatchRenderers)
            {
                renderer.Draw(control, pass, hightlightColor, transform, pickingColor);
            }
        }

        public override void StartDragging(DragActionType actionType, int hoveredPart, EditorSceneBase scene)
        {
            if (Selected)
                scene.StartTransformAction(new LocalOrientation(GlobalPosition, Framework.Mat3FromEulerAnglesDeg(Rotation)), actionType);
        }

        public ZoneTransform GetTransform()
        {
            Matrix3 rotMat = Framework.Mat3FromEulerAnglesDeg(Rotation);

            return new ZoneTransform(
                new Matrix4(rotMat) * Matrix4.CreateTranslation(Position),
                rotMat);
        }

        public override int GetPickableSpan()
        {
            if (!Visible)
                return 0;
            else
                return 1;
        }

        public void Save(ByamlNodeWriter writer, DictionaryNode objNode, int zoneID)
        {
            objNode.AddDynamicValue("Comment", null);
            objNode.AddDynamicValue("Id", "zone" + zoneID);
            objNode.AddDynamicValue("IsLinkDest", false);
            objNode.AddDynamicValue("LayerConfigName", Layer);

            {
                objNode.AddDynamicValue("Links", new Dictionary<string, dynamic>(), true);
            }

            objNode.AddDynamicValue("ModelName", null);
            objNode.AddDynamicValue("Rotate", LevelIO.Vector3ToDict(Rotation), true);
            objNode.AddDynamicValue("Scale", LevelIO.Vector3ToDict(Vector3.One), true);
            objNode.AddDynamicValue("Translate", LevelIO.Vector3ToDict(Position, 100f), true);

            objNode.AddDynamicValue("UnitConfig", new Dictionary<string, dynamic>
            {
                ["DisplayName"] = "ï¿½Rï¿½Cï¿½ï¿½(ï¿½ï¿½ï¿½ï¿½ï¿½Oï¿½zï¿½u)",
                ["DisplayRotate"] = LevelIO.Vector3ToDict(Vector3.Zero),
                ["DisplayScale"] = LevelIO.Vector3ToDict(Vector3.One),
                ["DisplayTranslate"] = LevelIO.Vector3ToDict(Vector3.Zero),
                ["GenerateCategory"] = "",
                ["ParameterConfigName"] = "Zone",
                ["PlacementTargetFile"] = "Map"
            }, true);

            objNode.AddDynamicValue("UnitConfigName", Zone.StageName);
        }

        public override void SetTransform(Vector3? pos, Vector3? rot, Vector3? scale, int part, out Vector3? prevPos, out Vector3? prevRot, out Vector3? prevScale)
        {
            prevPos = null;
            prevRot = null;
            prevScale = null;

            if (pos.HasValue)
            {
                prevPos = Position;
                Position = pos.Value;
            }

            if (rot.HasValue)
            {
                prevRot = Rotation;
                Rotation = rot.Value;
            }
        }

        public override void ApplyTransformActionToSelection(AbstractTransformAction transformAction, ref TransformChangeInfos infos)
        {
            if (!Selected)
                return;

            Vector3 pp = Position, pr = Rotation;

            var newPos = transformAction.NewPos(GlobalPosition, out bool posHasChanged);

            var newRot = transformAction.NewRot(Rotation, out bool rotHasChanged);

            if (posHasChanged)
                GlobalPosition = newPos;
            if (rotHasChanged)
                Rotation = newRot;

            infos.Add(this, 0,
                posHasChanged ? new Vector3?(pp) : null,
                rotHasChanged ? new Vector3?(pr) : null,
                null);
        }

        public override bool TrySetupObjectUIControl(EditorSceneBase scene, ObjectUIControl objectUIControl)
        {
            if (!Selected)
                return false;
            objectUIControl.AddObjectUIContainer(new PropertyProvider(this, scene), "Transform");
            return true;
        }

        public new class PropertyProvider : IObjectUIContainer
        {
            PropertyCapture? capture = null;

            ZonePlacement placement;
            EditorSceneBase scene;
            public PropertyProvider(ZonePlacement placement, EditorSceneBase scene)
            {
                this.placement = placement;
                this.scene = scene;
            }

            public void DoUI(IObjectUIControl control)
            {
                placement.Layer = control.TextInput(placement.Layer, "Layer");

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    placement.Position = control.Vector3Input(placement.Position, "Position", 1, 16);
                else
                    placement.Position = control.Vector3Input(placement.Position, "Position", 0.125f, 2);

                if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
                    placement.Rotation = control.Vector3Input(placement.Rotation, "Rotation", 45, 18, -180, 180, true);
                else
                    placement.Rotation = control.Vector3Input(placement.Rotation, "Rotation", 5, 2, -180, 180, true);
            }

            public void OnValueChangeStart()
            {
                capture = new PropertyCapture(placement);
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

        public override string ToString() => Zone.StageName;
    }
}
