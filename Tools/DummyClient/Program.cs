using DummyClient.Handlers;
using DummyClient.Service;
using System.Text;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandHandler.Initialize(); // 명령어 핸들러 초기화

            Console.WriteLine("GameServer Test Dummy Client");
            Console.WriteLine("Enter command: [quit] or [help]");

            while (true)
            {
                string? command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command)) continue;

                if (command.Equals("quit", StringComparison.OrdinalIgnoreCase)) break;

                if (!CommandHandler.Execute(command))
                {
                    Console.WriteLine("Unknown command. Type 'help' for available commands.");
                }

                Console.WriteLine("------------------------------------------------------------");
            }
        }
    }
}
