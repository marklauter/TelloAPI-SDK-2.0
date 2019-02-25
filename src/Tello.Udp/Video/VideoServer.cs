using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class VideoServer : IMessageRelayService<IVideoSample>, IVideoSampleProvider
    {
        public VideoServer(double frameRate, int secondsToBuffer, int port)
        {
            _frameComposer = new VideoFrameComposer(frameRate, secondsToBuffer);
            _receiver = new UdpReceiver(port);
            _receiver.MessageReceived += _receiver_RelayMessageReceived;
            _receiver.MessageRelayExceptionThrown += _receiver_RelayExceptionThrown;
        }

        private readonly UdpReceiver _receiver;
        private readonly VideoFrameComposer _frameComposer;

        public event EventHandler<MessageReceivedArgs<IVideoSample>> MessageReceived;
        public event EventHandler<MessageRelayExceptionThrownArgs> MessageRelayExceptionThrown;

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
            sample = null;
            return _receiver.State == ReceiverStates.Listening && _frameComposer.TryGetSample(out sample, timeout);
        }

        public bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            frame = null;
            return _receiver.State == ReceiverStates.Listening && _frameComposer.TryGetFrame(out frame, timeout);
        }

        private void _receiver_RelayMessageReceived(object sender, MessageReceivedArgs<DataNotification> e)
        {
            try
            {
                var frame = _frameComposer.AddSample(e.Message.Data);
                if (frame != null)
                {
                    MessageReceived?.Invoke(this, new MessageReceivedArgs<IVideoSample>(frame));
                }
            }
            catch (Exception ex)
            {
                MessageRelayExceptionThrown?.Invoke(this, new MessageRelayExceptionThrownArgs(ex));
            }
        }

        private void _receiver_RelayExceptionThrown(object sender, MessageRelayExceptionThrownArgs e)
        {
            MessageRelayExceptionThrown?.Invoke(this, e);
        }
    }
}
