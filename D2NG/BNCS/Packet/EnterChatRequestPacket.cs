using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class EnterChatRequestPacket : BncsPacket
    {
        public EnterChatRequestPacket(string username) : base(
            BuildPacket(
                (byte)Sid.ENTERCHAT,
                Encoding.ASCII.GetBytes(username),
                Encoding.ASCII.GetBytes("\0\0")
                ))
        {
        }
    }
}
