namespace D2NG.BNCS.Packet
{
    public class QueryRealmsRequestPacket : BncsPacket
    {
        public QueryRealmsRequestPacket() : 
            base(BuildPacket(Sid.QUERYREALMS2))
        {
        }
    }
}
