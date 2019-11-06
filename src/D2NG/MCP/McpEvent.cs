using D2NG.MCP.Packet;
using System;
using System.Threading;

namespace D2NG.MCP
{
    internal class McpEvent : IDisposable
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        public McpPacket _packet;

        public void Reset()
        {
            _event.Reset();
            _packet = null;
        }

        public McpPacket WaitForPacket()
        {
            _event.WaitOne();
            return _packet!;
        }

        public void Set(McpPacket packet)
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
