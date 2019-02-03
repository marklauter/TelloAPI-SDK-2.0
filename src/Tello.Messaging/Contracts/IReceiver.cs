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

    public interface IReceiver
    {
        ReceiverStates State { get; }

        void Listen(Action<IReceiver, INotification> messageHandler, Action<IReceiver, Exception> errorHandler);
        void Stop(Action<IMessenger> continuation);
    }
}
