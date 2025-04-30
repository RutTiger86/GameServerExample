using log4net;
using Server.Core.Interface;
using Server.Data.ClientWorld;
using Server.Utill;
using StackExchange.Redis;
using WorldServer.Session;

namespace WorldServer.Services
{
    public interface IAuthenticateService
    {
        public Task TrySessionAuthenticateAsync(ISession session, CwEnterWorld packet);
    }

    public class AuthenticateService(ILogFactory logFactory, IWorldRedisSession redisSession, ISessionManager<ClientSession> clientSessionManager) : IAuthenticateService
    {
        private readonly ILog log = logFactory.CreateLogger<ClientService>();


        public async Task TrySessionAuthenticateAsync(ISession session, CwEnterWorld packet)
        {
            if (packet == null || session is not ClientSession clientSession)
            {
                return ;
            }

            string token = packet.Token;

            // 1. 토큰 검증

            var sessionInfo =  await redisSession.GetSessionInfoWithIdByTokenAsync(token);

            if (sessionInfo.SessionId == null || sessionInfo.Entries == null)
            {
                log.Warn($"Invalid or expired token {clientSession.Ip} : {clientSession.Port}");
                clientSession.Send(new WcEnterWorld { Success = false, ErrorMessage = "Invalid or expired token" });
                clientSession.Close();
                return;
            }

            var sessionIdEntry = sessionInfo.Entries.FirstOrDefault(entry => entry.Name == "account_db_id");

            if (sessionIdEntry.Equals(new HashEntry()))
            {
                log.Warn($"account_id is empty: {clientSession.Ip} : {clientSession.Port}");
                clientSession.Send(new WcEnterWorld { Success = false, ErrorMessage = "Wrong Account Id" });
                clientSession.Close();
                return;
            }

            var accountId = (long)sessionIdEntry.Value;

            // 2. 인증된 세션으로 전환
            var sessionUpdaeResult = clientSessionManager.UpdateSessionId(clientSession.SessionId, (long)sessionInfo.SessionId);

            if (sessionUpdaeResult == false)
            {
                log.Warn($"sessionId Update fail : {clientSession.Ip} : {clientSession.Port}");
                clientSession.Send(new WcEnterWorld { Success = false, ErrorMessage = "Wrong Session Id" });
                clientSession.Close();
                return;
            }

            clientSession.Authenticate(accountId);
            clientSession.Send(new WcEnterWorld { Success = true });
            log.Info($"Session authenticated: {clientSession.SessionId}, Account: {accountId}");
        }
    }
}
