using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Models.Configs
{
    public class AuthDBServerSetting
    {
        public required string ConnectIP { get; set; }
        public int ConnectPort { get; set; }
        public int ReceiveBufferSize { get; set; }
        public int Timeout { get; set; }
    }
}
