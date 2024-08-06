using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class CLI
    {
        public static void Run(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments; Expected [<DLL-FILENAME>, <HEX-ENCODED-SEED>, <SECURITY-LEVEL>]");
                Environment.Exit(1);
            }
            var _args = args;
            var dllFilename = _args[0];
            var seed = HexUtils.Hex(_args[1]);
            var securityLevel = int.Parse(_args[2]);
            var ddw = new DynamicDLLWrapper();
            ddw.LoadDLL(dllFilename);
            var key = ddw.GenerateKey(seed, securityLevel);
            Console.WriteLine($"Output_key: {HexUtils.ToHexString(key)}");
        }
    }
}
