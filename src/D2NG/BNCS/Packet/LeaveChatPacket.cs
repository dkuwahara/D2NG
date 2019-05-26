namespace D2NG.BNCS.Packet
{
    public class LeaveChatPacket : BncsPacket
    {
        public LeaveChatPacket() : base(BuildPacket(Sid.LEAVECHAT))
        {
        }
    }
}