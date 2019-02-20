using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class StateServer : IMessageRelayService<IRawDroneState>
    {
        public StateServer(int port)
        {
            _receiver = new UdpReceiver(port);
            _receiver.MessageReceived += _receiver_RelayMessageReceived;
            _receiver.MessageRelayExceptionThrown += _receiver_RelayExceptionThrown;
        }

        private readonly UdpReceiver _receiver;

        public event EventHandler<MessageReceivedArgs<IRawDroneState>> MessageReceived;
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

        private void _receiver_RelayMessageReceived(object sender, MessageReceivedArgs<DataNotification> e)
        {
            try
            {
                var droneState = RawDroneStateUdp.FromDatagram(e.Message.Data);
                var eventArgs = new MessageReceivedArgs<IRawDroneState>(droneState);
                MessageReceived?.Invoke(this, eventArgs);
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
