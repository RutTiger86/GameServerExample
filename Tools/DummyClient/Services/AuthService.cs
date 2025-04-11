using DummyClient.Sessions;
using Server.Core;
using Server.Data.ClientAuth;
using System.Net;

namespace DummyClient.Service
{
    public class AuthService
    {
        private static readonly AuthService authService = new AuthService();

        private static AuthSession? authSession;

        public static AuthService Instance
        {
            get
            {
                return authService;
            }
        }

        private Connector<AuthSession>? connector;
        public AuthService()
        {
        }

        public void SetAuthCommand(string command)
        {
            switch (command.ToLower())
            {
                case var cmd when cmd.StartsWith("auth-longin"):
                    break;
                case var cmd when cmd.StartsWith("auth-worldlist"):
                    break;
                case var cmd when cmd.StartsWith("auth-enterworld"):
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        public async void ConnectAuth(string IpAddr, int port)
        {
            if (IPAddress.TryParse(IpAddr, out IPAddress? address))
            {
                authSession = new AuthSession(IpAddr);
                connector = new Connector<AuthSession>(() => authSession);
                await connector.StartConnectorAsync(new IPEndPoint(address, port));
            }
        }

        public void SendLogin(string id, string password)
        {
            if (connector == null)
                return;
            if (authSession == null)
                return;

            var loginPacket = new CaLogin()
            {
                AccountId = id,
                Password = password
            };

            authSession.Send(loginPacket);
        }

        public void SendWorldList()
        {
            if (connector == null)
                return;
            if (authSession == null)
                return;

            var worldList = new CaWorldList();

            authSession.Send(worldList);
        }

        public void SendEnterWorld(string worldId)
        {

        }

        public void DisconnectAuth()
        {
            connector?.Stop();
            connector = null;
        }

    }
}
