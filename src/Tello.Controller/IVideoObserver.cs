using Messenger;
using System;
//using System.Collections.Generic;
using Tello.Events;

namespace Tello.Controller
{
    public interface IVideoObserver : IObserver<IEnvelope>
    {
        event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
        event EventHandler<Exception> ExceptionThrown;

        //Queue<IEnvelope> VideoSegments();
    }
}
