using System;

namespace Tello.Controller.Contracts
{
    //public sealed class Message
    //{
    //    public Message(byte[] data)
    //    {
    //        Data = data;
    //    }
    //    public byte[] Data { get; }

    //    /// <summary>
    //    /// set reply inside the received event to send a message back to tello
    //    /// </summary>
    //    public byte[] Reply { get; set; }
    //}

    public interface IMessage
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
        Listening,
        Error
    }

    public interface IReceiver
    {
        ReceiverStates State { get; }

        void Listen(Action<IReceiver, IMessage> messageHandler);
        void Stop(Action<IMessenger> continuation);
    }
}
