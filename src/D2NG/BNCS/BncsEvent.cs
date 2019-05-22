﻿using D2NG.BNCS.Packet;
using System.Threading;

namespace D2NG.BNCS
{
    class BncsEvent
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        private BncsPacket _packet;

        public void Reset()
        {
            _event.Reset();
            _packet = null;
        }

        public BncsPacket WaitForPacket()
        {
            _event.WaitOne();
            return _packet;
        }

        public void Set(BncsPacket packet)
        {
            _packet = packet;
            _event.Set();
        }

        public void Dispose()
        {
            _event.Dispose();
        }
    }
}