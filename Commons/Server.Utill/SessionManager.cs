using log4net;
using Server.Core;
using Server.Utill;
using Server.Utill.Interface;

namespace Server.Utill
{
    public class SessionManager<TSession>(ILogFactory logFactory) where TSession : Session, ILogCreater<TSession>
    {
        private readonly ILog log = logFactory.CreateLogger<SessionManager<TSession>>();

        int sessionId = 0;
        private readonly Dictionary<int, TSession> sessions = [];
        private readonly object lockObj = new();

        public TSession Generate()
        {
            lock (lockObj)
            {
                int Id = ++sessionId;

                TSession session = TSession.Create(logFactory);
                session.SessionId = sessionId;
                sessions.Add(Id, session);

                log.Info($"Connected : {sessionId}");

                return session;
            }
        }

        public TSession? Find(int id)
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
