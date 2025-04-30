using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServerManager.Models.Configs
{
    public class InitialServerInfo
    {
        public int ServerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
    }
}
