using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectXTexNet;

namespace FileFormats3DW
{
    //code from SwitchToolbox

    public class DDSCompressor
    {
        public struct Bitmap
        {
            public int Width;
            public int Height;
            public byte[] Data;

            public Bitmap(byte[] data, int width, int height)
            {
                Width = width;
                Height = height;
                Data = data;
            }
        }

        public enum DDS_DXGI_FORMAT : uint
        {
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
            DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
            DXGI_FORMAT_R32G32B32A32_UINT = 3,
            DXGI_FORMAT_R32G32B32A32_SINT = 4,
            DXGI_FORMAT_R32G32B32_TYPELESS = 5,
            DXGI_FORMAT_R32G32B32_FLOAT = 6,
            DXGI_FORMAT_R32G32B32_UINT = 7,
            DXGI_FORMAT_R32G32B32_SINT = 8,
            DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
            DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
            DXGI_FORMAT_R16G16B16A16_UNORM = 11,
            DXGI_FORMAT_R16G16B16A16_UINT = 12,
            DXGI_FORMAT_R16G16B16A16_SNORM = 13,
            DXGI_FORMAT_R16G16B16A16_SINT = 14,
            DXGI_FORMAT_R32G32_TYPELESS = 15,
            DXGI_FORMAT_R32G32_FLOAT = 16,
            DXGI_FORMAT_R32G32_UINT = 17,
            DXGI_FORMAT_R32G32_SINT = 18,
            DXGI_FORMAT_R32G8X24_TYPELESS = 19,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
            DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
            DXGI_FORMAT_R10G10B10A2_UNORM = 24,
            DXGI_FORMAT_R10G10B10A2_UINT = 25,
            DXGI_FORMAT_R11G11B10_FLOAT = 26,
            DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
            DXGI_FORMAT_R8G8B8A8_UNORM = 28,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
            DXGI_FORMAT_R8G8B8A8_UINT = 30,
            DXGI_FORMAT_R8G8B8A8_SNORM = 31,
            DXGI_FORMAT_R8G8B8A8_SINT = 32,
            DXGI_FORMAT_R16G16_TYPELESS = 33,
            DXGI_FORMAT_R16G16_FLOAT = 34,
            DXGI_FORMAT_R16G16_UNORM = 35,
            DXGI_FORMAT_R16G16_UINT = 36,
            DXGI_FORMAT_R16G16_SNORM = 37,
            DXGI_FORMAT_R16G16_SINT = 38,
            DXGI_FORMAT_R32_TYPELESS = 39,
            DXGI_FORMAT_D32_FLOAT = 40,
            DXGI_FORMAT_R32_FLOAT = 41,
            DXGI_FORMAT_R32_UINT = 42,
            DXGI_FORMAT_R32_SINT = 43,
            DXGI_FORMAT_R24G8_TYPELESS = 44,
            DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
            DXGI_FORMAT_R8G8_TYPELESS = 48,
            DXGI_FORMAT_R8G8_UNORM = 49,
            DXGI_FORMAT_R8G8_UINT = 50,
            DXGI_FORMAT_R8G8_SNORM = 51,
            DXGI_FORMAT_R8G8_SINT = 52,
            DXGI_FORMAT_R16_TYPELESS = 53,
            DXGI_FORMAT_R16_FLOAT = 54,
            DXGI_FORMAT_D16_UNORM = 55,
            DXGI_FORMAT_R16_UNORM = 56,
            DXGI_FORMAT_R16_UINT = 57,
            DXGI_FORMAT_R16_SNORM = 58,
            DXGI_FORMAT_R16_SINT = 59,
            DXGI_FORMAT_R8_TYPELESS = 60,
            DXGI_FORMAT_R8_UNORM = 61,
            DXGI_FORMAT_R8_UINT = 62,
            DXGI_FORMAT_R8_SNORM = 63,
            DXGI_FORMAT_R8_SINT = 64,
            DXGI_FORMAT_A8_UNORM = 65,
            DXGI_FORMAT_R1_UNORM = 66,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
            DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
            DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
            DXGI_FORMAT_BC1_TYPELESS = 70,
            DXGI_FORMAT_BC1_UNORM = 71,
            DXGI_FORMAT_BC1_UNORM_SRGB = 72,
            DXGI_FORMAT_BC2_TYPELESS = 73,
            DXGI_FORMAT_BC2_UNORM = 74,
            DXGI_FORMAT_BC2_UNORM_SRGB = 75,
            DXGI_FORMAT_BC3_TYPELESS = 76,
            DXGI_FORMAT_BC3_UNORM = 77,
            DXGI_FORMAT_BC3_UNORM_SRGB = 78,
            DXGI_FORMAT_BC4_TYPELESS = 79,
            DXGI_FORMAT_BC4_UNORM = 80,
            DXGI_FORMAT_BC4_SNORM = 81,
            DXGI_FORMAT_BC5_TYPELESS = 82,
            DXGI_FORMAT_BC5_UNORM = 83,
            DXGI_FORMAT_BC5_SNORM = 84,
            DXGI_FORMAT_B5G6R5_UNORM = 85,
            DXGI_FORMAT_B5G5R5A1_UNORM = 86,
            DXGI_FORMAT_B8G8R8A8_UNORM = 87,
            DXGI_FORMAT_B8G8R8X8_UNORM = 88,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
            DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
            DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
            DXGI_FORMAT_BC6H_TYPELESS = 94,
            DXGI_FORMAT_BC6H_UF16 = 95,
            DXGI_FORMAT_BC6H_SF16 = 96,
            DXGI_FORMAT_BC7_TYPELESS = 97,
            DXGI_FORMAT_BC7_UNORM = 98,
            DXGI_FORMAT_BC7_UNORM_SRGB = 99,
            DXGI_FORMAT_AYUV = 100,
            DXGI_FORMAT_Y410 = 101,
            DXGI_FORMAT_Y416 = 102,
            DXGI_FORMAT_NV12 = 103,
            DXGI_FORMAT_P010 = 104,
            DXGI_FORMAT_P016 = 105,
            DXGI_FORMAT_420_OPAQUE = 106,
            DXGI_FORMAT_YUY2 = 107,
            DXGI_FORMAT_Y210 = 108,
            DXGI_FORMAT_Y216 = 109,
            DXGI_FORMAT_NV11 = 110,
            DXGI_FORMAT_AI44 = 111,
            DXGI_FORMAT_IA44 = 112,
            DXGI_FORMAT_P8 = 113,
            DXGI_FORMAT_A8P8 = 114,
            DXGI_FORMAT_B4G4R4A4_UNORM = 115,
            DXGI_FORMAT_P208 = 130,
            DXGI_FORMAT_V208 = 131,
            DXGI_FORMAT_V408 = 132,


            DXGI_FORMAT_ASTC_4X4_UNORM = 134,
            DXGI_FORMAT_ASTC_4X4_UNORM_SRGB = 135,
            DXGI_FORMAT_ASTC_5X4_TYPELESS = 137,
            DXGI_FORMAT_ASTC_5X4_UNORM = 138,
            DXGI_FORMAT_ASTC_5X4_UNORM_SRGB = 139,
            DXGI_FORMAT_ASTC_5X5_TYPELESS = 141,
            DXGI_FORMAT_ASTC_5X5_UNORM = 142,
            DXGI_FORMAT_ASTC_5X5_UNORM_SRGB = 143,
            DXGI_FORMAT_ASTC_6X5_TYPELESS = 145,
            DXGI_FORMAT_ASTC_6X5_UNORM = 146,
            DXGI_FORMAT_ASTC_6X5_UNORM_SRGB = 147,
            DXGI_FORMAT_ASTC_6X6_TYPELESS = 149,
            DXGI_FORMAT_ASTC_6X6_UNORM = 150,
            DXGI_FORMAT_ASTC_6X6_UNORM_SRGB = 151,
            DXGI_FORMAT_ASTC_8X5_TYPELESS = 153,
            DXGI_FORMAT_ASTC_8X5_UNORM = 154,
            DXGI_FORMAT_ASTC_8X5_UNORM_SRGB = 155,
            DXGI_FORMAT_ASTC_8X6_TYPELESS = 157,
            DXGI_FORMAT_ASTC_8X6_UNORM = 158,
            DXGI_FORMAT_ASTC_8X6_UNORM_SRGB = 159,
            DXGI_FORMAT_ASTC_8X8_TYPELESS = 161,
            DXGI_FORMAT_ASTC_8X8_UNORM = 162,
            DXGI_FORMAT_ASTC_8X8_UNORM_SRGB = 163,
            DXGI_FORMAT_ASTC_10X5_TYPELESS = 165,
            DXGI_FORMAT_ASTC_10X5_UNORM = 166,
            DXGI_FORMAT_ASTC_10X5_UNORM_SRGB = 167,
            DXGI_FORMAT_ASTC_10X6_TYPELESS = 169,
            DXGI_FORMAT_ASTC_10X6_UNORM = 170,
            DXGI_FORMAT_ASTC_10X6_UNORM_SRGB = 171,
            DXGI_FORMAT_ASTC_10X8_TYPELESS = 173,
            DXGI_FORMAT_ASTC_10X8_UNORM = 174,
            DXGI_FORMAT_ASTC_10X8_UNORM_SRGB = 175,
            DXGI_FORMAT_ASTC_10X10_TYPELESS = 177,
            DXGI_FORMAT_ASTC_10X10_UNORM = 178,
            DXGI_FORMAT_ASTC_10X10_UNORM_SRGB = 179,
            DXGI_FORMAT_ASTC_12X10_TYPELESS = 181,
            DXGI_FORMAT_ASTC_12X10_UNORM = 182,
            DXGI_FORMAT_ASTC_12X10_UNORM_SRGB = 183,
            DXGI_FORMAT_ASTC_12X12_TYPELESS = 185,
            DXGI_FORMAT_ASTC_12X12_UNORM = 186,
            DXGI_FORMAT_ASTC_12X12_UNORM_SRGB = 187,

            DXGI_FORMAT_FORCE_UINT = 0xFFFFFFFF
        }

        //Huge thanks to gdkchan and AbooodXD for the method of decomp BC5/BC4.

        //Todo. Add these to DDS code and add in methods to compress and decode more formats
        //BC7 also needs to be decompressed properly since OpenTK can't decompress those

        //BC4 actually breaks a bit with artifacts so i'll need to go back and fix


        private static byte[] BCnDecodeTile(byte[] Input, int Offset, bool IsBC1)
        {
            Color[] CLUT = new Color[4];

            int c0 = Get16(Input, Offset + 0);
            int c1 = Get16(Input, Offset + 2);

            CLUT[0] = DecodeRGB565(c0);
            CLUT[1] = DecodeRGB565(c1);
            CLUT[2] = CalculateCLUT2(CLUT[0], CLUT[1], c0, c1, IsBC1);
            CLUT[3] = CalculateCLUT3(CLUT[0], CLUT[1], c0, c1, IsBC1);

            int Indices = Get32(Input, Offset + 4);

            int IdxShift = 0;

            byte[] Output = new byte[4 * 4 * 4];

            int OOffset = 0;

            for (int TY = 0; TY < 4; TY++)
            {
                for (int TX = 0; TX < 4; TX++)
                {
                    int Idx = (Indices >> IdxShift) & 3;

                    IdxShift += 2;

                    Color Pixel = CLUT[Idx];

                    Output[OOffset + 0] = Pixel.B;
                    Output[OOffset + 1] = Pixel.G;
                    Output[OOffset + 2] = Pixel.R;
                    Output[OOffset + 3] = Pixel.A;

                    OOffset += 4;
                }
            }
            return Output;
        }
        private static Color DecodeRGB565(int Value)
        {
            int B = ((Value >> 0) & 0x1f) << 3;
            int G = ((Value >> 5) & 0x3f) << 2;
            int R = ((Value >> 11) & 0x1f) << 3;

            return Color.FromArgb(
                R | (R >> 5),
                G | (G >> 6),
                B | (B >> 5));
        }
        private static Color CalculateCLUT2(Color C0, Color C1, int c0, int c1, bool IsBC1)
        {
            if (c0 > c1 || !IsBC1)
            {
                return Color.FromArgb(
                    (2 * C0.R + C1.R) / 3,
                    (2 * C0.G + C1.G) / 3,
                    (2 * C0.B + C1.B) / 3);
            }
            else
            {
                return Color.FromArgb(
                    (C0.R + C1.R) / 2,
                    (C0.G + C1.G) / 2,
                    (C0.B + C1.B) / 2);
            }
        }
        private static Color CalculateCLUT3(Color C0, Color C1, int c0, int c1, bool IsBC1)
        {
            if (c0 > c1 || !IsBC1)
            {
                return
                    Color.FromArgb(
                        (2 * C1.R + C0.R) / 3,
                        (2 * C1.G + C0.G) / 3,
                        (2 * C1.B + C0.B) / 3);
            }

            return Color.Transparent;
        }
        public static Bitmap DecompressBC1(Byte[] data, int width, int height, bool IsSRGB)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 8;

                    byte[] Tile = BCnDecodeTile(data, IOffs, true);

                    int TOffset = 0;

                    for (int TY = 0; TY < 4; TY++)
                    {
                        for (int TX = 0; TX < 4; TX++)
                        {
                            int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                            Output[OOffset + 0] = Tile[TOffset + 0];
                            Output[OOffset + 1] = Tile[TOffset + 1];
                            Output[OOffset + 2] = Tile[TOffset + 2];
                            Output[OOffset + 3] = Tile[TOffset + 3];

                            TOffset += 4;
                        }
                    }
                }
            }
            return new Bitmap(Output, W * 4, H * 4);
        }
        public static Bitmap DecompressBC3(Byte[] data, int width, int height, bool IsSRGB)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 16;

                    byte[] Tile = BCnDecodeTile(data, IOffs + 8, false);

                    byte[] Alpha = new byte[8];

                    Alpha[0] = data[IOffs + 0];
                    Alpha[1] = data[IOffs + 1];

                    CalculateBC3Alpha(Alpha);

                    int AlphaLow = Get32(data, IOffs + 2);
                    int AlphaHigh = Get16(data, IOffs + 6);

                    ulong AlphaCh = (uint)AlphaLow | (ulong)AlphaHigh << 32;

                    int TOffset = 0;

                    for (int TY = 0; TY < 4; TY++)
                    {
                        for (int TX = 0; TX < 4; TX++)
                        {
                            int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                            byte AlphaPx = Alpha[(AlphaCh >> (TY * 12 + TX * 3)) & 7];

                            Output[OOffset + 0] = Tile[TOffset + 0];
                            Output[OOffset + 1] = Tile[TOffset + 1];
                            Output[OOffset + 2] = Tile[TOffset + 2];
                            Output[OOffset + 3] = AlphaPx;

                            TOffset += 4;
                        }
                    }
                }
            }

            return new Bitmap(Output, W * 4, H * 4);
        }
        public static Bitmap DecompressBC4(Byte[] data, int width, int height, bool IsSNORM)
          {
              int W = (width + 3) / 4;
              int H = (height + 3) / 4;

              byte[] Output = new byte[W * H * 64];

              for (int Y = 0; Y < H; Y++)
              {
                  for (int X = 0; X < W; X++)
                  {
                      int IOffs = (Y * W + X) * 8;

                      byte[] Red = new byte[8];

                      Red[0] = data[IOffs + 0];
                      Red[1] = data[IOffs + 1];

                      CalculateBC3Alpha(Red);

                      int RedLow = Get32(data, IOffs + 2);
                      int RedHigh = Get16(data, IOffs + 6);

                      ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;

                      int TOffset = 0;
                      int TW = Math.Min(width - X * 4, 4);
                      int TH = Math.Min(height - Y * 4, 4);

                      for (int TY = 0; TY < 4; TY++)
                      {
                          for (int TX = 0; TX < 4; TX++)
                          {
                              int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                              byte RedPx = Red[(RedCh >> (TY * 12 + TX * 3)) & 7];

                              Output[OOffset + 0] = RedPx;
                              Output[OOffset + 1] = RedPx;
                              Output[OOffset + 2] = RedPx;
                              Output[OOffset + 3] = 255;

                              TOffset += 4;
                          }
                      }
                  }
              }

              return new Bitmap(Output, W * 4, H * 4);
          }
        public static byte[] DecompressBC5(Byte[] data, int width, int height, bool IsSNORM, bool IsByteArray)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[width * height * 4];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 16;
                    byte[] Red = new byte[8];
                    byte[] Green = new byte[8];

                    Red[0] = data[IOffs + 0];
                    Red[1] = data[IOffs + 1];

                    Green[0] = data[IOffs + 8];
                    Green[1] = data[IOffs + 9];

                    if (IsSNORM == true)
                    {
                        CalculateBC3AlphaS(Red);
                        CalculateBC3AlphaS(Green);
                    }
                    else
                    {
                        CalculateBC3Alpha(Red);
                        CalculateBC3Alpha(Green);
                    }

                    int RedLow = Get32(data, IOffs + 2);
                    int RedHigh = Get16(data, IOffs + 6);

                    int GreenLow = Get32(data, IOffs + 10);
                    int GreenHigh = Get16(data, IOffs + 14);

                    ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;
                    ulong GreenCh = (uint)GreenLow | (ulong)GreenHigh << 32;

                    int TW = Math.Min(width - X * 4, 4);
                    int TH = Math.Min(height - Y * 4, 4);

                    if (IsSNORM == true)
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                if (IsSNORM == true)
                                {
                                    RedPx += 0x80;
                                    GreenPx += 0x80;
                                }

                                float NX = (RedPx / 255f) * 2 - 1;
                                float NY = (GreenPx / 255f) * 2 - 1;
                                float NZ = (float)Math.Sqrt(1 - (NX * NX + NY * NY));

                                Output[OOffset + 0] = Clamp((NX + 1) * 0.5f);
                                Output[OOffset + 1] = Clamp((NY + 1) * 0.5f);
                                Output[OOffset + 2] = Clamp((NZ + 1) * 0.5f);
                                Output[OOffset + 3] = 0xff;
                            }
                        }
                    }
                    else
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                Output[OOffset + 0] = RedPx;
                                Output[OOffset + 1] = GreenPx;
                                Output[OOffset + 2] = 255;
                                Output[OOffset + 3] = 255;

                            }
                        }
                    }
                }
            }
            return Output;
        }
        public static Bitmap DecompressBC5(Byte[] data, int width, int height, bool IsSNORM)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)

                {
                    int IOffs = (Y * W + X) * 16;
                    byte[] Red = new byte[8];
                    byte[] Green = new byte[8];

                    Red[0] = data[IOffs + 0];
                    Red[1] = data[IOffs + 1];

                    Green[0] = data[IOffs + 8];
                    Green[1] = data[IOffs + 9];

                    if (IsSNORM == true)
                    {
                        CalculateBC3AlphaS(Red);
                        CalculateBC3AlphaS(Green);
                    }
                    else
                    {
                        CalculateBC3Alpha(Red);
                        CalculateBC3Alpha(Green);
                    }

                    int RedLow = Get32(data, IOffs + 2);
                    int RedHigh = Get16(data, IOffs + 6);

                    int GreenLow = Get32(data, IOffs + 10);
                    int GreenHigh = Get16(data, IOffs + 14);

                    ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;
                    ulong GreenCh = (uint)GreenLow | (ulong)GreenHigh << 32;

                    int TW = Math.Min(width - X * 4, 4);
                    int TH = Math.Min(height - Y * 4, 4);


                    if (IsSNORM == true)
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                if (IsSNORM == true)
                                {
                                    RedPx += 0x80;
                                    GreenPx += 0x80;
                                }

                                float NX = (RedPx / 255f) * 2 - 1;
                                float NY = (GreenPx / 255f) * 2 - 1;
                                float NZ = (float)Math.Sqrt(1 - (NX * NX + NY * NY));

                                Output[OOffset + 0] = Clamp((NZ + 1) * 0.5f);
                                Output[OOffset + 1] = Clamp((NY + 1) * 0.5f);
                                Output[OOffset + 2] = Clamp((NX + 1) * 0.5f);
                                Output[OOffset + 3] = 0xff;
                            }
                        }
                    }
                    else
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                Output[OOffset + 0] = 255;
                                Output[OOffset + 1] = GreenPx;
                                Output[OOffset + 2] = RedPx;
                                Output[OOffset + 3] = 255;

                            }
                        }
                    }
                }
            }

            return new Bitmap(Output, W * 4, H * 4);
        }
        public static unsafe byte[] CompressBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format, float AlphaRef = 0.5f, bool fastCompress = false)
        {
            long inputRowPitch = width * 4;
            long inputSlicePitch = width * height * 4;

            if (data.Length == inputSlicePitch)
            {
                byte* buf;
                buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
                Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

                DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                    width, height, DXGI_FORMAT.R8G8B8A8_UNORM, inputRowPitch,
                    inputSlicePitch, (IntPtr)buf, null);

                TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                    DXGI_FORMAT.R8G8B8A8_UNORM, TEX_DIMENSION.TEXTURE2D);

                ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                    new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

                var compFlags = TEX_COMPRESS_FLAGS.DEFAULT;
              //  compFlags |= TEX_COMPRESS_FLAGS.PARALLEL;

                if (format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM ||
                    format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB ||
                    format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS)
                {
                    if (fastCompress)
                        compFlags |= TEX_COMPRESS_FLAGS.BC7_QUICK;
                }

                if (format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB ||
                format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB ||
                format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB ||
                format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB ||
                format == DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB)
                {
                    compFlags |= TEX_COMPRESS_FLAGS.SRGB;
                }

                using (var comp = scratchImage.Compress((DXGI_FORMAT)format, compFlags, 0.5f))
                {
                    long outRowPitch;
                    long outSlicePitch;
                    TexHelper.Instance.ComputePitch((DXGI_FORMAT)format, width, height, out outRowPitch, out outSlicePitch, CP_FLAGS.NONE);

                    byte[] result = new byte[outSlicePitch];
                    Marshal.Copy(comp.GetImage(0).Pixels, result, 0, result.Length);

                    inputImage = null;
                    scratchImage.Dispose();


                    return result;
                }
            }
            return null;
        }
        public static unsafe byte[] DecompressBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format)
        {
            Console.WriteLine(format);
            Console.WriteLine(width);
            Console.WriteLine(height);

            long inputRowPitch;
            long inputSlicePitch;
            TexHelper.Instance.ComputePitch((DXGI_FORMAT)format, width, height, out inputRowPitch, out inputSlicePitch, CP_FLAGS.NONE);

            DXGI_FORMAT FormatDecompressed;

            if (format.ToString().Contains("SRGB"))
                FormatDecompressed = DXGI_FORMAT.R8G8B8A8_UNORM_SRGB;
            else
                FormatDecompressed = DXGI_FORMAT.R8G8B8A8_UNORM;

            byte* buf;
            buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
            Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

            DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                width, height, (DXGI_FORMAT)format, inputRowPitch,
                inputSlicePitch, (IntPtr)buf, null);

            TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                (DXGI_FORMAT)format, TEX_DIMENSION.TEXTURE2D);

            ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

            using (var decomp = scratchImage.Decompress(0, FormatDecompressed))
            {
                byte[] result = new byte[4 * width * height];
                Marshal.Copy(decomp.GetImage(0).Pixels, result, 0, result.Length);

                inputImage = null;
                scratchImage.Dispose();

                return result;
            }
        }
        public static unsafe byte[] DecodePixelBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format, float AlphaRef = 0.5f)
        {
            if (format == DDS_DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM)
            {
                byte[] result = new byte[data.Length];
                Array.Copy(data, result, data.Length);
                return result;
            }

            return Convert(data, width, height, (DXGI_FORMAT)format, DXGI_FORMAT.R8G8B8A8_UNORM);
        }
        public static unsafe byte[] EncodePixelBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format, float AlphaRef = 0.5f)
        {
            if (format == DDS_DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM || format == DDS_DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB)
                return data;

            return Convert(data, width, height, DXGI_FORMAT.R8G8B8A8_UNORM, (DXGI_FORMAT)format);
        }
        public static unsafe byte[] Convert(Byte[] data, int width, int height, DXGI_FORMAT inputFormat, DXGI_FORMAT outputFormat)
        {
            long inputRowPitch;
            long inputSlicePitch;
            TexHelper.Instance.ComputePitch(inputFormat, width, height, out inputRowPitch, out inputSlicePitch, CP_FLAGS.NONE);

            if (data.Length == inputSlicePitch)
            {
                byte* buf;
                buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
                Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

                DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                    width, height, inputFormat, inputRowPitch,
                    inputSlicePitch, (IntPtr)buf, null);

                TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                    inputFormat, TEX_DIMENSION.TEXTURE2D);

                ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                    new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

                var convFlags = TEX_FILTER_FLAGS.DEFAULT;

                if (inputFormat == DXGI_FORMAT.B8G8R8A8_UNORM_SRGB ||
                 inputFormat == DXGI_FORMAT.B8G8R8X8_UNORM_SRGB ||
                 inputFormat == DXGI_FORMAT.R8G8B8A8_UNORM_SRGB)
                {
                    convFlags |= TEX_FILTER_FLAGS.SRGB;
                }

                using (var decomp = scratchImage.Convert(0, outputFormat, convFlags, 0.5f))
                {
                    long outRowPitch;
                    long outSlicePitch;
                    TexHelper.Instance.ComputePitch(outputFormat, width, height, out outRowPitch, out outSlicePitch, CP_FLAGS.NONE);

                    byte[] result = new byte[outSlicePitch];
                    Marshal.Copy(decomp.GetImage(0).Pixels, result, 0, result.Length);

                    inputImage = null;
                    scratchImage.Dispose();


                    return result;
                }
            }
            return null;
        }
        /*    public static byte[] CompressBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format, float AlphaRef)
            {
                return DirectXTex.ImageCompressor.Compress(data, width, height, (int)format, AlphaRef);
            }*/

        public static byte[] DecompressCompLibBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format)
        {
            byte[] output = null;

            switch (format)
            {
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt1(data, (int)width, (int)height);
                    break;
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt5(data, (int)width, (int)height);
                    break;
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt4(data, (int)width, (int)height);
                    break;
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                case DDS_DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt4(data, (int)width, (int)height);
                    break;
                //case DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                //case DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                //    output = CSharpImageLibrary.DDS.Dxt.DecompressBc7(data, (int)width, (int)height);
                //    break;
                default:
                    output = DecompressBlock(data, width, height, format);
                        break;

            }

            return output;
        }

        public static Bitmap DecompressCompLibBlock(Byte[] data, int width, int height, DDS_DXGI_FORMAT format, bool GetBitmap)
        {
            return new Bitmap(DecompressBlock(data, width, height, format), (int)width, (int)height);
        }

        public static int Get16(byte[] Data, int Address)
        {
            return
                Data[Address + 0] << 0 |
                Data[Address + 1] << 8;
        }

        public static int Get32(byte[] Data, int Address)
        {
            return
                Data[Address + 0] << 0 |
                Data[Address + 1] << 8 |
                Data[Address + 2] << 16 |
                Data[Address + 3] << 24;
        }

        private static byte Clamp(float Value)
        {
            if (Value > 1)
            {
                return 0xff;
            }
            else if (Value < 0)
            {
                return 0;
            }
            else
            {
                return (byte)(Value * 0xff);
            }
        }

        private static void CalculateBC3Alpha(byte[] Alpha)
        {
            for (int i = 2; i < 8; i++)
            {
                if (Alpha[0] > Alpha[1])
                {
                    Alpha[i] = (byte)(((8 - i) * Alpha[0] + (i - 1) * Alpha[1]) / 7);
                }
                else if (i < 6)
                {
                    Alpha[i] = (byte)(((6 - i) * Alpha[0] + (i - 1) * Alpha[1]) / 7);
                }
                else if (i == 6)
                {
                    Alpha[i] = 0;
                }
                else /* i == 7 */
                {
                    Alpha[i] = 0xff;
                }
            }
        }
        private static void CalculateBC3AlphaS(byte[] Alpha)
        {
            for (int i = 2; i < 8; i++)
            {
                if ((sbyte)Alpha[0] > (sbyte)Alpha[1])
                {
                    Alpha[i] = (byte)(((8 - i) * (sbyte)Alpha[0] + (i - 1) * (sbyte)Alpha[1]) / 7);
                }
                else if (i < 6)
                {
                    Alpha[i] = (byte)(((6 - i) * (sbyte)Alpha[0] + (i - 1) * (sbyte)Alpha[1]) / 7);
                }
                else if (i == 6)
                {
                    Alpha[i] = 0x80;
                }
                else /* i == 7 */
                {
                    Alpha[i] = 0x7f;
                }
            }
        }
    }
}