using Server.Core.Interface;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public abstract class Session : ISession
    {
        public int SessionId { get; set; }

        private Socket? socket;
        private byte[]? recvBuffer;
        private static readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        private readonly object lockObj = new();
        private readonly Queue<ArraySegment<byte>> sendQueue = new();

        protected int BufferSize { get; private set; } = 4096;

        public Action<Exception> ErrorHandler { get; set; } = ex => Console.WriteLine($"[ERROR] {ex.Message}{Environment.NewLine}{ex}");

        private readonly SocketAsyncEventArgs sendArgs;
        private readonly SocketAsyncEventArgs recvArgs;

        private bool isSending = false;

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        protected Session()
        {
            sendArgs = new();
            recvArgs = new();

            recvArgs.Completed += OnRecvCompleted;
            sendArgs.Completed += OnSendCompleted;
        }


        public void Start(Socket clientSocket)
        {
            socket = clientSocket;
            recvBuffer = bufferPool.Rent(BufferSize);

            StartReceive();
            OnConnected(socket.RemoteEndPoint!);
        }

        public void Close()
        {
            try
            {
                lock (lockObj)
                {
                    if (socket == null)
                        return;

                    isSending = false;
                    OnDisconnected(socket.RemoteEndPoint!);
                    socket.Close();
                    socket = null;

                    if (recvBuffer != null)
                    {
                        bufferPool.Return(recvBuffer);
                        recvBuffer = null;
                    }

                    sendQueue.Clear();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(new Exception($"[ERROR] Close error: {ex.Message}"));
            }
        }

        private void HandleError(Exception ex)
        {
            ErrorHandler(ex);
            Close();
        }

        #region Recieve
        private void StartReceive()
        {
            try
            {
                if (socket == null || recvBuffer == null)
                    return;

                recvArgs.SetBuffer(recvBuffer, 0, BufferSize);

                bool pending = socket.ReceiveAsync(recvArgs);
                if (!pending)
                    OnRecvCompleted(this, recvArgs);
            }
            catch (Exception ex)
            {
                HandleError(new Exception($"[ERROR] Receive failed: {ex.Message}", ex));
            }
        }

        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    var segment = new ArraySegment<byte>(recvBuffer!, 0, args.BytesTransferred);
                    int processed = OnRecv(segment);
                    if (processed < 0 || processed > args.BytesTransferred)
                    {
                        Close();
                        return;
                    }

                    StartReceive();
                }
                catch (Exception ex)
                {
                    HandleError(new Exception($"[ERROR] OnRecvCompleted failed: {ex.Message}", ex));
                }
            }
            else
            {
                Close();
            }
        }
        #endregion

        #region Send

        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            bool sendNow = false;
            lock (lockObj)
            {
                foreach (var buff in sendBuffList)
                    sendQueue.Enqueue(buff);

                if (!isSending)
                {
                    isSending = true;
                    sendNow = true;
                }
            }

            if (sendNow)
                RegisterSend();
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            bool sendNow = false;
            lock (lockObj)
            {
                sendQueue.Enqueue(sendBuff);

                if (!isSending)
                {
                    isSending = true;
                    sendNow = true;
                }
            }

            if (sendNow)
                RegisterSend();
        }

        private void RegisterSend()
        {
            if (socket == null)
                return;

            List<ArraySegment<byte>> pendingList = [];
            lock (lockObj)
            {
                while (sendQueue.Count > 0)
                    pendingList.Add(sendQueue.Dequeue());
            }

            sendArgs.BufferList = pendingList;
            try
            {
                bool pending = socket.SendAsync(sendArgs);
                if (!pending)
                    OnSendCompleted(this, sendArgs);
            }
            catch (Exception ex)
            {
                HandleError(new Exception($"[ERROR] RegisterSend failed: {ex.Message}", ex));
            }
        }

        private void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                OnSend(args.BytesTransferred);

                lock (lockObj)
                {
                    if (sendQueue.Count > 0)
                    {
                        // 다음 송신
                        RegisterSend();
                        return;
                    }
                    isSending = false;
                    sendArgs.BufferList = null;
                }
            }
            else
            {
                Close();
            }
        }

        #endregion

    }
}
