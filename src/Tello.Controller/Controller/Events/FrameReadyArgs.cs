using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public sealed class FrameReadyArgs : EventArgs
    {
        public FrameReadyArgs(IVideoFrame frame)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public IVideoFrame Frame { get; }
    }
}
