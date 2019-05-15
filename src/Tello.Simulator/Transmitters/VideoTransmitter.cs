using System.Linq;

namespace Tello.Simulator.Transmitters
{
    public sealed class VideoTransmitter : Transmitter
    {
        public void AddVideoSegment(byte[] segment)
        {
            if (segment != null)
            {
                _available = 0;
                Buffer = Buffer.Union(segment).ToArray();
            }
        }
    }
}

