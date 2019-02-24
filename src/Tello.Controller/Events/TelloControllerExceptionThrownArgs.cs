using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class TelloControllerExceptionThrownArgs : EventArgs
    {
        public TelloControllerExceptionThrownArgs(TelloControllerException ex)
        {
            Exception = ex;
        }

        public TelloControllerException Exception { get; }
    }

    public class FlightControllerCommandExceptionThrownArgs : TelloControllerExceptionThrownArgs
    {
        public FlightControllerCommandExceptionThrownArgs(Commands command, TelloControllerException ex, TimeSpan elapsed) : base(ex)
        {
            Command = command;
            Elapsed = elapsed;
        }

        public Commands Command { get; }
        public TimeSpan Elapsed { get; }
    }
}
