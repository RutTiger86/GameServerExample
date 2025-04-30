namespace AuthServerManager.Models.Configs
{
    public class AppConfig
    {
        public RedisSetting? Redis { get; set; }
        public List<InitialServerInfo> Servers { get; set; } = new();
    }
}
