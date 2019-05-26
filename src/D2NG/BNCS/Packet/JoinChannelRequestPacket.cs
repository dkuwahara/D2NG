using Serilog;
using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class JoinChannelRequestPacket : BncsPacket
    {
        public JoinChannelRequestPacket(uint flags, string channel) :
            base(
                BuildPacket(
                    Sid.JOINCHANNEL,
                    BitConverter.GetBytes(flags),
                    Encoding.ASCII.GetBytes(channel),
                    Encoding.ASCII.GetBytes("\0")
                )
            )
        {
            Log.Verbose($"JoinChannelRequestPacket\n" +
                $"\tType: {Type}\n" +
                $"\tFlags: 0x{flags,2:2X}\n" +
                $"\tChannel: {channel}\n");
        }
    }
}