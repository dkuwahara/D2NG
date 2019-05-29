namespace D2NG.BNCS.Packet
{
    internal class LeaveGamePacket : BncsPacket
    {
        public LeaveGamePacket() : 
            base(
                BuildPacket(
                    Sid.LEAVEGAME
                )
            )
        {
        }
    }
}