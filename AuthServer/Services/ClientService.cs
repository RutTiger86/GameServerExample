using AuthServer.Models.Configs;
using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Data.ClientAuth;
using Server.Utill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public interface IClientService
    {
        Task HandleAccountVerifyAsync(ISession session, DaGetAccountVerifyInfo packet);
    }

    public class ClientService(ILogFactory logFactory, SessionManager<ClientSession, ClientAuthPacketManager> sessionManager, ConfigManager<AppConfig> configManager): IClientService
    {
        private readonly ILog log = logFactory.CreateLogger<ClientService>();
        private readonly PasswordHasher passwordHasher = new PasswordHasher(configManager.config!.Secure!.PBKDF2Iterations, configManager.config.Secure.HashSize);
        private readonly SessionManager<ClientSession, ClientAuthPacketManager> sessionManager = sessionManager;

        public async Task HandleAccountVerifyAsync(ISession session, DaGetAccountVerifyInfo accountInfo)
        {
            log.Info($"Id : {accountInfo.Id}, AccountID : {accountInfo.AccountId}");

            var clientSession = sessionManager.Find(accountInfo.SessionId);
            if (clientSession == null)
                return;

            AcLogin acLogin = new()
            {
                Result = LoginDenyReason.AccountNotFound
            };

            if (accountInfo.Id > 0)
            {
                bool verifyResult = await Task.Run(() =>
                {
                    return passwordHasher.VerifyPassword(
                        clientSession.LoginInfo!.Password!,
                        accountInfo.Salt.ToByteArray(),
                        accountInfo.PasswordHash.ToByteArray());
                });

                acLogin.Result = verifyResult ? LoginDenyReason.None : LoginDenyReason.InvalidPassword;
            }

            clientSession.Send(acLogin);
        }
    }
}
