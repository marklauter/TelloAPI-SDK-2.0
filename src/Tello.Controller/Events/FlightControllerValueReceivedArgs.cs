using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class FlightControllerValueReceivedArgs : EventArgs
    {
        public FlightControllerValueReceivedArgs(Responses responseType, string value, TimeSpan elapsed)
        {
            ResponseType = responseType;
            Value = value;
            Elapsed = elapsed;
        }

        public Responses ResponseType { get; }
        public string Value { get; }
        public TimeSpan Elapsed { get; }
    }
}
