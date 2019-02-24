using System;

namespace Tello.Controller
{
    public class TelloControllerException : Exception
    {
        public TelloControllerException()
        {
        }

        public TelloControllerException(string message) : base(message)
        {
        }

        public TelloControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
