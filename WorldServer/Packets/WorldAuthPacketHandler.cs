using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Utill;
using WorldServer.Services;

namespace WorldServer.Packets
{
    public class WorldAuthPacketHandler(ILogFactory logFactory, IClientService clientService)
    {
        private readonly ILog log = logFactory.CreateLogger<WorldAuthPacketHandler>();
        private readonly IClientService clientService = clientService;
        public void AwServerStateHandler(ISession session, IMessage packet)
        {

        }
    }
}
