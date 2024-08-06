using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class HttpServer
    {
        public static HttpListener listener;


        public static async Task HandleIncomingConnections(Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                var timeText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss:fff", CultureInfo.InvariantCulture);
                Console.WriteLine($"[{timeText}]: {req.HttpMethod} {req.Url.ToString()}");

                try
                {
                    await handler(req, resp);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("An error occured in request handler:");
                    Console.WriteLine(ex.ToString());

                    byte[] data = Encoding.UTF8.GetBytes("Internal Server Error");
                    resp.ContentType = "text/plain";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.StatusCode = 500;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
            }
        }


        public static void Start(int port, Func<HttpListenerRequest, HttpListenerResponse, Task> requestHandler)
        {
            // Create a Http server and start listening for incoming connections
            var url = $"http://localhost:{port.ToString()}/";
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections(requestHandler);
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}
