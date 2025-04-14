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

    public class ClientService(ILogFactory logFactory, ISessionManager<ClientSession> sessionManager, ConfigManager<AppConfig> configManager): IClientService
    {
        private readonly ILog log = logFactory.CreateLogger<ClientService>();
        private readonly PasswordHasher passwordHasher = new PasswordHasher(configManager.config!.Secure!.PBKDF2Iterations, configManager.config.Secure.HashSize);

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

            if (clientSession.LoginInfo != null && accountInfo.Id > 0)
            {
                bool verifyResult = await Task.Run(() =>
                {
                    return passwordHasher.VerifyPassword(
                        clientSession.LoginInfo.Password!,
                        accountInfo.Salt.ToByteArray(),
                        accountInfo.PasswordHash.ToByteArray());
                });

                acLogin.Result = verifyResult ? LoginDenyReason.None : LoginDenyReason.InvalidPassword;
            }


            //TODO : 서버 외부 오픈 상태 확인 (계정 권한 확인 ) 
            
            //TODO : 계정 제제 상태 확인 필요  


            // 로그인 불가시 세션 계정 정보 삭제 
            if(acLogin.Result != LoginDenyReason.None )
            {
                clientSession.LoginInfo = null;
            }

            clientSession.Send(acLogin);
        }
    }
}
