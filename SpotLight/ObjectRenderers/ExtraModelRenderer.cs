using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
using System;

namespace Spotlight.ObjectRenderers
{
    public static class ExtraModelRenderer
    {
        private static bool initialized = false;

        private static readonly Dictionary<string, ExtraModel> models = new Dictionary<string, ExtraModel>();

        public static ShaderProgram ExtraModelShaderProgram;

        public static void Initialize()
        {
            if (initialized)
                return;

            #region Shader Generation
            ExtraModelShaderProgram = new ShaderProgram(
                    new FragmentShader(
                        @"#version 330
                    uniform sampler2D tex;
                    uniform vec4 highlight_color;
                    in vec4 fragColor;
                    void main(){
                        float hc_a   = highlight_color.w;
                        vec4 color = fragColor;
                        gl_FragColor = vec4(color.rgb * (1-hc_a) + highlight_color.rgb * hc_a, color.a);
                    }"),
                    new VertexShader(
                        @"#version 330
                    layout(location = 0) in vec4 position;
                    layout(location = 1) in vec4 color;
                    uniform mat4 mtxMdl;
                    uniform mat4 mtxCam;
                    out vec4 fragColor;

                    void main(){
                        fragColor = color;
                        gl_Position = mtxCam*mtxMdl*position;
                    }"));
            #endregion


        #region Create Extra Models

            #region AreaCubeBase
            List<int> indices = new List<int>();

            float r = 5;
            float t = 10;
            float b = 0;

            float[] data = new float[]
            {
                -r, t, -r,  0, 0.5f, 1, 1,
                 r, t, -r,  0, 0.5f, 1, 1,
                -r, t,  r,  0, 0.5f, 1, 1,
                 r, t,  r,  0, 0.5f, 1, 1,

                -r, b, -r,  0, 0, 1, 1,
                 r, b, -r,  0, 0, 1, 1,
                -r, b,  r,  0, 0, 1, 1,
                 r, b,  r,  0, 0, 1, 1,

                 0, b,  0,  0, 0, 0.5f, 1,
            };

            //-x to x
            indices.Add(0b000);
            indices.Add(0b001);
            indices.Add(0b010);
            indices.Add(0b011);
            indices.Add(0b100);
            indices.Add(0b101);
            indices.Add(0b110);
            indices.Add(0b111);

            //-z to z
            indices.Add(0b000);
            indices.Add(0b010);
            indices.Add(0b001);
            indices.Add(0b011);
            indices.Add(0b100);
            indices.Add(0b110);
            indices.Add(0b101);
            indices.Add(0b111);

            //-y to y
            indices.Add(0b000);
            indices.Add(0b100);
            indices.Add(0b001);
            indices.Add(0b101);
            indices.Add(0b010);
            indices.Add(0b110);
            indices.Add(0b011);
            indices.Add(0b111);

            //floor
            indices.Add(8);
            indices.Add(0b100);
            indices.Add(8);
            indices.Add(0b101);
            indices.Add(8);
            indices.Add(0b110);
            indices.Add(8);
            indices.Add(0b111);

            Submit(PrimitiveType.Lines, "AreaCubeBase", indices, data);
            #endregion

            #region AreaCubeCenter
            indices = new List<int>();


            t = 5f;
            b = -5f;

            data = new float[]
            {
                -r, t, -r,  0, 0.5f, 1, 1,
                 r, t, -r,  0, 0.5f, 1, 1,
                -r, t,  r,  0, 0.5f, 1, 1,
                 r, t,  r,  0, 0.5f, 1, 1,

                -r, b, -r,  0, 0, 1, 1,
                 r, b, -r,  0, 0, 1, 1,
                -r, b,  r,  0, 0, 1, 1,
                 r, b,  r,  0, 0, 1, 1,
            };

            //-x to x
            indices.Add(0b000);
            indices.Add(0b001);
            indices.Add(0b010);
            indices.Add(0b011);
            indices.Add(0b100);
            indices.Add(0b101);
            indices.Add(0b110);
            indices.Add(0b111);

            //-z to z
            indices.Add(0b000);
            indices.Add(0b010);
            indices.Add(0b001);
            indices.Add(0b011);
            indices.Add(0b100);
            indices.Add(0b110);
            indices.Add(0b101);
            indices.Add(0b111);

            //-y to y
            indices.Add(0b000);
            indices.Add(0b100);
            indices.Add(0b001);
            indices.Add(0b101);
            indices.Add(0b010);
            indices.Add(0b110);
            indices.Add(0b011);
            indices.Add(0b111);

            Submit(PrimitiveType.Lines, "AreaCubeCenter", indices, data);
            #endregion

#if ODYSSEY
            #region AreaCubeTop

            t = -10;
            b = 0;

            data = new float[]
            {
                -r, t, -r,  0, 0.5f, 1, 1,
                 r, t, -r,  0, 0.5f, 1, 1,
                -r, t,  r,  0, 0.5f, 1, 1,
                 r, t,  r,  0, 0.5f, 1, 1,

                -r, b, -r,  0, 0, 1, 1,
                 r, b, -r,  0, 0, 1, 1,
                -r, b,  r,  0, 0, 1, 1,
                 r, b,  r,  0, 0, 1, 1,

                 0, b,  0,  0, 0, 0.5f, 1,
            };

            Submit(PrimitiveType.Lines, "AreaCubeTop", indices, data);
            #endregion

#endif

            #region AreaCylinder
            indices = new List<int>();

            r = 5;
            t = 5;
            b = 0;
            int cylinderEdgeCount = 16;

            data = new float[(cylinderEdgeCount * 2 + 1) * 7];

            int i = 0;

            for (int edgeIndex = 0; edgeIndex < cylinderEdgeCount; edgeIndex++)
            {
                float x = (float)Math.Sin(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;
                float z = (float)Math.Cos(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;

                //top
                data[i++] = x;
                data[i++] = t;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0.5f;
                data[i++] = 1;
                data[i++] = 1;

                //bottom
                data[i++] = x;
                data[i++] = b;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0f;
                data[i++] = 1;
                data[i++] = 1;

                //top
                indices.Add(2 * edgeIndex);
                indices.Add((2 * edgeIndex + 2) % (cylinderEdgeCount * 2));

                //bottom
                indices.Add(2 * edgeIndex + 1);
                indices.Add((2 * edgeIndex + 1 + 2) % (cylinderEdgeCount * 2));

                //top to bottom
                indices.Add(2 * edgeIndex);
                indices.Add(2 * edgeIndex + 1);

                indices.Add(2 * edgeIndex + 1);
                indices.Add(cylinderEdgeCount * 2);
            }

            //floor
            data[i++] = 0;
            data[i++] = b;
            data[i++] = 0;

            data[i++] = 0;
            data[i++] = 0f;
            data[i++] = 0.5f;
            data[i++] = 1;

            Submit(PrimitiveType.Lines, "AreaCylinder", indices, data);
            #endregion

#if ODYSSEY
            #region AreaCylinderTop
            r = 5;
            t = -5;
            b = 0;

            data = new float[(cylinderEdgeCount * 2 + 1) * 7];

            i = 0;

            for (int edgeIndex = 0; edgeIndex < cylinderEdgeCount; edgeIndex++)
            {
                float x = (float)Math.Sin(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;
                float z = (float)Math.Cos(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;

                //top
                data[i++] = x;
                data[i++] = t;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0.5f;
                data[i++] = 1;
                data[i++] = 1;

                //bottom
                data[i++] = x;
                data[i++] = b;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0f;
                data[i++] = 1;
                data[i++] = 1;
            }

            //floor
            data[i++] = 0;
            data[i++] = b;
            data[i++] = 0;

            data[i++] = 0;
            data[i++] = 0f;
            data[i++] = 0.5f;
            data[i++] = 1;

            Submit(PrimitiveType.Lines, "AreaCylinderTop", indices, data);
            #endregion

            #region AreaCylinderCenter
            indices = new List<int>();

            r = 5;
            t = 5;
            b = 0;

            data = new float[(cylinderEdgeCount * 2 + 1) * 7];

            i = 0;

            for (int edgeIndex = 0; edgeIndex < cylinderEdgeCount; edgeIndex++)
            {
                float x = (float)Math.Sin(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;
                float z = (float)Math.Cos(Math.PI * 2 * edgeIndex / cylinderEdgeCount) * r;

                //top
                data[i++] = x;
                data[i++] = t;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0.5f;
                data[i++] = 1;
                data[i++] = 1;

                //bottom
                data[i++] = x;
                data[i++] = b;
                data[i++] = z;

                data[i++] = 0;
                data[i++] = 0f;
                data[i++] = 1;
                data[i++] = 1;

                //top
                indices.Add(2 * edgeIndex);
                indices.Add((2 * edgeIndex + 2) % (cylinderEdgeCount * 2));

                //bottom
                indices.Add(2 * edgeIndex + 1);
                indices.Add((2 * edgeIndex + 1 + 2) % (cylinderEdgeCount * 2));

                //top to bottom
                indices.Add(2 * edgeIndex);
                indices.Add(2 * edgeIndex + 1);

                indices.Add(2 * edgeIndex + 1);
                indices.Add(cylinderEdgeCount * 2);
            }

            Submit(PrimitiveType.Lines, "AreaCylinderCenter", indices, data);
            #endregion
            
#endif

            #region AreaSphere
            indices = new List<int>();

            int sphereSubDivisionsU = 16;
            int sphereSubDivisionsV = 8;

            r = 5f;

            data = new float[(sphereSubDivisionsU * (sphereSubDivisionsV - 1) + 2) * 7];

            int lastIndex = sphereSubDivisionsU * (sphereSubDivisionsV - 1) + 1;

            i = 0;

            //top
            data[i++] = 0;
            data[i++] = r;
            data[i++] = 0;

            data[i++] = 0;
            data[i++] = 0.5f;
            data[i++] = 1;
            data[i++] = 1;

            void addIndex(int u, int v) => indices.Add(1 + v + (sphereSubDivisionsV-1) * u);

            for (int uIndex = 0; uIndex < sphereSubDivisionsU; uIndex++)
            {
                double angleU = Math.PI * 2 * uIndex / sphereSubDivisionsU;

                float _x = (float)Math.Sin(angleU) * r;
                float _z = (float)Math.Cos(angleU) * r;

                indices.Add(0);
                addIndex(uIndex, 0);

                for (int vIndex = 0; vIndex < sphereSubDivisionsV-1; vIndex++)
                {
                    double angleV = Math.PI * (vIndex + 1) / sphereSubDivisionsV;

                    float vRadius = (float)Math.Sin(angleV);

                    float x = _x * vRadius;
                    float z = _z * vRadius;

                    float y = (float)Math.Cos(angleV) * r;

                    data[i++] = x;
                    data[i++] = y;
                    data[i++] = z;

                    data[i++] = 0;
                    data[i++] = 0.5f - (float)(angleV / Math.PI) * 0.5f;
                    data[i++] = 1;
                    data[i++] = 1;


                    //u+
                    addIndex(uIndex, vIndex);
                    addIndex((uIndex + 1) % sphereSubDivisionsU, vIndex);

                    
                    //v+
                    addIndex(uIndex, vIndex);
                    if (vIndex < sphereSubDivisionsV - 2)
                        addIndex(uIndex, (vIndex + 1));
                    else
                        indices.Add(lastIndex);
                }
            }

            //bottom
            data[i++] = 0;
            data[i++] = -r;
            data[i++] = 0;

            data[i++] = 0;
            data[i++] = 0f;
            data[i++] = 1f;
            data[i++] = 1;

            Submit(PrimitiveType.Lines, "AreaSphere", indices, data);
            #endregion

            #region TransparentWall
            indices = new List<int>();

            r = 5;

            data = new float[]
            {
                -r,  r, 0,  0, 0, 0.75f, 0.5f,
                 r,  r, 0,  0, 0, 0.75f, 0.5f,
                -r, -r, 0,  0, 0, 0.75f, 0.5f,
                 r, -r, 0,  0, 0, 0.75f, 0.5f,

                -r,  r, 0,  0.75f, 0, 0.125f, 0.5f,
                 r,  r, 0,  0.75f, 0, 0.125f, 0.5f,
                -r, -r, 0,  0.75f, 0, 0.125f, 0.5f,
                 r, -r, 0,  0.75f, 0, 0.125f, 0.5f,
            };
            //front
            indices.Add(0b00);
            indices.Add(0b10);
            indices.Add(0b01);

            indices.Add(0b01);
            indices.Add(0b10);
            indices.Add(0b11);

            //back
            indices.Add(0b101);
            indices.Add(0b110);
            indices.Add(0b100);

            indices.Add(0b111);
            indices.Add(0b110);
            indices.Add(0b101);

            Submit(PrimitiveType.Triangles, "TransparentWall", indices, data, Pass.TRANSPARENT);
#if ODYSSEY
            Submit(PrimitiveType.Triangles, "TransparentWallNoAction", indices, data, Pass.TRANSPARENT);
            Submit(PrimitiveType.Triangles, "TransparentWallMoveLimit", indices, data, Pass.TRANSPARENT);
#endif
            #endregion

            #endregion

            initialized = true;
        }

        private static void Submit(PrimitiveType primitiveType, string modelName, List<int> indices, float[] data, Pass pass = Pass.OPAQUE)
        {
            int[] buffers = new int[2];
            GL.GenBuffers(2, buffers);

            int indexBuffer = buffers[0];
            int vaoBuffer = buffers[1];

            VertexArrayObject vao = new VertexArrayObject(vaoBuffer, indexBuffer);
            vao.AddAttribute(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);
            vao.AddAttribute(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 3);
            vao.Submit();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vaoBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

            models.Add(modelName, new ExtraModel(vao, indices.Count, primitiveType, pass));
        }

        public static bool TryDraw(string modelName, GL_ControlModern control, Pass pass, Vector4 highlightColor)
        {
            if (models.TryGetValue(modelName, out ExtraModel extraModel))
            {
                extraModel.Draw(control, pass, highlightColor);
                return true;
            }
            else
                return false;
        }

        public static void TryGetModel(string modelName, out ExtraModel cachedModel)
        {
            models.TryGetValue(modelName, out cachedModel);
        }

        public sealed class ExtraModel
        {
            public VertexArrayObject Vao { get; set; }
            public int IndexCount { get; set; }
            public PrimitiveType PrimitiveType { get; set; }
            public Pass Pass { get; set; }

            public ExtraModel(VertexArrayObject vao, int indexCount, PrimitiveType primitiveType, Pass pass)
            {
                Vao = vao;
                IndexCount = indexCount;
                PrimitiveType = primitiveType;
                Pass = pass;
            }

            public void Draw(GL_ControlModern control, Pass pass, Vector4 highlightColor)
            {
                if (pass == Pass.PICKING)
                {
                    control.CurrentShader = Framework.SolidColorShaderProgram;

                    GL.LineWidth(5);
                    control.CurrentShader.SetVector4("color", control.NextPickingColor());
                }
                else if (pass == Pass)
                {
                    control.CurrentShader = ExtraModelShaderProgram;

                    if (pass == Pass.TRANSPARENT)
                        GL.Enable(EnableCap.Blend);

                    GL.LineWidth(3);
                    control.CurrentShader.SetVector4("highlight_color", highlightColor);
                }
                else
                    return;

                Vao.Use(control);

                GL.DrawElements(PrimitiveType, IndexCount, DrawElementsType.UnsignedInt, 0);

                GL.LineWidth(2);

                GL.Disable(EnableCap.Blend);
            }
        }
    }
}
