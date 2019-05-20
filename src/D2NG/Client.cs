using D2NG.BNCS;
using Serilog;
using System.Collections.Generic;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; }

        public Client()
        {
            Bncs = new BattleNetChatServer();
        }

        public List<(string Name, string Description)> ListRealms() => this.Bncs.ListRealms();


        public void RealmLogon(string name)
        {
            var packet = Bncs.RealmLogon(name);
            Log.Information($"Connecting to {packet.McpIp}:{packet.McpPort}");
            Log.Information($"Battle.net Unique Name: {packet.McpUniqueName}");
        }
    }
}
