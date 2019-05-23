using D2NG.BNCS;
using D2NG.MCP;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; } = new BattleNetChatServer();
        public RealmServer Mcp { get; } = new RealmServer();

        public void Connect(string realm, string classicKey, string expansionKey) => Bncs.ConnectTo(realm, classicKey, expansionKey);

        public List<Character> Login(string username, string password)
        {
            Bncs.Login(username, password);
            Log.Information($"Logged in as {username}");
            var packet = Bncs.RealmLogon(Bncs.ListMcpRealms().First().Name);
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
            Log.Information($"Entering Chat");
            Bncs.EnterChat();
        }
    }
}
