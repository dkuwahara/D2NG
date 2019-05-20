using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.MCP.Packet
{
    public class McpPacket : D2NG.Packet
    {
        public McpPacket(byte[] packet) : base(packet)
        {
        }

        protected static byte[] BuildPacket(byte command, params IEnumerable<byte>[] args)
        {
            List<byte> packet = new List<byte>();

            List<byte> packetArray = new List<byte>();
            foreach (IEnumerable<byte> a in args)
            {
                packetArray.AddRange(a);
            }

            UInt16 arrayCount = (UInt16)(packetArray.Count + 3);
            packet.AddRange(BitConverter.GetBytes(arrayCount));
            packet.Add((byte)command);
            packet.AddRange(packetArray);

            byte[] bytes = new byte[arrayCount];
            packet.CopyTo(bytes);
            return bytes;
        }
    }
}
