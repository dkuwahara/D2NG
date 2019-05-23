using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class QueryRealmsResponsePacket : BncsPacket
    {
        public List<Realm> Realms { get; } = new List<Realm>();

        public QueryRealmsResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if (Sid.QUERYREALMS2 != (Sid)reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            if (packet.Length != reader.ReadUInt16())
            {
                throw new BncsPacketException("Packet length does not match");
            }

            _ = reader.ReadUInt32();
            var count = reader.ReadUInt32();

            for (int i = 0; i < count; i++)
            {
                reader.ReadUInt32();
                Realms.Add(new Realm(ReadString(reader), ReadString(reader)));
            }

            reader.Close();
        }
    }
}