using System;

namespace Tello.Messaging
{
    public class RelayExceptionThrownArgs : EventArgs
    {
        public RelayExceptionThrownArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}