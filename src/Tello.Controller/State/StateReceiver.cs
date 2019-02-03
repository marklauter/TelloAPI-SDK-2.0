using System;
using Tello.Messaging;

namespace Tello.Controller.State
{
    // this will couple with either the Tello.UDP receiver, or with the Tello.Emulator StateServer
    public sealed class StateReceiver : IReceiver<DroneState>
    {
        public StateReceiver(IReceiver<INotification> receiver)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        private readonly IReceiver<INotification> _receiver;

        public ReceiverStates State => _receiver.State;

        public void Listen(Action<IReceiver<DroneState>, DroneState> messageHandler, Action<IReceiver<DroneState>, Exception> errorHandler)
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
