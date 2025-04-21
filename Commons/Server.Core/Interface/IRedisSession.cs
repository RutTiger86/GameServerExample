using StackExchange.Redis;

namespace Server.Core.Interface
{
    public interface IRedisSession
    {
        public Task RegisterSessionAsync(long sessionId, string ip, int port, int sessionState);
        public Task UpdateSessionLoginInfoAsync(long sessionId, string accountId, long accountDbId, int sessionState);
        public Task UpdateSessionTokenAsync(long sessionId, string token);
        public Task<long> GenerateSessionIdAsync();
        public Task<long?> GetSessionIdByTokenAsync(string token);
        public Task<(long? SessionId, HashEntry[]? Entries)> GetSessionInfoWithIdByTokenAsync(string token);
    }
}
