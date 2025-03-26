﻿using PacketGenerator.Extensions;
using System.Runtime.InteropServices;
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
                    string packetHandlerName = Path.GetFileName(file).Split(".")[0];

                    registerFormat += string.Format(PacketManagerFormat.managerRegisterFormat, enumName, packetName, packetHandlerName);
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
