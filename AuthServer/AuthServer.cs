using AuthServer.Commons;
using AuthServer.Session;
using log4net;
using Server.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AuthServer
{
    public class AuthServer(ILogFactory logFactory, ConfigManager configManager, SessionManager sessionManager)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthServer>();
        private readonly ConfigManager configManager = configManager;
        private readonly SessionManager sessionManager = sessionManager;

        private Listener<ClientSession>? clientListener;

        /// <summary>
        /// 서버 시작
        /// </summary>
        public void Start()
        {
            log.Info("[AuthServer] Starting server...");

            var serverConfig = configManager.AuthServer;
            if (serverConfig == null)
            {
                log.Error("[AuthServer] Server configuration is null. Startup aborted.");
                return;
            }

            try
            {
                IPEndPoint endPoint =  new(IPAddress.Parse(serverConfig.ListenIP), serverConfig.ListenPort);
                clientListener = new Listener<ClientSession>(endPoint, sessionManager.Generate, serverConfig.Backlog , HandleClientListenerError);
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
