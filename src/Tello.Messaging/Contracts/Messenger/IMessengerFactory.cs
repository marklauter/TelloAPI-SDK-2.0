namespace Tello.Messaging.Contracts
{
    public interface IMessengerFactory
    {
        IMessenger CreateMessenger(params object[] args);
    }
}
