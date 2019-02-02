using System;

namespace Tello.Controller.Video
{
    public sealed class FrameReadyArgs : EventArgs
    {
        public FrameReadyArgs(Frame frame)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public Frame Frame { get; }
    }
}
