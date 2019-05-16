using Messenger.Simulator;
using System.Text;
using System.Threading.Tasks;

namespace Tello.Simulator.Messaging
{
    internal delegate string CommandReceivedHandler(Command command);

    internal sealed class DroneMessageHandler : IDroneMessageHandler
    {
        public event CommandReceivedHandler CommandReceived;

        public Task<byte[]> Invoke(byte[] buffer)
        {
            if (buffer != null)
            {
                return Task.FromResult(Encoding.UTF8.GetBytes(CommandReceived?.Invoke((Command)buffer)));
            }
            else
            {
                return Task.FromResult(Encoding.UTF8.GetBytes("error"));
            }
        }
    }
}


