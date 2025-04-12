using Server.Core.Interface;
using System.Security.Cryptography.X509Certificates;

namespace Server.Utill.Interface
{
    public interface ILogCreater<TSession>
    {
        abstract static TSession Create(ILogFactory logFactory, IPacketManager packetManager, X509Certificate2? cert = null, IRedisSession? redisSession = null);
    }

}
