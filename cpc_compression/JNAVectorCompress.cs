using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cpc_compression
{
    internal class JNAVectorCompress
    {
        private const string DLL_FILENAME = "CompressAlgo1.dll";

        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VectorCompressInit();

        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int VectorCompress(IntPtr input, int inputLength, IntPtr outputBuffer, ref int outputLength);

        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void VectorCompressExit();

        public static byte[] Compress(byte[] data)
        {
            VectorCompressInit();
            byte[] output = null;
            var bufsSize = sizeof(byte) * data.Length;
            var inputPtr = Marshal.AllocHGlobal(bufsSize);
            var outputPtr = Marshal.AllocHGlobal(bufsSize * 30);
            var outputLength = 0;
            Marshal.Copy(data, outputLength, inputPtr, data.Length);
            var length = VectorCompress(inputPtr, data.Length, outputPtr, ref outputLength);
            if (length > 0)
            {
                output = new byte[length];
                Marshal.Copy(outputPtr, output, 0, length);
            }
            Marshal.FreeHGlobal(inputPtr);
            Marshal.FreeHGlobal(outputPtr);
            VectorCompressExit();
            return output;
        }
    }
}
