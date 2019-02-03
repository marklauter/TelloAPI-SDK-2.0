using System;
using Tello.Messaging;

namespace Tello.Controller.Video
{
    internal sealed class VideoFrame : IVideoFrame
    {
        public VideoFrame(byte[] content, int frameIndex, TimeSpan timeIndex, TimeSpan duration)
        {
            Content = content;
            Duration = duration;
            TimeIndex = timeIndex;
            Size = content.Length;
            Index = frameIndex;
        }

        public byte[] Content { get; }
        public TimeSpan Duration { get; } = TimeSpan.FromSeconds(1 / 30.0);
        public TimeSpan TimeIndex { get; }
        public int Size { get; }
        public int Index { get; }
    }
}
