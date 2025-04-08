using DummyClient.Sessions;
using Google.Protobuf;
using Server.Core.Interface;
using Server.Data.ClientAuth;

namespace DummyClient.Packets
{
    public class ClientAuthPacketHandler()
    {
        public void AcServerStateHandler(ISession session, IMessage packet)
        {
        }

        public void AcLoginHandler(ISession session, IMessage packet)
        {
            AcLogin? acLogin = packet as AcLogin;
            AuthSession? authSession = session as AuthSession;

            Console.WriteLine($"LoginResult :  {acLogin?.Result}");
        }

        public void AcWorldListHandler(ISession session, IMessage packet)
        {
        }

        public void AcEnterWorldHandler(ISession session, IMessage packet)
        {
        }

    }
}
