using System.Collections.Generic;

namespace D2NG
{
    class BNCSPacketReceivedEvent : BNCSEvent
    {
        private byte Type { get; set; }
        private List<byte> Packet { get; set; }

        public BNCSPacketReceivedEvent(List<byte> packet)
        {
            this.Packet = packet;
            this.Type = packet[1];
        }
    }
}
