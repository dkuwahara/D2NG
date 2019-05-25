using D2NG.BNCS.Hashing;
using Serilog;
using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class RealmLogonRequestPacket : BncsPacket
    {
        public RealmLogonRequestPacket(
            uint clientToken,
            uint serverToken,
            string realm,
            string password) :
            base(
                BuildPacket(
                    Sid.LOGONREALMEX,
                    BitConverter.GetBytes(clientToken),
                    Bsha1.DoubleHash(clientToken, serverToken, password),
                    Encoding.ASCII.GetBytes(realm + "\0")
                )
            )
        {
            Log.Verbose($"RealmLogonRequestPacket:\n" +
                $"\tType: {Type}\n" +
                $"\tClient Token: {clientToken}\n" +
                $"\tServer Token: {serverToken}\n" +
                $"\tRealm: {realm}\n" +
                $"\tPassword: ********");
        }
    }
}
