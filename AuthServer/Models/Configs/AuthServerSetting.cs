namespace AuthServer.Models.Configs
{
    public class AuthServerSetting
    {
        public required string ListenIP { get; set; }
        public int ListenPort { get; set; }
        public int ReceiveBufferSize { get; set; }
        public int Backlog { get; set; }
        public int MaxAcceptCount { get; set; }
        public int MaxConnection { get; set; }
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }
        public int WorkerThreadCount { get; set; }
    }
}
