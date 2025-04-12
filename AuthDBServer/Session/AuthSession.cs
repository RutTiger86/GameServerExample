using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Utill;
using Server.Utill.Interface;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace AuthDBServer.Session
{
    public class AuthSession(ILogFactory logFactory, IPacketManager packetManager) : PacketSession, ILogCreater<AuthSession>
    {
        private readonly ILog log = logFactory.CreateLogger<AuthSession>();
        private readonly IPacketManager packetManager = packetManager;

        public static AuthSession Create(ILogFactory logFactory, IPacketManager packetManager, X509Certificate2? cert = null, IRedisSession? redisSession = null)
        {
            return new AuthSession(logFactory, packetManager);
        }

        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name;

            AuthDbPacketId packetId = (AuthDbPacketId)Enum.Parse(typeof(AuthDbPacketId), packName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            log.Info($"AuthDBSession OnConnected : {endPoint}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            packetManager.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            log.Info($"AuthDBSession OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {

        }


    }
}
