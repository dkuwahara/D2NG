using System;
using System.Runtime.Serialization;

namespace D2NG.MCP
{
    [Serializable]
    public class CharLogonException : Exception
    {
        public CharLogonException()
        {
        }

        public CharLogonException(string message) : base(message)
        {
        }

        public CharLogonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CharLogonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}