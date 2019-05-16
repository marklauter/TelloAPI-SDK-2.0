using Messenger.Simulator;

namespace Tello.Simulator.Messaging
{
    internal class ReceiveResult : IReceiveResult
    {
        public ReceiveResult(byte[] buffer)
        {
            Buffer = buffer ?? throw new System.ArgumentNullException(nameof(buffer));
        }

        public byte[] Buffer { get; }
    }
}