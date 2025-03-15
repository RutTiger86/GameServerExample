using AuthServer.Commons;
using log4net;

namespace AuthServer.Session
{
    public class SessionManager(ILogFactory logFactory)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthServer>();

        int sessionId = 0;
        private readonly Dictionary<int, ClientSession> sessions = [];
        private readonly object lockObj = new();

        public ClientSession Generate()
        {
            lock (lockObj)
            {
                int Id = ++sessionId;

                ClientSession session = new(logFactory);
                session.SessionId = sessionId;
                sessions.Add(Id, session);

                log.Info($"Connected : {sessionId}");

                return session;
            }
        }

        public ClientSession? Find(int id)
        {
            lock (lockObj)
            {
                sessions.TryGetValue(id, out ClientSession? session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (lockObj)
            {
                sessions.Remove(session.SessionId);
            }
        }
    }
}
