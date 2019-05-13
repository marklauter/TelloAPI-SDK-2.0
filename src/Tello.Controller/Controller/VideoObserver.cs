using Messenger;
using System;
using System.Collections.Generic;
using Tello.Controller.Events;

namespace Tello.Controller
{
    public sealed class VideoObserver : Observer<IEnvelope>, IVideoObserver
    {
        public VideoObserver(IReceiver receiver) : base(receiver)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(IEnvelope message)
        {
            _videoSegments.Enqueue(message);
            VideoSampleReady?.Invoke(this, new VideoSampleReadyArgs(message));
        }

        private Queue<IEnvelope> _videoSegments = new Queue<IEnvelope>();

        public Queue<IEnvelope> VideoSegments()
        {
            var result = _videoSegments;
            _videoSegments = new Queue<IEnvelope>();
            return result;
        }

        public event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
    }
}
