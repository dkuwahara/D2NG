using System;
using System.Runtime.Serialization;

namespace D2NG
{
    [Serializable]
    public class ChatValidationException : Exception
    {
        public ChatValidationException()
        {
        }

        public ChatValidationException(string message) : base(message)
        {
        }

        public ChatValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChatValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}