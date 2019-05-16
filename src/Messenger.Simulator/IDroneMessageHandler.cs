using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public interface IDroneMessageHandler
    {
        Task<byte[]> Invoke(byte[] buffer);
    }
}
