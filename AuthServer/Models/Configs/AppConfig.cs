using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Models.Configs
{
    public class AppConfig 
    {
        public AuthServerSetting? AuthServer { get; set; }
        public AuthDBServerSetting? AuthDBServer { get; set; }
        public SecureSetting? Secure { get; set; }
    }
}
