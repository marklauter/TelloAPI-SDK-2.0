using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Tello.Controller.Video
{
    internal sealed class FrameComposer
    {
        public FrameComposer(double frameRate, int secondsToBuffer = 5)
        {
            _frameRate = frameRate;
            _frameDuration = TimeSpan.FromSeconds(1 / _frameRate);

            _frames = new RingBuffer<VideoFrame>((int)(_frameRate * secondsToBuffer));
        }

        #region fields
        private long _byteCount = 0;
        private long _frameIndex = 0;
        private readonly Stopwatch _frameRateWatch = new Stopwatch();
        private MemoryStream _stream = null;
        private readonly TimeSpan _frameDuration;
        private readonly double _frameRate;
        private readonly RingBuffer<VideoFrame> _frames;
        #endregion

        #region frame composition
        private bool IsNewH264Frame(byte[] sample)
        {
            // check sample for 0x00 0x00 0x00 0x01 header - H264 NALU frame start delimiter
            return sample.Length > 4 && sample[0] == 0x00 && sample[1] == 0x00 && sample[2] == 0x00 && sample[3] == 0x01;
        }

        private VideoFrame QueueFrame(MemoryStream stream, long frameIndex)
        {
            var frame = new VideoFrame(stream.ToArray(), frameIndex, TimeSpan.FromSeconds(frameIndex / _frameRate), _frameDuration);
            _frames.Push(frame);
            return frame;
        }

        private VideoFrame ComposeFrame(byte[] sample)
        {
            var frame = default(VideoFrame);
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
                        Debug.WriteLine($"\nFC {frame.TimeIndex}: f#{frame.Index}, composition rate: {(frame.Index / _frameRateWatch.Elapsed.TotalSeconds).ToString("#,#")}f/s, bit rate: {((uint)(_byteCount * 8 / frame.TimeIndex.TotalSeconds)).ToString("#,#")}b/s");
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
        /// <returns>Frame if this sample completes a frame, else null.</returns>
        public VideoFrame AddSample(byte[] sample)
        {
            return ComposeFrame(sample);
        }

        internal FrameCollection GetFrames(TimeSpan timeout)
        {
            var wait = new SpinWait();
            var clock = Stopwatch.StartNew();
            while (_frames.Count == 0 && clock.Elapsed < timeout)
            {
                wait.SpinOnce();
            }

            var frames = _frames.Flush();
            return frames != null && frames.Length > 0
                ? new FrameCollection(frames)
                : null;
        }

        private Stopwatch _frameStopWatch = new Stopwatch();
        private SpinWait _frameWait = new SpinWait();
        public bool TryGetFrame(out VideoFrame frame, TimeSpan timeout)
        {
            _frameStopWatch.Restart();
            while (!_frames.TryPop(out frame) && _frameStopWatch.Elapsed < timeout)
            {
                _frameWait.SpinOnce();
            }
            return frame != null;
        }
    }
}
