namespace AuthServer.Models.Configs
{
    public class SecureSetting
    {
        public required string CertPath { get; set; }
        public required string CertPassworld { get; set; }
        public required int PBKDF2Iterations { get; set; }
        public required int HashSize { get; set; }
    }
}
