using System;
using System.Runtime.Serialization;

namespace D2NG.BNCS.Packet
{
    [Serializable]
    public class BncsPacketException : Exception
    {
        public BncsPacketException()
        {
        }

        public BncsPacketException(string message) : base(message)
        {
        }

        public BncsPacketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BncsPacketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}