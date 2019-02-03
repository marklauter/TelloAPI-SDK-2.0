using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    public sealed class ServiceFactory : IRelayFactory<INotification>, IMessengerFactory
    {
        private static readonly Tello _tello = new Tello();

        public IMessenger CreateMessenger(params object[] args)
        {
            return _tello;
        }

        public IRelay<INotification> CreateReceiver(params object[] args)
        {
            return _tello;
        }
    }
}
