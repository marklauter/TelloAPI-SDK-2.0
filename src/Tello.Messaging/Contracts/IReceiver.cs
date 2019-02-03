using System;

namespace Tello.Messaging
{
    public interface INotification
    {
        byte[] Data { get; }

        /// <summary>
        /// set reply inside the received event to send a message back to tello
        /// </summary>
        byte[] Reply { get; set; }
    }

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
