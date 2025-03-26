using AuthServer.Commons;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Data.WorldAuth;
using System.Net;

namespace AuthServer.Session
{
    public class WorldSession(ILogFactory logFactory) : PacketSession
    {
        private readonly ILog log = logFactory.CreateLogger<WorldSession>();

        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name.Replace("_", string.Empty);
            WorldAuthPacketId packetId  = (WorldAuthPacketId)Enum.Parse(typeof(WorldAuthPacketId), packName);
            
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            log.Info($"WorldSession OnConnected : {endPoint}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            log.Info($"WorldSession OnRecvPacket");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            log.Info($"WorldSession OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {

        }
    }
}
