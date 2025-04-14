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
            AcWorldList? acWorldList = packet as AcWorldList;
            AuthSession? authSession = session as AuthSession;

            if(acWorldList ==null)
            {
                Console.WriteLine($"WorldList Packet is Null!!");
                return;
            }

            foreach (var server in acWorldList.Worlds)
            {
                Console.WriteLine($"[World] [ {server.WorldId} ] {server.Name}  ({server.Status})");
            }
        }

        public void AcEnterWorldHandler(ISession session, IMessage packet)
        {
            AcEnterWorld? enterWorld = packet as AcEnterWorld;
            AuthSession? authSession = session as AuthSession;

            Console.WriteLine($"[EnterWorld] Result : {enterWorld?.Result}, Token : {enterWorld?.Token}");
        }

    }
}
