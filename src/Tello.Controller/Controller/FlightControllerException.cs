using System;

namespace Tello.Controller
{
    public class FlightControllerException : Exception
    {
        public FlightControllerException()
        {
        }

        public FlightControllerException(string message) : base(message)
        {
        }

        public FlightControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
