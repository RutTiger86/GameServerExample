using AuthDBServer.Models.Configs;
using AuthDBServer.Session;
using log4net;
using Server.Core;
using Server.Utill;
using System.Net;

namespace AuthDBServer
{
    public class AuthDBServer(ILogFactory logFactory, ConfigManager<AppConfig> configManager, SessionManager<AuthSession, AuthDbPacketManager> authSessionManager)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthDBServer>();
        private readonly ConfigManager<AppConfig> configManager = configManager;

        private Listener<AuthSession>? clientListener;

        /// <summary>
        /// 서버 시작
        /// </summary>
        public void Start()
        {
            log.Info("[AuthDBServer] Starting Auth DB Server...");

            var serverConfig = configManager.config?.AuthDbServer;
            if (serverConfig == null)
            {
                log.Error("[AuthDBServer] Server configuration is null. Startup aborted.");
                return;
            }

            try
            {
                IPEndPoint endPoint = new(IPAddress.Parse(serverConfig.ListenIP), serverConfig.ListenPort);
                clientListener = new Listener<AuthSession>(endPoint, authSessionManager.Generate, serverConfig.Backlog, HandleClientListenerError);
                clientListener.StartListener(serverConfig.MaxAcceptCount);

                log.Info($"[AuthDBServer] Server listening on {serverConfig.ListenIP}:{serverConfig.ListenPort}");
            }
            catch (Exception ex)
            {
                log.Error($"[AuthDBServer] Failed to start server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 서버 종료
        /// </summary>
        public void Stop()
        {
            log.Info("[AuthServer] Stopping server...");

            try
            {
                clientListener?.Stop();
                log.Info("[AuthServer] Server stopped.");
            }
            catch (Exception ex)
            {
                log.Error($"[AuthServer] Error while stopping server: {ex.Message}", ex);
            }
        }


        private void HandleClientListenerError(Exception ex)
        {
            log.Error($"[ClientListenerError]: {ex.Message}", ex);
        }
    }
}
