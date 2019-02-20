using System;

namespace Tello.Messaging
{
    public class MessageRelayExceptionThrownArgs : EventArgs
    {
        public MessageRelayExceptionThrownArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}