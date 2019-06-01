using Serilog;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Packet.Server
{
    internal class AddExpPacket
    {
        public AddExpPacket(D2gsPacket packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            switch (id)
            {
                case 0x1A:
                    Experience = reader.ReadByte();
                    break;
                case 0x1B:
                    Experience = reader.ReadUInt16();
                    break;
                case 0x1C:
                    Experience = reader.ReadUInt32();
                    break;
                default:
                    throw new D2GSPacketException("Unexpected packet id");
            }
            reader.Close();
            Log.Verbose($"(0x{id,2:X2}) Add Experience:\n" +
                $"\tExperience: {Experience}");
        }

        public uint Experience { get; internal set; }
    }
}