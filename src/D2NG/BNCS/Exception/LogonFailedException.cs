using System;
using System.Runtime.Serialization;

namespace D2NG.BNCS.Packet
{
    [Serializable]
    public class LogonFailedException : Exception
    {
        public LogonFailedException()
        {
        }

        public LogonFailedException(string message) : base(message)
        {
        }

        public LogonFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogonFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}