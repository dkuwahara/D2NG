using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class JoinChannelRequestPacket : BncsPacket
    {
        public JoinChannelRequestPacket(string channel)
            : base(
                  BuildPacket(
                      (byte)Sid.JOINCHANNEL,
                      BitConverter.GetBytes(0x05),
                      Encoding.ASCII.GetBytes(channel),
                      Encoding.ASCII.GetBytes("\0")
                      ))
        {
        }
    }
}