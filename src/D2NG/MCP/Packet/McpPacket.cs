using System;
using System.Collections.Generic;

namespace D2NG.MCP.Packet
{
    public class McpPacket : D2NG.Packet
    {
        public McpPacket(byte[] packet) : base(packet)
        {
        }

        public byte Type { get => Raw[2]; }

        protected static byte[] BuildPacket(byte command, params IEnumerable<byte>[] args)
        {
            var packet = new List<byte>();

            var packetArray = new List<byte>();
            foreach (IEnumerable<byte> a in args)
            {
                packetArray.AddRange(a);
            }

            UInt16 arrayCount = (UInt16)(packetArray.Count + 3);
            packet.AddRange(BitConverter.GetBytes(arrayCount));
            packet.Add(command);
            packet.AddRange(packetArray);

            return packet.ToArray();
        }
    }
}
