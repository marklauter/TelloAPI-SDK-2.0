using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class FlightControllerExceptionThrownArgs : EventArgs
    {
        public FlightControllerExceptionThrownArgs(FlightControllerException ex)
        {
            Exception = ex;
        }

        public FlightControllerException Exception { get; }
    }

    public class FlightControllerCommandExceptionThrownArgs : FlightControllerExceptionThrownArgs
    {
        public FlightControllerCommandExceptionThrownArgs(Commands command, FlightControllerException ex, TimeSpan elapsed) : base(ex)
        {
            Command = command;
            Elapsed = elapsed;
        }

        public Commands Command { get; }
        public TimeSpan Elapsed { get; }
    }
}
