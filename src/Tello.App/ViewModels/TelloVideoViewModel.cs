// <copyright file="TelloVideoViewModel.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Tello.App.MvvM;
using Tello.Controller;

namespace Tello.App.ViewModels
{
    public class TelloVideoViewModel : ViewModel
    {
        private readonly IVideoObserver videoObserver;
        private readonly Stopwatch watch = new Stopwatch();

        public TelloVideoViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IVideoObserver videoObserver)
            : base(dispatcher, notifier)
        {
            this.videoObserver = videoObserver ?? throw new ArgumentNullException(nameof(videoObserver));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            this.videoObserver.VideoSampleReady += this.Observer_VideoSampleReady;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            this.videoObserver.VideoSampleReady -= this.Observer_VideoSampleReady;
        }

        private TimeSpan timeIndex = TimeSpan.FromSeconds(0);

        private void Observer_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            this.timeIndex = this.watch.Elapsed;
            var sample = new VideoSample(e.Message.Data, this.timeIndex, this.watch.Elapsed - this.timeIndex);
            if (!this.watch.IsRunning)
            {
                this.watch.Start();
            }

            this.samples.Enqueue(sample);
        }

        private readonly ConcurrentQueue<VideoSample> samples = new ConcurrentQueue<VideoSample>();

        public VideoSample GetSample()
        {
            var wait = default(SpinWait);
            VideoSample result;
            while (this.samples.Count == 0 || !this.samples.TryDequeue(out result))
            {
                wait.SpinOnce();
            }

            return result;
        }

        // todo: implementation - UWP should override this class and implement a MediaStreamSource property which can be used by the MediaElement control
        // public MediaStreamSource

        // todo: maybe we can actually decode the h264 and just provide a Bitmap or Image property???
    }
}
