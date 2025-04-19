using PacketGenerator.Extensions;
using System.Text.RegularExpressions;

namespace PacketManagerGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string folderPath = $@"..\..\..\Protos";//$(ProjectDir)Protos
            string outputPath = $@"..\..\..\..\..\";//"$(SolutionDir)

            if (args.Length == 2)
            {
                folderPath = args[0];//$(ProjectDir)Protos
                outputPath = args[1];//"$(SolutionDir)
            }


            string[] protoFiles = Directory.GetFiles(folderPath, "*.proto", SearchOption.TopDirectoryOnly);

            var authProtoFiles = protoFiles
                .Where(file => Path.GetFileName(file).Contains("Auth", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            string authPattern = @"\b[A-Z]A_";

            foreach (string protoFile in authProtoFiles)
            {
                string fileName = Path.GetFileName(protoFile).Split(".")[0];
                string usingString = $"using AuthServer.Packets;{Environment.NewLine}using Server.Data.{fileName};{Environment.NewLine}";


                string authServerManagerText = string.Format(PacketManagerFormat.managerFormat, usingString, fileName, CrateFormatString(protoFile, authPattern));
                File.WriteAllText($@"{outputPath}\AuthServer\Packets\{fileName}PacketManager.cs", authServerManagerText);
            }

            var authDbProtoFiles = protoFiles
                .Where(file => Path.GetFileName(file).Contains("AuthDb", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            string authDbPattern = @"\bAD_";

            foreach (string protoFile in authDbProtoFiles)
            {
                string fileName = Path.GetFileName(protoFile).Split(".")[0];
                string usingString = $"using AuthDBServer.Packets;{Environment.NewLine}using Server.Data.{fileName};{Environment.NewLine}";

                string authServerManagerText = string.Format(PacketManagerFormat.managerFormat, usingString, fileName, CrateFormatString(protoFile, authDbPattern));
                File.WriteAllText($@"{outputPath}\AuthDBServer\Packets\{fileName}PacketManager.cs", authServerManagerText);
            }


            var clientProtoFiles = protoFiles
                .Where(file => Path.GetFileName(file).Contains("Client", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            string clientPattern = @"\b[A-Z]C_";

            foreach (string protoFile in clientProtoFiles)
            {
                string fileName = Path.GetFileName(protoFile).Split(".")[0];
                string usingString = $"using DummyClient.Packets;{Environment.NewLine}using Server.Data.{fileName};{Environment.NewLine}";

                string clientManagerText = string.Format(PacketManagerFormat.managerFormat, usingString, fileName, CrateFormatString(protoFile, clientPattern));
                File.WriteAllText($@"{outputPath}\Tools\DummyClient\Packets\{fileName}PacketManager.cs", clientManagerText);
            }


            var worldProtoFiles = protoFiles
               .Where(file => Path.GetFileName(file).Contains("World", StringComparison.OrdinalIgnoreCase))
               .ToArray();

            string worldPattern = @"\b[A-Z]W_";

            foreach (string protoFile in worldProtoFiles)
            {
                string fileName = Path.GetFileName(protoFile).Split(".")[0];
                string usingString = $"using WorldServer.Packets;{Environment.NewLine}using Server.Data.{fileName};{Environment.NewLine}";

                string worldManagerText = string.Format(PacketManagerFormat.managerFormat, usingString, fileName, CrateFormatString(protoFile, worldPattern));
                File.WriteAllText($@"{outputPath}\WorldServer\Packets\{fileName}PacketManager.cs", worldManagerText);
            }
        }

        public static string CrateFormatString(string file, string pattern)
        {
            string registerFormat = string.Empty;

            bool startParsing = false;
            string enumName = string.Empty;
            foreach (string line in File.ReadAllLines(file))
            {
                if (!startParsing && line.Contains("enum") && line.Contains("PacketId"))
                {
                    enumName = line.Replace("enum", "").Replace("{", "").Trim();
                    startParsing = true;
                    continue;
                }

                if (!startParsing)
                    continue;

                if (line.Contains("}"))
                    break;

                string[] names = line.Trim().Split("=");
                if (names.Length == 0)
                    continue;

                string name = names[0].TrimEnd();

                if (Regex.IsMatch(name, pattern))
                {
                    string packetName = name.ToPascalCase();

                    registerFormat += string.Format(PacketManagerFormat.managerRegisterFormat, enumName, packetName);
                }
            }

            return registerFormat; ;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
        }
    }
}
