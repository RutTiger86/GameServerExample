using AuthServer.Commons;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Data.ClientAuth;
using System.Net;

namespace AuthServer.Session
{
    public class ClientSession(ILogFactory logFactory) : PacketSession
    {
        private readonly ILog log = logFactory.CreateLogger<ClientSession>();

        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name.Replace("_", string.Empty);
            ClientAuthPacketId packetId  = (ClientAuthPacketId)Enum.Parse(typeof(ClientAuthPacketId), packName);
            
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)packetId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            log.Info($"ClientSession OnConnected : {endPoint}");
        }

        public override void OnRecvPacket(ReadOnlySpan<byte> buffer)
        {
            log.Info($"ClientSession OnRecvPacket");
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
