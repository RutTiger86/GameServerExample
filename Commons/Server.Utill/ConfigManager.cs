using log4net;
using System.Text.Json;

namespace Server.Utill
{
    public class ConfigManager<TConfig> where TConfig : new()
    {
        private readonly ILog log;
        public TConfig? config { get; private set; }

        public ConfigManager(ILogFactory logFactory)
        {
            config = new TConfig();
            log = logFactory.CreateLogger<ConfigManager<TConfig>>();
        }


        private static readonly JsonSerializerOptions s_readOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public object? GetSettingConfig(string settingName)
        {
            var property = config?.GetType().GetProperty(settingName);
            return property?.GetValue(this);
        }

        /// <summary>
        /// JSON 파일 로드
        /// </summary>
        public bool Load(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                config = JsonSerializer.Deserialize<TConfig>(json, s_readOptions);

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
