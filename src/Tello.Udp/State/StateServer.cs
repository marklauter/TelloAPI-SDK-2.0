using System;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class StateServer : IRelayService<IDroneState>
    {
        public StateServer(int port)
        {
            _receiver = new UdpReceiver(port);
        }

        private readonly UdpReceiver _receiver;

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IRelayService<IDroneState>, IDroneState> messageHandler, Action<IRelayService<IDroneState>, Exception> errorHandler)
        {
            _receiver.Listen(
                (receiver, notification) => 
                {
                    try
                    {
                        messageHandler?.Invoke(this, DroneState.FromDatagram(notification.Data));
                    }
                    catch(Exception ex)
                    {
                        errorHandler?.Invoke(this, ex);
                    }
                },
                (receiver, ex) => errorHandler?.Invoke(this, ex));
        }

        public void Stop() => _receiver.Stop();
    }
}
