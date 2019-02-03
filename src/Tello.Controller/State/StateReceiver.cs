using System;
using Tello.Messaging;

namespace Tello.Controller.State
{
    public sealed class StateReceiver : IReceiver<DroneState>
    {
        public StateReceiver(IReceiver<INotification> receiver)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        private readonly IReceiver<INotification> _receiver;

        public ReceiverStates State => _receiver.State;

        public void Stop()
        {
            _receiver.Stop();
        }

        public void Listen(Action<IReceiver<DroneState>, DroneState> messageHandler, Action<IReceiver<DroneState>, Exception> errorHandler)
        {
            _receiver.Listen(
                (receiver, notification) => messageHandler?.Invoke(this, DroneState.FromDatagram(notification.Data)),
                (receiver, ex) => errorHandler?.Invoke(this, ex));
        }
    }
}
