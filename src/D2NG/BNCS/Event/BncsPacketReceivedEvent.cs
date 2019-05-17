
namespace D2NG.BNCS.Packet
{
    public class BncsPacketReceivedEvent : BncsEvent
    {
        public BncsPacket Packet { get; }

        public BncsPacketReceivedEvent(BncsPacket packet)
        {
            this.Packet = packet;
        }        
    }
}
