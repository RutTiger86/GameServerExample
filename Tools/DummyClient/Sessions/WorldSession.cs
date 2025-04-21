using Google.Protobuf;
using Server.Core;
using Server.Data.ClientWorld;
using Server.Utill;
using System.Net;
using System.Security.Authentication;

namespace DummyClient.Sessions
{
    public class WorldSession : PacketSession
    {

        private ClientWorldPacketManager packetManager;

        public WorldSession(string hostName) 
        {
            packetManager = new ClientWorldPacketManager(new Log4NetFactory(), new Packets.ClientWorldPacketHandler());
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

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"World Connected {endPoint.AddressFamily}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"World DisConnected {endPoint.AddressFamily}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            Console.WriteLine($"World OnRecvPacket");
            packetManager.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"World OnSend");
        }
    }
}
