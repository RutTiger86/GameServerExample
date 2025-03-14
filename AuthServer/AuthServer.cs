using AuthServer.Commons;
using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AuthServer
{
    public class AuthServer
    {
        private readonly ILog log;
        private readonly ConfigManager configManager;

        private Socket listenerSocket;
        private CancellationTokenSource cts;

        public AuthServer(ILogFactory logFactory, ConfigManager configManager)
        {
            log = logFactory.CreateLogger<AuthServer>();
            this.configManager = configManager;
        }

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
                listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(serverConfig.ListenIP), serverConfig.ListenPort));
                listenerSocket.Listen(serverConfig.Backlog);

                log.Info($"[AuthServer] Server listening on {serverConfig.ListenIP}:{serverConfig.ListenPort}");

                cts = new CancellationTokenSource();

                StartAcceptLoop(cts.Token);
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
                cts?.Cancel();
                listenerSocket?.Close();
                log.Info("[AuthServer] Server stopped.");
            }
            catch (Exception ex)
            {
                log.Error($"[AuthServer] Error while stopping server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Accept 루프 시작
        /// </summary>
        private void StartAcceptLoop(CancellationToken cancellationToken)
        {
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var clientSocket = await listenerSocket.AcceptAsync();
                        log.Info($"[AuthServer] Client connected: {clientSocket.RemoteEndPoint}");

                        var session = new ClientSession(clientSocket, log);
                        session.Start();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"[AuthServer] Accept failed: {ex.Message}", ex);
                    }
                }
            });
        }
    }
}
