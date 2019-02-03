using System;
using Tello.Messaging;

namespace Tello.Controller.Video
{
    internal sealed class VideoReceiver : IRelay<IVideoFrame>
    {
        public VideoReceiver(IRelay<INotification> receiver, double frameRate, int secondsToBuffer)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _frameComposer = new FrameComposer(frameRate, secondsToBuffer);
        }

        private readonly FrameComposer _frameComposer;
        private readonly IRelay<INotification> _receiver;

        public FrameCollection GetSample(TimeSpan timeout)
        {
            return _frameComposer.GetFrames(timeout);
        }

        public bool TryReadFrame(out VideoFrame frame, TimeSpan timeout)
        {
            return _frameComposer.TryGetFrame(out frame, timeout);
        }

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IRelay<IVideoFrame>, IVideoFrame> messageHandler, Action<IRelay<IVideoFrame>, Exception> errorHandler)
        {
            _receiver.Listen(
                (receiver, notification) =>
                {
                    var frame = _frameComposer.AddSample(notification.Data);
                    if (frame != null)
                    {
                        messageHandler?.Invoke(this, frame);
                    }
                },
                (receiver, ex) => errorHandler?.Invoke(this, ex));
        }

        public void Stop()
        {
            _receiver.Stop();
        }
    }
}
