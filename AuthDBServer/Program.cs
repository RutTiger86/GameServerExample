using AuthDB.Data;
using AuthDBServer.Models.Configs;
using AuthDBServer.Packets;
using AuthDBServer.Repositories;
using AuthDBServer.Session;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Utill;
using Server.Utill.Interface;
using System.Reflection;

namespace AuthDBServer
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
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<AuthDbContext>(options =>
                    {
                        options.UseSqlServer(connectionString);
                    }
                    );
                    ConfigureServices(services);
                })
                .Build();

            // 03. ConfigManager 로드
            var configManager = AppHost.Services.GetRequiredService<ConfigManager<AppConfig>>();
            if (!configManager.Load("appsettings.json"))
            {
                return;
            }

            // 04. 서버 시작 예시
            var server = AppHost.Services.GetRequiredService<AuthDBServer>();
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
                        AppHost.Services.GetRequiredService<AuthDBServer>().Stop();
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


            services.AddSingleton<SessionManager<AuthSession, AuthDbPacketManager>>();
            services.AddSingleton<AuthDbPacketHandler>();
            services.AddSingleton<AuthDbPacketManager>();
            services.AddSingleton<AuthDBServer>();


            services.AddScoped<IAuthRepository, AuthRepository>();
        }
    }
}
