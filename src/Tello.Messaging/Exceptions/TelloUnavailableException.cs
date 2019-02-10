using System;

namespace Tello.Messaging
{
    public class TelloUnavailableException : Exception
    {
        public TelloUnavailableException()
        {
        }

        public TelloUnavailableException(string message) : base(message)
        {
        }

        public TelloUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
