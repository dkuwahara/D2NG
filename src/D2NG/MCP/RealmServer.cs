using System.Collections.Generic;
using System.Net;
using D2NG.MCP.Packet;
using Serilog;

namespace D2NG.MCP
{
    public class RealmServer
    {
        private McpConnection Connection { get; } = new McpConnection();

        internal RealmServer()
        {

        }

        internal void Connect(IPAddress ip, short port)
        {
            Connection.Connect(ip, port);
        }

        internal void Logon(uint mcpCookie, uint mcpStatus, List<byte> mcpChunk, string mcpUniqueName)
        {
            var packet = new McpStartupRequestPacket(mcpCookie, mcpStatus, mcpChunk, mcpUniqueName);
            Connection.WritePacket(packet);

            var response = new McpStartupResponsePacket(Connection.ReadPacket());
        }
    }
}