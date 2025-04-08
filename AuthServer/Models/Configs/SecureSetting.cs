using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Models.Configs
{
    public class SecureSetting
    {
        public required string CertPath { get; set; }
        public required string CertPassworld { get; set; }
    }
}
