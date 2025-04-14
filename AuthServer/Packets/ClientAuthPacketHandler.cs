using AuthServer.Services;
using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.ClientAuth;
using Server.Utill;

namespace AuthServer.Packets
{
    public class ClientAuthPacketHandler(ILogFactory logFactory, IGameServerRegistry  gameServerRegistry, AuthDBSession authDBSession)
    {
        private readonly ILog log = logFactory.CreateLogger<ClientAuthPacketHandler>();
        public void CaServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void CaLoginHandler(ISession session, IMessage packet)
        {
            CaLogin? caLogin = packet as CaLogin;
            ClientSession? clientSession = session as ClientSession;

            if (caLogin == null || clientSession == null)
            {
                return;
            }

            clientSession.LoginInfo = new Models.Account.LoginInfo()
            {
                AccountId = caLogin.AccountId,
                Password = caLogin.Password
            };
            
            authDBSession.SendLoginRequest(caLogin.AccountId, clientSession.SessionId);
        }
        public void CaWorldListHandler(ISession session, IMessage packet)
        {
            ClientSession? clientSession = session as ClientSession;

            var gameServerList = gameServerRegistry.GetAllServerList();

            AcWorldList acWorldList = new AcWorldList();
            acWorldList.Servers.AddRange(gameServerList.Select(p => new ServerInfo
            {
                Name = p.Name,
                ServerId = p.ServerId,
                Status = p.Status
            }));

            clientSession?.Send(acWorldList);
        }
        public void CaEnterWorldHandler(ISession session, IMessage packet)
        {

        }
    }
}
