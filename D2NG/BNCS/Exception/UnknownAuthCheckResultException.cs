using System;
using System.Runtime.Serialization;

namespace D2NG
{
    [Serializable]
    internal class UnknownAuthCheckResultException : Exception
    {
        public UnknownAuthCheckResultException()
        {
        }

        public UnknownAuthCheckResultException(string message) : base(message)
        {
        }

        public UnknownAuthCheckResultException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownAuthCheckResultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}