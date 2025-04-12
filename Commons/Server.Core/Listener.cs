using Server.Core.Interface;
using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public class Listener<TSession> where TSession : ISession
    {
        private readonly Socket listenerSocket;
        private readonly CancellationTokenSource cts;
        private readonly Action<Exception> errorHandler;
        private readonly Func<Task<TSession>> sessionFactory;
        private bool isStopping = false; // 종료 플래그

        public Listener(IPEndPoint endPoint, Func<Task<TSession>> sessionFactory, int backlog = 100, Action<Exception>? errorHandler = null)
        {
            listenerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(endPoint);
            listenerSocket.Listen(backlog);
            cts = new CancellationTokenSource();
            this.errorHandler = errorHandler ?? (ex => Console.WriteLine($"[ERROR] {ex.Message}", ex));
            this.sessionFactory = sessionFactory;
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
                isStopping = true; // 종료 플래그 활성화
                cts?.Cancel(); // 토큰 취소
                listenerSocket?.Close(); // 소켓 닫기 (Accept 깨기)
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
                        var clientSocket = await listenerSocket!.AcceptAsync();

                        ISession session = await sessionFactory();
                        session.Start(clientSocket);
                    }
                    catch (SocketException ex)
                    {
                        // 소켓 닫힘으로 인한 종료 예외는 무시
                        if (isStopping)
                        {
                            break; // 루프 탈출
                        }

                        errorHandler(ex); // 예기치 않은 소켓 예외
                    }
                    catch (Exception ex)
                    {
                        // 일반 예외 처리
                        if (isStopping)
                        {
                            break;
                        }

                        errorHandler(ex);
                    }
                }
            });
        }
    }
}
