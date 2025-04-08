﻿using AuthDBServer.Repositories;
using AuthDBServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Utill;

namespace AuthDBServer.Packets
{
    public class AuthDbPacketHandler(ILogFactory logFactory, IAuthRepository authRepository)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthDBServer>();
        private readonly IAuthRepository authRepository = authRepository;

        public void AdServerStateHandler(ISession session, IMessage packet)
        {
        }

        public async void AdGetAccountVerifyInfoHandler(ISession session, IMessage packet)
        {
            AdGetAccountVerifyInfo? adGetAccountVerifyInfo = packet as AdGetAccountVerifyInfo;
            AuthSession? authSession = session as AuthSession;

            if (adGetAccountVerifyInfo == null || authSession == null)
            {
                log.Error("[ERROR] Wrong packet or Session by [AdGetAccountVerifyInfoHandler]");
                return;
            }

            var account = await authRepository.GetAccountByAccountId(adGetAccountVerifyInfo.AccountId);

            DaGetAccountVerifyInfo response = new DaGetAccountVerifyInfo()
            {
                SessionId = adGetAccountVerifyInfo.SessionId,
                AccountId = account?.AccountId,
                AccountName = account?.AccountName,
                Id = account == null ? -1 : account.Id,
                LoginType = account == null ? 0 : (int)account.LoginType,
                PasswordHash = account?.PasswordHash != null ? ByteString.CopyFrom(account.PasswordHash) : ByteString.Empty,
                Salt = account?.Salt != null ? ByteString.CopyFrom(account.Salt) : ByteString.Empty
            };

            authSession.Send(response);
        }
    }
}
