using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class VideoSampleReadyArgs : EventArgs
    {
        public VideoSampleReadyArgs(IVideoFrame sample)
        {
            Sample = sample ?? throw new ArgumentNullException(nameof(sample));
        }

        public IVideoFrame Sample { get; }
    }
}
