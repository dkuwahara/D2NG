using D2NG.D2GS;
using D2NG.D2GS.Packet;
using Serilog;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Packet.Server
{
    internal class BaseAttributePacket
    {
        public Attribute Attribute { get; }
        public int Value { get; }

        public BaseAttributePacket(D2gsPacket packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            Attribute = (Attribute)reader.ReadByte();
            switch (id)
            {
                case 0x1D:
                    Value = reader.ReadByte();
                    break;
                case 0x1E:
                    Value = reader.ReadInt16();
                    break;
                case 0x1F:
                    Value = reader.ReadInt32();
                    break;
                default:
                    throw new D2GSPacketException("Unexpected packet id");
            }
            reader.Close();
            Log.Verbose($"(0x{id,2:X2}) Base Attribute:\n" +
                $"\tAttribute: {Attribute}:" +
                $"\t{Value}");
        }
    }
}