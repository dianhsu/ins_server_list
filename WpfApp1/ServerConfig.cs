using System;
using System.Collections.Generic;

namespace WpfApp1
{
    public class ServerConfig
    {
        public static string xiaocao = "43.249.192.191";
        public static List<Tuple<string, ushort>> other = new List<Tuple<string, ushort>>()
        {
            new Tuple<string, ushort>("192.168.50.2",27015),
            new Tuple<string, ushort>("134.175.62.172",27131),
            new Tuple<string, ushort>("119.188.247.66",27801),
        };
        public static List<Tuple<string, ushort>> xiaocao_list = new List<Tuple<string, ushort>>()
        {
 
        };
    }
}
