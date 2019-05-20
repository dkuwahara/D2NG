using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace D2NG.MCP
{
    class McpConnection : Connection
    {

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
        }
    }
}
