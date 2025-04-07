using AuthServer.Models.Configs;
using AuthServer.Packets;
using AuthServer.Session;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Utill;
using System.Reflection;

namespace AuthServer
{
    internal class Program
    {
        public static IHost? AppHost { get; private set; }

        static void Main(string[] args)
        {
            // 01. Log4Net 설정
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // 02. Host 빌드 및 서비스 DI
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.HostingEnvironment.ApplicationName = "AuthServer";
                    ConfigureServices(services);
                }).Build();

            // 03. ConfigManager 로드
            var configManager = AppHost.Services.GetRequiredService<ConfigManager<AppConfig>>();
            if (!configManager.Load("appsettings.json"))
            {
                return;
            }

            // 04. 서버 시작 예시
            var server = AppHost.Services.GetRequiredService<AuthServer>();
            server.Start();

            CommandProcess();
        }

        private static void CommandProcess()
        {
            var logFactory = AppHost!.Services.GetRequiredService<ILogFactory>();

            ILog log = logFactory.CreateLogger<Program>();

            while (true)
            {
                Console.WriteLine("Enter command: [quit]");
                string? command = Console.ReadLine();

                if (command == null)
                    continue;

                switch (command.ToLower())
                {
                    case "quit":
                        AppHost.Services.GetRequiredService<AuthServer>().Stop();
                        return;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogFactory, Log4NetFactory>(); // log4net factory
            services.AddSingleton<ConfigManager<AppConfig>>();               // config

            services.AddSingleton<SessionManager<ClientSession,ClientAuthPacketManager>>();

            services.AddSingleton<AuthDbPacketHandler>();               
            services.AddSingleton<AuthDbPacketManager>(); 
            services.AddSingleton<ClientAuthPacketHandler>();
            services.AddSingleton<ClientAuthPacketManager>(); 
            services.AddSingleton<WorldAuthPacketHandler>();
            services.AddSingleton<WorldAuthPacketManager>();

            services.AddSingleton<AuthServer>();
        }
    }
}
