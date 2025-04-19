using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Utill;
using WorldServer.Services;

namespace WorldServer.Packets
{
    public class ClientWorldPacketHandler(ILogFactory logFactory, IClientService clientService)
    {
        private readonly ILog log = logFactory.CreateLogger<ClientWorldPacketHandler>();
        private readonly IClientService clientService = clientService;
        public void CwServerStateHandler(ISession session, IMessage packet)
        {

        }
    }
}
