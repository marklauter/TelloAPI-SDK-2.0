using System;

namespace Messenger
{
    public interface IMessage
    {
        /// <summary>
        /// marshalled data
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// time that the message was created/initiated
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// id for use in queuing systems
        /// </summary>
        Guid Id { get; }
    }

    public interface IMessage<T> : IMessage
    {
        /// <summary>
        /// typed message
        /// </summary>
        T Message { get; }
    }
}
