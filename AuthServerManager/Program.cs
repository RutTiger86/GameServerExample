using AuthServerManager.Models.Configs;
using AuthServerManager.Services;
using AuthServerManager.Sessions;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Utill;
using StackExchange.Redis;
using System.Reflection;

namespace AuthServerManager
{

    internal class Program
    {
        public static IHost? AppHost { get; private set; }
        static void Main(string[] args)
        {
            Console.WriteLine("================= Auth Server Manager ====================");
            // 01. Log4Net 설정
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // 02. Host 빌드 및 서비스 DI
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.HostingEnvironment.ApplicationName = "AuthServerManager";
                    ConfigureServices(services);
                }).Build();

            // 03. ConfigManager 로드
            var configManager = AppHost.Services.GetRequiredService<ConfigManager<AppConfig>>();
            if (!configManager.Load("appsettings.json"))
            {
                return;
            }

            CommandProcess();
        }

        private static void CommandProcess()
        {
            var logFactory = AppHost!.Services.GetRequiredService<ILogFactory>();
            var commandService = AppHost.Services.GetRequiredService<CommandService>();

            ILog log = logFactory.CreateLogger<Program>();

            commandService.Execute("externally-close");
            commandService.Execute("init-world");
            commandService.Execute("world");

            Console.WriteLine("Enter command: [quit] or [help]");
            while (true)
            {
                string? command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command)) continue;

                switch (command.ToLower())
                {
                    case "quit":
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

            services.AddSingleton<CommandService>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<ConfigManager<AppConfig>>();
                var host = config.config!.Redis!.GetConnectionString();
                return ConnectionMultiplexer.Connect(host);
            });

            services.AddSingleton<ILogFactory, Log4NetFactory>(); // log4net factory
            services.AddSingleton<IAuthManagerRedisSession, RedisSession>();
        }
    }
}
