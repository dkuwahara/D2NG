using D2NG;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using D2NG.BNCS.Packet;

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
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
            Log.Debug("Waiting for input");
            Console.ReadKey(false);
        }

        private void HandleChatEvent(BncsPacketReceivedEvent obj)
        {
            var chatEvent = new ChatEventPacket(obj.Packet.Raw);
            switch (chatEvent.Eid)
            {
                case Eid.JOIN:
                    Console.WriteLine($"{chatEvent.Username} has joined the channel");
                    break;
                case Eid.LEAVE:
                    Console.WriteLine($"{chatEvent.Username} has left the channel");
                    break;
                case Eid.WHISPER:
                    Console.WriteLine($"From <{chatEvent.Username}>: {chatEvent.Text}");
                    break;
                case Eid.TALK:
                    Console.WriteLine($"<{chatEvent.Username}>: {chatEvent.Text}");
                    break;
                case Eid.CHANNEL:
                    Console.WriteLine($"Joined channel: {chatEvent.Text}");
                    break;
                case Eid.INFO:
                    Console.WriteLine($"INFO: {chatEvent.Text}");
                    break;
                case Eid.ERROR:
                    Console.WriteLine($"ERROR: {chatEvent.Text}");
                    break;
                case Eid.EMOTE:
                    Console.WriteLine($"<{chatEvent.Username} {chatEvent.Text}>");
                    break;
                default:
                    break;
            }
        
        }
    }
}
