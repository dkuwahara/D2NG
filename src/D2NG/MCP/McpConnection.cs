using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace D2NG.MCP
{
    class McpConnection : Connection
    {

        internal override byte[] ReadPacket()
        {
            throw new NotImplementedException();
        }

        internal override void WritePacket(byte[] packet)
        {
            throw new NotImplementedException();
        }
    }
}
