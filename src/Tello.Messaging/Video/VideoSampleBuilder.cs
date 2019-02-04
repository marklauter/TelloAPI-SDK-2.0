using System;
using System.IO;

namespace Tello.Messaging
{
    public class VideoSampleBuilder : IVideoSampleBuilder
    {
        public VideoSampleBuilder(IVideoSample sample)
        {
            if (sample == null || sample.Size == 0)
            {
                throw new ArgumentNullException(nameof(sample));
            }

            _sampleBuffer = new MemoryStream(sample.Size);
            TimeIndex = sample.TimeIndex;
            Add(sample);
        }

        public VideoSampleBuilder(IVideoSample[] samples)
        {
            if (samples == null || samples.Length == 0)
            {
                throw new ArgumentNullException(nameof(samples));
            }

            var sample = samples[0];
            TimeIndex = sample.TimeIndex;
            Duration = sample.Duration;

            var sampleSize = 0;
            for(var i = 0; i < samples.Length; ++i)
            {
                sampleSize += samples[i].Size;
            }

            _sampleBuffer = new MemoryStream(sampleSize);
            _sampleBuffer.Write(sample.Content, 0, sample.Size);

            for (var i = 1; i < samples.Length; ++i)
            {
                sample = samples[i];
                _sampleBuffer.Write(sample.Content, 0, sample.Size);
                Duration += sample.Duration;
            }
        }

        public void Add(IVideoSample sample)
        {
            if (sample == null)
            {
                throw new ArgumentNullException(nameof(sample));
            }

            _sampleBuffer.Write(sample.Content, 0, sample.Size);
            Duration += sample.Duration;
        }

        private readonly MemoryStream _sampleBuffer;

        public byte[] Content => _sampleBuffer.ToArray();
        public TimeSpan Duration { get; private set; } = TimeSpan.FromSeconds(0.0);
        public TimeSpan TimeIndex { get; private set; }
        public int Size => (int)_sampleBuffer.Length;
    }
}
