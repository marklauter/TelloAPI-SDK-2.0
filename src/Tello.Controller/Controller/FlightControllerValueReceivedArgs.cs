using System;

namespace Tello.Controller
{
    public sealed class FlightControllerValueReceivedArgs : EventArgs
    {
        public FlightControllerValueReceivedArgs(ValueResponseTypes responseType, string value)
        {
            ResponseType = responseType;
            Value = value;
        }

        public ValueResponseTypes ResponseType { get; }
        public string Value { get; }
    }
}
