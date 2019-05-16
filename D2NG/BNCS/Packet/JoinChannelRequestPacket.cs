using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class JoinChannelRequestPacket : BncsPacket
    {
        public JoinChannelRequestPacket(string defaultChannel)
            : base(
                  BuildPacket(
                      0x0C,
                      BitConverter.GetBytes(0x05),
                      Encoding.ASCII.GetBytes(defaultChannel),
                      Encoding.ASCII.GetBytes("\0")
                      ))
        {
        }
    }
}