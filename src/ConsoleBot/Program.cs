using D2NG;
using Serilog;
using System;
using McMaster.Extensions.CommandLineUtils;
using D2NG.BNCS.Packet;
using System.Linq;
using D2NG.MCP;
using System.Collections.Generic;
using Serilog.Events;
using System.Threading;
using System.Diagnostics;
using D2NG.D2GS.Items;

namespace ConsoleBot
{
    class Program
    {
        static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Config File", LongName = "config", ShortName = "c")]
        public string ConfigFile { get; }

        [Option]
        public bool Verbose { get; set; }

        private readonly Client Client = new Client();

        private Config Config;

        private LogEventLevel LogLevel() => Verbose ? LogEventLevel.Verbose: LogEventLevel.Debug;

        private void OnExecute()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Is(LogLevel())
                .CreateLogger();

            if (ConfigFile != null)
            {
                Config = Config.FromFile(this.ConfigFile);
            }

            Client.OnReceivedPacketEvent(Sid.CHATEVENT, HandleChatEvent);

            try {
                Client.Connect(
                    Prompt.GetString($"Realm:", Config.Realm, ConsoleColor.Green),
                    Prompt.GetString($"Classic Key:", Config.ClassicKey, ConsoleColor.Blue),
                    Prompt.GetString($"Expansion Key:", Config.ExpansionKey, ConsoleColor.Blue));

                var characters = Client.Login(
                    Prompt.GetString("Username: ", Config.Username, ConsoleColor.Green), 
                    Prompt.GetPassword("Password: ", ConsoleColor.Red));

                Client.SelectCharacter(SelectCharacter(characters));

                Client.Chat.EnterChat();

                Client.Chat.JoinChannel("D2NG");

                Thread.Sleep(5_000);
                while(true)
                {
                    var time = Stopwatch.StartNew();
                    Client.CreateGame(Difficulty.Normal, $"d2ng{new Random().Next(1000)}", "d2ng");
                    Log.Information("In game");

                    // Wait for game load

                    // Stash Items
                    StashItems();

                    // Move to Act 5
                    
                    // Malah
                    
                    // Revive Merc
                    
                    // Kill Pindle

                    // Pickup Items

                    Thread.Sleep(30_000);

                    Client.Game.LeaveGame();
                    time.Stop();
                    Log.Information($"Game took: {time.Elapsed.TotalSeconds} seconds");
                    Thread.Sleep(30_000);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
            }
        }

        private void StashItems()
        {
            Log.Verbose($"Current Stash:\n\n{Client.Game.Stash}\n");
            var stashable = from item in Client.Game.Items
                            where item.container == Item.ContainerType.inventory
                            where item.Type != "tbk" && item.Type != "cm1" && item.Type != "cm2"
                            select item;

            Client.Game.SwitchSkill(D2NG.D2GS.Skill.telekinesis);
            Thread.Sleep(200);

            foreach (Item item in stashable)
            {
                Log.Verbose($"Stashable [{item.Type}] {item.Name}");
                var location = Client.Game.Stash.FindFreeSpace(item);
                if (location != null)
                {
                    Log.Verbose($"Attempting to place {item.Name}, {item.id,8:X} => {location}");
                    Client.Game.RemoveItemFromBuffer(item);
                    Thread.Sleep(500);
                    Client.Game.InsertItemToBuffer(item, location, Item.ItemContainer.Stash);
                    Thread.Sleep(600);
                    
                }
            }
            Log.Verbose($"New Stash:\n\n{Client.Game.Stash}\n");
        }

        private static Character SelectCharacter(List<Character> characters)
        {
            var charsPrompt = characters
                .Select((c, index) => $"{index + 1}. {c.Name} - Level {c.Level} {c.Class}")
                .Aggregate((i, j) => i + "\n" + j);

            return characters[Prompt.GetInt($"Select Character:\n{charsPrompt}\n", 1, ConsoleColor.Green) - 1];
        }

        private static void HandleChatEvent(BncsPacket obj)
        {
            var packet = new ChatEventPacket(obj.Raw);
            if (packet.Eid != Eid.SHOWUSER)
            {
                Log.Information(packet.RenderText());
            }
        }
    }
}
