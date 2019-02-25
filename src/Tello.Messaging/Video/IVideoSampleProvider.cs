using System;

namespace Tello.Messaging
{
    public interface IVideoSampleProvider
    {
        bool TryGetSample(out IVideoSample sample, TimeSpan timeout);
        bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout);
    }
}
