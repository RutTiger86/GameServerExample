using AuthServer.Models.Configs;
using AuthServer.Session;
using log4net;
using Server.Core;
using Server.Utill;
using Server.Utill.Interface;
using System.Net;

namespace AuthServer
{
    public class AuthServer(ILogFactory logFactory, ConfigManager<AppConfig> configManager, ISessionManager<ClientSession> clientSessionManager, AuthDBSession authDBSession, ClientAuthPacketManager clientAuthPacketManager)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthServer>();

        private Listener<ClientSession>? clientListener;
        private Connector<AuthDBSession>? connector;

        /// <summary>
        /// 서버 시작
        /// </summary>
        public void Start()
        {
            log.Info("[AuthServer] Starting Auth Server...");

            ConnectAuthDbServer();

            StartClientListen();
        }

        private async void ConnectAuthDbServer()
        {
            var serverConfig = configManager.config?.AuthDBServer;
            if (serverConfig == null)
            {
                log.Error("[AuthServer] Server configuration is null. Startup aborted.");
                return;
            }

            if (IPAddress.TryParse(serverConfig.ConnectIP, out IPAddress? address))
            {
                connector = new Connector<AuthDBSession>(() => authDBSession);
                await connector.StartConnectorAsync(new IPEndPoint(address, serverConfig.ConnectPort));
            }
        }

        private void StartClientListen()
        {
            var serverConfig = configManager.config?.AuthServer;
            if (serverConfig == null)
            {
                log.Error("[AuthServer] Server configuration is null. Startup aborted.");
                return;
            }

            try
            {
                IPEndPoint endPoint = new(IPAddress.Parse(serverConfig.ListenIP), serverConfig.ListenPort);
                clientListener = new Listener<ClientSession>(endPoint,
                    () =>clientSessionManager.Generate(clientAuthPacketManager), 
                    serverConfig.Backlog, 
                    HandleClientListenerError);
                clientListener.StartListener(serverConfig.MaxAcceptCount);

                log.Info($"[AuthServer] Server listening on {serverConfig.ListenIP}:{serverConfig.ListenPort}");
            }
            catch (Exception ex)
            {
                log.Error($"[AuthServer] Failed to start server: {ex.Message}", ex);
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
