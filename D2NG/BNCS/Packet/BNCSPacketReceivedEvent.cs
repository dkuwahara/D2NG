using D2NG.BNCS;
using System.Collections.Generic;

namespace D2NG
{
    public class BNCSPacketReceivedEvent : BNCSEvent
    {
        public BncsPacket Packet { get; }

        public BNCSPacketReceivedEvent(BncsPacket packet)
        {
            this.Packet = packet;
        }        
    }
}
