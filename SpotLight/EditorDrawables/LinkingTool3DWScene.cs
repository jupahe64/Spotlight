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
using Spotlight.Level;

namespace Spotlight.EditorDrawables
{
    public class DestinationChangedEventArgs : EventArgs
    {
        public I3dWorldObject LinkDestination { get; set; }
        public bool DestWasMovedToLinked { get; set; }

        public DestinationChangedEventArgs(I3dWorldObject linkDestination, bool destWasMovedToLinked)
        {
            LinkDestination = linkDestination;
            DestWasMovedToLinked = destWasMovedToLinked;
        }
    }

    public class LinkConnection
    {
        private I3dWorldObject source;
        public I3dWorldObject Dest { get; set; }
        public string Name;
        public string[] PossibleNames;

        public I3dWorldObject Source
        {
            get => source;
            set
            {
                source = value;

                if (source.Links != null)
                    PossibleNames = source.Links.Keys.ToArray();
                else
                    PossibleNames = Array.Empty<string>();
            }
        }

        public LinkConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            Source = source;
            Dest = dest;
            Name = name;
        }
    }

    public delegate void DestinationChangedEventHandler(object sender, DestinationChangedEventArgs e);

    public class LinkEdit3DWScene : SM3DWorldScene
    {
        public event DestinationChangedEventHandler DestinationChanged;

        readonly LinkManager linkManager;

        LinkConnection selectedConnection;

        public LinkConnection SelectedConnection
        {
            get => selectedConnection;
            set
            {
                if (value != selectedConnection)
                {
                    selectedConnection = value;

                    UpdateSelection(0);
                }
            }
        }

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
            if (e.Button != MouseButtons.Left)
            {
                if (selectedConnection?.Source == selectedConnection?.Dest)
                {
                    selectedConnection = null;
                    UpdateSelection(0);
                }

                if (linkDragMode!= LinkDragMode.None)
                {
                    control.CameraTarget = actionStartCamTarget;

                    linkDragMode = LinkDragMode.None;
                }

                return 0;
            }

            if (Hovered3dObject!=null && WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftCtrl))
            {
                SelectedConnection = new LinkConnection(Hovered3dObject, Hovered3dObject, Hovered3dObject.Links?.Keys.First()??"UnnamedConnection");
                linkDragMode = LinkDragMode.Dest;
                actionStartCamTarget = control.CameraTarget;
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
                    actionStartCamTarget = control.CameraTarget;
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
                    actionStartCamTarget = control.CameraTarget;
                    return 0;
                }

                linkDragMode = LinkDragMode.None;
            }

            return 0;
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

        public override void MarginScroll(MarginScrollEventArgs e, GL_ControlBase control)
        {
            if (linkDragMode != LinkDragMode.None)
            {
                control.CameraTarget += new Vector3(e.AmountX * control.FactorX * control.CameraDistance * 0.25f, -e.AmountY * control.FactorY * control.CameraDistance * 0.25f, 0) * control.InvertedRotationMatrix;
            }
        }

        public override uint KeyDown(KeyEventArgs e, GL_ControlBase control, bool isRepeat)
        {
            if (e.KeyCode == Keys.ControlKey && !isRepeat)
            {
                return base.KeyDown(e, control, isRepeat) | REDRAW_PICKING | FORCE_REENTER;
            }
            else
                return base.KeyDown(e, control, isRepeat);
        }

        public override uint KeyUp(KeyEventArgs e, GL_ControlBase control)
        {
            if (e.KeyCode == Keys.ControlKey)
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
                        DestinationChangedEventArgs args = null;

                        if (TryAddConnection(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name))
                        {
                            UpdateLinkDestinations();

                            BeginUndoCollection();
                            AddToUndo(new RevertableConnectionAddition(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name));

                            bool moveDestToLinked = !WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift);

                            if (moveDestToLinked)
                                MoveObjectToLinked(Hovered3dObject);

                            EndUndoCollection();
                            SelectedConnection.Dest = Hovered3dObject;

                            args = new DestinationChangedEventArgs(Hovered3dObject, moveDestToLinked);
                        }
                        else //SelectedConnection.Source doesn't support links
                        {
                            selectedConnection = null;
                        }

                        UpdateSelection(0);

                        if (args != null)
                            DestinationChanged?.Invoke(this, args);
                    }
                }
                else if (linkDragMode == LinkDragMode.Source)
                    ChangeSelectedConnection(Hovered3dObject, SelectedConnection.Dest, SelectedConnection.Name, !WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift));

                else
                    ChangeSelectedConnection(SelectedConnection.Source, Hovered3dObject, SelectedConnection.Name, !WinInput.Keyboard.IsKeyDown(WinInput.Key.LeftShift));
            }

            linkDragMode = LinkDragMode.None;

            return base.MouseUp(e, control) | REDRAW;
        }

        private void MoveObjectToLinked(I3dWorldObject obj)
        {
            if (!EditZone.LinkedObjects.Contains(obj))
            {
                EditZone.LinkedObjects.Add(obj);
                AddToUndo(new RevertableSingleAddition(obj, EditZone.LinkedObjects));

                foreach (var list in EditZone.ObjLists.Values)
                {
                    int index = list.IndexOf(obj);

                    if (index != -1)
                    {
                        list.RemoveAt(index);
                        AddToUndo(new RevertableSingleDeletion(obj, index, list));
                        break;
                    }
                }
            }
        }

        private void MoveObjectToObjList(I3dWorldObject obj, ObjectList list)
        {
            if (!list.Contains(obj))
            {
                list.Add(obj);
                AddToUndo(new RevertableSingleAddition(obj, list));

                int index = EditZone.LinkedObjects.IndexOf(obj);

                if (index != -1)
                {
                    EditZone.LinkedObjects.RemoveAt(index);
                    AddToUndo(new RevertableSingleDeletion(obj, index, EditZone.LinkedObjects));
                }
            }
        }


        private void ChangeSelectedConnection(I3dWorldObject newSource, I3dWorldObject newDest, string newName, bool moveDestToLinked = false)
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

            //don't set SelectedConnection to null
            base.RemoveConnection(
                SelectedConnection.Source, 
                SelectedConnection.Dest, 
                SelectedConnection.Name
                );

            if(TryAddConnection(newSource, newDest, newName))
            {
                if(moveDestToLinked)
                    MoveObjectToLinked(newDest);

                if(SelectedConnection.Dest != newDest)
                    DestinationChanged?.Invoke(this, new DestinationChangedEventArgs(newDest, moveDestToLinked));

                UpdateLinkDestinations();
            }

            SelectedConnection.Source = newSource;
            SelectedConnection.Dest = newDest;
            SelectedConnection.Name = newName;
        }

        public override void RemoveConnection(I3dWorldObject source, I3dWorldObject dest, string name)
        {
            base.RemoveConnection(source, dest, name);

            if (this is LinkEdit3DWScene les)
            {
                if (les.SelectedConnection?.Source == source && les.SelectedConnection?.Dest == dest && les.SelectedConnection?.Name == name)
                    les.SelectedConnection = null;

                control.Refresh();
            }
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
                control.PlainText(scene.SelectedConnection.Source + " to " + scene.SelectedConnection.Dest);

                connectionName = control.DropDownTextInput("Connection Name", connectionName, scene.SelectedConnection.PossibleNames, false);
                if(control.Button("Reverse Connection"))
                {
                    scene.BeginUndoCollection();

                    scene.ChangeSelectedConnection(
                        scene.SelectedConnection.Dest,
                        scene.SelectedConnection.Source,
                        scene.SelectedConnection.Name, 
                        scene.EditZone.LinkedObjects.Contains(scene.SelectedConnection.Dest));

                    if (scene.SelectedConnection.Source.LinkDestinations.Count == 0 && scene.EditZone.LinkedObjects.Contains(scene.SelectedConnection.Source) &&
                        scene.SelectedConnection.Source.TryGetObjectList(scene.EditZone, out ObjectList list))
                    {
                        scene.MoveObjectToObjList(scene.SelectedConnection.Source, list);
                    }

                    scene.EndUndoCollection();

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
                //scene.GL_Control.Refresh();
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

            if(linkDragMode==LinkDragMode.None && WinInput.Keyboard.IsKeyUp(WinInput.Key.LeftCtrl))
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

        class LinkManager : AbstractGlDrawable
        {
            LinkEdit3DWScene scene;

            static ShaderProgram LinksShaderProgram;

            static bool Initialized = false;

            public LinkManager(LinkEdit3DWScene scene)
            {
                this.scene = scene;
                
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
                }"));

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

                        if (_obj == scene.SelectedConnection?.Source)
                            sourceObjPoint = _objPoint;

                        if (_obj.Links != null)
                        {
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
                                            GL.End();
                                            GL.LineWidth(2);
                                            GL.Begin(PrimitiveType.Lines);
                                            GL.VertexAttrib4(1, new Vector4(0.75f, 1, 0.75f, 1));
                                            GL.Vertex3(_objPoint);
                                            GL.VertexAttrib4(1, new Vector4(1, 0.75f, 0.75f, 1));
                                            GL.Vertex3(objPoint);
                                            GL.End();
                                            GL.LineWidth(1);
                                            GL.Begin(PrimitiveType.Lines);
                                        }
                                        else if(!_obj.IsSelectedAll() && !obj.IsSelectedAll()) //other wise the connection is drawn by the LinkRenderer
                                        {
                                            GL.VertexAttrib4(1, new Vector4(0.75f, 0.75f, 0.75f, 1));
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

                        bool isInvalidConnection = (scene.linkDragMode != LinkDragMode.None) && (scene.Hovered3dObject == null || 
                            (scene.linkDragMode == LinkDragMode.Source && scene.Hovered3dObject == scene.SelectedConnection.Dest) || 
                            (scene.linkDragMode == LinkDragMode.Dest && scene.Hovered3dObject == scene.SelectedConnection.Source));

                        GL.DepthFunc(DepthFunction.Always);

                        GL.LineWidth(6);
                        GL.Begin(PrimitiveType.Lines);
                        GL.VertexAttrib4(1, new Vector4(0, 0, 0, 1));
                        GL.Vertex3(sourcePoint);
                        GL.Vertex3(destPoint);
                        GL.End();

                        GL.LineWidth(3);
                        GL.Begin(PrimitiveType.Lines);
                        GL.VertexAttrib4(1, isInvalidConnection ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(0, 1, 0, 1));
                        GL.Vertex3(sourcePoint);
                        GL.VertexAttrib4(1, isInvalidConnection ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(1, 0, 0, 1));
                        GL.Vertex3(destPoint);
                        GL.End();

                        control.UpdateModelMatrix(new Matrix4(control.InvertedRotationMatrix) * Matrix4.CreateTranslation(sourcePoint));
                        GL.Begin(PrimitiveType.Quads);
                        GL.VertexAttrib4(1, isInvalidConnection ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(0, 1, 0, 1));

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
                        GL.VertexAttrib4(1, isInvalidConnection ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(1, 0, 0, 1));

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
        }
    }
}
