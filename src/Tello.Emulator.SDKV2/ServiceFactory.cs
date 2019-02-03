using System;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    public sealed class ServiceFactory : IReceiverFactory<INotification>, IMessengerFactory
    {
        private static Tello _tello = new Tello();

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

            return _tello;
        }

        public IReceiver<INotification> CreateReceiver(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!(args[0] is int))
            {
                throw new ArgumentException($"{nameof(args)}[0] should be type of int.");
            }

            return _tello;
        }
    }
}
