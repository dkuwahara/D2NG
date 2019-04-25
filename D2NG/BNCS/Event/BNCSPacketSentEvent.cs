using System.Collections.Generic;

namespace D2NG
{
    public class BNCSPacketSentEvent : BNCSEvent
    {
        private byte[] Packet { get; set; }

        public byte Type { get; set; }

        public BNCSPacketSentEvent(byte[] packet)
        {
            this.Packet = packet;
            this.Type = packet[1];
        }
    }
}