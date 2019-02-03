namespace Tello.Messaging
{
    public interface IReceiverFactory<T>
    {
        IReceiver<T> CreateReceiver(params object[] args);
    }
}
