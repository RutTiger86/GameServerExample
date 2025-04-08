using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Interface
{
    public interface IPacketSession
    {
        // Span 기반 패킷 처리 (Zero-Copy 가능)
        public abstract void OnRecvPacket(ReadOnlyMemory<byte> buffer);
    }
}
