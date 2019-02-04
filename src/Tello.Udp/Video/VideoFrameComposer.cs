using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class VideoFrameComposer
    {
        internal VideoFrameComposer(double frameRate, int secondsToBuffer = 5)
        {
            _frameRate = frameRate;
            _frameDuration = TimeSpan.FromSeconds(1 / _frameRate);
            _frames = new RingBuffer<IVideoFrame>((int)(_frameRate * secondsToBuffer));
        }

        #region fields
        private long _byteCount = 0;
        private int _frameIndex = 0;
        private readonly Stopwatch _frameRateWatch = new Stopwatch();

        private MemoryStream _stream = null;
        private readonly TimeSpan _frameDuration;
        private readonly double _frameRate;
        private readonly RingBuffer<IVideoFrame> _frames;
        #endregion

        #region frame composition
        private bool IsNewH264Frame(byte[] sample)
        {
            // check sample for 0x00 0x00 0x00 0x01 header - H264 NALU frame start delimiter
            return sample.Length > 4 && sample[0] == 0x00 && sample[1] == 0x00 && sample[2] == 0x00 && sample[3] == 0x01;
        }

        private IVideoFrame QueueFrame(MemoryStream stream, int frameIndex)
        {
            var frame = new VideoFrame(stream.ToArray(), frameIndex, TimeSpan.FromSeconds(frameIndex / _frameRate), _frameDuration);
            _frames.Push(frame);
            return frame;
        }

        private IVideoFrame ComposeFrame(byte[] sample)
        {
            var frame = default(IVideoFrame);
            if (IsNewH264Frame(sample))
            {
                // close out existing frame
                if (_stream != null)
                {
                    frame = QueueFrame(_stream, _frameIndex);
                    _stream.Dispose();
                    Interlocked.Increment(ref _frameIndex);
                    #region debug
#if DEBUG
                    // write a frame sample to debug output ~every 5 seconds so we can see how we're doing with performance
                    if (_frameIndex % (_frameRate * 5) == 0)
                    {
                        Debug.WriteLine($"\nFC {frame.TimeIndex}: f#{frame.FrameIndex}, composition rate: {(frame.FrameIndex / _frameRateWatch.Elapsed.TotalSeconds).ToString("#,#")}f/s, bit rate: {((uint)(_byteCount * 8 / frame.TimeIndex.TotalSeconds)).ToString("#,#")}b/s");
                    }
#endif
                    #endregion
                }
                else
                {
                    // first frame, so start timer
                    _frameRateWatch.Start();
                }
                _stream = new MemoryStream(1024 * 16);
            }

            // don't start writing until we have a received a frame start sequence
            if (_stream != null)
            {
                _stream.Write(sample, 0, sample.Length);
                Interlocked.Add(ref _byteCount, sample.Length);
            }
            return frame;
        }
        #endregion

        /// <summary>
        /// returning a frame now instead of raising event for OnFrameReady
        /// </summary>
        /// <param name="sample"></param>
        /// <returns>IVideoFrame if this sample completes a frame, else null.</returns>
        internal IVideoFrame AddSample(byte[] sample)
        {
            return ComposeFrame(sample);
        }

        internal bool TryGetSample(out IVideoSample sample, TimeSpan timeout)
        {
            var wait = new SpinWait();
            var clock = Stopwatch.StartNew();
            while (_frames.Count == 0 && clock.Elapsed < timeout)
            {
                wait.SpinOnce();
            }

            var frames = _frames.Flush();
            sample = frames != null && frames.Length > 0
                ? sample = new VideoSampleBuilder(frames)
                : null;
            return sample != null;
        }

        internal bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            var wait = new SpinWait();
            var clock = Stopwatch.StartNew();
            while (!_frames.TryPop(out frame) && clock.Elapsed < timeout)
            {
                wait.SpinOnce();
            }
            return frame != null;
        }
    }
}
