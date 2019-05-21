using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.MCP.Packet
{
    public class ListCharactersServerPacket : McpPacket
    {
        public ListCharactersServerPacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (packet.Length != reader.ReadUInt16())
            {
                throw new McpPacketException("Packet length does not match");
            }
            if (Mcp.CHARLIST2 != (Mcp) reader.ReadByte())
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }

            _ = reader.ReadUInt16();
            _ = reader.ReadUInt32();
            var totalReturned = reader.ReadUInt16();

            Characters = new List<Character>();
            for(int x = 0; x < totalReturned; x++)
            {
                Characters.Add(new Character(
                    reader.ReadUInt32(),
                    ReadString(reader),
                    ReadString(reader)
                    ));
            }
        }

        public List<Character> Characters { get; }
    }
}