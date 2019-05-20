using System;
using System.Runtime.Serialization;

namespace D2NG
{
    [Serializable]
    public class UnableToConnectException : Exception
    {
        public UnableToConnectException()
        {
        }

        public UnableToConnectException(string message) : base(message)
        {
        }

        public UnableToConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}