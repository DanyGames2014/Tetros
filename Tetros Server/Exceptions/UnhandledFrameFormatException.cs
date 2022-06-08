using System;
using System.Runtime.Serialization;

namespace Tetros
{
    [Serializable]
    internal class UnhandledFrameFormatException : Exception
    {
        public UnhandledFrameFormatException()
        {
        }

        public UnhandledFrameFormatException(string message) : base(message)
        {
        }

        public UnhandledFrameFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnhandledFrameFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}