namespace WorldServer.Services
{
    public class CommandService
    {
        private Dictionary<string, Action<string[]>> commandMap = new();
        public CommandService() 
        {
            Initialize();
        }

        public void Initialize()
        {
            commandMap["help"] = Help;
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
            Console.WriteLine("- help");
            Console.WriteLine("- quit");
        }
    }
}
