using System;

namespace D2NG.MCP.Packet
{
    public class ListCharactersClientPacket : McpPacket
    {
        private const int NumCharacters = 8;

        public ListCharactersClientPacket() :
            base(
                BuildPacket(
                    Mcp.CHARLIST2,
                    BitConverter.GetBytes(NumCharacters)
                )
            )
        {
        }
    }
}