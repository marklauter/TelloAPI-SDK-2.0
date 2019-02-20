using System;

namespace Tello.Messaging
{
    public enum ReceiverStates
    {
        Stopped,
        Listening
    }

    /// <summary>
    /// Listens for messages, signals, or state changes and relays them to subscribers via the MessageReceived event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageRelayService<T>
    {
        /// <summary>
        /// Listening or Stopped. Default state can be set at the implementer's discretion, but if Listening, then Start must be called during construction process.
        /// </summary>
        ReceiverStates State { get; }

        /// <summary>
        /// Subscribe to this event to receive message notifications. For examlpe, an incoming Tello state over UDP.
        /// </summary>
        event EventHandler<MessageReceivedArgs<T>> MessageReceived;

        /// <summary>
        /// Suscribe to this event to receive exception notifications.
        /// </summary>
        event EventHandler<MessageRelayExceptionThrownArgs> MessageRelayExceptionThrown;

        /// <summary>
        /// Starts the listening service.
        /// Events will not be fired until Start has been called.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the listening service.
        /// No more events will be fired after Stop has been called.
        /// </summary>
        void Stop();
    }
}
