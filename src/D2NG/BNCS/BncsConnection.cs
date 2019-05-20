using D2NG.BNCS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using D2NG.BNCS.Event;

namespace D2NG.BNCS
{
    class BncsConnection : Connection
    {
        /**
         * Default port used to connected to BNCS
         */
        public static readonly int DefaultPort = 6112;

        /**
         * Events on send and receive
         */
        public event EventHandler<BncsPacketReceivedEvent> PacketReceived;

        public event EventHandler<BncsPacketSentEvent> PacketSent;

        public void Connect(String realm)
        {
             Log.Debug("[{0}] Resolving {1}", GetType(), realm);
             var server = Dns.GetHostAddresses(realm).First();

            Connect(server, DefaultPort);
        }

        internal override byte[] ReadPacket()
        {
            List<byte> buffer;
            do
            {
                buffer = new List<byte>();
                // Get the first 4 bytes, packet type and length
                ReadUpTo(ref buffer, 4);
                short packetLength = BitConverter.ToInt16(buffer.ToArray(), 2);

                // Read the rest of the packet and return it
                ReadUpTo(ref buffer, packetLength)
            } while (buffer[1] == 0x00);

            var packet = new BncsPacket(buffer.ToArray());
            PacketReceived?.Invoke(this, new BncsPacketReceivedEvent(packet));
            return buffer.ToArray();
        }

        private void ReadUpTo(ref List<byte> bncsBuffer, int count)
        {
            while (bncsBuffer.Count < count)
            {
                byte temp = (byte)_stream.ReadByte();
                bncsBuffer.Add(temp);
            }
        }

        internal override void WritePacket(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
            PacketSent?.Invoke(this, new BncsPacketSentEvent(packet));
        }

    }
}
