using Serilog;
using System.IO;
using System.Net;
using System.Text;

namespace D2NG.MCP.Packet
{
    internal class JoinGameResponsePacket : McpPacket
    {
        public ushort RequestId { get; }
        public ushort GameToken { get; }
        public IPAddress D2gsIp { get; }
        public uint GameHash { get; }
        public uint Result { get; }

        public JoinGameResponsePacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(Raw), Encoding.ASCII);
            if (Raw.Length != reader.ReadUInt16())
            {
                throw new McpPacketException("Packet length does not match");
            }
            if (Mcp.JOINGAME != (Mcp)reader.ReadByte())
            {
                throw new McpPacketException("Expected Packet Type Not Found");
            }

            RequestId = reader.ReadUInt16();
            GameToken = reader.ReadUInt16();
            _ = reader.ReadUInt16();

            D2gsIp = new IPAddress(reader.ReadUInt32());

            GameHash = reader.ReadUInt32();
            Result = reader.ReadUInt32();
            Validate(Result);
        }

        private void Validate(uint result)
        {
            switch (result)
            {
                case 0x00:
                    break;
                case 0x29:
                    throw new JoinGameException("Password incorrect");
                case 0x2A:
                    throw new JoinGameException("Game does not exist");
                case 0x2B:
                    throw new JoinGameException("Game is full");
                case 0x2C:
                    throw new JoinGameException("You do not meet the level requirements for the game");
                case 0x6E:
                    throw new JoinGameException("A dead hardcore chracter cannot join a game");
                case 0x71:
                    throw new JoinGameException("A non-hardcore character cannot join a hardcore game");
                case 0x73:
                    throw new JoinGameException("Unable to join a Nightmare game");
                case 0x74:
                    throw new JoinGameException("Unable to join a Hell Game");
                case 0x78:
                    throw new JoinGameException("A non-expansion character cannot join a game created by an expansion character");
                case 0x79:
                    throw new JoinGameException("An expansion character cannot join a game created by a non-expansion character");
                case 0x7D:
                    throw new JoinGameException("A non-ladder character cannot join a ladder game");
                default:
                    throw new JoinGameException("Unknown game join failure");
            }
        }
    }
}