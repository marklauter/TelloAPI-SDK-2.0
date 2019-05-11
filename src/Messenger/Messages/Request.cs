using System;

namespace Messenger
{
    public abstract class Request : IRequest
    {
        protected Request(TimeSpan? timeout = null)
        {
            Timeout = timeout ?? TimeSpan.MaxValue;
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        public Request(byte[] data, TimeSpan? timeout = null) : this(timeout)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            Data = data;
        }

        public byte[] Data { get; protected set; }

        public Guid Id { get; }

        public TimeSpan Timeout { get; }

        public DateTime Timestamp { get; }
    }

    public abstract class Request<T> : Request, IRequest<T>
    {
        public Request(T message, TimeSpan? timeout = null) : base(timeout)
        {
            Message = message;
            Data = Serialize(message);
        }

        protected abstract byte[] Serialize(T message);

        public T Message { get; }
    }
}
