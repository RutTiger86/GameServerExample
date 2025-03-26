using DummyClient.Extensions;
using Google.Protobuf;
using Server.Core;
using Server.Data.ClientAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Sessions
{
    public class AuthSession : PacketSession
    {
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
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Auth OnSend");
        }
    }
}
