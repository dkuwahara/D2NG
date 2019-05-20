using System;
using System.Collections.Generic;

namespace D2NG.BNCS.Packet
{
    public class BncsPacket : D2NG.Packet
    {
        protected readonly byte PrefixByte = 0xFF;

        public byte Type { get => Raw[1]; }

        public BncsPacket(byte[] packet) :
            base(packet)
        { }

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
