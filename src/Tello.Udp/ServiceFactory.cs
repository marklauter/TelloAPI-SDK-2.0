using System;
using Tello.Messaging;

namespace Tello.Udp
{
    public sealed class ServiceFactory : IRelayFactory<INotification>, IMessengerFactory
    {
        public IMessenger CreateMessenger(params object[] args)
        {
            if (args == null || args.Length == 0)
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

            return new MessengerService((string)args[0], (int)args[1]);
        }

        public IRelay<INotification> CreateReceiver(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!(args[0] is int))
            {
                throw new ArgumentException($"{nameof(args)}[0] should be type of int.");
            }

            return new ReceiverService((int)args[0]);
        }
    }
}
