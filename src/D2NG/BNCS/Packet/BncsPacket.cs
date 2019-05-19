using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class BncsPacket
    {
        protected readonly byte PrefixByte = 0xFF;

        public byte Type { get => Raw[1]; }

        public byte[] Raw { get; }

        public BncsPacket(byte[] packet)
        {
            Raw = packet;
        }

        public static String ReadString(BinaryReader reader)
        {
            var text = new StringBuilder();
            while (reader.PeekChar() != 0)
            {
                text.Append(reader.ReadChar());
            }
            reader.ReadChar();
            return text.ToString();
        }

        protected static byte[] BuildPacket(byte command, params IEnumerable<byte>[] args)
        {
            var packet = new List<byte> { 0xFF, command };
            var packetArray = new List<byte>();

            foreach (var a in args)
            {
                packetArray.AddRange(a);
            }

            UInt16 packetSize = (UInt16)(packetArray.Count + 4);
            packet.AddRange(BitConverter.GetBytes(packetSize));
            packet.AddRange(packetArray);

            return packet.ToArray();
        }
    }
}
