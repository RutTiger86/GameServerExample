using AuthServer.Models;
using AuthServer.Models.Configs;
using log4net;
using Server.Data.ClientAuth;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public interface IGameServerRegistry
    {
        public void RegisterServer(GameServerInfo info);
        public void UpdateHeartbeat(int serverId, ServerStatus status);

        public List<GameServerInfo> GetAllServerList(bool includeInvisible = false);
    }

    public class GameServerRegistry : IGameServerRegistry
    {
        private readonly Dictionary<int, GameServerInfo> servers = new();

        private readonly ILog log;

        private readonly ConfigManager<AppConfig> configManager;

        public GameServerRegistry(ILogFactory logFactory, ConfigManager<AppConfig> configManager)
        {
            this.log = logFactory.CreateLogger<GameServerRegistry>();
            this.configManager = configManager;
            RegisterInitialServer();

        }

        private void RegisterInitialServer()
        {
            foreach(var server in configManager.config!.Servers)
            {
                RegisterServer(new GameServerInfo()
                {
                    ServerId = server.ServerId,
                    Name = server.Name,
                    IsVisible = server.Visible,
                    Status = ServerStatus.Preparing,
                    LastHeartbeat = DateTime.Now,
                });
            }
        }

        public void RegisterServer(GameServerInfo info)
        {
            servers[info.ServerId] = info;
        }

        public void UpdateHeartbeat(int serverId, ServerStatus status)
        {
            if (servers.TryGetValue(serverId, out var info))
            {
                info.Status = status;
                info.LastHeartbeat = DateTime.UtcNow;
            }
        }

        public List<GameServerInfo> GetAllServerList(bool includeInvisible = false)
        {
            // 일정 시간 내 heartbeat 없으면 OFFLINE 처리
            foreach (var info in servers.Values)
            {
                if ((DateTime.UtcNow - info.LastHeartbeat).TotalSeconds > 60)
                {
                    info.Status = ServerStatus.Preparing;
                }
            }

            if (includeInvisible)
                return servers.Values.ToList();
            else
                return servers.Values.Where(s => s.IsVisible).ToList();
        }
    }

}
