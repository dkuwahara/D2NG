using System;
using System.Runtime.Serialization;

namespace D2NG.BNCS.Hashing
{
    [Serializable]
    public class CdKeyException : Exception
    {
        public CdKeyException()
        {
        }

        public CdKeyException(string message) : base(message)
        {
        }

        public CdKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CdKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}