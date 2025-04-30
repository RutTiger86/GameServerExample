using AuthServerManager.Enums;
using AuthServerManager.Models;
using AuthServerManager.Models.Configs;
using AuthServerManager.Sessions;
using log4net;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServerManager.Services
{
    public class CommandService
    {
        private Dictionary<string, Func<string[], Task>> commandMap = new();
        private IAuthManagerRedisSession redisSession;
        private ConfigManager<AppConfig> configManager;

        private readonly ILog log ;
        public CommandService(ILogFactory logFactory, IAuthManagerRedisSession redisSession, ConfigManager<AppConfig> configManager)
        {
            this.redisSession = redisSession;
            this.configManager = configManager; 
            log = logFactory.CreateLogger<CommandService>();
            Initialize();
        }

        public void Initialize()
        {
            commandMap["help"] = HelpAsync;
            commandMap["init-world"] = InitWorldInfo;
            commandMap["init-world-force"] = InitWorldInfoForce;
            commandMap["externally-open"] = ExternallyOpen;
            commandMap["externally-close"] = ExternallyClose;
            commandMap["world-open"] = WorldOpen;
            commandMap["world-close"] = WorldClose;
            commandMap["world"] = GetWorldInfo;
            commandMap["externally"] = GetExternally;
            // 추가 가능
        }

        public bool Execute(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = args[0].ToLower();

            if (commandMap.TryGetValue(command, out var action))
            {
                action(args);
                return true;
            }

            return false;
        }

        static Task HelpAsync(string[] args)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("- init-world");
            Console.WriteLine("- init-world-force");
            Console.WriteLine("- externally");
            Console.WriteLine("- externally-open");
            Console.WriteLine("- externally-close");
            Console.WriteLine("- world-open {WorldId}");
            Console.WriteLine("- world-close {WorldId}");
            Console.WriteLine("- world {WorldId or Null}");
            Console.WriteLine("- quit");

            return Task.CompletedTask;
        }

        async Task InitWorldInfo(string[] args)
        {
            List<WorldStateInfo> worldStateInfos = new List<WorldStateInfo>();
            foreach (var server in configManager.config!.Servers)
            {
                worldStateInfos.Add(new WorldStateInfo()
                {
                    WorldId = server.ServerId,
                    Name = server.Name,
                    IsVisible = server.Visible,
                    Status = WorldState.PREPARING,
                    LastHeartbeat = DateTime.Now,
                });
            }

            await redisSession.InitWorldInfoAsync(worldStateInfos);
            log.Info($"InitWorldInfo Done!, Count {worldStateInfos.Count}");
        }

        async Task InitWorldInfoForce(string[] args)
        {
            List<WorldStateInfo> worldStateInfos = new List<WorldStateInfo>();
            foreach (var server in configManager.config!.Servers)
            {
                worldStateInfos.Add(new WorldStateInfo()
                {
                    WorldId = server.ServerId,
                    Name = server.Name,
                    IsVisible = server.Visible,
                    Status = WorldState.PREPARING,
                    LastHeartbeat = DateTime.Now,
                });
            }

            await redisSession.InitWorldInfoAsync(worldStateInfos,true);
            log.Info($"InitWorldInfo Force Done!, Count {worldStateInfos.Count}");
        }

        async Task GetExternally(string[] args)
        {
            var IsOpen  = await redisSession.GetIsExternallyOpenAsync();
            log.Info($"Externally Open State is {IsOpen} ");
        }

        async Task ExternallyOpen(string[] args)
        {
            await redisSession.SetIsExternallyOpenAsync(true);
            log.Info($"Externally Open ");
        }
        async Task ExternallyClose(string[] args)
        {
            await redisSession.SetIsExternallyOpenAsync(false);
            log.Info($"Externally Close ");
        }

        async Task WorldOpen(string[] args)
        {
            var worldId = int.Parse(args[1]);

            await redisSession.OpenWorldAsync(worldId);
            log.Info($"World Open , WorldID :  {worldId}");
        }

        async Task WorldClose(string[] args)
        {
            var worldId = int.Parse(args[1]);
            await redisSession.CloseWorldAsync(worldId);

            log.Info($"World Close , WorldID :  {worldId}");
        }

        async Task GetWorldInfo(string[] args)
        {
            log.Info($"GetWorldInfo");
            if (args.Length > 1 && int.TryParse(args[1], out int worldId))
            {
                var worldStateInfo = await redisSession.GetWorldStateInfoAsync(worldId);
                log.Info(worldStateInfo?.GetStringInfo());
            }
            else
            {
                var worldStateInfoList = await redisSession.GetAllWorldsAsync();

                foreach (var worldStateInfo in worldStateInfoList)
                {
                    log.Info(worldStateInfo.GetStringInfo());
                }
            }
        }
    }
}
