using System.Text;

namespace D2NG.MCP.Packet
{
    internal class CharLogonRequestPacket : McpPacket
    {
        public CharLogonRequestPacket(string name) :
            base(
                BuildPacket(
                    (byte)Mcp.CHARLOGON,
                    Encoding.ASCII.GetBytes(name + '\0')
                )
            )
        {
        }
    }
}