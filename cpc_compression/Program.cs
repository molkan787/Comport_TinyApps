using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace cpc_compression
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"CPC_Compression v{version}");
            var input = File.ReadAllBytes(args[0]);
            var compressed = JNAVectorCompress.Compress(input);
            File.WriteAllBytes(args[1], compressed);
            Console.WriteLine("File Successfully compressed.");
        }
    }
}
