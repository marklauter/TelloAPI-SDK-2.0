using System;
using Tello.Controller.Messages;

namespace Tello.Controller
{
    public sealed class FlightControllerValueReceivedArgs : EventArgs
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
