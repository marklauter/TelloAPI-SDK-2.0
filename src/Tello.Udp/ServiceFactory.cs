using System;
using Tello.Messaging;
using Tello.Messaging.Contracts;

namespace Tello.Udp
{
    public sealed class ServiceFactory : IReceiverFactory<INotification>, IMessengerFactory
    {
        public IMessenger CreateMessenger(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!(args[0] is string))
            {
                throw new ArgumentException($"{nameof(args)}[0] should be type of string.");
            }

            if (!(args[1] is int))
            {
                throw new ArgumentException($"{nameof(args)}[1] should be type of int.");
            }

            return new Messenger((string)args[0], (int)args[1]);
        }

        public IReceiver<INotification> CreateReceiver(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!(args[0] is int))
            {
                throw new ArgumentException($"{nameof(args)}[0] should be type of int.");
            }

            return new Receiver((int)args[0]);
        }
    }
}
