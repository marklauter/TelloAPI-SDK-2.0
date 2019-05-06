using System;

namespace Messenger
{
    public interface IEnvelope
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

    public interface IEnvelope<T> : IEnvelope
    {
        /// <summary>
        /// typed message
        /// </summary>
        T Message { get; }
    }
}
