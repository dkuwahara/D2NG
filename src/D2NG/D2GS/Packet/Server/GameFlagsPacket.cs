using D2NG.D2GS.Packet;
using Serilog;
using System.IO;
using System.Text;

namespace D2NG.D2GS
{
    class GameFlagsPacket
    {
        public Difficulty Difficulty { get; }
        public bool Hardcore { get; }
        public bool Expansion { get; }
        public bool Ladder { get; }

        public GameFlagsPacket(D2gsPacket packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            if (D2gs.GAMEFLAGS != (D2gs)reader.ReadByte())
            {
                throw new D2GSPacketException("Expected Packet Type Not Found");
            }
            Difficulty = (Difficulty)(reader.ReadByte() << 3);
            _ = reader.ReadUInt16();
            Hardcore = reader.ReadUInt16() != 0;
            Expansion = reader.ReadByte() != 0;
            Ladder = reader.ReadByte() != 0;
            reader.Close();

            Log.Verbose($"({packet.Raw[0]}) Game flags:\n" +
                        $"\tDifficulty: {Difficulty}\n" +
                        $"\tType: {(Hardcore ? "Hardcore" : "Softcore")}" +
                        $" {(Expansion ? "Expansion" : "")}" +
                        $" {(Ladder ? "Ladder" : "")}");
        }
    }
}
