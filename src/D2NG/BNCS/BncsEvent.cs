using D2NG.BNCS.Packet;
using Serilog;
using System;
using System.Threading;

namespace D2NG.BNCS
{
    class BncsEvent : IDisposable
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
            Log.Verbose("Waiting for packet");
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
