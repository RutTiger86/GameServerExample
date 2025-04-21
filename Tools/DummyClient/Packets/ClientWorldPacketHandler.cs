using Google.Protobuf;
using Server.Core.Interface;

namespace DummyClient.Packets
{
    public class ClientWorldPacketHandler()
    {
        public void WcServerStateHandler(ISession session, IMessage packet)
        {
        }

        public void WcEnterWorldHandler(ISession session, IMessage packet)
        {
        }
    }
}
