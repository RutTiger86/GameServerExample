using AuthServer.Models.Configs;
using AuthServer.Session;
using Google.Protobuf;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Server.Core.Interface;
using Server.Data.AuthDb;
using Server.Data.ClientAuth;
using Server.Data.WorldAuth;
using Server.Utill;
using Server.Utill.Interface;
using System.Security.Cryptography;

namespace AuthServer.Services
{
    public interface IClientService
    {
        Task TryAccountVerifyAsync(ISession session, DaGetAccountVerifyInfo packet);
        Task SendWorldList(ISession session, IMessage packet);
        Task TryLogin(ISession session, CaLogin packet);
        Task TryEnterWorld(ISession session, CaEnterWorld packet);
    }

    public class ClientService(ILogFactory logFactory, IServiceProvider serviceProvider) : IClientService
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILog log = logFactory.CreateLogger<ClientService>();

        public async Task TryAccountVerifyAsync(ISession session, DaGetAccountVerifyInfo accountInfo)
        {
            var configManager = serviceProvider.GetRequiredService<ConfigManager<AppConfig>>();
            var passwordHasher = new PasswordHasher(configManager.config!.Secure!.PBKDF2Iterations, configManager.config.Secure.HashSize);
            var sessionManager = serviceProvider.GetRequiredService<ISessionManager<ClientSession>>();
            var redisSession = serviceProvider.GetRequiredService<IAuthRedisSession>();

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

            if (acLogin.Result == LoginDenyReason.None)
            {
                await redisSession.UpdateSessionLoginInfoAsync(clientSession.SessionId, accountInfo.AccountId, accountInfo.Id, (int) SessionState.Authenticated);
            }
            else
            {
                clientSession.LoginInfo = null;
            }

            clientSession.Send(acLogin);
        }

        public async Task SendWorldList(ISession session, IMessage packet)
        {
            var sessionManager = serviceProvider.GetRequiredService<ISessionManager<ClientSession>>();
            var redisSession = serviceProvider.GetRequiredService<IAuthRedisSession>();
            ClientSession? clientSession = session as ClientSession;

            var gameServerList = await redisSession.GetAllWorldsAsync();

            AcWorldList acWorldList = new AcWorldList();
            acWorldList.Worlds.AddRange(gameServerList.Select(p => new WorldInfo
            {
                Name = p.Name,
                WorldId = p.WorldId,
                Status = p.Status
            }));

            clientSession?.Send(acWorldList);
        }

        public async Task TryLogin(ISession session, CaLogin packet)
        {
            if (packet == null || session is not ClientSession clientSession)
            {
                return ;
            }

            var redisSession = serviceProvider.GetRequiredService<IAuthRedisSession>();
            bool isExternally =  await redisSession.GetIsExternallyOpenAsync();
            if (isExternally)
            {

                var authDBSession = serviceProvider.GetRequiredService<AuthDBSession>();

                clientSession.LoginInfo = new Models.Account.LoginInfo()
                {
                    AccountId = packet.AccountId,
                    Password = packet.Password
                };

                authDBSession.SendLoginRequest(packet.AccountId, clientSession.SessionId);
            }
            else
            {
                AcLogin acLogin = new()
                {
                    Result = LoginDenyReason.InternalAccessOnly
                };

                clientSession.Send(acLogin);
            }            
        }

        public async Task TryEnterWorld(ISession session, CaEnterWorld packet)
        {
            var redisSession = serviceProvider.GetRequiredService<IAuthRedisSession>();
            ClientSession? clientSession = session as ClientSession;

            AcEnterWorld acEnterWorld = new AcEnterWorld()
            {
                Result = EnteredWorldDenyReason.None
            };

            var serverInfo =  await redisSession.GetWorldStateInfoAsync(packet.WorldId);

            if (serverInfo== null ||  serverInfo.IsVisible == false || serverInfo.Status == WorldState.Maintenance || serverInfo.Status == WorldState.Preparing)
            {
                acEnterWorld.Result = EnteredWorldDenyReason.WorldMaintenance;
            }

            if (acEnterWorld.Result == EnteredWorldDenyReason.None)
            {
                string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

                await redisSession.UpdateSessionTokenAsync(clientSession!.SessionId, token);
                acEnterWorld.Token = token;
            }

            clientSession?.Send(acEnterWorld);
        }
    }
}
