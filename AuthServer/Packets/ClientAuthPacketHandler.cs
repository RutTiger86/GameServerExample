using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.ClientAuth;
using Server.Utill;

namespace AuthServer.Packets
{
    public class ClientAuthPacketHandler(ILogFactory logFactory)
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

            //TODO : AuthDB 서버로 넘겨서 DB 조회 후 검증 처리, 검증결과 Client로 전송 
            if (AuthDBSession.Instance != null)
            {
                AuthDBSession.Instance.SendLoginRequest(caLogin.AccountId, clientSession.SessionId);
            }
        }
        public void CaWorldListHandler(ISession session, IMessage packet)
        {

        }
        public void CaEnterWorldHandler(ISession session, IMessage packet)
        {

        }
    }
}
