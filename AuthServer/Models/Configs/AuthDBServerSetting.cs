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
