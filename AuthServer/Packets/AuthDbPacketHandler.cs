using AuthServer.Models.Configs;
using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Data.ClientAuth;
using Server.Utill;

namespace AuthServer.Packets
{
    public class AuthDbPacketHandler(ILogFactory logFactory, SessionManager<ClientSession, ClientAuthPacketManager> sessionManager, ConfigManager<AppConfig> configManager)
    {
        private readonly ILog log = logFactory.CreateLogger<AuthDbPacketHandler>();
        private readonly PasswordHasher passwordHasher = new PasswordHasher(configManager.config!.Secure!.PBKDF2Iterations, configManager.config.Secure.HashSize);

        public void DaServerStateHandler(ISession session, IMessage packet)
        {

        }

        public void DaGetAccountVerifyInfoHandler(ISession session, IMessage packet)
        {
            if (packet is DaGetAccountVerifyInfo accountInfo)
            {
                log.Info($"Id : {accountInfo.Id}, AccountID : {accountInfo.AccountId}, AccountName : {accountInfo}");

                var clientSession = sessionManager.Find(accountInfo.SessionId);

                if (clientSession != null)
                {
                    AcLogin acLogin = new()
                    {
                        Result = LoginDenyReason.AccountNotFound
                    };

                    if (accountInfo.Id > 0)
                    {
                        bool verifyResult = passwordHasher.VerifyPassword(clientSession.LoginInfo!.Password!, accountInfo.Salt.ToByteArray(), accountInfo.PasswordHash.ToByteArray());

                        acLogin = new AcLogin()
                        {
                            Result = verifyResult ? LoginDenyReason.None : LoginDenyReason.InvalidPassword,
                        };
                    }

                    clientSession.Send(acLogin);
                }
            }
        }
    }
}
