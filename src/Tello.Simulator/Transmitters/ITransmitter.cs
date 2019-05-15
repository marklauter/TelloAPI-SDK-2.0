using System.Threading.Tasks;

namespace Tello.Simulator.Transmitters
{
    public interface ITransmitter
    {
        int Available { get; }
        Task<ReceiveResult> ReceiveAsync();
    }
}
