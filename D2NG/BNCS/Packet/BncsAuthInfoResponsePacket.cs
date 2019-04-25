using Serilog;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class BncsAuthInfoResponsePacket : BncsPacket
    {
        public BncsAuthInfoResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            _ = reader.ReadByte();
            byte type = reader.ReadByte();
            Log.Debug("{0} == {1}", this.Type, type);
            LogonType = reader.ReadInt32();
            ServerToken = reader.ReadUInt32();
            _ = reader.ReadInt32();
            MpqFileTime = reader.ReadUInt64();

            var offset = 24;

            MpqFileName = ReadNullTerminatedString(Encoding.ASCII.GetString(Raw), ref offset);
            FormulaString = ReadNullTerminatedString(Encoding.ASCII.GetString(Raw), ref offset);
        }

        public int LogonType { get; }
        public uint ServerToken { get; }
        public ulong MpqFileTime { get; }
        public string FormulaString { get; }
        public string MpqFileName { get; }

        private static string ReadNullTerminatedString(string packet, ref int offset)
        {
            int zero = packet.IndexOf('\0', offset);
            string output;
            if (zero == -1)
            {
                zero = packet.Length;
                output = packet.Substring(offset, zero - offset);
                offset = 0;
            }
            else
            {
                output = packet.Substring(offset, zero - offset);
                offset = zero + 1;
            }
            return output;
        }

        public override string ToString()
        {
            return $"{nameof(LogonType)}: {LogonType}, {nameof(ServerToken)}: {ServerToken}, {nameof(MpqFileTime)}: {MpqFileTime}, {nameof(FormulaString)}: {FormulaString}, {nameof(MpqFileName)}: {MpqFileName}";
        }

    }
}