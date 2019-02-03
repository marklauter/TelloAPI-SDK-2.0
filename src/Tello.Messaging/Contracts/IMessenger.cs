using System;
using System.Threading.Tasks;

namespace Tello.Controller.Contracts
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

        void Connect(TimeSpan timeout, Action<IMessenger> continuation);
        void Disconnect(Action<IMessenger> continuation);

        Task<IResponse> SendAsync(IRequest request);
    }
}
