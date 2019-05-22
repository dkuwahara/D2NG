using D2NG.MCP.Packet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace D2NG.MCP
{
    internal class McpEvent
    {
        private AutoResetEvent _event = new AutoResetEvent(false);

        public McpPacket Packet { get; private set; }

        public McpPacket WaitForPacket()
        {
            _event.WaitOne();
            return Packet;
        }

        public void Set(McpPacket packet)
        {
            Packet = packet;
            _event.Set();
        }
    }
}
