using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public class Listener<T> where T : ISession, new()
    {
        private readonly Socket listenerSocket;
        private readonly  CancellationTokenSource cts;
        private readonly Action<Exception> errorHandler;

        public Listener(IPEndPoint endPoint, int backlog = 100, Action<Exception>? errorHandler = null)
        {
            listenerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(endPoint);
            listenerSocket.Listen(backlog);
            cts = new CancellationTokenSource();
            this.errorHandler = errorHandler ?? (ex => Console.WriteLine($"[ERROR] {ex.Message}", ex));
        }

        /// <summary>
        /// 서버 시작
        /// </summary>
        public void StartListener(int maxAcceptCount = 10)
        {
            if (listenerSocket == null)
                errorHandler(new InvalidOperationException("Listener socket is not initialized."));

            for (int i = 0; i < maxAcceptCount; i++)
            {
                StartAcceptLoops(cts.Token);
            }
        }

        /// <summary>
        /// 서버 종료
        /// </summary>
        public void Stop()
        {
            try
            {
                cts?.Cancel();
                listenerSocket?.Close();
            }
            catch (Exception ex)
            {
                errorHandler(ex);
            }
        }

        /// <summary>
        /// Accept 루프 여러 개 시작 (멀티 Accept)
        /// </summary>
        private void StartAcceptLoops(CancellationToken cancellationToken)
        {
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var clientSocket = await listenerSocket.AcceptAsync();
                        ISession session = new T();
                        session.Start(clientSocket);
                    }
                    catch (Exception ex)
                    {
                        errorHandler(ex);
                    }
                }
            });
        }
    }
}
