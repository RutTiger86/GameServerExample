using Server.Data.WorldAuth;
using Server.Utill;
using Server.Utill.Interface;
using StackExchange.Redis;
using WorldServer.Models.Configs;

namespace WorldServer.Session
{

    public class RedisSession : IRedisSession
    {
        private readonly IDatabase db;

        public RedisSession(ConfigManager<AppConfig> configManager)
        {
            var redis = ConnectionMultiplexer.Connect(configManager.config!.Redis!.GetConnectionString());
            db = redis.GetDatabase();
        }

        public async Task RegisterSessionAsync(long sessionId, string ip, int port)
        {
            var key = $"session:{sessionId}";
            var data = new HashEntry[]
            {
            new("ip", ip),
            new("port", port.ToString()),
            new("connect_time", DateTime.UtcNow.ToString("o")),
            new("status", (int)SessionState.InWorld)
            };

            await db.HashSetAsync(key, data);
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(30)); // TTL 설정
        }

        public async Task UpdateSessionLoginInfoAsync(long sessionId, string accountId, long accountDbId)
        {
            var key = $"session:{sessionId}";
            var data = new HashEntry[]
            {
                new("account_id", accountId),
                new("account_db_id", accountDbId.ToString()),
                new("status", (int)SessionState.Authenticated)
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

        public async Task<HashEntry[]?> GetSessionInfoByTokenAsync(string token)
        {
            var sessionId = await GetSessionIdByTokenAsync(token);
            if (sessionId == null) return null;

            var key = $"session:{sessionId}";
            return await db.HashGetAllAsync(key);
        }

        public async Task<long> GenerateSessionIdAsync()
        {
            return await db.StringIncrementAsync("session_id_gen");
        }
    }
}
