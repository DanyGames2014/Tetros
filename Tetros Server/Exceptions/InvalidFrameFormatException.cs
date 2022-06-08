using System;
using System.Runtime.Serialization;

namespace Tetros
{
    [Serializable]
    internal class InvalidFrameFormatException : Exception
    {
        public InvalidFrameFormatException()
        {
        }

        public InvalidFrameFormatException(string message) : base(message)
        {
        }

        public InvalidFrameFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFrameFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}