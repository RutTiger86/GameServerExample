namespace AuthServerManager.Models.Configs
{
    public class RedisSetting
    {
        public required string ConnectIP { get; set; }
        public required int ConnectPort { get; set; }
        public required string Passworld { get; set; }

        public string GetConnectionString()
        {
            return $"{ConnectIP}:{ConnectPort},password={Passworld}";

        }
    }
   
}
