using log4net;
using Server.Core.Interface;
using Server.Utill.Interface;
using System.Security.Cryptography.X509Certificates;

namespace Server.Utill
{
    public class SessionManager<TSession, TPacketManager>(ILogFactory logFactory, TPacketManager packetManager)
        where TSession : ISession, ILogCreater<TSession>
        where TPacketManager : IPacketManager
    {
        private readonly ILog log = logFactory.CreateLogger<SessionManager<TSession, TPacketManager>>();
        private readonly IPacketManager packetManager = packetManager;

        long sessionId = 0;
        private readonly Dictionary<long, TSession> sessions = [];
        private readonly object lockObj = new();

        private X509Certificate2? cert = null;
        private IRedisSession? redisSession = null;

        public void SetCert(X509Certificate2 cert)
        {
            this.cert = cert;
        }

        public void SetRedis(IRedisSession redisSession)
        {
            this.redisSession = redisSession;
        }

        public async Task<TSession> Generate()
        {
            long id = ++sessionId;
            if (redisSession != null)
            {
                id = await redisSession.GenerateSessionIdAsync();
            }

            TSession session = TSession.Create(logFactory, packetManager, cert, redisSession);
            session.SessionId = sessionId;

            lock (lockObj)
            {
                sessions.Add(id, session);
            }

            log.Info($"Connected : {sessionId}");
            return session;
        }

        public TSession? Find(long id)
        {
            lock (lockObj)
            {
                sessions.TryGetValue(id, out TSession? session);
                return session;
            }
        }

        public void Remove(TSession session)
        {
            lock (lockObj)
            {
                sessions.Remove(session.SessionId);
            }
        }
    }
}
