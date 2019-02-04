using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class FlightControllerValueReceivedArgs : EventArgs
    {
        public FlightControllerValueReceivedArgs(Responses responseType, string value)
        {
            ResponseType = responseType;
            Value = value;
        }

        public Responses ResponseType { get; }
        public string Value { get; }
    }
}
