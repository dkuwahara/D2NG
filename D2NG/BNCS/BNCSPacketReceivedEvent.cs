using D2NG.BNCS;
using System.Collections.Generic;

namespace D2NG
{
    public class BNCSPacketReceivedEvent : BNCSEvent
    {
        public BncsReceivedPacket Packet { get; }

        public BNCSPacketReceivedEvent(BncsReceivedPacket packet)
        {
            this.Packet = packet;
        }
        
    }
}
