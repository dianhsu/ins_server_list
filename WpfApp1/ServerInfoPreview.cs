using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class ServerInfoPreview
    {
        public string Name { get; set; }
        public string Map { get; set; }
        public string Players { get; set; }

        public string Host { get; set; }
        public ushort Port { get; set; }
        public long Ping { get; set; }
    }
}
