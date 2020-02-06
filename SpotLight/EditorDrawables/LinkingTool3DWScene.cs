using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework;
using OpenTK;
using WinInput = System.Windows.Input;

namespace SpotLight.EditorDrawables
{
    class LinkEdit3DWScene : SM3DWorldScene
    {
        LinkManager linkManager;

        public LinkConnection SelectedConnection = null;

        public LinkEdit3DWScene()
        {
            linkManager = new LinkManager(this);

            StaticObjects.Add(linkManager);
        }

        enum LinkDragMode
        {
            None,
            Source,
            Dest
        }

        LinkDragMode linkDragMode = LinkDragMode.None;

        public override uint MouseDown(MouseEventArgs e, GL_ControlBase control)
        {
            if (WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift))
            {
                SelectedConnection = new LinkConnection(Hovered3dObject, Hovered3dObject, "UnnamedConnection");
                linkDragMode = LinkDragMode.Dest;
            }
            else if (SelectedConnection != null)
            {
                //Check if mouse is on the Dest Point
                Point linkingPoint = GL_Control.ScreenCoordFor(SelectedConnection.Dest.GetLinkingPoint(this));
                Point nextToLinkingPoint = GL_Control.ScreenCoordFor(SelectedConnection.Dest.GetLinkingPoint(this) + control.InvertedRotationMatrix.Row0 * 0.25f);
                Vector2 diff = new Vector2(e.X, e.Y) - new Vector2(linkingPoint.X, linkingPoint.Y);
                Vector2 nextToDiff = new Vector2(nextToLinkingPoint.X, nextToLinkingPoint.Y) - new Vector2(linkingPoint.X, linkingPoint.Y);
                if (diff.LengthSquared < nextToDiff.LengthSquared)
                {
                    linkDragMode = LinkDragMode.Dest;
                    return 0;
                }

                //Check if mouse is on the Source Point
                linkingPoint = GL_Control.ScreenCoordFor(SelectedConnection.Source.GetLinkingPoint(this));
                nextToLinkingPoint = GL_Control.ScreenCoordFor(SelectedConnection.Source.GetLinkingPoint(this) + control.InvertedRotationMatrix.Row0 * 0.25f);
                diff = new Vector2(e.X, e.Y) - new Vector2(linkingPoint.X, linkingPoint.Y);
                nextToDiff = new Vector2(nextToLinkingPoint.X, nextToLinkingPoint.Y) - new Vector2(linkingPoint.X, linkingPoint.Y);
                if (diff.LengthSquared < nextToDiff.LengthSquared)
                {
                    linkDragMode = LinkDragMode.Source;
                    return 0;
                }

                linkDragMode = LinkDragMode.None;
            }

            return base.MouseDown(e, control);
        }

        public override uint MouseMove(MouseEventArgs e, Point lastMousePos, GL_ControlBase control)
        {
            uint var = base.MouseMove(e, lastMousePos, control);
            if (linkDragMode != LinkDragMode.None)
                var |= NO_CAMERA_ACTION;

            if (linkDragMode != LinkDragMode.None)
                var |= REDRAW;

            return var;
        }

        public override uint KeyDown(KeyEventArgs e, GL_ControlBase control)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                return base.KeyDown(e, control) | REDRAW_PICKING | FORCE_REENTER;
            }
            else
                return base.KeyDown(e, control);
        }

        public override uint KeyUp(KeyEventArgs e, GL_ControlBase control)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                return base.KeyUp(e, control) | REDRAW_PICKING | FORCE_REENTER;
            }
            else
                return base.KeyUp(e, control);
        }

        public override uint MouseUp(MouseEventArgs e, GL_ControlBase control)
        {
            if(linkDragMode != LinkDragMode.None && Hovered3dObject != null)
            {
                if(SelectedConnection.Source == SelectedConnection.Dest) //just created
                {
                    if (SelectedConnection.Source == Hovered3dObject)
                        SelectedConnection = null;
                    else
                    {
                        AddConnection(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name);

                        UpdateLinkDestinations();

                        AddToUndo(new RevertableConnectionAddition(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name));

                        SelectedConnection.Dest = Hovered3dObject;

                        UpdateSelection(0);
                    }
                }
                else if (linkDragMode == LinkDragMode.Source)
                    ChangeSelectedConnection(Hovered3dObject, SelectedConnection.Dest, SelectedConnection.Name);

                else
                    ChangeSelectedConnection(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name);
            }

            linkDragMode = LinkDragMode.None;

            return base.MouseUp(e, control) | REDRAW;
        }
        
        private void ChangeSelectedConnection(I3dWorldObject newSource, I3dWorldObject newDest, string newName)
        {
            if (newSource == SelectedConnection.Source && newDest == SelectedConnection.Dest && newName == SelectedConnection.Name)
                return; //nothing has changed

            if (newSource == newDest)
                return; //a link can't load to itself

            if(newName == "")
                return; //a links name can't be empty

            if (newSource is Rail)
                return; //Rails can't have links

            AddToUndo(new RevertableConnectionChange(
                newSource, newDest, newName,
                SelectedConnection.Source,
                SelectedConnection.Dest,
                SelectedConnection.Name
                ));

            RemoveConnection(
                SelectedConnection.Source, 
                SelectedConnection.Dest, 
                SelectedConnection.Name
                );

            AddConnection(newSource, newDest, newName);

            UpdateLinkDestinations();

            SelectedConnection = new LinkConnection(newSource, newDest, newName);
        }
        
        public override uint MouseClick(MouseEventArgs e, GL_ControlBase control)
        {
            if (e.Button == MouseButtons.Right)
            {
                linkDragMode = LinkDragMode.None;
                return REDRAW;
            }

            if (linkManager.hoveredConnection != null)
            {
                SelectedConnection = linkManager.hoveredConnection;
                UpdateSelection(REDRAW);
                return REDRAW;
            }
            else
            {
                if (SelectedConnection != null)
                {
                    SelectedConnection = null;
                    return REDRAW;
                }


                return 0;
            }
        }

        public class ConnectionUI : IObjectUIContainer
        {
            LinkEdit3DWScene scene;

            string connectionName;

            public ConnectionUI(LinkEdit3DWScene scene)
            {
                this.scene = scene;

                connectionName = scene.SelectedConnection.Name;
            }

            public void DoUI(IObjectUIControl control)
            {
                connectionName = control.TextInput(connectionName, "Connection Name");
                if(control.Button("Reverse Connection"))
                {
                    scene.ChangeSelectedConnection(
                        scene.SelectedConnection.Dest,
                        scene.SelectedConnection.Source,
                        scene.SelectedConnection.Name);

                    scene.GL_Control.Refresh();
                }
            }

            public void OnValueChangeStart()
            {
                
            }

            public void OnValueChanged()
            {
                
            }

            public void OnValueSet()
            {
                scene.ChangeSelectedConnection(scene.SelectedConnection.Source, scene.SelectedConnection.Dest, connectionName);
                scene.GL_Control.Refresh();
            }

            public void UpdateProperties()
            {

            }
        }

        public override void SetupObjectUIControl(ObjectUIControl objectUIControl)
        {
            objectUIControl.ClearObjectUIContainers();

            if (SelectedConnection != null)
                objectUIControl.AddObjectUIContainer(new ConnectionUI(this), "Link Connection Settings");


            objectUIControl.Refresh();
        }

        public override uint MouseEnter(int inObjectIndex, GL_ControlBase control)
        {
            uint var = base.MouseEnter(inObjectIndex, control);

            if(linkDragMode==LinkDragMode.None && WinInput.Keyboard.IsKeyUp(WinInput.Key.LeftShift))
                Hovered = null;

            return var;
        }

        public override void DeleteSelected()
        {
            if (SelectedConnection == null)
                return;

            AddToUndo(new RevertableConnectionDeletion(
                SelectedConnection.Source,
                SelectedConnection.Dest,
                SelectedConnection.Name));

            RemoveConnection(
                SelectedConnection.Source,
                SelectedConnection.Dest,
                SelectedConnection.Name);
            
            SelectedConnection = null;

            UpdateSelection(REDRAW);
        }

        public class LinkConnection
        {
            public I3dWorldObject Source;
            public I3dWorldObject Dest;
            public string Name;

            public LinkConnection(I3dWorldObject source, I3dWorldObject dest, string name)
            {
                Source = source;
                Dest = dest;
                Name = name;
            }
        }

        class LinkManager : AbstractGlDrawable
        {
            LinkEdit3DWScene scene;

            static ShaderProgram LinksShaderProgram;

            static bool Initialized = false;

            public LinkManager(LinkEdit3DWScene scene)
            {
                this.scene = scene;
            }

            /// <summary>
            /// Prepares to draw models
            /// </summary>
            /// <param name="control">The GL_Control that's currently in use</param>
            public override void Prepare(GL_ControlModern control)
            {
                if (!Initialized)
                {
                    LinksShaderProgram = new ShaderProgram(
                        new FragmentShader(
                  @"#version 330
                in vec4 fragColor;
                in vec2 fragUV;
                void main(){
                    float d = length(fragUV);
                    if(d<0.9)
                        gl_FragColor = fragColor;
                    else if(d<1.0)
                        gl_FragColor = vec4(0,0,0,1);
                    else
                        discard;
                }"),
                        new VertexShader(
                  @"#version 330
                layout(location = 0) in vec4 position;
                layout(location = 1) in vec4 color;
                layout(location = 2) in vec2 uv;

                out vec4 fragColor;
                out vec2 fragUV;

                uniform mat4 mtxMdl;
                uniform mat4 mtxCam;
                void main(){
                    gl_Position = mtxCam*mtxMdl*position;
                    fragColor = color;
                    fragUV = uv;
                }"), control);

                    Initialized = true;
                }
            }

            public override void Draw(GL_ControlModern control, Pass pass)
            {
                control.ResetModelMatrix();

                control.CurrentShader = LinksShaderProgram;



                if (pass == Pass.OPAQUE)
                {
                    Vector3 sourceObjPoint = Vector3.Zero;
                    Vector3 destObjPoint = Vector3.Zero;

                    GL.LineWidth(1);
                    GL.Begin(PrimitiveType.Lines);
                    GL.VertexAttrib2(2, Vector2.Zero);
                    foreach (I3dWorldObject _obj in scene.Get3DWObjects())
                    {
                        Vector3 _objPoint = _obj.GetLinkingPoint(scene);

                        if (_obj == scene.SelectedConnection?.Dest)
                            destObjPoint = _objPoint;

                        if (_obj.Links != null)
                        {
                            if (_obj == scene.SelectedConnection?.Source)
                                sourceObjPoint = _objPoint;

                            foreach (KeyValuePair<string, List<I3dWorldObject>> link in _obj.Links)
                            {
                                foreach (I3dWorldObject obj in link.Value)
                                {
                                    if (_obj     != scene.SelectedConnection?.Source ||
                                        obj      != scene.SelectedConnection?.Dest ||
                                        link.Key != scene.SelectedConnection?.Name)
                                    {
                                        Vector3 objPoint = obj.GetLinkingPoint(scene);

                                        if (_obj      == hoveredConnection?.Source &&
                                            obj      == hoveredConnection?.Dest &&
                                            link.Key == hoveredConnection?.Name)
                                        {
                                            GL.VertexAttrib4(1, EditableObject.selectColor);
                                            GL.Vertex3(_objPoint);
                                            GL.Vertex3(objPoint);
                                        }
                                        else{
                                            GL.VertexAttrib4(1, new Vector4(1, 1, 1, 1));
                                            GL.Vertex3(_objPoint);
                                            GL.Vertex3(objPoint);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GL.End();

                    if (scene.SelectedConnection != null)
                    {
                        Vector3 hoveredObjPoint = -scene.GL_Control.GetPointUnderMouse();

                        Vector3 sourcePoint = (scene.linkDragMode == LinkDragMode.Source) ? hoveredObjPoint : sourceObjPoint;
                        Vector3 destPoint = (scene.linkDragMode == LinkDragMode.Dest) ? hoveredObjPoint : destObjPoint;

                        GL.DepthFunc(DepthFunction.Always);

                        GL.LineWidth(6);
                        GL.Begin(PrimitiveType.Lines);
                        GL.VertexAttrib4(1, new Vector4(0, 0, 0, 1));
                        GL.Vertex3(sourcePoint);
                        GL.Vertex3(destPoint);
                        GL.End();

                        GL.LineWidth(3);
                        GL.Begin(PrimitiveType.Lines);
                        GL.VertexAttrib4(1, new Vector4(0, 1, 0, 1));
                        GL.Vertex3(sourcePoint);
                        GL.VertexAttrib4(1, new Vector4(1, 0, 0, 1));
                        GL.Vertex3(destPoint);
                        GL.End();

                        control.UpdateModelMatrix(new Matrix4(control.InvertedRotationMatrix) * Matrix4.CreateTranslation(sourcePoint));
                        GL.Begin(PrimitiveType.Quads);
                        GL.VertexAttrib4(1, new Vector4(0, 1, 0, 1));

                        GL.VertexAttrib2(2, new Vector2(-1, -1));
                        GL.Vertex3(-0.25f, -0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(1, -1));
                        GL.Vertex3(0.25f, -0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(1, 1));
                        GL.Vertex3(0.25f, 0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(-1, 1));
                        GL.Vertex3(-0.25f, 0.25f, 0);
                        GL.End();

                        control.UpdateModelMatrix(new Matrix4(control.InvertedRotationMatrix) * Matrix4.CreateTranslation(destPoint));
                        GL.Begin(PrimitiveType.Quads);
                        GL.VertexAttrib4(1, new Vector4(1, 0, 0, 1));

                        GL.VertexAttrib2(2, new Vector2(-1, -1));
                        GL.Vertex3(-0.25f, -0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(1, -1));
                        GL.Vertex3(0.25f, -0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(1, 1));
                        GL.Vertex3(0.25f, 0.25f, 0);
                        GL.VertexAttrib2(2, new Vector2(-1, 1));
                        GL.Vertex3(-0.25f, 0.25f, 0);
                        GL.End();

                        GL.DepthFunc(DepthFunction.Lequal);
                    }
                    GL.LineWidth(2);
                }
                else if (pass == Pass.PICKING && scene.linkDragMode==LinkDragMode.None)
                {
                    GL.LineWidth(6);
                    GL.Begin(PrimitiveType.Lines);
                    GL.VertexAttrib2(2, Vector2.Zero);
                    foreach (I3dWorldObject _obj in scene.Get3DWObjects())
                    {
                        if (_obj.Links != null)
                        {
                            foreach (List<I3dWorldObject> link in _obj.Links.Values)
                            {
                                foreach (I3dWorldObject obj in link)
                                {
                                    GL.VertexAttrib4(1, control.NextPickingColor());
                                    GL.Vertex3(_obj.GetLinkingPoint(scene));
                                    GL.Vertex3(obj.GetLinkingPoint(scene));
                                }
                            }
                        }
                    }
                    GL.End();
                    GL.LineWidth(2);
                }
            }

            public override int GetPickableSpan()
            {
                if (scene.linkDragMode != LinkDragMode.None)
                    return 0;

                int span = 0;
                foreach (I3dWorldObject _obj in scene.Get3DWObjects())
                {
                    if (_obj.Links != null)
                    {
                        foreach (List<I3dWorldObject> link in _obj.Links.Values)
                        {
                            foreach (I3dWorldObject obj in link)
                            {
                                span++;
                            }
                        }
                    }
                }
                return span;
            }

            public LinkConnection hoveredConnection = null;

            public override uint MouseEnter(int index, GL_ControlBase control)
            {
                int part = 0;
                foreach (I3dWorldObject _obj in scene.Get3DWObjects())
                {
                    if (_obj.Links != null)
                    {
                        foreach (KeyValuePair<string, List<I3dWorldObject>> link in _obj.Links)
                        {
                            foreach (I3dWorldObject obj in link.Value)
                            {
                                if (part == index)
                                {
                                    hoveredConnection = new LinkConnection(_obj, obj, link.Key);
                                    return REDRAW;
                                }
                                else
                                    part++;
                            }
                        }
                    }
                }

                return 0;
            }

            public override uint MouseLeave(int index, GL_ControlBase control)
            {
                hoveredConnection = null;
                return REDRAW;
            }

            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                throw new NotImplementedException();
            }

            public override void Prepare(GL_ControlLegacy control)
            {
                throw new NotImplementedException();
            }
        }
    }
}
