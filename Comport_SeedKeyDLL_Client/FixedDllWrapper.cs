using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class FixedDllWrapper
    {
        private const string dllName = "C:\\Users\\Seghier\\Documents\\Comport\\MED41_MED41_15_20_11_4141152011.dll";

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GenerateKeyExOpt(IntPtr seed, int seedLength, int securityLevel, IntPtr paramByteBuffer2, IntPtr paramByteBuffer3, IntPtr outputBuffer, int outputBufferSize, ref int resultKeyLength);

        public static byte[] GenerateKeyExOpt(byte[] seed, int securityLevel)
        {
            byte[] key = null;
            var seedPtr = Marshal.AllocHGlobal(sizeof(byte) * seed.Length);
            Marshal.Copy(seed, 0, seedPtr, seed.Length);
            var outputPtr = Marshal.AllocHGlobal(sizeof(byte) * seed.Length);
            int resultKeyLength = 0;
            var result = GenerateKeyExOpt(seedPtr, seed.Length, securityLevel, IntPtr.Zero, IntPtr.Zero, outputPtr, seed.Length, ref resultKeyLength);
            if (result == 0)
            {
                key = new byte[resultKeyLength];
                Marshal.Copy(outputPtr, key, 0, resultKeyLength);
            }
            Marshal.FreeHGlobal(seedPtr);
            Marshal.FreeHGlobal(outputPtr);
            return key;
        }

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetKeyLength();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetECUName();

        public static string ManagedGetECUName()
        {
            var ptr = GetECUName();
            return Marshal.PtrToStringAnsi(ptr);
        }

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetConfiguredAccessTypes(int[] buffer);

        public static int[] GetConfiguredAccessTypes()
        {
            var buffer = new int[16];
            var count = GetConfiguredAccessTypes(buffer);
            return buffer.Take(count).ToArray();
        }

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSeedLength(int securityLevel);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetKeyLength(int securityLevel);


        public static void DumpInfo()
        {
            Console.WriteLine($"DllName: {dllName}");
            var ecuName = ManagedGetECUName();
            Console.WriteLine($"ECU Name: {ecuName}");
            var accessTypes = GetConfiguredAccessTypes();
            Console.WriteLine($"Configured Access Types: {string.Join(",", accessTypes)}");
            foreach (int secLevel in accessTypes)
            {
                var seedLength = GetSeedLength(secLevel);
                var keyLength = GetKeyLength(secLevel);
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($"Access Type {secLevel}");
                Console.WriteLine($"Seed Length: {seedLength}, Key Length: {keyLength}");
                var testSeed = new byte[seedLength].Select((b, i) => (byte)i).ToArray();
                var generatedKey = GenerateKeyExOpt(testSeed, secLevel);
                Console.WriteLine($"Test Seed-Key: {HexUtils.ToHexString(testSeed)} -> {HexUtils.ToHexString(generatedKey)}");
                Console.WriteLine("---------------------------------------------------");
            }
        }
    }
}
