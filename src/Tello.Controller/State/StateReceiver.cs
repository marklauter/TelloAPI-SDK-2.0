using System;
using Tello.Messaging;

namespace Tello.Controller.State
{
    // this will couple with either the Tello.UDP receiver, or with the Tello.Emulator StateServer
    internal sealed class StateReceiver : IRelay<DroneState>
    {
        public StateReceiver(IRelay<INotification> receiver)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        private readonly IRelay<INotification> _receiver;

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IRelay<DroneState>, DroneState> messageHandler, Action<IRelay<DroneState>, Exception> errorHandler)
        {
            _receiver.Listen(
                (receiver, notification) => messageHandler?.Invoke(this, DroneState.FromDatagram(notification.Data)),
                (receiver, ex) => errorHandler?.Invoke(this, ex));
        }

        public void Stop()
        {
            _receiver.Stop();
        }
    }
}
