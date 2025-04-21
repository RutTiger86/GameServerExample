using Server.Core.Interface;
using StackExchange.Redis;

namespace Server.Core
{

    public class RedisSession : IRedisSession
    {
        private readonly IDatabase db;

        public RedisSession(IConnectionMultiplexer connectionMultiplexer)
        {
            db = connectionMultiplexer.GetDatabase();
        }

        public async Task RegisterSessionAsync(long sessionId, string ip, int port, int sessionState)
        {
            var key = $"session:{sessionId}";
            var data = new HashEntry[]
            {
            new("ip", ip),
            new("port", port.ToString()),
            new("connect_time", DateTime.UtcNow.ToString("o")),
            new("status", sessionState)
            };

            await db.HashSetAsync(key, data);
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(30)); // TTL 설정
        }

        public async Task UpdateSessionLoginInfoAsync(long sessionId, string accountId, long accountDbId, int sessionState)
        {
            var key = $"session:{sessionId}";
            var data = new HashEntry[]
            {
                new("account_id", accountId),
                new("account_db_id", accountDbId.ToString()),
                new("status", sessionState)
            };

            await db.HashSetAsync(key, data);
        }

        public async Task UpdateSessionTokenAsync(long sessionId, string token)
        {
            var key = $"login_token:{token}";
            await db.StringSetAsync(key, sessionId, TimeSpan.FromMinutes(30)); // TTL 설정
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

        public async Task<long> GenerateSessionIdAsync()
        {
            return await db.StringIncrementAsync("session_id_gen");
        }
    }
}
