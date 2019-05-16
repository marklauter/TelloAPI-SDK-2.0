using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public interface IDroneTransmitter
    {
        int Available { get; }
        Task<IReceiveResult> ReceiveAsync();
    }
}
