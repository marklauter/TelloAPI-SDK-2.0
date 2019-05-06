using System;

namespace Tello.Messaging
{
    public class Request : IMessage
    {
        private Request(byte[] data, TimeSpan? timeout = null)
        {
            Data = data;
            Timeout = timeout.HasValue
                ? timeout.Value
                : TimeSpan.MaxValue;

            Id = Guid.NewGuid();
        }

        public static IMessage FromData(byte[] data, TimeSpan? timeout = null)
        {
            return new Request(data, timeout);
        }

        public byte[] Data { get; }

        public Guid Id { get; }

        public TimeSpan Timeout { get; }
    }
}
