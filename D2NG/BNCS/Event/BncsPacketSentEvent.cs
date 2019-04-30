namespace D2NG.BNCS.Event
{
    public class BncsPacketSentEvent : BncsEvent
    {
        public byte[] Packet { get; set; }

        public byte Type { get; set; }

        public BncsPacketSentEvent(byte[] packet)
        {
            this.Packet = packet;
            this.Type = packet[1];
        }
    }
}