using D2NG.MCP.Packet;
using System;
using System.Collections.Generic;

namespace D2NG.MCP
{
    class McpConnection : Connection
    {
        public event EventHandler<McpPacket> PacketReceived;

        public event EventHandler<McpPacket> PacketSent;

        internal override byte[] ReadPacket()
        {
            List<byte> buffer;
            do
            {
                buffer = new List<byte>();
                // Get the first 3 bytes, packet type and length
                ReadUpTo(ref buffer, 3);
                short packetLength = BitConverter.ToInt16(buffer.ToArray(), 0);

                // Read the rest of the packet and return it
                ReadUpTo(ref buffer, packetLength);

            } while (buffer[2] == 0x00);
            PacketReceived?.Invoke(this, new McpPacket(buffer.ToArray()));
            return buffer.ToArray();
        }

        private void ReadUpTo(ref List<byte> buffer, int count)
        {
            while (buffer.Count < count)
            {
                byte temp = (byte)_stream.ReadByte();
                buffer.Add(temp);
            }
        }

        internal override void WritePacket(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
            PacketSent?.Invoke(this, new McpPacket(packet));
        }
    }
}
