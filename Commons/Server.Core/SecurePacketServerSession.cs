using Server.Core.Interface;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Server.Core
{
    public abstract class SecurePacketServerSession : ISession
    {
        public int SessionId { get; set; }
        public Action<Exception>? ErrorHandler { get; set; }

        private readonly X509Certificate2 serverCertificate;
        private readonly bool requireClientCertificate;
        private readonly SslProtocols sslProtocols;

        private SslStream? sslStream;
        private TcpClient? tcpClient;

        private readonly byte[] recvBuffer = new byte[4096];
        private readonly ConcurrentQueue<ArraySegment<byte>> sendQueue = new();
        private readonly SemaphoreSlim sendLock = new(1, 1);

        private readonly int HeaderSize = 4; // size(2) + packetId(2)

        protected SecurePacketServerSession(
            X509Certificate2 certificate,
            bool requireClientCert = false,
            SslProtocols sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13)
        {
            this.serverCertificate = certificate;
            this.requireClientCertificate = requireClientCert;
            this.sslProtocols = sslProtocols;
        }

        public void Start(Socket socket)
        {
            tcpClient = new TcpClient { Client = socket };
            _ = RunAsync();
        }
        private async Task RunAsync()
        {
            try
            {
                using var netStream = tcpClient!.GetStream();
                sslStream = new SslStream(netStream, false, ValidateClientCertificate);

                await sslStream.AuthenticateAsServerAsync(
                    serverCertificate,
                    clientCertificateRequired: requireClientCertificate,
                    enabledSslProtocols: sslProtocols,
                    checkCertificateRevocation: false);

                OnConnected(tcpClient.Client.RemoteEndPoint!);

                while (true)
                {
                    int bytesRead = await sslStream.ReadAsync(recvBuffer, 0, recvBuffer.Length);
                    if (bytesRead == 0)
                        break;

                    int processed = OnRecv(new ArraySegment<byte>(recvBuffer, 0, bytesRead));
                    if (processed <= 0 || processed > bytesRead)
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(ex);
            }
            finally
            {
                OnDisconnected(tcpClient?.Client.RemoteEndPoint!);
                sslStream?.Dispose();
                tcpClient?.Close();
            }
        }

        /// <summary>
        /// TLS 클라이언트 인증서 검증 로직
        /// </summary>
        private bool ValidateClientCertificate(object sender, X509Certificate? cert, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            if (!requireClientCertificate)
                return true;

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine($"[TLS] Client certificate error: {sslPolicyErrors}");
            return false;
        }

        /// <summary>
        /// 송신 요청: 내부 큐에 등록 후 순차 전송
        /// </summary>
        public void Send(ArraySegment<byte> data)
        {
            sendQueue.Enqueue(data);
            _ = TrySendAsync();
        }

        private async Task TrySendAsync()
        {
            await sendLock.WaitAsync();
            try
            {
                if (sslStream == null)
                    return;

                while (sendQueue.TryDequeue(out var data))
                {
                    await sslStream.WriteAsync(data.Array!, data.Offset, data.Count);
                    await sslStream.FlushAsync();

                    OnSend(data.Count);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(ex);
            }
            finally
            {
                sendLock.Release();
            }
        }
        public int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            if (buffer.Array == null)
                return 0;

            Span<byte> span = buffer.AsSpan();

            while (true)
            {
                if (span.Length < HeaderSize)
                    break;

                ushort size = BitConverter.ToUInt16(span.Slice(0, 2));
                ushort packetId = BitConverter.ToUInt16(span.Slice(2, 2));

                if (span.Length < size)
                    break;

                var packetBuffer = new ReadOnlyMemory<byte>(buffer.Array, buffer.Offset + processLen, size);
                OnRecvPacket(packetBuffer);

                processLen += size;
                span = span.Slice(size);
            }

            return processLen;
        }

        // 반드시 상속에서 구현해야 할 추상 메서드
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecvPacket(ReadOnlyMemory<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);
    }



}
