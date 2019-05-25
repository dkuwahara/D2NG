using D2NG.BNCS.Hashing;
using Serilog;
using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class LogonRequestPacket : BncsPacket
    {
        public LogonRequestPacket(
            uint clientToken,
            uint serverToken,
            string username,
            string password) :
            base(
                BuildPacket(
                    Sid.LOGONRESPONSE2,
                    BitConverter.GetBytes(clientToken),
                    BitConverter.GetBytes(serverToken),
                    Bsha1.DoubleHash(clientToken, serverToken, password.ToLowerInvariant()),
                    Encoding.ASCII.GetBytes(username + "\0")
                )
            )
        {
            Log.Verbose($"LogonRequestPacket:\n" +
                $"\tClient Token: {clientToken}" +
                $"\tServer Token: {serverToken}" +
                $"\tUsername: {username}");
        }
    }
}
