using AuthServer.Models.Configs;
using AuthServer.Packets;
using AuthServer.Services;
using AuthServer.Session;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Utill;
using Server.Utill.Interface;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace AuthServer
{
    internal class Program
    {
        public static IHost? AppHost { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine("================= Auth Server ====================");
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

            //04. 인증서 등록 
            var clientSessionManager = AppHost.Services.GetRequiredService<ISessionManager<ClientSession>>();
            var cert = new X509Certificate2(configManager.config!.Secure!.CertPath, configManager.config.Secure.CertPassworld);
            clientSessionManager.SetCert(cert);

            //05. Client Session Redis 등록  
            var redisSession = AppHost.Services.GetRequiredService<IRedisSession>();
            clientSessionManager.SetRedis(redisSession);

            // 06. 서버 시작 예시
            var server = AppHost.Services.GetRequiredService<AuthServer>();
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
                        AppHost.Services.GetRequiredService<AuthServer>().Stop();
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

            services.AddSingleton<AuthDBSession>();
            services.AddSingleton<AuthDbPacketHandler>();
            services.AddSingleton<AuthDbPacketManager>();
            services.AddSingleton<ClientAuthPacketHandler>();
            services.AddSingleton<ClientAuthPacketManager>();
            services.AddSingleton<WorldAuthPacketHandler>();
            services.AddSingleton<WorldAuthPacketManager>();
            services.AddSingleton<CommandService>();

            services.AddSingleton<AuthServer>();

            services.AddSingleton<ILogFactory, Log4NetFactory>(); // log4net factory
            services.AddSingleton<ISessionManager<ClientSession>, SessionManager<ClientSession>>();
            services.AddSingleton<IWorldServerRegistry, WorldServerRegistry>();
            services.AddSingleton<IRedisSession, RedisSession>();
            services.AddSingleton<IClientService, ClientService>();

        }
    }
}

