using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Core;
using Server.Utill.Interface;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Server.Data.ClientWorld;

namespace WorldServer.Session
{

    public class ClientSession(ILogFactory logFactory, IPacketManager packetManager) : PacketSession , ILogCreater<ClientSession>
    {
        private readonly ILog log = logFactory.CreateLogger<ClientSession>();

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
            string ip = ((IPEndPoint)endPoint).Address.ToString();
            int port = ((IPEndPoint)endPoint).Port;

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
