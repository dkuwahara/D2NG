using System;
using System.Linq;
using System.Text;

namespace D2NG.BNCS.Packet
{
    internal class NotifyJoinPacket : BncsPacket
    {
        public NotifyJoinPacket(string name, string password) : 
            base(
                BuildPacket(
                    Sid.NOTIFYJOIN,
                    Encoding.ASCII.GetBytes(PlatformCode).Reverse().ToArray(),
                    BitConverter.GetBytes(Version),
                    Encoding.ASCII.GetBytes($"{name}\0"),
                    Encoding.ASCII.GetBytes($"{password}\0")
                )
            )
        {
        }
    }
}