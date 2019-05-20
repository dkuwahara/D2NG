using System;
using System.Runtime.Serialization;

namespace D2NG.MCP.Packet
{
    [Serializable]
    internal class McpPacketException : Exception
    {
        public McpPacketException()
        {
        }

        public McpPacketException(string message) : base(message)
        {
        }

        public McpPacketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected McpPacketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}