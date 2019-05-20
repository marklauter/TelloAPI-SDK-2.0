using System;

namespace Tello.Events
{
    public sealed class VideoStreamingStateChangedArgs : EventArgs
    {
        public VideoStreamingStateChangedArgs(bool isStreaming)
        {
            IsStreaming = isStreaming;
        }

        public bool IsStreaming { get; }
    }
}
