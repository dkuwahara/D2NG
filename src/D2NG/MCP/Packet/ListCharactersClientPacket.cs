using System;

namespace D2NG.MCP.Packet
{
    public class ListCharactersClientPacket : McpPacket
    {
        private const int NumCharacters = 8;

        public ListCharactersClientPacket() :
            base(
                BuildPacket(
                    0x19,
                    BitConverter.GetBytes(NumCharacters)
                    )
            )
        {
        }
    }
}