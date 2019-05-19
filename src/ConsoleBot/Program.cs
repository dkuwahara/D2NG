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

            try
            {
                client.Bncs.OnReceivedPacketEvent(Sid.CHATEVENT, HandleChatEvent);
                client.Bncs.ConnectTo(config.Realm, config.ClassicKey, config.ExpansionKey);
                client.Bncs.Login(config.Username, config.Password);
                client.Bncs.EnterChat();

                var realms = client.Bncs.ListRealms();
                var realmPrompt = realms.Select((r, index) => $"{index + 1}. {r.Name} - {r.Description}")
                    .Aggregate((i, j) => i + "\n" + j);

                var realmSelection = Prompt.GetInt(realmPrompt) - 1;

                Log.Information($"Selected {realms[realmSelection]}");
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
