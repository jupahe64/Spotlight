using GL_EditorFramework;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using SpotLight.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotLight.EditorDrawables
{
    public class ZonePlacement : TransformableObject
    {
        public readonly SM3DWorldZone Zone;
        public ZonePlacement(Vector3 pos, Vector3 rot, Vector3 scale, SM3DWorldZone zone)
            : base(pos, rot, scale)
        {
            Zone = zone;

            UpdateBatch();
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!Visible)
                return;

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

            ZoneTransform transformBackup = SceneDrawState.ZoneTransform;

            Matrix4 transform = new Matrix4(rotMat) * Matrix4.CreateTranslation(Selected ? editorScene.SelectionTransformAction.NewPos(Position) : Position);

            Vector4 pickingColor = control.NextPickingColor();

            foreach (var renderer in batchRenderers.Values)
            {
                renderer.Draw(control, pass, hightlightColor, transform, pickingColor);
            }

            SceneDrawState.ZoneTransform = transformBackup;
        }

        Dictionary<Type, IBatchRenderer> batchRenderers = new Dictionary<Type, IBatchRenderer>();

        public IBatchRenderer GetBatchRenderer(Type batchType)
        {
            if (!batchRenderers.ContainsKey(batchType) && typeof(IBatchRenderer).IsAssignableFrom(batchType))
                batchRenderers.Add(batchType, (IBatchRenderer)Activator.CreateInstance(batchType));

            return batchRenderers[batchType];
        }

        public void UpdateBatch()
        {
            batchRenderers.Clear();

            SceneObjectIterState.InLinks = false;
            foreach (KeyValuePair<string, ObjectList> keyValuePair in Zone.ObjLists)
            {
                if (keyValuePair.Key == SM3DWorldZone.MAP_PREFIX + "SkyList")
                    continue;

                foreach (I3dWorldObject obj in keyValuePair.Value)
                    obj.AddToZoneBatch(this);
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in Zone.LinkedObjects)
                obj.AddToZoneBatch(this);
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

        public override string ToString() => Zone.LevelName;
    }

    public interface IBatchRenderer
    {
        void Draw(GL_ControlModern control, Pass pass, Vector4 highlightColor, Matrix4 zoneTransform, Vector4 pickingColor);
    }
}
