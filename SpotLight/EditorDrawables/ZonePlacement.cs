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

            Zone.UpdateRenderBatch();
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
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
}
