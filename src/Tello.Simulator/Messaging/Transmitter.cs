using Messenger.Simulator;
using System;
using System.Threading.Tasks;

namespace Tello.Simulator.Messaging
{
    internal class Transmitter : IDroneTransmitter
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

        public Task<IReceiveResult> ReceiveAsync()
        {
            _available = 0;
            var result = new ReceiveResult(Buffer) as IReceiveResult;
            Buffer = Array.Empty<byte>();
            return Task.FromResult(result);
        }
    }
}
