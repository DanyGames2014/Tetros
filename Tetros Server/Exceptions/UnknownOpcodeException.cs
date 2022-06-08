using System;
using System.Runtime.Serialization;

namespace Tetros
{
    [Serializable]
    internal class UnknownOpcodeException : Exception
    {
        public UnknownOpcodeException()
        {
        }

        public UnknownOpcodeException(string message) : base(message)
        {
        }

        public UnknownOpcodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownOpcodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}