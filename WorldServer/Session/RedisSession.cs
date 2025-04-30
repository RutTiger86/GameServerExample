using Server.Core.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace WorldServer.Session
{
    public interface IWorldRedisSession
    {
        public Task<long?> GetSessionIdByTokenAsync(string token);
        public Task<(long? SessionId, HashEntry[]? Entries)> GetSessionInfoWithIdByTokenAsync(string token);
    }

    public class RedisSession : IWorldRedisSession
    {
        private readonly IDatabase db;

        public RedisSession(IConnectionMultiplexer connectionMultiplexer)
        {
            db = connectionMultiplexer.GetDatabase();
        }

        public async Task<long?> GetSessionIdByTokenAsync(string token)
        {
            var key = $"login_token:{token}";
            var value = await db.StringGetAsync(key);
            return value.HasValue ? (long?)value : null;
        }

        public async Task<(long? SessionId, HashEntry[]? Entries)> GetSessionInfoWithIdByTokenAsync(string token)
        {
            var sessionId = await GetSessionIdByTokenAsync(token);
            if (sessionId == null)
                return (null, null);

            var key = $"session:{sessionId}";
            var entries = await db.HashGetAllAsync(key);

            return (sessionId, entries);
        }
    }
}
