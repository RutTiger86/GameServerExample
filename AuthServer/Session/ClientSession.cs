using AuthServer.Models.Account;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Data.ClientAuth;
using Server.Utill;
using Server.Utill.Interface;
using System.Net;

namespace AuthServer.Session
{
    public class ClientSession(ILogFactory logFactory) : PacketSession , ILogCreater<ClientSession>
    {
        private readonly ILog log = logFactory.CreateLogger<ClientSession>();

        public LoginInfo? LoginInfo { get; set; }
        public static ClientSession Create(ILogFactory logFactory)
        {
            return new ClientSession(logFactory);
        }

        public void Send(IMessage packet)
        {
            string packName = packet.Descriptor.Name;
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

        public override void OnRecvPacket(ReadOnlyMemory<byte> buffer)
        {
            ClientAuthPacketManager.Instance.OnRecvPacket(this, buffer);
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
