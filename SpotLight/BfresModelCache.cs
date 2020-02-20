using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFormats3DW;
using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.Bfres.Helpers;
using SZS;
using OpenTK;

namespace SpotLight
{
    public static class BfresModelCache
    {
        private static bool initialized = false;

        public static ShaderProgram BfresShaderProgram;
        private static ShaderProgram ExtraModelShaderProgram;
        static Dictionary<string, CachedModel> cache = new Dictionary<string, CachedModel>();

        struct ExtraModel
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
        }

        static Dictionary<string, ExtraModel> extraModels = new Dictionary<string, ExtraModel>();

        static Dictionary<string, Dictionary<string, int>> texArcCache = new Dictionary<string, Dictionary<string, int>>();

        public static int DefaultTetxure;

        public static int NoTetxure;

        public static void Initialize(GL_ControlModern control)
        {
            if (initialized)
                return;

            #region Shader Generation
            BfresShaderProgram = new ShaderProgram(
                    new FragmentShader(
                        @"#version 330
                    uniform sampler2D tex;
                    uniform vec4 highlight_color;
                    in vec2 fragUV;
                    in vec4 fragColor;
                    void main(){
                        float hc_a   = highlight_color.w;
                        vec4 color = fragColor * texture(tex,fragUV);
                        gl_FragColor = vec4(color.rgb * (1-hc_a) + highlight_color.rgb * hc_a, color.a);
                        //gl_FragColor = vec4(color.a, color.a, color.a, 1);
                    }"),
                    new VertexShader(
                        @"#version 330
                    layout(location = 0) in vec4 position;
                    layout(location = 1) in vec2 uv;
                    layout(location = 2) in vec4 color;
                    uniform mat4 mtxMdl;
                    uniform mat4 mtxCam;
                    out vec2 fragUV;
                    out vec4 fragColor;

                    void main(){
                        fragUV = uv;
                        fragColor = color;
                        gl_Position = mtxCam*mtxMdl*position;
                    }"), control);

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
                    }"), control);
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

            //-y to y
            indices.Add(0b000);
            indices.Add(0b010);
            indices.Add(0b001);
            indices.Add(0b011);
            indices.Add(0b100);
            indices.Add(0b110);
            indices.Add(0b101);
            indices.Add(0b111);

            //-z to z
            indices.Add(0b000);
            indices.Add(0b100);
            indices.Add(0b001);
            indices.Add(0b101);
            indices.Add(0b010);
            indices.Add(0b110);
            indices.Add(0b011);
            indices.Add(0b111);

            //new Vector4(0, 0.5f, 1, 1)

            SubmitExtraModel(control, PrimitiveType.Lines, "AreaCubeBase", indices, data);
            #endregion

            #region AreaCylinder
            indices = new List<int>();

            r = 5;
            t = 5;
            b = 0;
            int v = 16;

            data = new float[v * 2 * 7];

            int i = 0;

            for (int edgeIndex = 0; edgeIndex < v; edgeIndex++)
            {
                float x = (float)Math.Sin(Math.PI * 2 * edgeIndex / v) * r;
                float z = (float)Math.Cos(Math.PI * 2 * edgeIndex / v) * r;

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
                indices.Add((2 * edgeIndex + 2) % (v * 2));

                //bottom
                indices.Add(2 * edgeIndex + 1);
                indices.Add((2 * edgeIndex + 1 + 2) % (v * 2));

                //top to bottom
                indices.Add(2 * edgeIndex);
                indices.Add(2 * edgeIndex + 1);
            }

            SubmitExtraModel(control, PrimitiveType.Lines, "AreaCylinder", indices, data);
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

            SubmitExtraModel(control, PrimitiveType.Triangles, "TransparentWall", indices, data, Pass.TRANSPARENT);
            #endregion

            #endregion


            DefaultTetxure = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DefaultTetxure);

            var bmp = Properties.Resources.DefaultTexture;
            var bmpData = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, 32, 32),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 32, 32, 0, PixelFormat.Bgr, PixelType.UnsignedByte, bmpData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            NoTetxure = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, NoTetxure);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 1, 1, 0, PixelFormat.Bgr, PixelType.UnsignedByte, new uint[] { 0xFFFFFFFF });
            bmp.UnlockBits(bmpData);

            initialized = true;
        }

        private static void SubmitExtraModel(GL_ControlModern control, PrimitiveType primitiveType, string modelName, List<int> indices, float[] data, Pass pass = Pass.OPAQUE)
        {
            int[] buffers = new int[2];
            GL.GenBuffers(2, buffers);

            int indexBuffer = buffers[0];
            int vaoBuffer = buffers[1];

            VertexArrayObject vao = new VertexArrayObject(vaoBuffer, indexBuffer);
            vao.AddAttribute(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);
            vao.AddAttribute(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 3);
            vao.Initialize(control);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vaoBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

            extraModels.Add(Path.GetFileNameWithoutExtension(modelName), new ExtraModel(vao, indices.Count, primitiveType, pass));
        }

        public static void Submit(string modelName, Stream stream, GL_ControlModern control, string textureArc = null)
        {
            ResFile bfres = new ResFile(stream);

            if (!cache.ContainsKey(modelName) && bfres.Models.Count>0)
                cache[modelName] = new CachedModel(bfres, textureArc, control);
        }

        public static bool TryDraw(string modelName, GL_ControlModern control, Pass pass, Vector4 highlightColor)
        {
            if (cache.ContainsKey(modelName))
            {
                cache[modelName].Draw(control, pass, highlightColor);
                return true;
            }
            else if (extraModels.TryGetValue(modelName, out ExtraModel extraModel) && (pass==Pass.PICKING || pass==extraModel.Pass))
            {
                if (pass == Pass.PICKING)
                {
                    control.CurrentShader = Renderers.ColorBlockRenderer.SolidColorShaderProgram;

                    GL.LineWidth(5);
                    control.CurrentShader.SetVector4("color", control.NextPickingColor());
                }
                else
                {
                    control.CurrentShader = ExtraModelShaderProgram;

                    if (pass == Pass.TRANSPARENT)
                        GL.Enable(EnableCap.Blend);

                    GL.LineWidth(3);
                    control.CurrentShader.SetVector4("highlight_color", highlightColor);
                }

                extraModel.Vao.Use(control);
                
                GL.DrawElements(extraModel.PrimitiveType, extraModel.IndexCount, DrawElementsType.UnsignedInt, 0);

                GL.LineWidth(2);

                GL.Disable(EnableCap.Blend);

                return true;
            }
            else
                return false;
        }



        public static bool Contains(string modelName) => cache.ContainsKey(modelName) || extraModels.ContainsKey(modelName);

        public const bool LoadTextures = true;

        public struct CachedModel
        {
            static readonly float white = BitConverter.ToSingle(new byte[] {255, 255, 255, 255},0);

            VertexArrayObject[] vaos;
            readonly int[] indexBufferLengths;
            readonly int[] textures;
            readonly (int,int)[] wrapModes;
            readonly Pass[] passes;

            public CachedModel(ResFile bfres, string textureArc, GL_ControlModern control)
            {
                if (LoadTextures && textureArc != null && File.Exists(Program.ObjectDataPath + textureArc + ".szs"))
                {
                    SarcData objArc = SARC.UnpackRamN(YAZ0.Decompress(Program.ObjectDataPath + textureArc + ".szs"));

                    if (!texArcCache.ContainsKey(textureArc))
                    {
                        Dictionary<string, int> arc = new Dictionary<string, int>();
                        texArcCache.Add(textureArc, arc);
                        foreach(KeyValuePair<string,Texture> keyValuePair in new ResFile(new MemoryStream(objArc.Files[textureArc + ".bfres"])).Textures)
                        {
                            arc.Add(keyValuePair.Key, UploadTexture(keyValuePair.Value));
                        }
                    }
                }

                Model mdl = bfres.Models[0];

                vaos = new VertexArrayObject[mdl.Shapes.Count];
                indexBufferLengths = new int[mdl.Shapes.Count];
                textures           = new int[mdl.Shapes.Count];
                wrapModes =    new (int,int)[mdl.Shapes.Count];
                passes = new Pass[mdl.Shapes.Count];
                
                int shapeIndex = 0;

                foreach(Shape shape in mdl.Shapes.Values)
                {
                    uint[] indices = shape.Meshes[0].GetIndices().ToArray();

                    if (LoadTextures)
                    {
                        if (mdl.Materials[shape.MaterialIndex].TextureRefs.Count != 0)
                        {
                            int Target = 0;
                            for (int i = 0; i < mdl.Materials[shape.MaterialIndex].TextureRefs.Count; i++)
                            {
                                if (mdl.Materials[shape.MaterialIndex].TextureRefs[i].Name.Contains("_alb"))
                                {
                                    Target = i;
                                    break;
                                }
                            }
                            TextureRef texRef = mdl.Materials[shape.MaterialIndex].TextureRefs[Target];

                            TexSampler sampler = mdl.Materials[shape.MaterialIndex].Samplers[Target].TexSampler;

                            if (texRef.Texture != null)
                            {
                                textures[shapeIndex] = UploadTexture(texRef.Texture);




                            }
                            else if (textureArc != null)
                            {
                                if (texArcCache.ContainsKey(textureArc) && texArcCache[textureArc].ContainsKey(texRef.Name))
                                {
                                    textures[shapeIndex] = texArcCache[textureArc][texRef.Name];
                                }
                                else
                                {
                                    textures[shapeIndex] = -2;
                                }
                            }

                            wrapModes[shapeIndex] = ((int)GetWrapMode(sampler.ClampX),
                                    (int)GetWrapMode(sampler.ClampY));
                        }
                        else
                        {
                            textures[shapeIndex] = -1;
                        }
                    }
                    //else
                    //    textures[shapeIndex] = -2;
                    //Apparently this is unreachable. I (Super Hackio) don't exactly know why...

                    switch (mdl.Materials[shape.MaterialIndex].RenderState.FlagsMode)
                    {
                        case RenderStateFlagsMode.AlphaMask:
                        case RenderStateFlagsMode.Translucent:
                        case RenderStateFlagsMode.Custom:
                            passes[shapeIndex] = Pass.TRANSPARENT;
                            break;
                        default:
                            passes[shapeIndex] = Pass.OPAQUE;
                            break;
                    }

                    Matrix4[] transforms = GetTransforms(mdl.Skeleton.Bones.Values.ToArray());

                    //Create a buffer instance which stores all the buffer data
                    VertexBufferHelper helper = new VertexBufferHelper(mdl.VertexBuffers[shapeIndex], ByteConverter.Big);

                    //Set each array first from the lib if exist. Then add the data all in one loop
                    Syroot.Maths.Vector4F[] vec4Positions = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4Normals = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv1 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv2 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4c0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4t0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4b0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4w0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4i0 = new Syroot.Maths.Vector4F[0];

                    //For shape morphing
                    Syroot.Maths.Vector4F[] vec4Positions1 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4Positions2 = new Syroot.Maths.Vector4F[0];

                    foreach (VertexAttrib att in mdl.VertexBuffers[shapeIndex].Attributes.Values)
                    {
                        if (att.Name == "_p0")
                            vec4Positions = helper["_p0"].Data;
                        if (att.Name == "_n0")
                            vec4Normals = helper["_n0"].Data;
                        if (att.Name == "_u0")
                            vec4uv0 = helper["_u0"].Data;
                        if (att.Name == "_u1")
                            vec4uv1 = helper["_u1"].Data;
                        if (att.Name == "_u2")
                            vec4uv2 = helper["_u2"].Data;
                        if (att.Name == "_c0")
                            vec4c0 = helper["_c0"].Data;
                        if (att.Name == "_t0")
                            vec4t0 = helper["_t0"].Data;
                        if (att.Name == "_b0")
                            vec4b0 = helper["_b0"].Data;
                        if (att.Name == "_w0")
                            vec4w0 = helper["_w0"].Data;
                        if (att.Name == "_i0")
                            vec4i0 = helper["_i0"].Data;

                        if (att.Name == "_p1")
                            vec4Positions1 = helper["_p1"].Data;
                        if (att.Name == "_p2")
                            vec4Positions2 = helper["_p2"].Data;
                    }

                    indexBufferLengths[shapeIndex] = indices.Length;

                    float[] bufferData = new float[6 * vec4Positions.Length];

                    int _i = 0;
                    for (int i = 0; i < vec4Positions.Length; i++)
                    {
                        Vector3 pos = Vector3.Zero;
                        Vector3 pos1 = Vector3.Zero;
                        Vector3 pos2 = Vector3.Zero;
                        Vector3 nrm = Vector3.Zero;
                        Vector2 uv0 = Vector2.Zero;
                        Vector2 uv1 = Vector2.Zero;
                        Vector2 uv2 = Vector2.Zero;
                        List<float> boneWeights = new List<float>();
                        List<int> boneIds = new List<int>();
                        Vector4 tan = Vector4.Zero;
                        Vector4 bitan = Vector4.Zero;
                        Vector4 col = Vector4.One;

                        if (vec4Positions.Length > 0)
                            pos = new Vector3(vec4Positions[i].X, vec4Positions[i].Y, vec4Positions[i].Z);
                        if (vec4Positions1.Length > 0)
                            pos1 = new Vector3(vec4Positions1[i].X, vec4Positions1[i].Y, vec4Positions1[i].Z);
                        if (vec4Positions2.Length > 0)
                            pos2 = new Vector3(vec4Positions2[i].X, vec4Positions2[i].Y, vec4Positions2[i].Z);
                        if (vec4Normals.Length > 0)
                            nrm = new Vector3(vec4Normals[i].X, vec4Normals[i].Y, vec4Normals[i].Z);
                        if (vec4uv0.Length > 0)
                            uv0 = new Vector2(vec4uv0[i].X, vec4uv0[i].Y);
                        if (vec4uv1.Length > 0)
                            uv1 = new Vector2(vec4uv1[i].X, vec4uv1[i].Y);
                        if (vec4uv2.Length > 0)
                            uv2 = new Vector2(vec4uv2[i].X, vec4uv2[i].Y);
                        if (vec4w0.Length > 0)
                        {
                            boneWeights.Add(vec4w0[i].X);
                            boneWeights.Add(vec4w0[i].Y);
                            boneWeights.Add(vec4w0[i].Z);
                            boneWeights.Add(vec4w0[i].W);
                        }
                        if (vec4i0.Length > 0)
                        {
                            boneIds.Add((int)vec4i0[i].X);
                            boneIds.Add((int)vec4i0[i].Y);
                            boneIds.Add((int)vec4i0[i].Z);
                            boneIds.Add((int)vec4i0[i].W);

                        }

                        if (vec4t0.Length > 0)
                            tan = new Vector4(vec4t0[i].X, vec4t0[i].Y, vec4t0[i].Z, vec4t0[i].W);
                        if (vec4b0.Length > 0)
                            bitan = new Vector4(vec4b0[i].X, vec4b0[i].Y, vec4b0[i].Z, vec4b0[i].W);
                        if (vec4c0.Length > 0)
                            col = new Vector4(vec4c0[i].X, vec4c0[i].Y, vec4c0[i].Z, vec4c0[i].W);

                        if (shape.VertexSkinCount == 1)
                        {
                            int boneIndex = shape.BoneIndex;
                            if (boneIds.Count > 0)
                                boneIndex = mdl.Skeleton.MatrixToBoneList[boneIds[0]];

                            //Check if the bones are a rigid type
                            //In game it seems to not transform if they are not rigid
                            if (mdl.Skeleton.Bones[boneIndex].RigidMatrixIndex != -1)
                            {
                                Matrix4 sb = transforms[boneIndex];
                                pos = Vector3.TransformPosition(pos, sb);
                                nrm = Vector3.TransformNormal(nrm, sb);
                            }
                        }

                        if (shape.VertexSkinCount == 0)
                        {
                            int boneIndex = shape.BoneIndex;

                            Matrix4 NoBindFix = transforms[boneIndex];
                            pos = Vector3.TransformPosition(pos, NoBindFix);
                            nrm = Vector3.TransformNormal(nrm, NoBindFix);
                        }
                        bufferData[_i] = pos.X * 0.01f;
                        bufferData[_i + 1] = pos.Y * 0.01f;
                        bufferData[_i + 2] = pos.Z * 0.01f;
                        if (vec4uv0.Length>0)
                        {
                            bufferData[_i + 3] = uv0.X;
                            bufferData[_i + 4] = uv0.Y;
                        }
                        if (vec4c0.Length > 0)
                        {
                            bufferData[_i + 5] = BitConverter.ToSingle(new byte[]{
                            (byte)(col.X * 255),
                            (byte)(col.Y * 255),
                            (byte)(col.Z * 255),
                            (byte)(col.W * 255)}, 0);
                        }
                        else
                            bufferData[_i + 5] = white;
                        _i += 6;
                    }
                    int[] buffers = new int[2];
                    GL.GenBuffers(2, buffers);

                    int indexBuffer = buffers[0];
                    int vaoBuffer = buffers[1];

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, vaoBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * vec4Positions.Length, bufferData, BufferUsageHint.StaticDraw);

                    vaos[shapeIndex] = new VertexArrayObject(vaoBuffer, indexBuffer);
                    vaos[shapeIndex].AddAttribute(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
                    vaos[shapeIndex].AddAttribute(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 6, sizeof(float) * 3);
                    vaos[shapeIndex].AddAttribute(2, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(float) * 6, sizeof(float) * 5);

                    vaos[shapeIndex].Initialize(control);

                    shapeIndex++;
                }
            }

            static Matrix4[] GetTransforms(Bone[] bones)
            {
                Matrix4[] ret = new Matrix4[bones.Count()];
                
                for(int i = 0; i<bones.Length; i++)
                {
                    Bone bone = bones[i];
                    ret[i] = Matrix4.CreateScale(new Vector3(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
                    
                    while(true)
                    {
                        ret[i] = ret[i] * 
                              Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z)) *
                              Matrix4.CreateTranslation(new Vector3(bone.Position.X, bone.Position.Y, bone.Position.Z));

                        if (bone.ParentIndex == 0xFFFF)
                            break;
                        bone = bones[bone.ParentIndex];
                    }
                }

                return ret;
            }

            public void Draw(GL_ControlModern control, Pass pass, Vector4 highlightColor)
            {
                if (pass == Pass.PICKING)
                {
                    control.CurrentShader = Renderers.ColorBlockRenderer.SolidColorShaderProgram;
                    control.CurrentShader.SetVector4("color", control.NextPickingColor());
                }
                else
                {
                    if(pass == Pass.TRANSPARENT)
                    {
                        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                        GL.Enable(EnableCap.Blend);
                    }

                    control.CurrentShader = BfresShaderProgram;

                    control.CurrentShader.SetVector4("highlight_color", highlightColor);

                    GL.ActiveTexture(TextureUnit.Texture0);
                }

                if (pass == Pass.OPAQUE && highlightColor.W != 0)
                {
                    GL.Enable(EnableCap.StencilTest);
                    GL.Clear(ClearBufferMask.StencilBufferBit);
                    GL.ClearStencil(0);
                    GL.StencilFunc(StencilFunction.Always, 0x1, 0x1);
                    GL.StencilOp(StencilOp.Keep, StencilOp.Replace, StencilOp.Replace);
                }

                for (int i = 0; i<vaos.Length; i++)
                {
                    if (pass == passes[i])
                    {
                        if (textures[i] == -1)
                            GL.BindTexture(TextureTarget.Texture2D, NoTetxure);
                        else if (textures[i] == -2)
                            GL.BindTexture(TextureTarget.Texture2D, DefaultTetxure);
                        else
                            GL.BindTexture(TextureTarget.Texture2D, textures[i]);

                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapModes[i].Item1);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapModes[i].Item2);
                    }

                    if (pass == passes[i] || pass == Pass.PICKING)
                    {
                        vaos[i].Use(control);

                        GL.DrawElements(BeginMode.Triangles, indexBufferLengths[i], DrawElementsType.UnsignedInt, 0);
                    }
                    else if (pass == Pass.OPAQUE && highlightColor.W != 0)
                    {
                        GL.ColorMask(false, false, false, false);
                        GL.DepthMask(false);

                        vaos[i].Use(control);

                        GL.DrawElements(BeginMode.Triangles, indexBufferLengths[i], DrawElementsType.UnsignedInt, 0);

                        GL.ColorMask(true, true, true, true);
                        GL.DepthMask(true);
                    }
                }

                GL.Disable(EnableCap.Blend);

                if (pass == Pass.OPAQUE && highlightColor.W != 0)
                {
                    control.CurrentShader = Renderers.ColorBlockRenderer.SolidColorShaderProgram;
                    control.CurrentShader.SetVector4("color", new Vector4(highlightColor.Xyz, 1));

                    GL.LineWidth(3.0f);
                    GL.StencilFunc(StencilFunction.Equal, 0x0, 0x1);
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

                    //GL.DepthFunc(DepthFunction.Always);

                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

                    
                    for (int i = 0; i < vaos.Length; i++)
                    {
                        vaos[i].Use(control);
                        GL.DrawElements(BeginMode.Triangles, indexBufferLengths[i], DrawElementsType.UnsignedInt, 0);
                    }
                    

                    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

                    //GL.DepthFunc(DepthFunction.Lequal);

                    GL.Disable(EnableCap.StencilTest);
                    GL.LineWidth(2);
                }
            }
        }
        
        private static void GetPixelFormats(GX2SurfaceFormat Format, out PixelInternalFormat pixelInternalFormat)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.T_BC1_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case GX2SurfaceFormat.T_BC1_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case GX2SurfaceFormat.T_BC2_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case GX2SurfaceFormat.T_BC2_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case GX2SurfaceFormat.T_BC3_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case GX2SurfaceFormat.T_BC3_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case GX2SurfaceFormat.T_BC4_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case GX2SurfaceFormat.T_BC4_SNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case GX2SurfaceFormat.T_BC5_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case GX2SurfaceFormat.T_BC5_SNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedSignedRgRgtc2;
                    break;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    break;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    break;
                default:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    break;
            }
        }

        private static TextureWrapMode GetWrapMode(GX2TexClamp texClamp)
        {
            switch (texClamp)
            {
                case GX2TexClamp.Clamp:
                    return TextureWrapMode.Clamp;
                case GX2TexClamp.ClampBorder:
                    return TextureWrapMode.ClampToBorder;
                case GX2TexClamp.Mirror:
                    return TextureWrapMode.MirroredRepeat;
                default:
                    return TextureWrapMode.Repeat;
            }
        }

        /// <summary>
        /// Uploads a texture to the OpenGL texture units
        /// </summary>
        /// <param name="texture">Texture to upload data from</param>
        /// <returns>Integer ID of the uploaded texture</returns>
        private static int UploadTexture(Texture texture)
        {
            #region Deswizzle
            uint bpp = GX2.surfaceGetBitsPerPixel((uint)texture.Format) >> 3;

            GX2.GX2Surface surf = new GX2.GX2Surface
            {
                bpp = bpp,
                height = texture.Height,
                width = texture.Width,
                aa = (uint)texture.AAMode,
                alignment = texture.Alignment,
                depth = texture.Depth,
                dim = (uint)texture.Dim,
                format = (uint)texture.Format,
                use = (uint)texture.Use,
                pitch = texture.Pitch,
                data = texture.Data,
                numMips = texture.MipCount,
                mipOffset = texture.MipOffsets,
                mipData = texture.MipData,
                tileMode = (uint)texture.TileMode,
                swizzle = texture.Swizzle,
                numArray = texture.ArrayLength
            };
            
            if (surf.mipData == null)
                surf.numMips = 1;

            byte[] deswizzled = GX2.Decode(surf, 0, 0);


            #endregion
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);


            if (texture.Format == GX2SurfaceFormat.T_BC4_UNorm)
            {
                deswizzled = DDSCompressor.DecompressBC4(deswizzled, (int)texture.Width, (int)texture.Height, false).Data;
            }
            else if (texture.Format == GX2SurfaceFormat.T_BC4_SNorm)
            {
                deswizzled = DDSCompressor.DecompressBC4(deswizzled, (int)texture.Width, (int)texture.Height, true).Data;
            }
            else if (texture.Format == GX2SurfaceFormat.T_BC5_UNorm)
            {
                deswizzled = DDSCompressor.DecompressBC5(deswizzled, (int)texture.Width, (int)texture.Height, false, true);
            }
            else if (texture.Format == GX2SurfaceFormat.T_BC5_SNorm)
            {
                deswizzled = DDSCompressor.DecompressBC5(deswizzled, (int)texture.Width, (int)texture.Height, true, true);
            }
            else
            {
                GetPixelFormats(texture.Format, out PixelInternalFormat internalFormat);
                
                if (internalFormat != PixelInternalFormat.Rgba)
                {
                    GL.CompressedTexImage2D(TextureTarget.Texture2D, 0, (InternalFormat)internalFormat, (int)texture.Width, (int)texture.Height, 0, deswizzled.Length, deswizzled);

                    goto DATA_UPLOADED;
                }
            }

            #region channel reassign

            byte[] sources = new byte[] { 0, 0, 0, 0, 0, 0xFF };

            for (int i = 0; i < deswizzled.Length; i += 4)
            {
                sources[0] = deswizzled[i];
                sources[1] = deswizzled[i + 1];
                sources[2] = deswizzled[i + 2];
                sources[3] = deswizzled[i + 3];

                deswizzled[i] = sources[(int)texture.CompSelR];
                deswizzled[i + 1] = sources[(int)texture.CompSelG];
                deswizzled[i + 2] = sources[(int)texture.CompSelB];
                deswizzled[i + 3] = sources[(int)texture.CompSelA];
                //deswizzled[i + 3] = 0xFF;
            }
            #endregion

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)texture.Width, (int)texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, deswizzled);

        DATA_UPLOADED:

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return tex;
        }
    }
}
