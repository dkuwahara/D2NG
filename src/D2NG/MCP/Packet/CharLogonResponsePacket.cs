using D2NG.MCP.Packet;
using System.IO;
using System.Text;

namespace D2NG
{
    internal class CharLogonResponsePacket : McpPacket
    {
        public CharLogonResponsePacket(McpPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(Raw), Encoding.ASCII);
            if (Raw.Length != reader.ReadUInt16())
            {
                throw new McpPacketException("Packet length does not match");
            }
            if (Mcp.CHARLOGON != (Mcp)reader.ReadByte())
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }
            Result = reader.ReadUInt32();
        }

        public uint Result { get; }
    }
}