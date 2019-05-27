using Serilog;
using System;
using System.Text;

namespace D2NG.MCP.Packet
{
    public class CreateGameRequestPacket : McpPacket
    {
        public CreateGameRequestPacket(ushort id, Difficulty difficulty, string name, string password) :
            base(
                BuildPacket(
                    Mcp.CREATEGAME,
                    BitConverter.GetBytes(id),
                    BitConverter.GetBytes((uint)difficulty),
                    new byte[] { 0x01, 0xFF, 0x08 },
                    Encoding.ASCII.GetBytes($"{name}\0"),
                    Encoding.ASCII.GetBytes($"{password}\0"),
                    Encoding.ASCII.GetBytes($"\0")
                )
            )
        {
            Log.Verbose($"CreateGameRequestPacket\n" +
                $"\tRequest Id {id}\n" +
                $"\tDifficulty {difficulty}\n" +
                $"\tGame Name {name}\n" +
                $"\tPassword {password}\n");
        }
    }
}