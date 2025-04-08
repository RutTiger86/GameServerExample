using Server.Core.Interface;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Server.Core
{
    public class Connector<TSession> where TSession : ISession
    {
        private readonly Func<TSession> sessionFactory;
        private readonly Action<Exception> errorHandler;
        private bool isStopping = false; // 종료 플래그
        private readonly CancellationTokenSource cts;
        private Socket socket;

        public Connector(Func<TSession> sessionFactory, Action<Exception>? errorHandler = null)
        {
            this.sessionFactory = sessionFactory;
            this.errorHandler = errorHandler ?? (ex => Console.WriteLine($"[ERROR] {ex.Message}", ex));
            this.cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 서버 연결 시작 (한 번만)
        /// </summary>
        public async Task StartConnectorAsync(IPEndPoint endPoint)
        {
            if (isStopping)
            {
                errorHandler(new InvalidOperationException("Connector is stopped."));
                return;
            }

            try
            {
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(endPoint, cts.Token);

                ISession session = sessionFactory.Invoke();
                session.Start(socket); // 세션 시작
            }
            catch (OperationCanceledException)
            {
                // 연결 취소
                if (isStopping)
                    Console.WriteLine("[INFO] Connection attempt cancelled.");
            }
            catch (Exception ex)
            {
                errorHandler(ex);
            }
        }

        /// <summary>
        /// 커넥터 종료
        /// </summary>
        public void Stop()
        {
            isStopping = true;
            cts.Cancel(); // 연결 시도 중단
        }
    }
}
