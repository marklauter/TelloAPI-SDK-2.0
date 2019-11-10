// <copyright file="MainPage.xaml.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#define EMULATOR_OFF

#if EMULATOR_ON
using Messenger.Simulator;
using Tello.Simulator;
#else
using System.Net;
using Messenger.Udp;
#endif

using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Repository.Sqlite;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tello.App.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        private readonly IUIDispatcher dispatcher;
        private readonly IUINotifier notifier;
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
                this.dispatcher,
                this.notifier,
                new SqliteRepository((null, "tello.sqlite")),
                transceiver,
                stateReceiver,
                videoReceiver);
        }

        public MainPage()
        {
            this.InitializeComponent();

            this.dispatcher = new UIDispatcher(SynchronizationContext.Current);
            this.notifier = new UINotifier();

            this.ViewModel = this.CreateMainViewModel(this.dispatcher, this.notifier);
            this.DataContext = this.ViewModel;
            this.ViewModel.ControllerViewModel.PropertyChanged += this.ControllerViewModel_PropertyChanged;
        }

        private void ControllerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = typeof(TelloControllerViewModel).GetProperty(e.PropertyName);
            var value = property.GetValue(sender);
            Debug.WriteLine($"{nameof(this.ControllerViewModel_PropertyChanged)} - property '{e.PropertyName}', value '{value}'");

            if (e.PropertyName == nameof(TelloControllerViewModel.IsVideoStreaming) && (bool)value)
            {
                this.VideoElement.Play();
            }
            else
            {
                this.VideoElement.Stop();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel.Open();

            this.InitializeVideo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.Close();

            base.OnNavigatedFrom(e);
        }

        #region video display

        private bool videoInitilized = false;

        private void InitializeVideo()
        {
            if (!this.videoInitilized)
            {
                this.videoInitilized = true;

                var videoEncodingProperties = VideoEncodingProperties.CreateH264();
                videoEncodingProperties.Height = 720;
                videoEncodingProperties.Width = 960;

                var mediaStreamSource = new MediaStreamSource(new VideoStreamDescriptor(videoEncodingProperties))
                {
                    // never turn live on because it tries to skip frame which breaks the h264 decoding
                    //IsLive = true,
                    BufferTime = TimeSpan.FromSeconds(0.0),
                };

                mediaStreamSource.SampleRequested += this.MediaStreamSource_SampleRequested;

                this.VideoElement.SetMediaStreamSource(mediaStreamSource);
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
            var sample = this.ViewModel.VideoViewModel.GetSample();
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
