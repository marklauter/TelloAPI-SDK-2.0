using System;

namespace Messenger
{
    public class Envelope : IEnvelope
    {
        public Envelope(byte[] data)
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

            Timestamp = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public DateTime Timestamp { get; }

        public byte[] Data { get; }
    }
}
