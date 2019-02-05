using System;

namespace Tello.Messaging
{
    public class RelayMessageReceivedArgs<T> : EventArgs
    {
        public RelayMessageReceivedArgs(T message)
        {
            Message = message;
        }

        public T Message { get; }
    }
}
