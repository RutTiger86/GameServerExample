using log4net;
using Server.Core;
using Server.Core.Interface;
using Server.Utill.Interface;
using System.Security.Cryptography.X509Certificates;

namespace Server.Utill
{
    public interface ISessionManager<TSession>
    {
        void SetCert(X509Certificate2 cert);
        void SetRedis(IRedisSession redisSession);

        Task<TSession> Generate(IPacketManager packetManager);
        TSession? Find(long id);
        void Remove(TSession session);

        bool UpdateSessionId(long oldId, long newId);
    }

    public class SessionManager<TSession>(ILogFactory logFactory) :ISessionManager<TSession>
        where TSession : ISession, ILogCreater<TSession>
    {
        private readonly ILog log = logFactory.CreateLogger<SessionManager<TSession>>();

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

        public async Task<TSession> Generate(IPacketManager packetManager)
        {
            long id = ++sessionId;
            if (redisSession != null)
            {
                id = await redisSession.GenerateSessionIdAsync();
            }
            TSession session = TSession.Create(logFactory, packetManager, cert, redisSession);
            session.SessionId = id;

            lock (lockObj)
            {
                sessions.Add(id, session);
            }

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

        public bool UpdateSessionId(long oldId, long newId)
        {
            lock (lockObj)
            {
                if (!sessions.TryGetValue(oldId, out var session))
                    return false;

                sessions.Remove(oldId);
                session.SessionId = newId;
                sessions[newId] = session;
                return true;
            }
        }
    }
}
