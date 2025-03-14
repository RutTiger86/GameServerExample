using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface ISession
    {
        public void Start(Socket socket);
    }
    public abstract class Session : ISession
    {
        private Socket? socket;
        private byte[]? recvBuffer;
        private static readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        protected int BufferSize { get; private set; } = 4096; // 고정 버퍼 크기
        
        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket clientSocket)
        {
            socket = clientSocket;
            recvBuffer = bufferPool.Rent(BufferSize); // 버퍼 임대

            // 비동기 수신 시작
            StartReceive();
        }

        private void StartReceive()
        {
            try
            {
                if (socket == null)
                    return;

                if (recvBuffer == null)
                    return;

                socket.BeginReceive(recvBuffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Receive failed: {ex.Message}");
                Close();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (socket == null)
                    return;

                int received = socket.EndReceive(ar);
                if (received <= 0)
                {
                    Console.WriteLine("[INFO] Client disconnected.");
                    Close();
                    return;
                }

                // 받은 데이터 처리
                Console.WriteLine($"[INFO] Received {received} bytes.");

                // 다음 수신
                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Receive callback error: {ex.Message}");
                Close();
            }
        }

        public void Close()
        {
            socket?.Close();

            if(recvBuffer!= null)
                bufferPool.Return(recvBuffer); // 버퍼 반납
        }
    }
}
