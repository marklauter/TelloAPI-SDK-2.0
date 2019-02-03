using System;
using System.IO;
using Tello.Messaging;

namespace Tello.Controller.Video
{
    public sealed class FrameCollection : IVideoFrameCollection
    {
        public FrameCollection(IVideoSample frame = null)
        {
            _sample = new MemoryStream(frame.Size);
            if (frame != null)
            {
                Add(frame);
            }
        }

        public FrameCollection(IVideoFrame[] frames)
        {
            if (frames == null || frames.Length == 0)
            {
                throw new ArgumentNullException(nameof(frames));
            }

            Count = frames.Length;

            var frame = frames[0];
            TimeIndex = frame.TimeIndex;
            Duration += frame.Duration;

            var sampleSize = 0;
            for(var i = 0; i < frames.Length; ++i)
            {
                sampleSize += frames[i].Size;
            }

            _sample = new MemoryStream(sampleSize);
            _sample.Write(frame.Content, 0, frame.Size);

            for (var i = 1; i < frames.Length; ++i)
            {
                frame = frames[i];
                _sample.Write(frame.Content, 0, frame.Size);
                Duration += frame.Duration;
            }
        }

        public void Add(IVideoSample frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (Count == 0)
            {
                TimeIndex = frame.TimeIndex;
            }

            _sample.Write(frame.Content, 0, frame.Size);
            Duration += frame.Duration;
            ++Count;
        }

        private readonly MemoryStream _sample;

        public byte[] Content => _sample.ToArray();
        public TimeSpan Duration { get; private set; } = TimeSpan.FromSeconds(0.0);
        public TimeSpan TimeIndex { get; private set; }
        public int Size => (int)_sample.Length;
        public int Count { get; private set; } = 0;
    }
}
