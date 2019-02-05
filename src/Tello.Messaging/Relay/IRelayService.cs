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

        event EventHandler<RelayMessageReceivedArgs<T>> RelayMessageReceived;
        event EventHandler<RelayExceptionThrownArgs> RelayExceptionThrown;

        void Start();
        void Stop();
    }
}
