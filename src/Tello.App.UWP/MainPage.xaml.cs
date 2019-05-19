#define EMULATOR_OFF

#if EMULATOR_ON
using Messenger.Simulator;
using Tello.Simulator;
#else
using Messenger.Udp;
using System.Net;
#endif

using Repository.Sqlite;
using System.Threading;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Media.MediaProperties;
using Windows.Media.Core;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

namespace Tello.App.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        private readonly IUIDispatcher _dispatcher;
        private readonly IUINotifier _notifier;
#if EMULATOR_ON
        private DroneSimulator _simulator;
#endif

        private MainViewModel CreateMainViewModel(IUIDispatcher dispatcher, IUINotifier notifier)
        {
#if EMULATOR_ON
            _simulator = new DroneSimulator();
            var transceiver = new SimTransceiver(_simulator.MessageHandler);
            var stateReceiver = new SimReceiver(_simulator.StateTransmitter);
            var videoReceiver = new SimReceiver(_simulator.VideoTransmitter);
#else
            var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889);
            var stateReceiver = new UdpReceiver(8890);
            var videoReceiver = new UdpReceiver(11111);
#endif
            return new MainViewModel(
                _dispatcher,
                _notifier,
                new SqliteRepository((null, "tello.sqlite")),
                transceiver,
                stateReceiver,
                videoReceiver);
        }

        public MainPage()
        {
            InitializeComponent();

            _dispatcher = new UIDispatcher(SynchronizationContext.Current);
            _notifier = new UINotifier();

            ViewModel = CreateMainViewModel(_dispatcher, _notifier);
            DataContext = ViewModel;
            ViewModel.VideoViewModel.VideoStreamStarted += VideoViewModel_VideoStreamStarted;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.Open();

            InitializeVideo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Close();

            base.OnNavigatedFrom(e);
        }

        #region video display

        private bool _isPlaying = false;
        private void VideoViewModel_VideoStreamStarted(object sender, bool e)
        {
            Debug.WriteLine($"{nameof(VideoViewModel_VideoStreamStarted)}");
            if (!_isPlaying)
            {
                _isPlaying = true;
                VideoElement.Play();
            }
        }

        private bool _videoInitilized = false;
        private void InitializeVideo()
        {
            if (!_videoInitilized)
            {
                _videoInitilized = true;

                var videoEncodingProperties = VideoEncodingProperties.CreateH264();
                videoEncodingProperties.Height = 720;
                videoEncodingProperties.Width = 960;

                var mediaStreamSource = new MediaStreamSource(new VideoStreamDescriptor(videoEncodingProperties))
                {
                    // never turn live on because it tries to skip frame which breaks the h264 decoding
                    //IsLive = true,
                    BufferTime = TimeSpan.FromSeconds(0.0)
                };

                mediaStreamSource.SampleRequested += MediaStreamSource_SampleRequested;

                VideoElement.SetMediaStreamSource(mediaStreamSource);
                // never turn real time playback on
                //_mediaElement.RealTimePlayback = true;
            }
        }

        private void MediaStreamSource_SampleRequested(
            MediaStreamSource sender,
            MediaStreamSourceSampleRequestedEventArgs args)
        {
#if EMULATOR_ON
#else
            var sample = ViewModel.VideoViewModel.GetSample();
            //Debug.WriteLine($"{nameof(MediaStreamSource_SampleRequested)} - video ready? {sample != null}");
            if (sample != null)
            {
                //Debug.WriteLine($"{nameof(MediaStreamSource_SampleRequested)} - got sample time index {sample.TimeIndex}, length {sample.Buffer.Length}b, duration {sample.Duration}");
                args.Request.Sample = MediaStreamSample.CreateFromBuffer(sample.Buffer.AsBuffer(), sample.TimeIndex);
                args.Request.Sample.Duration = sample.Duration; 
            }
#endif
        }
        #endregion
    }
}
