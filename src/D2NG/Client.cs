using D2NG.BNCS;
using D2NG.BNCS.Packet;
using D2NG.D2GS;
using D2NG.MCP;
using D2NG.MCP.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2NG
{
    public class Client
    {
        internal BattleNetChatServer Bncs { get; } = new BattleNetChatServer();
        internal RealmServer Mcp { get; } = new RealmServer();
        internal GameServer D2gs { get; } = new GameServer();

        public Chat Chat { get; }

        private Character _character;
        private string _mcpRealm;

        public Client()
        {
            Chat = new Chat(Bncs);
        }

        public void OnReceivedPacketEvent(Sid sid, Action<BncsPacket> action) => Bncs.OnReceivedPacketEvent(sid, action);
        public void OnReceivedPacketEvent(Mcp mcp, Action<McpPacket> action) => Mcp.OnReceivedPacketEvent(mcp, action);

        public void OnSentPacketEvent(Sid sid, Action<BncsPacket> action) => Bncs.OnSentPacketEvent(sid, action);
        public void OnSentPacketEvent(Mcp mcp, Action<McpPacket> action) => Mcp.OnSentPacketEvent(mcp, action);

        /// <summary>
        /// Connect to a Battle.net Realm
        /// </summary>
        /// <param name="realm">Realm to connect to. e.g. useast.battle.net </param>
        /// <param name="classicKey">26-character Diablo II Classic CD Key</param>
        /// <param name="expansionKey">26-character Diablo II: Lord of Destruction CD Key</param>
        public void Connect(string realm, string classicKey, string expansionKey) => Bncs.ConnectTo(realm, classicKey, expansionKey);

        /// <summary>
        /// Login to Battle.Net with credentials and receive the list of available characters to select.
        /// </summary>
        /// <param name="username">Account name</param>
        /// <param name="password">Password used to login</param>
        /// <returns>A list of Characters associated with the account</returns>
        public List<Character> Login(string username, string password)
        {
            Bncs.Login(username, password);
            Log.Information($"Logged in as {username}");
            RealmLogon();
            return Mcp.ListCharacters();
        }

        private void RealmLogon()
        {
            if (_mcpRealm is null)
            {
                _mcpRealm = Bncs.ListMcpRealms().First();
            }
            var packet = Bncs.RealmLogon(_mcpRealm);
            Log.Information($"Connecting to {packet.McpIp}:{packet.McpPort}");
            Mcp.Connect(packet.McpIp, packet.McpPort);
            Mcp.Logon(packet.McpCookie, packet.McpStatus, packet.McpChunk, packet.McpUniqueName);
            Log.Information($"Connected to {packet.McpIp}:{packet.McpPort}");
        }

        /// <summary>
        /// Select one of the available characters on the account.
        /// </summary>
        /// <param name="character">Character with name matching one of the account characters</param>
        public void SelectCharacter(Character character)
        {
            Log.Information($"Selecting {character.Name}");
            Mcp.CharLogon(character);
            _character = character;
        }

        /// <summary>
        /// Create a new game 
        /// </summary>
        /// <param name="difficulty">One of Normal, Nightmare or Hell</param>
        /// <param name="name">Name of the game to be created</param>
        /// <param name="password">Password used to protect the game</param>
        public void CreateGame(Difficulty difficulty, string name, string password)
        {
            Log.Information($"Creating {difficulty} game: {name}");
            Mcp.CreateGame(difficulty, name, password);
            Log.Debug($"Game {name} created");
            JoinGame(name, password);
        }

        /// <summary>
        /// Join a game
        /// </summary>
        /// <param name="name">Name of the game being joined</param>
        /// <param name="password">Password used to protect the game</param>
        public void JoinGame(string name, string password)
        {
            Log.Information($"Joining game: {name}");
            var packet = Mcp.JoinGame(name, password);
            //Mcp.Disconnect();
            Log.Debug($"Connecting to D2GS Server {packet.D2gsIp}");
            D2gs.Connect(packet.D2gsIp);
            D2gs.GameLogon(packet.GameHash, packet.GameToken, _character);
            Bncs.NotifyJoin(name, password);
        }

        /// <summary>
        /// Leave a game
        /// </summary>
        public void LeaveGame()
        {
            Log.Information("Leaving game.");
            D2gs.LeaveGame();
            Bncs.LeaveGame();
            RealmLogon();
            Mcp.CharLogon(_character);
        }
    }
}
