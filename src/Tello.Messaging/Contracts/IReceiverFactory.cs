namespace Tello.Messaging.Contracts
{
    public interface IReceiverFactory<T>
    {
        IReceiver<T> CreateReceiver(params object[] args);
    }
}
