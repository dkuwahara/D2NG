namespace D2NG
{
    public class BncsPacketSentEvent : BncsEvent
    {
        private byte[] Packet { get; set; }

        public byte Type { get; set; }

        public BncsPacketSentEvent(byte[] packet)
        {
            this.Packet = packet;
            this.Type = packet[1];
        }
    }
}