using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class RealmLogonResponsePacket : BncsPacket
    {
        public uint McpCookie { get; }
        public uint McpStatus { get; }
        public List<byte> McpChunk { get; }
        public IPAddress McpIp { get; }
        public short McpPort { get; }
        public string McpUniqueName { get; }

        public RealmLogonResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if (Sid.LOGONREALMEX != (Sid) reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            if (packet.Length != reader.ReadUInt16())
            {
                throw new BncsPacketException("Packet length does not match");
            }
            McpCookie = reader.ReadUInt32();
            McpStatus = reader.ReadUInt32();

            McpChunk = new List<byte>();

            for(int i = 0; i < 2; i++)
            {
                McpChunk.AddRange(reader.ReadBytes(4));
            }

            McpIp = new IPAddress(reader.ReadUInt32());
            McpPort = IPAddress.NetworkToHostOrder((short) reader.ReadInt32());

            for (int i = 0; i < 12; i++)
            {
                McpChunk.AddRange(reader.ReadBytes(4));
            }

            McpUniqueName = ReadString(reader);
        }
    }
}