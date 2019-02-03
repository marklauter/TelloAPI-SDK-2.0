using System;

namespace Tello.Controller
{
    public sealed class FlightControllerExceptionThrownArgs : EventArgs
    {
        public FlightControllerExceptionThrownArgs(FlightControllerException ex)
        {
            Exception = ex;
        }

        public FlightControllerException Exception { get; }
    }
}
