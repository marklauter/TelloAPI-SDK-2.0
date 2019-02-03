namespace Tello.Messaging
{
    public interface IRelayFactory<T>
    {
        IRelay<T> CreateReceiver(params object[] args);
    }
}
