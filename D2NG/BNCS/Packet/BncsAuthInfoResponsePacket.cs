using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class BncsAuthInfoResponsePacket : BncsPacket
    {
        private readonly byte AuthInfoType = 0x50;
        public BncsAuthInfoResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if (AuthInfoType != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            } 

            LogonType = reader.ReadInt32();
            ServerToken = reader.ReadUInt32();
            _ = reader.ReadInt32();
            MpqFileTime = reader.ReadUInt64();

            var offset = 24;

            MpqFileName = ReadNullTerminatedString(Encoding.ASCII.GetString(Raw), ref offset);
            FormulaString = ReadNullTerminatedString(Encoding.GetEncoding("ISO-8859-1").GetString(Raw), ref offset);
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