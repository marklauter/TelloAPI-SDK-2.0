using System;
using Tello.App.MvvM;
using Tello.Controller;

namespace Tello.App.ViewModels
{
    public class TelloVideoViewModel : ViewModel
    {
        private readonly IVideoObserver _videoObserver;

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
            _videoObserver.VideoSampleReady += VideoSampleReady;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _videoObserver.VideoSampleReady -= VideoSampleReady;
        }

        private void VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            throw new NotImplementedException(nameof(VideoSampleReady));
        }


        //todo: implementation - UWP should override this class and implement a MediaStreamSource property which can be used by the MediaElement control
        //public MediaStreamSource

        //todo: maybe we can actually decode the h264 and just provide a Bitmap or Image property???
    }
}
