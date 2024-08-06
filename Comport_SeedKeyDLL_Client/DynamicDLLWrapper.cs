using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class DynamicDLLWrapper
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GenerateKeyExOptDelegate(IntPtr seed, int seedLength, int securityLevel, IntPtr paramByteBuffer2, IntPtr paramByteBuffer3, IntPtr outputBuffer, int outputBufferSize, ref int resultKeyLength);
        private GenerateKeyExOptDelegate GenerateKeyExOpt;
        private IntPtr DLLHandle = IntPtr.Zero;

        public byte[] GenerateKey(byte[] seed, int securityLevel)
        {
            byte[] key = null;
            var seedPtr = Marshal.AllocHGlobal(sizeof(byte) * seed.Length);
            Marshal.Copy(seed, 0, seedPtr, seed.Length);
            var outputPtr = Marshal.AllocHGlobal(sizeof(byte) * seed.Length);
            int resultKeyLength = 0;
            var result = GenerateKeyExOpt.Invoke(seedPtr, seed.Length, securityLevel, IntPtr.Zero, IntPtr.Zero, outputPtr, seed.Length, ref resultKeyLength);
            if (result == 0)
            {
                key = new byte[resultKeyLength];
                Marshal.Copy(outputPtr, key, 0, resultKeyLength);
            }
            Marshal.FreeHGlobal(seedPtr);
            Marshal.FreeHGlobal(outputPtr);
            return key;
        }

        public void LoadDLL(string fileName)
        {
            DLLHandle = Kernel32.LoadLibrary(fileName);
            if (DLLHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Exception(string.Format("Failed to load library (ErrorCode: {0})", errorCode));
            }

            IntPtr funcaddr = Kernel32.GetProcAddress(DLLHandle, "GenerateKeyExOpt");
            GenerateKeyExOpt = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(GenerateKeyExOptDelegate)) as GenerateKeyExOptDelegate;
        }

        public void Unload()
        {
            if (DLLHandle != IntPtr.Zero)
            {
                Kernel32.FreeLibrary(DLLHandle);
            }
        }
    
    }
}
