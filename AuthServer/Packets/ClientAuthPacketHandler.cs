using AuthServer.Services;
using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.ClientAuth;
using Server.Utill;

namespace AuthServer.Packets
{
    public class ClientAuthPacketHandler(ILogFactory logFactory, IClientService clientService)
    {
        private readonly ILog log = logFactory.CreateLogger<ClientAuthPacketHandler>();
        public void CaServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void CaLoginHandler(ISession session, IMessage packet)
        {
            if (packet is CaLogin loginInfo)
            {
                _ = clientService.TryLogin(session, loginInfo);
            }
           
        }
        public void CaWorldListHandler(ISession session, IMessage packet)
        { 
            _ = clientService.SendWorldList(session, packet);
        }
        public void CaEnterWorldHandler(ISession session, IMessage packet)
        {
            if (packet is CaEnterWorld enterWorldInfo)
            {
                _ = clientService.TryEnterWorld(session, enterWorldInfo);
            }           
        }
    }
}
