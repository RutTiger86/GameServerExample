using AuthServer.Commons;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace AuthServer
{
    internal class Program
    {
        public static IHost? AppHost { get; private set; }

        static void Main(string[] args)
        {
            // 01. Log4Net 설정
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // 02. Host 빌드 및 서비스 DI
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.HostingEnvironment.ApplicationName = "AuthServer";
                    ConfigureServices(services);
                }).Build();

            // 03. ConfigManager 로드
            var configManager = AppHost.Services.GetRequiredService<ConfigManager>();
            if (!configManager.Load("appsettings.json"))
            {
                return;
            }

            // 04. 서버 시작 예시
             var server = AppHost.Services.GetRequiredService<AuthServer>();
             server.Start();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogFactory, Log4NetFactory>(); // log4net factory
            services.AddSingleton<ConfigManager>();               // config
            services.AddSingleton<AuthServer>();
        }
    }
}
