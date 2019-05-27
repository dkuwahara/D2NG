using System;
using System.Runtime.Serialization;

namespace D2NG.MCP.Packet
{
    [Serializable]
    public class JoinGameException : Exception
    {
        public JoinGameException()
        {
        }

        public JoinGameException(string message) : base(message)
        {
        }

        public JoinGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JoinGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}