using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFormats3DW;
using GL_EditorFramework.GL_Core;
using OpenTK.Graphics.OpenGL;
using Syroot.BinaryData;
using Syroot.Maths;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.Bfres.Helpers;

namespace SpotLight
{
    public class BfresModelCache
    {
        private static bool initialized = false;

        public static ShaderProgram BfresShaderProgram;

        Dictionary<string, CachedModel> cache = new Dictionary<string, CachedModel>();

        public static int DefaultTetxure;

        public static void Initialize(GL_ControlModern control)
        {
            if (initialized)
                return;

            BfresShaderProgram = new ShaderProgram(
                new FragmentShader(
                    @"#version 330
                    uniform sampler2D tex;
                    in vec2 fragUV;
                    void main(){
                        gl_FragColor = texture(tex,fragUV);
                    }"),
                new VertexShader(
                    @"#version 330
                    layout(location = 0) in vec4 position;
                    layout(location = 1) in vec2 uv;
                    uniform mat4 mtxMdl;
                    uniform mat4 mtxCam;
                    out vec2 fragUV;

                    void main(){
                        fragUV = uv;
                        gl_Position = mtxCam*mtxMdl*position;
                    }"
                ),control);

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
            bmp.UnlockBits(bmpData);

            initialized = true;
        }
        
        public void Submit(string modelName, Stream stream, GL_ControlModern control)
        {
            if (!cache.ContainsKey(modelName))
                cache[modelName] = new CachedModel(stream, control);
        }

        public bool TryDraw(string modelName, GL_ControlModern control)
        {
            if (cache.ContainsKey(modelName))
            {
                cache[modelName].Draw(control);
                return true;
            }
            else
                return false;
        }

        public struct CachedModel
        {
            VertexArrayObject[] vaos;
            readonly int[] indexBufferLengths;
            readonly int[] textures;

            public CachedModel(Stream stream, GL_ControlModern control)
            {
                ResFile bfres = new ResFile(stream);

                Model mdl = bfres.Models[0];

                vaos = new VertexArrayObject[mdl.Shapes.Count];
                indexBufferLengths = new int[mdl.Shapes.Count];
                textures           = new int[mdl.Shapes.Count];
                
                int shapeIndex = 0;

                foreach(Shape shape in mdl.Shapes.Values)
                {
                    Vector4F[] positions = new VertexBufferHelper(mdl.VertexBuffers[shapeIndex], ByteConverter.Big)["_p0"].Data;

                    Vector4F[] uvs = new VertexBufferHelper(mdl.VertexBuffers[shapeIndex], ByteConverter.Big)["_u0"].Data;

                    uint[] indices = shape.Meshes[0].GetIndices().ToArray();
                    
                    Texture texture = mdl.Materials[shape.MaterialIndex].TextureRefs[0].Texture;
                    #region deswizzle
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
                    textures[shapeIndex] = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, textures[shapeIndex]);

                    GetPixelFormats(texture.Format, out PixelInternalFormat internalFormat, out PixelFormat format);

                    if (internalFormat == PixelInternalFormat.Rgba)
                        GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat,
                        (int)texture.Width, (int)texture.Height, 0, format, PixelType.UnsignedByte, deswizzled);
                    else
                        GL.CompressedTexImage2D(TextureTarget.Texture2D, 0, (InternalFormat)internalFormat,
                        (int)texture.Width, (int)texture.Height, 0, deswizzled.Length, deswizzled);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                    indexBufferLengths[shapeIndex] = indices.Length;

                    float[] bufferData = new float[5 * positions.Length];

                    int _i = 0;
                    for (int i = 0; i < positions.Length; i++)
                    {
                        bufferData[_i]     = positions[i].X * 0.01f;
                        bufferData[_i + 1] = positions[i].Y * 0.01f;
                        bufferData[_i + 2] = positions[i].Z * 0.01f;
                        bufferData[_i + 3] = uvs[i].X;
                        bufferData[_i + 4] = uvs[i].Y;
                        _i += 5;
                    }
                    int[] buffers = new int[2];
                    GL.GenBuffers(2, buffers);

                    int indexBuffer = buffers[0];
                    int vaoBuffer = buffers[1];

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, vaoBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 5 * positions.Length, bufferData, BufferUsageHint.StaticDraw);

                    vaos[shapeIndex] = new VertexArrayObject(vaoBuffer, indexBuffer);
                    vaos[shapeIndex].AddAttribute(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
                    vaos[shapeIndex].AddAttribute(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);

                    vaos[shapeIndex].Initialize(control);

                    shapeIndex++;
                }
            }

            public void Draw(GL_ControlModern control)
            {
                control.CurrentShader = BfresShaderProgram;

                GL.ActiveTexture(TextureUnit.Texture0);

                for(int i = 0; i<vaos.Length; i++)
                {
                    GL.BindTexture(TextureTarget.Texture2D, textures[i]);

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
    
    }
}
