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
            
                client.Bncs.ConnectTo(config.Realm, config.ClassicKey, config.ExpansionKey);

                var username = Prompt.GetString("Username: ", null, ConsoleColor.Red);
                var password = Prompt.GetPassword("Password: ", ConsoleColor.Red);
                
                client.Bncs.Login(username, password);
                client.Bncs.EnterChat();

                var realms = client.ListRealms();
                var realmPrompt = realms.Select((r, index) => $"{index + 1}. {r.Name} - {r.Description}")
                    .Aggregate((i, j) => i + "\n" + j);

                var realmSelection = Prompt.GetInt($"Select Realm:\n{realmPrompt}", 1, ConsoleColor.Red) - 1;

                Log.Information($"Selected {realms[realmSelection]}");

                client.RealmLogon(realms[realmSelection].Name);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
            Log.Debug("Waiting for input");
            Console.ReadKey(false);
        }

        private static void HandleChatEvent(BncsPacketReceivedEvent obj)
        {
            _ = new ChatEventPacket(obj.Packet.Raw);
        }
    }
}
