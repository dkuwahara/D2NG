using System;
using System.Text;

namespace D2NG.MCP.Packet
{
    internal class JoinGameRequestPacket : McpPacket
    {
        public JoinGameRequestPacket(ushort id, string name, string password) : 
            base(
                BuildPacket(
                    Mcp.JOINGAME,
                    BitConverter.GetBytes(id),
                    Encoding.ASCII.GetBytes($"{name}\0"),
                    Encoding.ASCII.GetBytes($"{password}\0")
                )
            )
        {
        }
    }
}