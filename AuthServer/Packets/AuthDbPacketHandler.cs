using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core;
using Server.Core.Interface;
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
    public class AuthDbPacketHandler(ILogFactory logFactory, SessionManager<ClientSession, ClientAuthPacketManager> sessionManager)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthDbPacketHandler>();

        private readonly SessionManager<ClientSession, ClientAuthPacketManager>  sessionManager = sessionManager;

        public void DaServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void DaGetAccountVerifyInfoHandler(ISession session, IMessage packet)
        {
            if (packet is DaGetAccountVerifyInfo accountInfo)
            {
                log.Info($"Id : {accountInfo.Id}, AccountID : {accountInfo.AccountId}, AccountName : {accountInfo}, PW : {accountInfo.PasswordHash}");

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
