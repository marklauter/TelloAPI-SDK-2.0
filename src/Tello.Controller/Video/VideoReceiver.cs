using System;
using Tello.Messaging;

namespace Tello.Controller.Video
{
    public sealed class VideoReceiver : IReceiver<Frame>
    {
        public VideoReceiver(double frameRate, int secondsToBuffer, IReceiver<INotification> receiver)
        {
            _frameComposer = new FrameComposer(frameRate, secondsToBuffer);

            _receiver = receiver;
        }

        private readonly FrameComposer _frameComposer;
        private readonly IReceiver<INotification> _receiver;

        public FrameCollection GetSample(TimeSpan timeout)
        {
            return _frameComposer.GetFrames(timeout);
        }

        public bool TryReadFrame(out Frame frame, TimeSpan timeout)
        {
            return _frameComposer.TryGetFrame(out frame, timeout);
        }

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IReceiver<Frame>, Frame> messageHandler, Action<IReceiver<Frame>, Exception> errorHandler)
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
