using System.IO;
using System;
using System.Runtime.InteropServices;
using Syroot.BinaryData;

namespace SZS
{
    public unsafe class YAZ0
    {
        [DllImport("nativelib/yaz0.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern byte* decompress(byte* src, uint src_len, uint* dest_len);

        [DllImport("nativelib/yaz0.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern byte* compress(byte* src, uint src_len, uint* dest_len, byte opt_compr);

        [DllImport("nativelib/yaz0.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void freePtr(void* ptr);

        public static unsafe byte[] Decompress(string fileName) =>
            Decompress(File.ReadAllBytes(fileName));
        public static unsafe byte[] Decompress(byte[] data)
        {
            uint src_len = (uint)data.Length;

            uint dest_len;
            fixed (byte* inputPtr = data)
            {
                byte* outputPtr = decompress(inputPtr, src_len, &dest_len);
                byte[] decomp = new byte[dest_len];
                Marshal.Copy((IntPtr)outputPtr, decomp, 0, (int)dest_len);
                freePtr(outputPtr);
                return decomp;
            }
        }

        public static byte[] Compress(Tuple<int, byte[]> sarc)
        {
            MemoryStream stream = new MemoryStream();
            Compress(stream, sarc);
            return stream.ToArray();
        }

        public static void Compress(Stream stream, Tuple<int, byte[]> sarc)
        {
            //  return new MemoryStream(EveryFileExplorer.YAZ0.Compress(
            //      stream.ToArray(), Runtime.Yaz0CompressionLevel, (uint)Alignment));

            using (var writer = new BinaryDataWriter(stream, false))
            {
                writer.ByteOrder = ByteOrder.BigEndian;
                writer.Write("Yaz0", BinaryStringFormat.NoPrefixOrTermination);
                writer.Write((uint)sarc.Item2.Length);
                writer.Write((uint)sarc.Item1);
                writer.Write(0);
                writer.Write(Compress(sarc.Item2, 3));
            }
        }

        private static unsafe byte[] Compress(byte[] src, byte opt_compr)
        {
            uint src_len = (uint)src.Length;

            uint dest_len;
            fixed (byte* inputPtr = src)
            {
                byte* outputPtr = compress(inputPtr, src_len, &dest_len, opt_compr);
                byte[] comp = new byte[dest_len];
                Marshal.Copy((IntPtr)outputPtr, comp, 0, (int)dest_len);
                freePtr(outputPtr);
                return comp;
            }
        }


        ////Compression could be optimized by using look-ahead.
        //public static unsafe byte[] Compress(string FileName, int level = 3, UInt32 res1 = 0, UInt32 res2 = 0) => Compress(File.ReadAllBytes(FileName),level,res1,res2);
        //public static unsafe byte[] Compress(byte[] Data, int level = 3, UInt32 reserved1 = 0, UInt32 reserved2 = 0)
        //{
        //    int maxBackLevel = (int)(0x10e0 * (level / 9.0) - 0x0e0);

        //    byte* dataptr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);

        //    byte[] result = new byte[Data.Length + Data.Length / 8 + 0x10];
        //    byte* resultptr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(result, 0);
        //    *resultptr++ = (byte)'Y';
        //    *resultptr++ = (byte)'a';
        //    *resultptr++ = (byte)'z';
        //    *resultptr++ = (byte)'0';
        //    *resultptr++ = (byte)((Data.Length >> 24) & 0xFF);
        //    *resultptr++ = (byte)((Data.Length >> 16) & 0xFF);
        //    *resultptr++ = (byte)((Data.Length >> 8) & 0xFF);
        //    *resultptr++ = (byte)((Data.Length >> 0) & 0xFF);
        //    {
        //        var res1 = BitConverter.GetBytes(reserved1);
        //        var res2 = BitConverter.GetBytes(reserved2);
        //        if (BitConverter.IsLittleEndian)
        //        {
        //            Array.Reverse(res1);
        //            Array.Reverse(res2);
        //        }
        //        *resultptr++ = (byte)res1[0];
        //        *resultptr++ = (byte)res1[1];
        //        *resultptr++ = (byte)res1[2];
        //        *resultptr++ = (byte)res1[3];
        //        *resultptr++ = (byte)res2[0];
        //        *resultptr++ = (byte)res2[1];
        //        *resultptr++ = (byte)res2[2];
        //        *resultptr++ = (byte)res2[3];
        //    }
        //    int length = Data.Length;
        //    int dstoffs = 16;
        //    int Offs = 0;
        //    while (true)
        //    {
        //        int headeroffs = dstoffs++;
        //        resultptr++;
        //        byte header = 0;
        //        for (int i = 0; i < 8; i++)
        //        {
        //            int comp = 0;
        //            int back = 1;
        //            int nr = 2;
        //            {
        //                byte* ptr = dataptr - 1;
        //                int maxnum = 0x111;
        //                if (length - Offs < maxnum) maxnum = length - Offs;
        //                //Use a smaller amount of bytes back to decrease time
        //                int maxback = maxBackLevel;//0x1000;
        //                if (Offs < maxback) maxback = Offs;
        //                maxback = (int)dataptr - maxback;
        //                int tmpnr;
        //                while (maxback <= (int)ptr)
        //                {
        //                    if (*(ushort*)ptr == *(ushort*)dataptr && ptr[2] == dataptr[2])
        //                    {
        //                        tmpnr = 3;
        //                        while (tmpnr < maxnum && ptr[tmpnr] == dataptr[tmpnr]) tmpnr++;
        //                        if (tmpnr > nr)
        //                        {
        //                            if (Offs + tmpnr > length)
        //                            {
        //                                nr = length - Offs;
        //                                back = (int)(dataptr - ptr);
        //                                break;
        //                            }
        //                            nr = tmpnr;
        //                            back = (int)(dataptr - ptr);
        //                            if (nr == maxnum) break;
        //                        }
        //                    }
        //                    --ptr;
        //                }
        //            }
        //            if (nr > 2)
        //            {
        //                Offs += nr;
        //                dataptr += nr;
        //                if (nr >= 0x12)
        //                {
        //                    *resultptr++ = (byte)(((back - 1) >> 8) & 0xF);
        //                    *resultptr++ = (byte)((back - 1) & 0xFF);
        //                    *resultptr++ = (byte)((nr - 0x12) & 0xFF);
        //                    dstoffs += 3;
        //                }
        //                else
        //                {
        //                    *resultptr++ = (byte)((((back - 1) >> 8) & 0xF) | (((nr - 2) & 0xF) << 4));
        //                    *resultptr++ = (byte)((back - 1) & 0xFF);
        //                    dstoffs += 2;
        //                }
        //                comp = 1;
        //            }
        //            else
        //            {
        //                *resultptr++ = *dataptr++;
        //                dstoffs++;
        //                Offs++;
        //            }
        //            header = (byte)((header << 1) | ((comp == 1) ? 0 : 1));
        //            if (Offs >= length)
        //            {
        //                header = (byte)(header << (7 - i));
        //                break;
        //            }
        //        }
        //        result[headeroffs] = header;
        //        if (Offs >= length) break;
        //    }
        //    while ((dstoffs % 4) != 0) dstoffs++;
        //    byte[] realresult = new byte[dstoffs];
        //    Array.Copy(result, realresult, dstoffs);
        //    return realresult;
        //}

        //public static byte[] Decompress(string Filename) => Decompress(File.ReadAllBytes(Filename)); 
        //public static byte[] Decompress(byte[] Data)
        //{
        //    UInt32 leng = (uint)(Data[4] << 24 | Data[5] << 16 | Data[6] << 8 | Data[7]);
        //    byte[] Result = new byte[leng];
        //    int Offs = 16;
        //    int dstoffs = 0;
        //    while (true)
        //    {
        //        byte header = Data[Offs++];
        //        for (int i = 0; i < 8; i++)
        //        {
        //            if ((header & 0x80) != 0) Result[dstoffs++] = Data[Offs++];
        //            else
        //            {
        //                byte b = Data[Offs++];
        //                int offs = ((b & 0xF) << 8 | Data[Offs++]) + 1;
        //                int length = (b >> 4) + 2;
        //                if (length == 2) length = Data[Offs++] + 0x12;
        //                for (int j = 0; j < length; j++)
        //                {
        //                    Result[dstoffs] = Result[dstoffs - offs];
        //                    dstoffs++;
        //                }
        //            }
        //            if (dstoffs >= leng) return Result;
        //            header <<= 1;
        //        }
        //    }
        //}
    }
}