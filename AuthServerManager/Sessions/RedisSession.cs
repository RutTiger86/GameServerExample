using AuthServerManager.Models;
using log4net;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthServerManager.Sessions
{
    public interface IAuthManagerRedisSession
    {
        Task InitWorldInfoAsync(List<WorldStateInfo> worldStateInfos, bool isForce = false);
        Task SetIsExternallyOpenAsync(bool isExternally);
        Task OpenWorldAsync(int worldId);
        Task CloseWorldAsync(int worldId);
        Task<WorldStateInfo?> GetWorldStateInfoAsync(int worldId);
        Task<List<WorldStateInfo>> GetAllWorldsAsync();

        Task<bool> GetIsExternallyOpenAsync();
    }

    public class RedisSession : IAuthManagerRedisSession
    {
        private readonly IDatabase db;
        private const string ExternallyOpenKey = "auth:externally_open";

        public RedisSession(IConnectionMultiplexer connectionMultiplexer)
        {
            db = connectionMultiplexer.GetDatabase();
        }

        public async Task InitWorldInfoAsync(List<WorldStateInfo> worldStateInfos, bool isForce = false)
        {
            var tasks = worldStateInfos.Select(world =>
            {
                var key = GetWorldKey(world.WorldId);
                var json = JsonSerializer.Serialize(world);
                var condition = isForce ? When.Always : When.NotExists;

                return db.StringSetAsync(key, json, when: condition);
            });

            await Task.WhenAll(tasks);
        }

        public Task SetIsExternallyOpenAsync(bool isExternally)
        {
            return db.StringSetAsync(ExternallyOpenKey, isExternally.ToString().ToLower());
        }

        public async Task<bool> GetIsExternallyOpenAsync()
        {
            var value = await db.StringGetAsync(ExternallyOpenKey);
            return value.HasValue && bool.TryParse(value, out var result) && result;
        }

        public async Task OpenWorldAsync(int worldId)
        {
            var key = GetWorldKey(worldId);
            var world = await GetWorldStateInfoAsync(worldId);
            if (world == null)
                return;

            world.Status = Enums.WorldState.SMOOTH;
            await db.StringSetAsync(key, JsonSerializer.Serialize(world));
            
        }

        public async Task CloseWorldAsync(int worldId)
        {
            var key = GetWorldKey(worldId);
            var world = await GetWorldStateInfoAsync(worldId);
            if (world == null)
                return;

            world.Status = Enums.WorldState.MAINTENANCE;
            await db.StringSetAsync(key, JsonSerializer.Serialize(world));
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
