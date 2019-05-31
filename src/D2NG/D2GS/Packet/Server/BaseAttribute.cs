using D2NG.D2GS;
using D2NG.D2GS.Packet;
using Serilog;
using System.IO;
using System.Text;

namespace D2NG
{
    internal class BaseAttribute
    {
        public Attribute Attribute { get; }
        public int Value { get; }

        public BaseAttribute(D2gsPacket packet)
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
            Log.Verbose($"({id}) Base Attribute:\n" +
                $"\tAttribute: {Attribute}\n" +
                $"\tValue: {Value}");
        }
    }
}