using D2NG.BNCS;
using D2NG.BNCS.Packet;
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

        public Chat Chat { get; }

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
            var packet = Bncs.RealmLogon();
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
            Log.Information("Entering Chat");
            Bncs.EnterChat();
        }
    }
}
