using System;

namespace Tello.Controller
{
    public sealed class FlightControllerResponseReceivedArgs : EventArgs
    {
        public FlightControllerResponseReceivedArgs(string response)
        {
            Response = response;
        }

        public string Response { get; }
    }
}
