using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.ClientWorld;
using Server.Utill;
using WorldServer.Services;

namespace WorldServer.Packets
{
    public class ClientWorldPacketHandler(ILogFactory logFactory, IClientService clientService, IAuthenticateService authenticateService)
    {
        private readonly ILog log = logFactory.CreateLogger<ClientWorldPacketHandler>();
        public void CwServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void CwEnterWorldHandler(ISession session, IMessage packet)
        {
            if (packet is CwEnterWorld enterWorldpacket)
            {
                _ = authenticateService.TrySessionAuthenticateAsync(session, enterWorldpacket);
            }
        }
    }
}
