using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.MCP.Packet
{
    internal class McpStartupRequestPacket : McpPacket
    {
        public McpStartupRequestPacket(uint mcpCookie, uint mcpStatus, List<byte> mcpChunk, string mcpUniqueName)
            : base(BuildPacket(
                (byte)Mcp.STARTUP,
                BitConverter.GetBytes(mcpCookie),
                BitConverter.GetBytes(mcpStatus),
                mcpChunk,
                Encoding.ASCII.GetBytes(mcpUniqueName)
                )
            )
        {
        }
    }
}