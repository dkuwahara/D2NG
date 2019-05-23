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

        public void OnReceivedPacketEvent(Sid sid, Action<BncsPacket> action) => Bncs.OnReceivedPacketEvent(sid, action);
        public void OnReceivedPacketEvent(Mcp mcp, Action<McpPacket> action) => Mcp.OnReceivedPacketEvent(mcp, action);

        public void OnSentPacketEvent(Sid sid, Action<BncsPacket> action) => Bncs.OnSentPacketEvent(sid, action);
        public void OnSentPacketEvent(Mcp mcp, Action<McpPacket> action) => Mcp.OnSentPacketEvent(mcp, action);

        public void Connect(string realm, string classicKey, string expansionKey) => Bncs.ConnectTo(realm, classicKey, expansionKey);

        public List<Character> Login(string username, string password)
        {
            Bncs.Login(username, password);
            Log.Information($"Logged in as {username}");
            var packet = Bncs.RealmLogon(Bncs.ListMcpRealms().First());
            Log.Information($"Connecting to {packet.McpIp}:{packet.McpPort}");
            Mcp.Connect(packet.McpIp, packet.McpPort);
            Mcp.Logon(packet.McpCookie, packet.McpStatus, packet.McpChunk, packet.McpUniqueName);
            Log.Information($"Connected to {packet.McpIp}:{packet.McpPort}");
            return Mcp.ListCharacters();
        }

        public void SelectCharacter(Character character)
        {
            Log.Information($"Selecting {character.Name}");
            Mcp.CharLogon(character);
            Log.Information("Entering Chat");
            Bncs.EnterChat();
        }
    }
}
