using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public class CommandService
    {
        private Dictionary<string, Action<string[]>> commandMap = new();
        private readonly IWorldServerRegistry worldServerRegistry;
        public CommandService(IWorldServerRegistry worldServerRegistry) 
        {
            this.worldServerRegistry = worldServerRegistry;
            Initialize();
        }

        public void Initialize()
        {
            commandMap["help"] = Help;
            commandMap["externally-open"] = ExternallyOpen;
            commandMap["externally-close"] = ExternallyClose;
            commandMap["world-open"] = WorldOpen;
            commandMap["world-close"] = WorldClose;
            // 추가 가능
        }

        public bool Execute(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = args[0].ToLower();

            if (commandMap.TryGetValue(command, out var action))
            {
                action(args);
                return true;
            }

            return false;
        }

        static void Help(string[] args)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("- externally-open");
            Console.WriteLine("- externally-close");
            Console.WriteLine("- world-open {WorldId}");
            Console.WriteLine("- world-close {WorldId}");
            Console.WriteLine("- quit");
        }

        void ExternallyOpen(string[] args)
        {
            worldServerRegistry.SetIsExternallyOpen(true);
        }
        void ExternallyClose(string[] args)
        {
            worldServerRegistry.SetIsExternallyOpen(false);
        }

        void WorldOpen(string[] args)
        {
            var worldId = int.Parse(args[1]);
            worldServerRegistry.OpenWorld(worldId);
        }

        void WorldClose(string[] args)
        {
            var worldId = int.Parse(args[1]);
            worldServerRegistry.CloseWorld(worldId);
        }
    }
}
