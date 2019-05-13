using D2NG.BNCS.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class BncsLogonRequestPacket : BncsPacket
    {
        public BncsLogonRequestPacket(
            uint clientToken,
            uint serverToken,
            string username,
            string password) :
            base(
                BuildPacket(
                    0x3A,
                    BitConverter.GetBytes(clientToken),
                    BitConverter.GetBytes(serverToken),
                    DoubleHashPassword(clientToken, serverToken, password),
                    Encoding.ASCII.GetBytes(username + "\0")
                )
            )
        {
        }

        private static IEnumerable<byte> DoubleHashPassword(uint clientToken, uint serverToken, string password)
        {
            return Bsha1.DoubleHash(clientToken, serverToken, password);
        }
    }
}
