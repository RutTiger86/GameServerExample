using AuthServer.Services;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Utill;

namespace AuthServer.Packets
{
    public class AuthDbPacketHandler(ILogFactory logFactory, IClientService clientService)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthDbPacketHandler>();
        private readonly IClientService clientService = clientService;
        public void DaServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void DaGetAccountVerifyInfoHandler(ISession session, IMessage packet)
        {
            if (packet is DaGetAccountVerifyInfo accountInfo)
            {
                _ = clientService.TryAccountVerifyAsync(session, accountInfo);
            }
        }
    }
}
