using System;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    public sealed class ServiceFactory : IReceiverFactory<INotification>, IMessengerFactory
    {
        private static Tello _tello = new Tello();

        public IMessenger CreateMessenger(params object[] args)
        {
            return _tello;
        }

        public IReceiver<INotification> CreateReceiver(params object[] args)
        {
            return _tello;
        }
    }
}
