using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Utill;
using WorldServer.Services;

namespace WorldServer.Packets
{
    public class WorldDbPacketHandler(ILogFactory logFactory, IClientService clientService)
    {
        private readonly ILog log = logFactory.CreateLogger<WorldDbPacketHandler>();
        private readonly IClientService clientService = clientService;
        public void DwServerStateHandler(ISession session, IMessage packet)
        {

        }
    }
}
