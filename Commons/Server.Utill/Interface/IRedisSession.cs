namespace Server.Utill.Interface
{
    public interface IRedisSession
    {
        public Task RegisterSessionAsync(long sessionId, string ip, int port);
        public Task UpdateSessionLoginInfoAsync(long sessionId, string accountId, long accountDbId);
        public Task UpdateSessionTokenAsync(long sessionId, string token);
        public Task<long> GenerateSessionIdAsync();
    }
}
