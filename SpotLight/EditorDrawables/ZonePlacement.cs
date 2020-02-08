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
            this.Zone = zone;
        }

        public override void Prepare(GL_ControlModern control)
        {
            base.Prepare(control);

            if (!Zone.IsPrepared)
            {
                Matrix3 rotMat = Framework.Mat3FromEulerAnglesDeg(Rotation);

                SceneDrawState.ZoneTransform = new ZoneTransform(
                    new Matrix4(rotMat) * Matrix4.CreateTranslation(Position),
                    rotMat);

                SceneObjectIterState.InLinks = false;
                foreach (ObjectList objects in Zone.ObjLists.Values)
                {
                    foreach (I3dWorldObject obj in objects)
                        obj.Prepare(control);
                }
                SceneObjectIterState.InLinks = true;
                foreach (I3dWorldObject obj in Zone.LinkedObjects)
                    obj.Prepare(control);

                Zone.IsPrepared = true;
            }

            SceneDrawState.ZoneTransform = ZoneTransform.Identity;
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (!Visible)
                return;

            if (Selected)
                SceneDrawState.HighlightColorOverride = General3dWorldObject.selectColor;
            else if(editorScene.Hovered==this)
                SceneDrawState.HighlightColorOverride = General3dWorldObject.hoverColor;

            Matrix3 rotMat = Framework.Mat3FromEulerAnglesDeg(Rotation);

            rotMat = Selected ? editorScene.CurrentAction.NewRot(rotMat) : rotMat;

            SceneDrawState.ZoneTransform = new ZoneTransform(
                new Matrix4(rotMat) * Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(Position) : Position),
                rotMat);

            SceneObjectIterState.InLinks = false;
            foreach (ObjectList objects in Zone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objects)
                    obj.Draw(control, pass, editorScene);
            }
            SceneObjectIterState.InLinks = true;
            foreach (I3dWorldObject obj in Zone.LinkedObjects)
                obj.Draw(control, pass, editorScene);

            SceneDrawState.ZoneTransform = ZoneTransform.Identity;

            SceneDrawState.HighlightColorOverride = null;
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

            Matrix3 rotMat = Framework.Mat3FromEulerAnglesDeg(Rotation);

            SceneDrawState.ZoneTransform = new ZoneTransform(
                new Matrix4(rotMat) * Matrix4.CreateTranslation(Position),
                rotMat);

            int count = 0;
            foreach (ObjectList objList in Zone.ObjLists.Values)
            {
                foreach (I3dWorldObject obj in objList)
                    count += obj.GetPickableSpan();            
            }

            foreach (I3dWorldObject obj in Zone.LinkedObjects)
                count += obj.GetPickableSpan();

            return count;
        }

        public override string ToString() => Zone.LevelName;
    }
}
