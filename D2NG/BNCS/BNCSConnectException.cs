using System;
using System.Runtime.Serialization;

namespace D2NG
{
    [Serializable]
    public class BncsConnectException : Exception
    {
        public BncsConnectException()
        {
        }

        public BncsConnectException(string message) : base(message)
        {
        }

        public BncsConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BncsConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}