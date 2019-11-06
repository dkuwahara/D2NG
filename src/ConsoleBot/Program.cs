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
using D2NG.D2GS;

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
            var game = Client.Game;
            Log.Verbose($"Stash:\n\n{game.Stash}\n");
            var stashable = from item in game.Items
                            where item.Container == ContainerType.inventory
                            where item.Type != "tbk" && item.Type != "cm1" && item.Type != "cm2"
                            select item;

            if(game.Me.ActiveSkills[Hand.Right] != Skill.Telekinesis)
            {
                Log.Verbose("Changing skill to Telekinesis");
                game.SwitchSkill(Hand.Right, Skill.Telekinesis);
                Thread.Sleep(200);
            }

            Log.Verbose("Finding stash");
            var stash = game.WorldObjects.Values.First(wo => wo.Code == 267);
            Log.Verbose($"Stash found at {stash.Location}");

            Log.Verbose($"Attempting to move from {game.Me.Location} to {stash.Location}");

            var points = new []
            {
                new Point(5096, 5018),
                new Point(5100, 5025),
                new Point(5105, 5050),
                new Point(5114, 5069)
            };
            foreach (var point in points)
            {
                Log.Verbose($"Moving to {point}");
                game.MoveTo(point);
                Thread.Sleep(500);
            }
            game.RequestUpdate(game.Me.Id);
            Thread.Sleep(1000);
            Log.Verbose($"Moving to {stash.Location}");
            game.MoveTo(stash);
            Thread.Sleep(2000);
            game.RequestUpdate(game.Me.Id);
            Log.Verbose("Interacting with stash");
            game.Interact(stash);
            Thread.Sleep(500);

            foreach (Item item in stashable)
            {
                Log.Verbose($"Stashable [{item.Type}] {item.Name}");
                var location = game.Stash.FindFreeSpace(item);
                if (location != null)
                {
                    Log.Verbose($"Attempting to place {item.Name}, {item.Id,8:X} => {location}");
                    game.RemoveItemFromBuffer(item);
                    Thread.Sleep(500);
                    game.InsertItemToBuffer(item, location, ItemContainer.Stash);
                    Thread.Sleep(600);
                }
            }
            Log.Verbose($"Stash:\n\n{game.Stash}\n");
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
