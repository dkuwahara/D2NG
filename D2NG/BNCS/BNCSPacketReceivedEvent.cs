using System.Collections.Generic;

namespace D2NG
{
    class BNCSPacketReceivedEvent : BNCSEvent
    {
        public byte Type { get; set; }
        public byte[] Packet { get; set; }

        public BNCSPacketReceivedEvent(byte[] packet)
        {
            this.Packet = packet;
            this.Type = packet[1];
        }
    }
}
