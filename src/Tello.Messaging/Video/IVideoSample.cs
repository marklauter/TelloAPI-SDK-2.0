using System;

namespace Tello.Messaging
{
    public interface IVideoSample
    {
        byte[] Content { get; }
        TimeSpan Duration { get; }
        TimeSpan TimeIndex { get; }
        int Size { get; }
    }
}
