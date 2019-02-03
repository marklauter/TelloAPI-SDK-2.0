using System;

namespace Tello.Messaging
{
    public enum ReceiverStates
    {
        Stopped,
        Listening
    }

    public interface IReceiver<T>
    {
        ReceiverStates State { get; }

        void Listen(Action<IReceiver<T>, T> messageHandler, Action<IReceiver<T>, Exception> errorHandler);
        void Stop();
    }
}
