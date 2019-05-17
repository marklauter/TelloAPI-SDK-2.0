using Messenger;
using System;
//using System.Collections.Generic;
using Tello.Events;

namespace Tello.Controller
{
    public sealed class VideoObserver : Observer<IEnvelope>, IVideoObserver
    {
        public VideoObserver(IReceiver receiver) : base(receiver)
        {
        }

        public override void OnError(Exception error)
        {
            try
            {
                ExceptionThrown?.Invoke(this, error);
            }
            catch { }
        }

        public override void OnNext(IEnvelope message)
        {
            try
            {
                //_videoSegments.Enqueue(message);
                VideoSampleReady?.Invoke(this, new VideoSampleReadyArgs(message));
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        //private Queue<IEnvelope> _videoSegments = new Queue<IEnvelope>();

        //public Queue<IEnvelope> VideoSegments()
        //{
        //    var result = _videoSegments;
        //    _videoSegments = new Queue<IEnvelope>();
        //    return result;
        //}

        public event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
        public event EventHandler<Exception> ExceptionThrown;
    }
}
