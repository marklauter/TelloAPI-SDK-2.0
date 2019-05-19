using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Tello.App.MvvM;
using Tello.Controller;

namespace Tello.App.ViewModels
{
    public sealed class VideoSample
    {
        public byte[] Buffer { get; }
        public TimeSpan TimeIndex { get; }
        public TimeSpan Duration { get; }

        public int Length => Buffer.Length;

        public VideoSample()
            : this(
                 Array.Empty<byte>(),
                 TimeSpan.FromSeconds(0),
                 TimeSpan.FromSeconds(0))
        {
        }

        public VideoSample(byte[] buffer, TimeSpan timeIndex, TimeSpan duration)
        {
            Buffer = buffer;
            TimeIndex = timeIndex;
            Duration = duration;
        }
    }

    public class TelloVideoViewModel : ViewModel
    {
        private readonly IVideoObserver _videoObserver;
        private readonly Stopwatch _watch = new Stopwatch();
        private bool _streamStarted = false;

        public event EventHandler<bool> VideoStreamStarted;
        public event EventHandler VideoSampleReady;

        public TelloVideoViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IVideoObserver videoObserver)
            : base(dispatcher, notifier)
        {
            _videoObserver = videoObserver ?? throw new ArgumentNullException(nameof(videoObserver));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _videoObserver.VideoSampleReady += Observer_VideoSampleReady;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _videoObserver.VideoSampleReady -= Observer_VideoSampleReady;
        }

        private TimeSpan _timeIndex = TimeSpan.FromSeconds(0);
        private void Observer_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            _timeIndex = _watch.Elapsed;
            _samples.Enqueue(new VideoSample(e.Message.Data, _timeIndex, _watch.Elapsed - _timeIndex));

            if (!_watch.IsRunning)
            {
                _watch.Start();
            }
            //VideoSampleReady?.Invoke(this, EventArgs.Empty);

            if (!_streamStarted)
            {
                _streamStarted = true;
                Debug.WriteLine($"{nameof(Observer_VideoSampleReady)} - stream started");
                Dispatcher.Invoke(() => VideoStreamStarted?.Invoke(this, true));
            }
        }

        private readonly ConcurrentQueue<VideoSample> _samples = new ConcurrentQueue<VideoSample>();

        public VideoSample GetSample()
        {
            return _samples.TryDequeue(out var result) ? result : null;
        }


        //todo: implementation - UWP should override this class and implement a MediaStreamSource property which can be used by the MediaElement control
        //public MediaStreamSource

        //todo: maybe we can actually decode the h264 and just provide a Bitmap or Image property???
    }
}
