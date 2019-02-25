using System;
using System.Collections.Generic;
using System.Text;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.App.ViewModels
{
    public class TelloVideoViewModel : ViewModel
    {
        private readonly IVideoSampleReadyNotifier _videoSampleReadyNotifier;
        private readonly IVideoSampleProvider _videoSampleProvider;

        public TelloVideoViewModel(IUIDispatcher dispatcher, IVideoSampleReadyNotifier videoSampleReadyNotifier, IVideoSampleProvider videoSampleProvider) : base(dispatcher)
        {
            _videoSampleReadyNotifier = videoSampleReadyNotifier ?? throw new ArgumentNullException(nameof(videoSampleReadyNotifier));
            _videoSampleProvider = videoSampleProvider ?? throw new ArgumentNullException(nameof(videoSampleProvider));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _videoSampleReadyNotifier.VideoSampleReady += OnVideoSampleReady;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _videoSampleReadyNotifier.VideoSampleReady -= OnVideoSampleReady;
        }

        private void OnVideoSampleReady(object sender, VideoSampleReadyArgs e)
        {
            
        }

        //todo: implementation - UWP should override this class and implement a MediaStreamSource property which can be used by the MediaElement control
        //public MediaStreamSource

        //todo: maybe we can actually decode the h264 and just provide a Bitmap or Image property???
    }
}
