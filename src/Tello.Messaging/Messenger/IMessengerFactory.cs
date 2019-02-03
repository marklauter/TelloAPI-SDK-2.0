namespace Tello.Messaging
{
    public interface IMessengerFactory
    {
        IMessenger CreateMessenger(params object[] args);
    }
}
