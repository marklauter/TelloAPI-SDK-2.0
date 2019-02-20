using System;

namespace Tello.Messaging
{
    public class MessageReceivedArgs<T> : EventArgs
    {
        public MessageReceivedArgs(T message)
        {
            Message = message;
            ReceivedUtc = DateTime.UtcNow;
        }

        public DateTime ReceivedUtc { get; }

        public T Message { get; }
    }
}
