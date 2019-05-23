using Serilog;
using System.IO;
using System.Text;

namespace D2NG.MCP.Packet
{
    class McpStartupResponsePacket : McpPacket
    {
        public McpStartupResponsePacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (packet.Length != reader.ReadUInt16())
            {
                throw new McpPacketException("Packet length does not match");
            }
            if (Mcp.STARTUP != (Mcp)reader.ReadByte())
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }

            var result = reader.ReadUInt32();

            switch (result)
            {
                case 0x02:
                case 0x0A:
                case 0x0B:
                case 0x0C:
                case 0x0D:
                    throw new McpStartUpException("Realm Unavailable");
                case 0x7E:
                    throw new McpStartUpException("CDKey banned from Realm Play");
                case 0x7F:
                    throw new McpStartUpException("IP banned temporarily");
                default:
                    Log.Debug("MCP Startup successful");
                    break;
            }
        }
    }
}
