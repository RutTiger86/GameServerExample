namespace AuthServer.Models.Configs
{
    public class AppConfig
    {
        public AuthServerSetting? AuthServer { get; set; }
        public AuthDBServerSetting? AuthDBServer { get; set; }
        public SecureSetting? Secure { get; set; }
        public RedisSetting? Redis { get; set; }
        public List<InitialServerInfo> Servers { get; set; } = new();
    }
}
