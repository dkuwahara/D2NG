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
            if (reader.ReadByte() != 0x19)
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }

            _ = reader.ReadUInt16();
            _ = reader.ReadUInt32();
            var totalReturned = reader.ReadUInt16();

            Characters = new List<McpCharacter>();
            for(int x = 0; x < totalReturned; x++)
            {
                Characters.Add(new McpCharacter(
                    reader.ReadUInt32(),
                    ReadString(reader),
                    ReadString(reader)
                    ));
            }
        }

        public List<McpCharacter> Characters { get; }
    }
}