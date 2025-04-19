namespace WorldServer.Models.Configs
{
    public class AppConfig
    {
        public long WorldId { get; set; }
        public WorldServerSetting? WorldServer { get; set; }
        public WorldDBServerSetting? WorldDBServer { get; set; }
        public RedisSetting? Redis { get; set; }
    }
}
