using AuthServer.Models.Configs;
using Server.Data.WorldAuth;
using Server.Utill;
using Server.Utill.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Session
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
            new("status", (int)SessionState.Connected)
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
            var key = $"session:{sessionId}";
            var data = new HashEntry[]
            {
                new("login_token", token),
                new("status", (int)SessionState.WaitingEntry)
            };

            await db.HashSetAsync(key, data);
            // TODO : 필요시 토큰 기준 TTL 재설정
        }

        public async Task<long> GenerateSessionIdAsync()
        {
            return await db.StringIncrementAsync("session_id_gen");
        }
    }

}
