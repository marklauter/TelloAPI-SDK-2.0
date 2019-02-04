using System;

namespace Tello.Controller
{
    public class FlightControllerResponseReceivedArgs : EventArgs
    {
        public FlightControllerResponseReceivedArgs(string response)
        {
            Response = response;
        }

        public string Response { get; }
    }
}
