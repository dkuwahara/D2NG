using System;

namespace D2NG.D2GS.Packet
{
    internal class PingPacket : D2gsPacket
    {
        public PingPacket() :
            base(
                BuildPacket(
                    D2gs.PING,
                    BitConverter.GetBytes(Environment.TickCount),
                    BitConverter.GetBytes(0),
                    BitConverter.GetBytes(0)
                )
            )
        {
        }
    }
}