using Serilog;
using System.IO;
using System.Text;

namespace D2NG.MCP.Packet
{
    internal class CreateGameResponsePacket : McpPacket
    {
        public CreateGameResponsePacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(Raw), Encoding.ASCII);
            if (Raw.Length != reader.ReadUInt16())
            {
                throw new McpPacketException("Packet length does not match");
            }
            if (Mcp.CREATEGAME != (Mcp)reader.ReadByte())
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }
            _ = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            var result = reader.ReadUInt32();

            switch(result)
            {
                case 0x00:
                    Log.Debug("Game created successfully");
                    break;
                case 0x1E:
                    throw new CreateGameException("Invalid Game Name");
                case 0x1F:
                    throw new CreateGameException("Game name already exists");
                case 0x20:
                    throw new CreateGameException("Game servers are down");
                case 0x6E:
                    throw new CreateGameException("A dead hardcore character cannot create games");
                default:
                    throw new CreateGameException("Unknown game creation failure");
            }
        }
    }
}