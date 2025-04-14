using AuthServer.Models;
using AuthServer.Models.Configs;
using log4net;
using Server.Data.ClientAuth;
using Server.Utill;

namespace AuthServer.Services
{
    public interface IWorldServerRegistry
    {
        public bool IsExternallyOpen { get; }
        public void SetIsExternallyOpen(bool isExternally);
        public void OpenWorld(int worldId);
        public void CloseWorld(int worldId);
        public void RegisterServer(WorldStateInfo info);
        public void UpdateHeartbeat(int worldId, WorldState status);
        public List<WorldStateInfo> GetAllServerList(bool includeInvisible = false);
        WorldStateInfo GetServer(int worldId);
    }

    public class WorldServerRegistry : IWorldServerRegistry
    {
        private readonly Dictionary<int, WorldStateInfo> servers = new();

        private readonly ILog log;

        private readonly ConfigManager<AppConfig> configManager;

        public bool isExternallyOpen = false;
        public bool IsExternallyOpen { get { return isExternallyOpen; } }

        public WorldServerRegistry(ILogFactory logFactory, ConfigManager<AppConfig> configManager)
        {
            this.log = logFactory.CreateLogger<WorldServerRegistry>();
            this.configManager = configManager;
            RegisterInitialServer();

        }
        public void SetIsExternallyOpen(bool isExternally)
        {
            isExternallyOpen = isExternally;
            log.Info($"Set Externally : {isExternally}");
        }

        private void RegisterInitialServer()
        {
            foreach (var server in configManager.config!.Servers)
            {
                RegisterServer(new WorldStateInfo()
                {
                    WorldId = server.ServerId,
                    Name = server.Name,
                    IsVisible = server.Visible,
                    Status = WorldState.Preparing,
                    LastHeartbeat = DateTime.Now,
                });
            }
        }

        public void RegisterServer(WorldStateInfo info)
        {
            servers[info.WorldId] = info;
        }

        public void UpdateHeartbeat(int worldId, WorldState status)
        {
            if (servers.TryGetValue(worldId, out var info))
            {
                info.Status = status;
                info.LastHeartbeat = DateTime.UtcNow;
            }
        }

        public List<WorldStateInfo> GetAllServerList(bool includeInvisible = false)
        {
            // 일정 시간 내 heartbeat 없으면 OFFLINE 처리
            foreach (var info in servers.Values)
            {
                if ((DateTime.UtcNow - info.LastHeartbeat).TotalSeconds > 60)
                {
                    info.Status = WorldState.Preparing;
                }
            }

            if (includeInvisible)
                return servers.Values.ToList();
            else
                return servers.Values.Where(s => s.IsVisible).ToList();
        }

        public WorldStateInfo GetServer(int worldId)
        {
            // 일정 시간 내 heartbeat 없으면 OFFLINE 처리
            foreach (var info in servers.Values)
            {
                if ((DateTime.UtcNow - info.LastHeartbeat).TotalSeconds > 60)
                {
                    info.Status = WorldState.Preparing;
                }
            }

            return servers.Where(p => p.Key == worldId).FirstOrDefault().Value;
        }

        public void OpenWorld(int worldId)
        {
            var worldInfo = servers.Where(p => p.Key == worldId).FirstOrDefault().Value;

            if (worldInfo != null)
            {
                worldInfo.Status = WorldState.Smooth;

                log.Info($"[OpenWorld] WorldID : {worldInfo.WorldId} ,  Status : {worldInfo.Status}");
            }
            else
            {
                log.Info($"[OpenWorld] Is Not Exist World! WorldID : {worldId}");
            }

        }
        public void CloseWorld(int worldId)
        {
            var worldInfo = servers.Where(p => p.Key == worldId).FirstOrDefault().Value;

            if (worldInfo != null)
            {
                worldInfo.Status = WorldState.Maintenance;

                log.Info($"[OpenWorld] WorldID : {worldInfo.WorldId} ,  Status : {worldInfo.Status}");
            }
            else
            {
                log.Info($"[OpenWorld] Is Not Exist World! WorldID : {worldId}");
            }
        }
    }

}
