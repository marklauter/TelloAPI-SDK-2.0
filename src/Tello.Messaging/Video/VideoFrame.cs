using System;

namespace Tello.Messaging
{
    public class VideoFrame : IVideoFrame
    {
        public VideoFrame(byte[] content, int frameIndex, TimeSpan timeIndex, TimeSpan duration)
        {
            Content = content;
            Duration = duration;
            TimeIndex = timeIndex;
            Size = content.Length;
            FrameIndex = frameIndex;
        }

        public byte[] Content { get; }
        public TimeSpan Duration { get; } 
        public TimeSpan TimeIndex { get; }
        public int Size { get; }
        public int FrameIndex { get; }
    }
}
