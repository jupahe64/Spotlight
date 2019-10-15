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

        static Dictionary<string, CachedModel> cache = new Dictionary<string, CachedModel>();

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
                        vec4 hc_rgb = vec4(highlight_color.xyz,1);
                        float hc_a   = highlight_color.w;
                        gl_FragColor = fragColor * texture(tex,fragUV) * (1-hc_a) + hc_rgb * hc_a;
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
        
        public static void Submit(string modelName, Stream stream, GL_ControlModern control, string textureArc = null)
        {
            if (!cache.ContainsKey(modelName))
                cache[modelName] = new CachedModel(stream, textureArc, control);
        }

        public static bool TryDraw(string modelName, GL_ControlModern control, Pass pass, Vector4 highlightColor)
        {
            if (cache.ContainsKey(modelName))
            {
                cache[modelName].Draw(control, pass, highlightColor);
                return true;
            }
            else
                return false;
        }

        public static bool Contains(string modelName) => cache.ContainsKey(modelName);

        public struct CachedModel
        {
            static readonly float white = BitConverter.ToSingle(new byte[] {255, 255, 255, 255},0);

            VertexArrayObject[] vaos;
            readonly int[] indexBufferLengths;
            readonly int[] textures;
            readonly (int,int)[] wrapModes;

            public CachedModel(Stream stream, string textureArc, GL_ControlModern control)
            {
                if (textureArc != null && File.Exists(Program.ObjectDataPath + textureArc + ".szs"))
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


                ResFile bfres = new ResFile(stream);

                Model mdl = bfres.Models[0];

                vaos = new VertexArrayObject[mdl.Shapes.Count];
                indexBufferLengths = new int[mdl.Shapes.Count];
                textures           = new int[mdl.Shapes.Count];
                wrapModes =    new (int,int)[mdl.Shapes.Count];
                
                int shapeIndex = 0;

                foreach(Shape shape in mdl.Shapes.Values)
                {
                    uint[] indices = shape.Meshes[0].GetIndices().ToArray();

                    if (mdl.Materials[shape.MaterialIndex].TextureRefs.Count != 0)
                    {
                        TextureRef texRef = mdl.Materials[shape.MaterialIndex].TextureRefs[0];
                        if (texRef.Texture != null)
                        {
                            textures[shapeIndex] = UploadTexture(texRef.Texture);

                            wrapModes[shapeIndex] = ((int)GetWrapMode(mdl.Materials[shape.MaterialIndex].Samplers[0].TexSampler.ClampX),
                                (int)GetWrapMode(mdl.Materials[shape.MaterialIndex].Samplers[0].TexSampler.ClampY));
                        }
                        else if (textureArc != null)
                        {
                            if (texArcCache.ContainsKey(textureArc) && texArcCache[textureArc].ContainsKey(texRef.Name))
                            {
                                textures[shapeIndex] = texArcCache[textureArc][texRef.Name];

                                wrapModes[shapeIndex] = ((int)TextureWrapMode.Repeat, (int)TextureWrapMode.Repeat);
                            }
                            else
                            {
                                textures[shapeIndex] = -2;

                                wrapModes[shapeIndex] = ((int)TextureWrapMode.Repeat, (int)TextureWrapMode.Repeat);
                            }
                        }
                    }
                    else
                    {
                        textures[shapeIndex] = -1;
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
                    control.CurrentShader = BfresShaderProgram;

                    control.CurrentShader.SetVector4("highlight_color", highlightColor);

                    GL.ActiveTexture(TextureUnit.Texture0);
                }

                for(int i = 0; i<vaos.Length; i++)
                {
                    if (pass != Pass.PICKING)
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

                    vaos[i].Use(control);

                    GL.DrawElements(BeginMode.Triangles, indexBufferLengths[i], DrawElementsType.UnsignedInt, 0);
                }
            }
        }
        
        private static void GetPixelFormats(GX2SurfaceFormat Format, out PixelInternalFormat pixelInternalFormat, out PixelFormat pixelFormat)
        {
            pixelFormat = PixelFormat.Rgba;
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
                case GX2SurfaceFormat.T_BC4_SNorm:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    break;
                case GX2SurfaceFormat.T_BC5_SNorm:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    break;
                case GX2SurfaceFormat.T_BC5_UNorm:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
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

            GetPixelFormats(texture.Format, out PixelInternalFormat internalFormat, out PixelFormat format);

            if (internalFormat == PixelInternalFormat.Rgba)
                GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat,
                (int)texture.Width/4, (int)texture.Height/4, 0, format, PixelType.UnsignedByte, deswizzled);
            else
                GL.CompressedTexImage2D(TextureTarget.Texture2D, 0, (InternalFormat)internalFormat,
                (int)texture.Width, (int)texture.Height, 0, deswizzled.Length, deswizzled);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return tex;
        }
    }
}
