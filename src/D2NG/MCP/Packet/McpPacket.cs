using System;
using System.Collections.Generic;
using System.Linq;

namespace D2NG.MCP.Packet
{
    public class McpPacket : D2NG.Packet
    {
        public McpPacket(byte[] packet) : base(packet)
        {
        }

        public byte Type { get => Raw[2]; }

        protected static byte[] BuildPacket(Mcp command, params IEnumerable<byte>[] args)
        {
            var packet = new List<byte>();
            var packetArray = args.SelectMany(a => a);
            packet.AddRange(BitConverter.GetBytes((UInt16)(packetArray.Count() + 3)));
            packet.Add((byte)command);
            packet.AddRange(packetArray);
            return packet.ToArray();
        }
    }
}
