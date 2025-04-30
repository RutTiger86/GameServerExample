using AuthServer.Models;
using Server.Core.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthServer.Session
{
    public interface IAuthRedisSession
    {
        public Task UpdateSessionLoginInfoAsync(long sessionId, string accountId, long accountDbId, int sessionState);
        public Task UpdateSessionTokenAsync(long sessionId, string token);

        public Task<WorldStateInfo?> GetWorldStateInfoAsync(int worldId);

        public Task<List<WorldStateInfo>> GetAllWorldsAsync();
        Task<bool> GetIsExternallyOpenAsync();
    }

    public class RedisSession : IRedisSession, IAuthRedisSession
    {
        private readonly IDatabase db;
        private const string ExternallyOpenKey = "auth:externally_open";

        public RedisSession(IConnectionMultiplexer connectionMultiplexer)
        {
            db = connectionMultiplexer.GetDatabase();
        }
        public async Task<bool> GetIsExternallyOpenAsync()
        {
            var value = await db.StringGetAsync(ExternallyOpenKey);
            return value.HasValue && bool.TryParse(value, out var result) && result;
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

        private string GetWorldKey(int worldId) => $"world:{worldId}";

        public async Task<WorldStateInfo?> GetWorldStateInfoAsync(int worldId)
        {
            var key = GetWorldKey(worldId);
            var json = await db.StringGetAsync(key);
            if (json.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<WorldStateInfo>(json!);
        }

        public async Task<List<WorldStateInfo>> GetAllWorldsAsync()
        {
            var server = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: "world:*").ToArray();

            var result = new List<WorldStateInfo>();

            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (!json.IsNullOrEmpty)
                {
                    var world = JsonSerializer.Deserialize<WorldStateInfo>(json!);
                    if (world != null)
                        result.Add(world);
                }
            }

            return result;
        }
    }
}
