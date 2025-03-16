using DummyClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Handlers
{
    public static class CommandHandler
    {
        private static Dictionary<string, Action<string[]>> commandMap = new();

        public static void Initialize()
        {
            commandMap["help"] = Help;
            commandMap["connect-auth"] = ConnectAuth;
            commandMap["disconnect-auth"] = DisConnectAuth;
            commandMap["auth-login"] = AuthLogin;
            commandMap["auth-enterworld"] = AuthEnterWorld;
            // 추가 가능
        }

        public static bool Execute(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = args[0].ToLower();

            if (commandMap.TryGetValue(command, out var action))
            {
                action(args);
                return true;
            }

            // auth- 같은 접두사 명령
            if (command.StartsWith("auth-"))
            {
                AuthService.Instance.SetAuthCommand(input);
                return true;
            }

            return false;
        }

        static void Help(string[] args)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("- connect-auth {IP} {Port}");
            Console.WriteLine("- auth-login {ID} {Password}");
            Console.WriteLine("- auth-enterworld {WorldId}");
            Console.WriteLine("- disconnect-auth");
            Console.WriteLine("- quit");
        }

        static void ConnectAuth(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: connect-auth {IP} {Port}");
                return;
            }

            if (int.TryParse(args[2], out int port))
            {
                AuthService.Instance.ConnectAuth(args[1], port);
                Console.WriteLine("ConnectAuth Success");
            }
            else
            {
                Console.WriteLine("Invalid port number.");
            }
        }
        static void DisConnectAuth(string[] args)
        {
            AuthService.Instance.DisconnectAuth();
            Console.WriteLine("DisConnectAuth Success");
        }

        static void AuthLogin(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: auth-login {ID} {Password}");
                return;
            }
            AuthService.Instance.SendLogin(args[1], args[2]);
        }

        static void AuthEnterWorld(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: auth-enterworld {WorldId}");
                return;
            }
            AuthService.Instance.SendEnterWorld(args[1]);
        }
    }

}
