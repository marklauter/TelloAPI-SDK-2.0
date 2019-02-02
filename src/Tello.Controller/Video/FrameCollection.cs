using System;
using System.IO;

namespace Tello.Controller.Video
{
    public sealed class FrameCollection
    {
        public FrameCollection(Frame frame = null)
        {
            _sample = new MemoryStream(frame.Size);
            if (frame != null)
            {
                Add(frame);
            }
        }

        public FrameCollection(Frame[] frames)
        {
            if (frames == null || frames.Length == 0)
            {
                throw new ArgumentNullException(nameof(frames));
            }

            Count = frames.Length;

            var frame = frames[0];
            TimeIndex = frame.TimeIndex;
            Duration += frame.Duration;

            _sample = new MemoryStream(frames.Length * 1024 * 16);
            _sample.Write(frame.Content, 0, frame.Size);

            for (var i = 1; i < frames.Length; ++i)
            {
                frame = frames[i];
                _sample.Write(frame.Content, 0, frame.Size);
                Duration += frame.Duration;
            }
        }

        public void Add(Frame frame)
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
        public long Size => _sample.Length;
        public TimeSpan TimeIndex { get; private set; }
        public int Count { get; private set; } = 0;
    }
}
