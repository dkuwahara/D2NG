using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class QueryRealmsRequestPacket : BncsPacket
    {
        public QueryRealmsRequestPacket() : 
            base(BuildPacket(0x40))
        {
        }
    }
}
