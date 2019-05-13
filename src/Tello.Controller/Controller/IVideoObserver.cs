using Messenger;
using System;
using System.Collections.Generic;
using Tello.Controller.Events;

namespace Tello.Controller
{
    public interface IVideoObserver : IObserver<IEnvelope>
    {
        event EventHandler<VideoSampleReadyArgs> VideoSampleReady;

        Queue<IEnvelope> VideoSegments();
    }
}
