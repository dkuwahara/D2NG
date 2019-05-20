using D2NG;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using D2NG.BNCS.Packet;
using System.Linq;

namespace ConsoleBot
{
    class Program
    {
        static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Config File", LongName = "config", ShortName = "c")]
        [Required]
        public String ConfigFile { get; }

        private void OnExecute()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt")
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Starting bot");
            Client client = new Client();

            Config config = Config.FromFile(this.ConfigFile);

            client.Bncs.OnReceivedPacketEvent(Sid.CHATEVENT, HandleChatEvent);

            try {
                
                var bncsRealmPrompt = client.BncsRealms.Select((r, index) => $"{index + 1}. {r}")
                    .Aggregate((i, j) => i + "\n" + j);

                var bncsRealmSelection = Prompt.GetInt($"Select Realm:\n{bncsRealmPrompt}\n", 1, ConsoleColor.Red) - 1;

                client.Bncs.ConnectTo(client.BncsRealms[bncsRealmSelection], config.ClassicKey, config.ExpansionKey);

                var username = Prompt.GetString("Username: ", null, ConsoleColor.Red);
                var password = Prompt.GetPassword("Password: ", ConsoleColor.Red);
                
                client.Bncs.Login(username, password);
                client.Bncs.EnterChat();

                var realms = client.ListMcpRealms();
                var realmPrompt = realms.Select((r, index) => $"{index + 1}. {r.Name} - {r.Description}")
                    .Aggregate((i, j) => i + "\n" + j);

                var realmSelection = Prompt.GetInt($"Select Realm:\n{realmPrompt}\n", 1, ConsoleColor.Red) - 1;

                Log.Information($"Selected {realms[realmSelection]}");

                client.McpLogon(realms[realmSelection].Name);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
        }

        private static void HandleChatEvent(BncsPacketReceivedEvent obj)
        {
            _ = new ChatEventPacket(obj.Packet.Raw);
        }
    }
}
