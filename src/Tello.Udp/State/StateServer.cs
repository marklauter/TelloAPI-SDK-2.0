using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class StateServer : IRelayService<IRawDroneState>
    {
        public StateServer(int port)
        {
            _receiver = new UdpReceiver(port);
            _receiver.RelayMessageReceived += _receiver_RelayMessageReceived;
            _receiver.RelayExceptionThrown += _receiver_RelayExceptionThrown;
        }

        private readonly UdpReceiver _receiver;

        public event EventHandler<RelayMessageReceivedArgs<IRawDroneState>> RelayMessageReceived;
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

        private void _receiver_RelayMessageReceived(object sender, RelayMessageReceivedArgs<DataNotification> e)
        {
            try
            {
                var droneState = DroneState.FromDatagram(e.Message.Data);
                var eventArgs = new RelayMessageReceivedArgs<IRawDroneState>(droneState);
                RelayMessageReceived?.Invoke(this, eventArgs);
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
