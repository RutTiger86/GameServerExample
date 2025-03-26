using DummyClient.Sessions;
using Server.Core;
using Server.Data.ClientAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        public void SetAuthCommand(string command)
        {
            switch (command.ToLower())
            {
                case var cmd when cmd.StartsWith("auth-longin"):
                    break;
                case var cmd when cmd.StartsWith("auth-enterworld"):
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        public async void ConnectAuth(string IpAddr , int port)
        {
            if (IPAddress.TryParse(IpAddr, out IPAddress? address))
            {
                authSession = new AuthSession();
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
                UserId = id,
                HashPassword = password
            };

            authSession.Send(loginPacket);
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
