using AuthServer.Models.Configs;
using log4net;
using System.Text.Json;

namespace AuthServer.Commons
{
    public class ConfigManager
    {
        private readonly ILog log;
        private AppConfig? config;

        public ConfigManager(ILogFactory logFactory)
        {
            log = logFactory.CreateLogger<ConfigManager>();
        }

        public AuthServerSetting? AuthServer
        {
            get
            {
                return config?.AuthServer;
            }
        }

        public AuthDBServerSetting? AuthDBServer
        {
            get
            {
                return config?.AuthDBServer;
            }
        }

        private static readonly JsonSerializerOptions s_readOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };


        /// <summary>
        /// JSON 파일 로드
        /// </summary>
        public bool Load(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                config = JsonSerializer.Deserialize<AppConfig>(json, s_readOptions);

                if (config == null)
                {
                    log.Error("Failed to deserialize appsettings.json (Config is null)");
                    return false;
                }

                log.Info("Successfully loaded appsettings.json");
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"Exception while loading config: {ex.Message}", ex);
                return false;
            }
        }
    }
}
