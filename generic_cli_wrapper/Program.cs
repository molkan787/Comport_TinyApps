using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generic_cli_wrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"====================== {i + 1} ====================");
                var output = Exec(args[0], args.Skip(1).ToArray());
                Console.WriteLine(output);
                Console.WriteLine("=============================================");
            }
        }

        static string Exec(string filename, string[] args)
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if(err?.Length > 0)
            {
                output += "\n" + err;
            }
            return output;
        }
    }
}
