using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public interface ITransmitter
    {
        int Available { get; }
        Task<IReceiveResult> ReceiveAsync();
    }
}
