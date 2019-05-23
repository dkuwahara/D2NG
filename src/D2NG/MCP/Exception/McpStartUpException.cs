using System;
using System.Runtime.Serialization;

namespace D2NG.MCP.Packet
{
    [Serializable]
    public class McpStartUpException : Exception
    {
        public McpStartUpException()
        {
        }

        public McpStartUpException(string message) : base(message)
        {
        }

        public McpStartUpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected McpStartUpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}