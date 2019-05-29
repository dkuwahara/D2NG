using System;
using System.Runtime.Serialization;

namespace D2NG.D2GS
{
    [Serializable]
    public class D2GSPacketException : Exception
    {
        public D2GSPacketException()
        {
        }

        public D2GSPacketException(string message) : base(message)
        {
        }

        public D2GSPacketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected D2GSPacketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}