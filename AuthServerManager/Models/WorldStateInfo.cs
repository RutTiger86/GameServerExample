using AuthServerManager.Enums;

namespace AuthServerManager.Models
{
    public class WorldStateInfo
    {
        public int WorldId { get; set; }
        public string Name { get; set; } = string.Empty;
        public WorldState Status { get; set; }
        public bool IsVisible { get; set; } = true;
        public DateTime LastHeartbeat { get; set; } = DateTime.MinValue;

        public string GetStringInfo()
        {
            return $"WorldId : {WorldId},   Name : {Name},  Status {Status},    IsVisible {IsVisible}";
        }
    }
}
