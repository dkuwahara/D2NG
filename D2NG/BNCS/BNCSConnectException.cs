using System;
using System.Runtime.Serialization;

namespace D2NG
{
    [Serializable]
    public class BNCSConnectException : Exception
    {
        public BNCSConnectException()
        {
        }

        public BNCSConnectException(string message) : base(message)
        {
        }

        public BNCSConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BNCSConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}