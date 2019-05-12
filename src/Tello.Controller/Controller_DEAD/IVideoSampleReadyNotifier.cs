using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Controller
{
    public interface IVideoSampleReadyNotifier
    {
        event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
    }
}
