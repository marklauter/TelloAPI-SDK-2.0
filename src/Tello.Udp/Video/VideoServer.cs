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
            _receiver.RelayMessageReceived += _receiver_RelayMessageReceived;
            _receiver.RelayExceptionThrown += _receiver_RelayExceptionThrown;
        }

        private readonly UdpReceiver _receiver;
        private readonly VideoFrameComposer _frameComposer;

        public event EventHandler<RelayMessageReceivedArgs<IVideoSample>> RelayMessageReceived;
        public event EventHandler<RelayExceptionThrownArgs> RelayExceptionThrown;

        public ReceiverStates State => _receiver.State;

        public void Start()
        {
            _receiver.Start();
        }

        public void Stop()
        {
            _receiver.Stop();
        }

        public bool TryGetSample(out IVideoSample sample, TimeSpan timeout)
        {
            return _frameComposer.TryGetSample(out sample, timeout);
        }

        public bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            return _frameComposer.TryGetFrame(out frame, timeout);
        }

        private void _receiver_RelayMessageReceived(object sender, RelayMessageReceivedArgs<DataNotification> e)
        {
            try
            {
                var frame = _frameComposer.AddSample(e.Message.Data);
                if (frame != null)
                {
                    RelayMessageReceived?.Invoke(this, new RelayMessageReceivedArgs<IVideoSample>(frame));
                }
            }
            catch (Exception ex)
            {
                RelayExceptionThrown?.Invoke(this, new RelayExceptionThrownArgs(ex));
            }
        }

        private void _receiver_RelayExceptionThrown(object sender, RelayExceptionThrownArgs e)
        {
            RelayExceptionThrown?.Invoke(this, e);
        }
    }
}
