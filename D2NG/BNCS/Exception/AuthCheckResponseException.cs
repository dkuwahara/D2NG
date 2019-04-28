using System;
using System.Runtime.Serialization;

namespace D2NG.BNCS.Packet
{
    [Serializable]
    internal class AuthCheckResponseException : Exception
    {
        public AuthCheckResponseException()
        {
        }

        public AuthCheckResponseException(string message) : base(message)
        {
        }

        public AuthCheckResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthCheckResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}