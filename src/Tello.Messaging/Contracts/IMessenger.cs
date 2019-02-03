using System;
using System.Threading.Tasks;

namespace Tello.Messaging
{
    public enum MessengerStates
    {
        Disconnected,
        Connecting,
        Connected,
        Error
    }

    public interface IMessenger
    {
        MessengerStates State { get; }
        void ClearError();

        void Connect(TimeSpan timeout, Action<IMessenger> continuation);
        void Disconnect(Action<IMessenger> continuation);

        Task<IResponse> SendAsync(IRequest request);
    }
}
