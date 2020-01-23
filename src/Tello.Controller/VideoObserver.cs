// <copyright file="VideoObserver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;

// using System.Collections.Generic;
using Tello.Events;

namespace Tello.Controller
{
    public sealed class VideoObserver : Observer<IEnvelope>, IVideoObserver
    {
        public VideoObserver(IReceiver receiver)
            : base(receiver)
        {
        }

        public override void OnError(Exception error)
        {
            try
            {
                this.ExceptionThrown?.Invoke(this, error);
            }
            catch
            {
            }
        }

        public override void OnNext(IEnvelope message)
        {
            try
            {
                // _videoSegments.Enqueue(message);
                this.VideoSampleReady?.Invoke(this, new VideoSampleReadyArgs(message));
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        // private Queue<IEnvelope> _videoSegments = new Queue<IEnvelope>();

        // public Queue<IEnvelope> VideoSegments()
        // {
        //    var result = _videoSegments;
        //    _videoSegments = new Queue<IEnvelope>();
        //    return result;
        // }
        public event EventHandler<VideoSampleReadyArgs> VideoSampleReady;

        public event EventHandler<Exception> ExceptionThrown;
    }
}
