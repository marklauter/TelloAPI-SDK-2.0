using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class VideoServer : IRelayService<IVideoSample>
    {
        public VideoServer(double frameRate, int secondsToBuffer, int port)
        {
            _frameComposer = new VideoFrameComposer(frameRate, secondsToBuffer);
            _receiver = new UdpReceiver(port);
        }

        private readonly VideoFrameComposer _frameComposer;
        private readonly UdpReceiver _receiver;

        public bool TryGetSample(out IVideoSample sample, TimeSpan timeout)
        {
            return _frameComposer.TryGetSample(out sample, timeout);
        }

        public bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            return _frameComposer.TryGetFrame(out frame, timeout);
        }

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IRelayService<IVideoSample>, IVideoSample> messageHandler, Action<IRelayService<IVideoSample>, Exception> errorHandler)
        {
            _receiver.Listen(
                (receiver, notification) =>
                {
                    try
                    {
                        var frame = _frameComposer.AddSample(notification.Data);
                        if (frame != null)
                        {
                            messageHandler?.Invoke(this, frame);
                        }
                    }
                    catch(Exception ex)
                    {
                        errorHandler?.Invoke(this, ex);
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
