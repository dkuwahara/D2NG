using D2NG.BNCS.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class RealmLogonRequestPacket : BncsPacket
    {
        public RealmLogonRequestPacket(
            uint clientToken,
            uint serverToken,
            string realmTitle,
            string password) :
            base(
                BuildPacket(
                    (byte)Sid.LOGONREALMEX,
                    BitConverter.GetBytes(clientToken),
                    Bsha1.DoubleHash(clientToken, serverToken, password),
                    Encoding.ASCII.GetBytes(realmTitle + "\0")
                )
            )
        {
        }
    }
}
