using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS
{
    public class BncsReceivedPacket
    {
        public byte Type { get => _packet[1]; }

        public byte[] Raw { get => _packet; } 

        private byte[] _packet;

        public BncsReceivedPacket(byte[] packet)
        {
            _packet = packet;
        }
    }
}
