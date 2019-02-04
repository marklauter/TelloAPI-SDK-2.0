using System;

namespace Tello.Messaging
{
    public enum ReceiverStates
    {
        Stopped,
        Listening
    }

    public interface IRelayService<T>
    {
        ReceiverStates State { get; }

        void Listen(Action<IRelayService<T>, T> messageHandler, Action<IRelayService<T>, Exception> errorHandler);
        void Stop();
    }
}
