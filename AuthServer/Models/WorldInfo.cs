using Server.Data.ClientAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class WorldStateInfo
    {
        public int WorldId { get; set; }
        public string Name { get; set; } = string.Empty;
        public WorldState Status { get; set; }
        public bool IsVisible { get; set; } = true;
        public DateTime LastHeartbeat { get; set; } = DateTime.MinValue;
    }
}
