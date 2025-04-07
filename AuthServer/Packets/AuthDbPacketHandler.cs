using AuthServer.Session;
using Google.Protobuf;
using Server.Core;
using Server.Data.AuthDb;
using Server.Data.ClientAuth;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Packets
{
    public class AuthDbPacketHandler(SessionManager<ClientSession, ClientAuthPacketManager> sessionManager)
    {
        private readonly SessionManager<ClientSession, ClientAuthPacketManager>  sessionManager = sessionManager;

        public void DaServerStateHandler(PacketSession session, IMessage packet)
        {

        }

        public void DaGetAccountVerifyInfoHandler(PacketSession session, IMessage packet)
        {
            if (packet is DaGetAccountVerifyInfo accountInfo)
            {
                Console.WriteLine($"Id : {accountInfo.Id}, AccountID : {accountInfo.AccountId}, AccountName : {accountInfo}, PW : {accountInfo.PasswordHash}");

                var clientSession =  sessionManager.Find(accountInfo.SessionId);

                if (clientSession != null)
                {
                    AcLogin acLogin = new AcLogin()
                    {
                        Result = LoginDenyReason.None
                    };

                    clientSession.Send(acLogin);
                }
            }
            else
            {
            }


        }
    }
}
