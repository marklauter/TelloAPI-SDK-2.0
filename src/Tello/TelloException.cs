using System;

namespace Tello
{
    public class TelloException : Exception
    {
        public TelloException()
        {
        }

        public TelloException(string message) : base(message)
        {
        }

        public TelloException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
