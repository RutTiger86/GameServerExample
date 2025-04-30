using StackExchange.Redis;

namespace Server.Core.Interface
{
    public interface IRedisSession
    {
        public Task RegisterSessionAsync(long sessionId, string ip, int port, int sessionState);
        public Task<long> GenerateSessionIdAsync();
    }
}
