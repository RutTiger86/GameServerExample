namespace AuthServer.Models.Account
{
    public class LoginInfo
    {
        public required string AccountId { get; set; }
        public string? Password { get; set; }
    }
}
