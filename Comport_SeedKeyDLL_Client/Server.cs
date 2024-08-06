using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class Server
    {
        public static void Run(string[] args)
        {
            int port = int.Parse(args[1]);
            dlls_lib_folder = args[2];
            HttpServer.Start(port, HandleRequest);
        }

        private static async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse res)
        {
            var dllName = req.QueryString["dllName"];
            var str_seed = req.QueryString["seed"];
            var str_securityAccessLevel = req.QueryString["securityAccessLevel"];

            var seed = HexUtils.Hex(str_seed);
            var securityAccessLevel = int.Parse(str_securityAccessLevel);

            var key = GenerateKey(dllName, seed, securityAccessLevel);

            byte[] data = Encoding.UTF8.GetBytes($"{{\"GeneratedKey\":\"{HexUtils.ToHexString(key)}\"}}");
            res.ContentType = "application/json";
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = data.LongLength;

            await res.OutputStream.WriteAsync(data, 0, data.Length);
            res.Close();
        }

        private static string dlls_lib_folder = "";
        private static byte[] GenerateKey(string dllName, byte[] seed, int level)
        {
            var dllFilename = $"{Path.Combine(dlls_lib_folder, dllName)}.dll";
            var ddw = new DynamicDLLWrapper();
            ddw.LoadDLL(dllFilename);
            var key = ddw.GenerateKey(seed, level);
            ddw.Unload();
            return key;
        }
    }
}
