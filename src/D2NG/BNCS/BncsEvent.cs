using D2NG.BNCS.Packet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace D2NG.BNCS
{
    class BncsEvent
    {
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        public BncsPacket Packet { get; private set; }

        public BncsPacket WaitForPacket()
        {
            _event.WaitOne();
            return Packet;
        }

        public void Set(BncsPacket packet)
        {
            Packet = packet;
            _event.Set();
        }

        public void Dispose()
        {
            _event.Dispose();
        }
    }
}
