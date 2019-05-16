using Messenger;
using System;

namespace Tello.Events
{
    public sealed class VideoSampleReadyArgs : EventArgs
    {
        public VideoSampleReadyArgs(IEnvelope message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public IEnvelope Message { get; }
    }
}
