using System.Text;
using System.Threading.Tasks;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateServer : UdpServer
    {
        public StateServer(int port, DroneState droneState) : base(port)
        {
            _droneState = droneState ?? throw new System.ArgumentNullException(nameof(droneState));
        }

        private readonly DroneState _droneState;

        protected async override Task<byte[]> GetDatagram()
        {
            // 5Hz state reporting
            await Task.Delay(200);
            return Encoding.UTF8.GetBytes(_droneState.ToString());
        }
    }
}
