using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Data.WorldDb;
using Server.Utill;
using System.Net;

namespace WorldServer.Session
{
    public class WorldDBSession(ILogFactory logFactory, WorldDbPacketManager packetManager) : PacketSession
    {
        private readonly ILog log = logFactory.CreateLogger<WorldDBSession>();

        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name;
            WorldDbPacketId packetId = (WorldDbPacketId)Enum.Parse(typeof(WorldDbPacketId), packName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            log.Info($"WorldDBSession OnConnected : {endPoint}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            packetManager.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            log.Info($"WorldDBSession OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {

        }


    }
}
