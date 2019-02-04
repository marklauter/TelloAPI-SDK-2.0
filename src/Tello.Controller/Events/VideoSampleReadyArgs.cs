using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class VideoSampleReadyArgs : EventArgs
    {
        public VideoSampleReadyArgs(IVideoSample sample)
        {
            Sample = sample ?? throw new ArgumentNullException(nameof(sample));
        }

        public IVideoSample Sample { get; }
    }
}
