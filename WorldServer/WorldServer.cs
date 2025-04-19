using log4net;
using Server.Core;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Models.Configs;
using WorldServer.Session;

namespace WorldServer
{

    public class WorldServer(ILogFactory logFactory, ConfigManager<AppConfig> configManager, ISessionManager<ClientSession> clientSessionManager, WorldDBSession WorldDBSession, ClientWorldPacketManager clientWorldPacketManager)
    {
        private readonly ILog log = logFactory.CreateLogger<WorldServer>();

        private Listener<ClientSession>? clientListener;
        private Connector<WorldDBSession>? connector;

        /// <summary>
        /// 서버 시작
        /// </summary>
        public void Start()
        {
            log.Info("[WorldServer] Starting World Server...");

            //ConnectWorldDbServer();

            StartClientListen();
        }

        private async void ConnectWorldDbServer()
        {
            var serverConfig = configManager.config?.WorldDBServer;
            if (serverConfig == null)
            {
                log.Error("[WorldServer] Server configuration is null. Startup aborted.");
                return;
            }

            if (IPAddress.TryParse(serverConfig.ConnectIP, out IPAddress? address))
            {
                connector = new Connector<WorldDBSession>(() => WorldDBSession);
                await connector.StartConnectorAsync(new IPEndPoint(address, serverConfig.ConnectPort));
            }
        }

        private void StartClientListen()
        {
            var serverConfig = configManager.config?.WorldServer;
            if (serverConfig == null)
            {
                log.Error("[WorldServer] Server configuration is null. Startup aborted.");
                return;
            }

            try
            {
                IPEndPoint endPoint = new(IPAddress.Parse(serverConfig.ListenIP), serverConfig.ListenPort);
                clientListener = new Listener<ClientSession>(endPoint,
                    () => clientSessionManager.Generate(clientWorldPacketManager),
                    serverConfig.Backlog,
                    HandleClientListenerError);
                clientListener.StartListener(serverConfig.MaxAcceptCount);

                log.Info($"[WorldServer] Server listening on {serverConfig.ListenIP}:{serverConfig.ListenPort}");
            }
            catch (Exception ex)
            {
                log.Error($"[WorldServer] Failed to start server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 서버 종료
        /// </summary>
        public void Stop()
        {
            log.Info("[WorldServer] Stopping server...");

            try
            {
                clientListener?.Stop();
                log.Info("[WorldServer] Server stopped.");
            }
            catch (Exception ex)
            {
                log.Error($"[WorldServer] Error while stopping server: {ex.Message}", ex);
            }
        }


        private void HandleClientListenerError(Exception ex)
        {
            log.Error($"[ClientListenerError]: {ex.Message}", ex);
        }
    }
}
