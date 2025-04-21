using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Core.Interface;
using Server.Data.ClientWorld;
using Server.Utill;
using Server.Utill.Interface;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace WorldServer.Session
{

    public class ClientSession(ILogFactory logFactory, IPacketManager packetManager) : PacketSession , ILogCreater<ClientSession>
    {
        private readonly ILog log = logFactory.CreateLogger<ClientSession>();

        private bool isAuthenticated = false;
        private long? accountId = null;
        private string ip = string.Empty;
        private int? port = null;

        public bool IsAuthenticated => isAuthenticated;
        public string Ip => ip;
        public int? Port => port;

        public void Authenticate(long accountId)
        {
            this.isAuthenticated = true;
            this.accountId = accountId;
        }

        public static ClientSession Create(ILogFactory logFactory, IPacketManager packetManager, X509Certificate2? cert = null, IRedisSession? redisSession = null)
        {
            return new ClientSession(logFactory, packetManager);
        }


        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name;
            ClientWorldPacketId packetId = (ClientWorldPacketId)Enum.Parse(typeof(ClientWorldPacketId), packName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override async void OnConnected(EndPoint endPoint)
        {
            ip = ((IPEndPoint)endPoint).Address.ToString();
            port = ((IPEndPoint)endPoint).Port;

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
