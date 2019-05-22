using D2NG.BNCS;
using D2NG.MCP;
using Serilog;
using System.Collections.Generic;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; } = new BattleNetChatServer();
        public RealmServer Mcp { get; } = new RealmServer();

        public List<BNCS.Packet.Realm> ListMcpRealms() => this.Bncs.ListMcpRealms();

        public void McpLogon(string name)
        {
            var packet = Bncs.RealmLogon(name);
            Log.Information($"Connecting to {packet.McpIp}:{packet.McpPort}");
            Mcp.Connect(packet.McpIp, packet.McpPort);
            Mcp.Logon(packet.McpCookie, packet.McpStatus, packet.McpChunk, packet.McpUniqueName);
            Log.Information($"Connected to {packet.McpIp}:{packet.McpPort}");
        }

        public void SelectCharacter(Character character)
        {
            Log.Information($"Selecting {character.Name}");
            Mcp.CharLogon(character);
            Log.Information($"Selected {character.Name}");
        }
    }
}
