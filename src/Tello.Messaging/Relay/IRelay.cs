using System;

namespace Tello.Messaging
{
    public enum ReceiverStates
    {
        Stopped,
        Listening
    }

    public interface IRelay<T>
    {
        ReceiverStates State { get; }

        void Listen(Action<IRelay<T>, T> messageHandler, Action<IRelay<T>, Exception> errorHandler);
        void Stop();
    }
}
