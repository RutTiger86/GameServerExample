namespace WorldServer.Models.Configs
{
    public class WorldDBServerSetting
    {
        public required string ConnectIP { get; set; }
        public int ConnectPort { get; set; }
        public int ReceiveBufferSize { get; set; }
        public int Timeout { get; set; }
    }
}
