using System;

namespace Messenger
{
    public interface IEnvelope
    {
        /// <summary>
        /// id for use in queuing systems or logs
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// time that the message was created/initiated
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// marshalled data
        /// </summary>
        byte[] Data { get; }
    }

    public interface IEnvelope<T> : IEnvelope
    {
        /// <summary>
        /// typed message
        /// </summary>
        T Message { get; }
    }
}
