using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utill.Interface
{
    public interface IRedisSession
    {
        public Task RegisterSessionAsync(long sessionId, string ip, int port);
        public Task<long> GenerateSessionIdAsync();
    }
}
