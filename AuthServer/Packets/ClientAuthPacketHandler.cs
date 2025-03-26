using AuthServer.Session;
using Google.Protobuf;
using Server.Core;
using Server.Data.ClientAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Packets
{
    internal class ClientAuthPacketHandler
    {
        public static void CaServerStateHandler(PacketSession session, IMessage packet)
        {

        }
        public static void CaLoginHandler(PacketSession session, IMessage packet)
        {
            CaLogin caLogin = packet as CaLogin;
            ClientSession clientSession = session as ClientSession;

            Console.WriteLine($"SessionId : {clientSession.SessionId}, ID : {caLogin.UserId}, PW : {caLogin.HashPassword}");
            //TODO : AuthDB 서버로 넘겨서 DB 조회 후 검증 처리, 검증결과 Client로 전송 

        }
        public static void CaWorldListHandler(PacketSession session, IMessage packet)
        {

        }
        public static void CaEnterWorldHandler(PacketSession session, IMessage packet)
        {

        }
    }
}
