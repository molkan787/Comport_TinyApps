using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comport_SeedKeyDLL_Client
{
    internal class HexUtils
    {
        public static byte[] Hex(string hexString)
        {
            var isOdd = hexString.Length % 2 == 1;
            var len = Convert.ToInt32(Math.Ceiling(hexString.Length / 2d));
            var data = new byte[len];
            var i = 0;
            var di = 0;
            if (isOdd)
            {
                data[0] = Convert.ToByte(hexString[0].ToString(), 16);
                i++;
                di++;
            }
            while (i < hexString.Length)
            {
                data[di] = Convert.ToByte(hexString.Substring(i, 2), 16);
                i += 2;
                di += 1;
            }
            return data;
        }
        public static string ToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }
    }
}
