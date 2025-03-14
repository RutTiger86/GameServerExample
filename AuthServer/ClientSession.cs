using log4net;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer
{
    public class ClientSession
    {
        private readonly Socket clientSocket;
        private readonly ILog log;

        public ClientSession(Socket clientSocket, ILog log)
        {
            this.clientSocket = clientSocket;
            this.log = log;
        }

        /// <summary>
        /// 세션 시작
        /// </summary>
        public void Start()
        {
            Task.Run(() => ReceiveLoop());
        }

        /// <summary>
        /// 수신 루프
        /// </summary>
        private async Task ReceiveLoop()
        {
            var buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                    if (received == 0)
                    {
                        log.Info($"[ClientSession] Client disconnected: {clientSocket.RemoteEndPoint}");
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    log.Info($"[ClientSession] Received: {message}");

                    // Echo 응답
                    await clientSocket.SendAsync(Encoding.UTF8.GetBytes($"Echo: {message}"), SocketFlags.None);
                }
            }
            catch (Exception ex)
            {
                log.Error($"[ClientSession] Error: {ex.Message}", ex);
            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}
