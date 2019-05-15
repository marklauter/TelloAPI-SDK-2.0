using System;
using System.Threading.Tasks;

namespace Tello.Simulator.Transmitters
{
    public class Transmitter : ITransmitter
    {
        private byte[] _buffer = Array.Empty<byte>();

        protected byte[] Buffer
        {
            get => _buffer;
            set
            {
                _buffer = value ?? throw new System.ArgumentNullException(nameof(value));
                _available = _buffer.Length;
            }
        }

        protected int _available = 0;
        public int Available => _available;

        public Task<ReceiveResult> ReceiveAsync()
        {
            _available = 0;
            var result = new ReceiveResult(Buffer);
            Buffer = Array.Empty<byte>();
            return Task.FromResult(result);
        }
    }
}
