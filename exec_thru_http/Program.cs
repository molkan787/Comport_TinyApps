using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace exec_thru_http
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var port = int.Parse(args[0]);
            HttpServer.Start(port, HandleRequest);
        }

        static async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse res)
        {
            var program = req.QueryString.Get("program");
            var args = req.QueryString.Get("args").Split(',').ToArray();
            var output = await Task.Run(() => Exec(program, args));
            var data = Encoding.UTF8.GetBytes(output);
            res.ContentLength64 = data.Length;
            res.ContentEncoding = Encoding.UTF8;
            res.ContentType = "text/plain";
            await res.OutputStream.WriteAsync(data, 0, data.Length);
            res.Close();
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
            if (err?.Length > 0)
            {
                output += "\n" + err;
            }
            return output;
        }
    }
}
