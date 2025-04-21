using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Core;
using Server.Core.Interface;
using Server.Utill;
using StackExchange.Redis;
using System.Reflection;
using WorldServer.Models.Configs;
using WorldServer.Packets;
using WorldServer.Services;
using WorldServer.Session;

namespace WorldServer
{

    internal class Program
    {
        public static IHost? AppHost { get; private set; }

        static void Main(string[] args)
        {

            Console.WriteLine("================= World Server ====================");
            // 01. Log4Net 설정
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // 02. Host 빌드 및 서비스 DI
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.HostingEnvironment.ApplicationName = "WorldServer";
                    ConfigureServices(services);
                }).Build();

            // 03. ConfigManager 로드
            var configManager = AppHost.Services.GetRequiredService<ConfigManager<AppConfig>>();
            if (!configManager.Load("appsettings.json"))
            {
                return;
            }

            // 04. 서버 시작 예시
            var server = AppHost.Services.GetRequiredService<WorldServer>();
            server.Start();

            CommandProcess();
        }

        private static void CommandProcess()
        {
            var logFactory = AppHost!.Services.GetRequiredService<ILogFactory>();
            var commandService = AppHost.Services.GetRequiredService<CommandService>();

            ILog log = logFactory.CreateLogger<Program>();
            Console.WriteLine("Enter command: [quit] or [help]");
            while (true)
            {
                string? command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command)) continue;

                switch (command.ToLower())
                {
                    case "quit":
                        AppHost.Services.GetRequiredService<WorldServer>().Stop();
                        return;

                    default:
                        if (!commandService.Execute(command))
                        {
                            Console.WriteLine("Unknown command. Type 'help' for available commands.");
                        }
                        break;
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ConfigManager<AppConfig>>();               // config

            services.AddSingleton<WorldDBSession>();

            services.AddSingleton<CommandService>();

            services.AddSingleton<ClientWorldPacketHandler>();
            services.AddSingleton<ClientWorldPacketManager>();
            services.AddSingleton<WorldAuthPacketHandler>();
            services.AddSingleton<WorldAuthPacketManager>();
            services.AddSingleton<WorldDbPacketHandler>();
            services.AddSingleton<WorldDbPacketManager>();

            services.AddSingleton<WorldServer>();

            services.AddSingleton<ILogFactory, Log4NetFactory>(); // log4net factory
            services.AddSingleton<ISessionManager<ClientSession>, SessionManager<ClientSession>>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<ConfigManager<AppConfig>>();
                var host = config.config!.Redis!.GetConnectionString();
                return ConnectionMultiplexer.Connect(host);
            });

            services.AddSingleton<IRedisSession, RedisSession>();

            services.AddSingleton<IClientService, ClientService>();
            services.AddSingleton<IAuthenticateService,  AuthenticateService>();
        }
    }
}
