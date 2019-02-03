using System;

namespace Tello.Messaging
{
    public class Request : IRequest
    {
        private Request(byte[] data, TimeSpan? timeout = null)
        {
            Data = data;
            Timeout = timeout.HasValue
                ? timeout.Value
                : TimeSpan.MaxValue;

            Id = Guid.NewGuid();
        }

        public static IRequest FromData(byte[] data, TimeSpan? timeout = null)
        {
            return new Request(data, timeout);
        }

        public byte[] Data { get; }

        public Guid Id { get; }

        public TimeSpan Timeout { get; }
    }
}
