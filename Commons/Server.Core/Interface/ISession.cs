using System.Net;
using System.Net.Sockets;

namespace Server.Core.Interface
{
    public interface ISession
    {
        public long SessionId { get; set; }
        public Action<Exception>? ErrorHandler { get; set; }
        public void Start(Socket socket);
        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);
    }
}
