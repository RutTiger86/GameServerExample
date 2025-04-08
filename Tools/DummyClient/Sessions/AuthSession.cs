using Google.Protobuf;
using Server.Core;
using Server.Data.ClientAuth;
using Server.Utill;
using System.Net;
using System.Security.Authentication;

namespace DummyClient.Sessions
{
    public class AuthSession : SecurePacketClientSession
    {

        private ClientAuthPacketManager packetManager;

        public AuthSession(string hostName, SslProtocols sslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13) : base(hostName, sslProtocols)
        {
            packetManager = new ClientAuthPacketManager(new Log4NetFactory(), new Packets.ClientAuthPacketHandler());
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

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Auth Connected {endPoint.AddressFamily}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Auth DisConnected {endPoint.AddressFamily}");
        }

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            Console.WriteLine($"Auth OnRecvPacket");
            packetManager.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Auth OnSend");
        }
    }
}
