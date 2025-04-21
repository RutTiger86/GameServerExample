using DummyClient.Sessions;
using Server.Core;
using Server.Data.ClientWorld;
using System.Net;

namespace DummyClient.Service
{
    public class WorldService
    {
        private static readonly WorldService worldService = new WorldService();

        private static WorldSession? worldSession;

        public static WorldService Instance
        {
            get
            {
                return worldService;
            }
        }

        private Connector<WorldSession>? connector;
        public WorldService()
        {
        }

        public void SetWorldCommand(string command)
        {
            switch (command.ToLower())
            {
                case var cmd when cmd.StartsWith("world-enterworld"):
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        public async void ConnectWorld(string IpAddr, int port)
        {
            if (IPAddress.TryParse(IpAddr, out IPAddress? address))
            {
                worldSession = new WorldSession(IpAddr);
                connector = new Connector<WorldSession>(() => worldSession);
                await connector.StartConnectorAsync(new IPEndPoint(address, port));
            }
        }

        public void SendEnterWorld(string loginToken)
        {
            if (connector == null)
                return;
            if (worldSession == null)
                return;

            var loginPacket = new CwEnterWorld()
            {
                Token = loginToken
            };

            worldSession.Send(loginPacket);
        }

        public void DisconnectWorld()
        {
            connector?.Stop();
            connector = null;
        }

    }
}
