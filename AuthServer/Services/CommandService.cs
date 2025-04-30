using AuthServer.Session;
using log4net;
using Server.Utill;

namespace AuthServer.Services
{
    public class CommandService
    {
        private Dictionary<string, Func<string[], Task>> commandMap = new();

        private readonly ILog log;
        private IAuthRedisSession redisSession;

        public CommandService(ILogFactory logFactory, IAuthRedisSession redisSession )
        {
            this.redisSession = redisSession;
            log = logFactory.CreateLogger<CommandService>();
            Initialize();
        }

        public void Initialize()
        {
            commandMap["help"] = HelpAsync;
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
            Console.WriteLine("- externally");
            Console.WriteLine("- world {WorldId or Null}");
            Console.WriteLine("- quit");

            return Task.CompletedTask;
        }

        async Task GetExternally(string[] args)
        {
            var IsOpen = await redisSession.GetIsExternallyOpenAsync();
            log.Info($"Externally Open State is {IsOpen} ");
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
