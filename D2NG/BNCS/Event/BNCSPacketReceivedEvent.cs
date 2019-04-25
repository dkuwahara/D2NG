
namespace D2NG.BNCS.Packet
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
