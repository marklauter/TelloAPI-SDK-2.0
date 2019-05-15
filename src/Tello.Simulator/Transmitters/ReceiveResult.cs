namespace Tello.Simulator.Transmitters
{
    public struct ReceiveResult
    {
        public ReceiveResult(byte[] buffer)
        {
            Buffer = buffer ?? throw new System.ArgumentNullException(nameof(buffer));
        }

        public byte[] Buffer { get; }
    }
}