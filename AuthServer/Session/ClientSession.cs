using AuthServer.Models.Account;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Core.Interface;
using Server.Data.ClientAuth;
using Server.Data.WorldAuth;
using Server.Utill;
using Server.Utill.Interface;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace AuthServer.Session
{
    public class ClientSession : SecurePacketServerSession, ILogCreater<ClientSession>
    {
        private readonly ILog log;
        private readonly IPacketManager packetManager;
        private readonly IRedisSession redisSession;

        public LoginInfo? LoginInfo { get; set; }

        public ClientSession(X509Certificate2 cert, ILogFactory logFactory, IPacketManager packetManager, IRedisSession redisSession)
       : base(cert)
        {
            log = logFactory.CreateLogger<ClientSession>();
            this.packetManager = packetManager;
            this.redisSession = redisSession;
        }

        public static ClientSession Create(ILogFactory logFactory, IPacketManager packetManager, X509Certificate2? cert = null, IRedisSession? redisSession = null)
        {
            if (cert == null)
            {
                throw new ArgumentNullException("cert is Null");
            }

            if (redisSession == null)
            {
                throw new ArgumentNullException("redisSession is Null");
            }

            return new ClientSession(cert, logFactory, packetManager, redisSession);
        }


        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name;
            ClientAuthPacketId packetId = (ClientAuthPacketId)Enum.Parse(typeof(ClientAuthPacketId), packName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override async void OnConnected(EndPoint endPoint)
        {
            string ip = ((IPEndPoint)endPoint).Address.ToString();
            int port = ((IPEndPoint)endPoint).Port;
            await redisSession.RegisterSessionAsync(SessionId, ip, port, (int)SessionState.Connected);

            log.Info($"[CONNECTED] SessionId: {SessionId} from {ip}:{port}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            packetManager.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            log.Info($"ClientSession OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {

        }

    }
}
