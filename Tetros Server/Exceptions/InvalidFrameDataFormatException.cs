using System;
using System.Runtime.Serialization;

namespace Tetros
{
    [Serializable]
    internal class InvalidFrameDataFormatException : Exception
    {
        public InvalidFrameDataFormatException()
        {
        }

        public InvalidFrameDataFormatException(string message) : base(message)
        {
        }

        public InvalidFrameDataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFrameDataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}