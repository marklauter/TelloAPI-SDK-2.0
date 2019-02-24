using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class TelloControllerValueReceivedArgs : EventArgs
    {
        public TelloControllerValueReceivedArgs(Responses responseType, string value, TimeSpan elapsed)
        {
            ResponseType = responseType;
            Value = value?.TrimEnd();
            Elapsed = elapsed;
        }

        public Responses ResponseType { get; }
        public string Value { get; }
        public TimeSpan Elapsed { get; }
    }
}
