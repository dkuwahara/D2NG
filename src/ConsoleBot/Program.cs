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

        private readonly Client Client = new Client();

        private Config Config;

        private void OnExecute()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt")
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Starting bot");
            
            Config = Config.FromFile(this.ConfigFile);

            Client.Bncs.OnReceivedPacketEvent(Sid.CHATEVENT, HandleChatEvent);

            try {

                var realm = Prompt.GetString($"Realm:", Config.Realm, ConsoleColor.Red);

                Client.Bncs.ConnectTo(realm, Config.ClassicKey, Config.ExpansionKey);

                var username = Prompt.GetString("Username: ", Config.Username, ConsoleColor.Red);
                var password = Prompt.GetPassword("Password: ", ConsoleColor.Red);
                
                Client.Bncs.Login(username, password);
                Client.Bncs.EnterChat();

                var mcpRealm = SelectMcpRealm();
                Client.McpLogon(mcpRealm);

                _ = SelectCharacter();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
        }


        private string SelectMcpRealm()
        {
            var realms = Client.ListMcpRealms();
            var realmPrompt = realms.Select((r, index) => $"{index + 1}. {r.Name} - {r.Description}")
                .Aggregate((i, j) => i + "\n" + j);

            var realmSelection = Prompt.GetInt($"Select MCP Realm:\n{realmPrompt}\n", 1, ConsoleColor.Red) - 1;
            var selection = realms[realmSelection];
            Log.Information($"Selected {selection}");
            return selection.Name;
        }

        private string SelectCharacter()
        {
            var characters = Client.Mcp.ListCharacters();

            var charsPrompt = characters.Select((r, index) => $"{index + 1}. {r.Name} - {r.Statstring}")
                .Aggregate((i, j) => i + "\n" + j);

            var charSelection = Prompt.GetInt($"Select Character:\n{charsPrompt}\n", 1, ConsoleColor.Red) - 1;

            Log.Information($"Selected {characters[charSelection].Name}");
            return characters[charSelection].Name;
        }
             
        private static void HandleChatEvent(BncsPacketReceivedEvent obj)
        {
            _ = new ChatEventPacket(obj.Packet.Raw);
        }
    }
}
