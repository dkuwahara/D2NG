using System;
using System.Runtime.Serialization;

namespace D2NG.MCP.Packet
{
    [Serializable]
    public class CreateGameException : Exception
    {
        public CreateGameException()
        {
        }

        public CreateGameException(string message) : base(message)
        {
        }

        public CreateGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CreateGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}