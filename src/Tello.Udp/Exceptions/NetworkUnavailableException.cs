using System;

namespace Tello.Udp
{
    public class NetworkUnavailableException : Exception
    {
        public NetworkUnavailableException()
        {
        }

        public NetworkUnavailableException(string message) : base(message)
        {
        }

        public NetworkUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
