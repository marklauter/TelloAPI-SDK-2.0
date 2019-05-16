using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public interface IDroneSim
    {
        Task<byte[]> Invoke(byte[] buffer);
    }
}
