using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "start-server")
            {
                Server.Run(args);
            }
            else
            {
                CLI.Run(args);
            }
        }

    }
}
