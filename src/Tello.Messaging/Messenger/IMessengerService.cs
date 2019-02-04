using System.Threading.Tasks;

namespace Tello.Messaging
{
    public enum MessengerStates
    {
        Disconnected,
        Connecting,
        Connected
    }

    public interface IMessengerService 
    {
        MessengerStates State { get; }

        void Connect();
        void Disconnect();

        Task<IResponse> SendAsync(IRequest request);
    }
}
