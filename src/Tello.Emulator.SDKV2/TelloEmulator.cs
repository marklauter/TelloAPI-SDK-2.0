using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    //todo: add debug output to every call in the system so the user (programmer) can see what's happening (use the Log static class)
    public class TelloEmulator : IMessengerService
    {
        public TelloEmulator()
        {
            _droneState = new DroneState();

            StateServer = new StateServer(_droneState);
            VideoServer = new VideoServer(30, 0);

            _stateManager = new StateManager((DroneState)_droneState, (VideoServer)VideoServer, (StateServer)StateServer);
            _commandInterpreter = new CommandInterpreter(_stateManager);
        }

        public IRelayService<IDroneState> StateServer { get; }
        public IRelayService<IVideoSample> VideoServer { get; }
        public Position Position => _stateManager.Position;

        private readonly IDroneState _droneState;
        private readonly StateManager _stateManager;
        private readonly CommandInterpreter _commandInterpreter;


        public void PowerOn()
        {
            _stateManager.PowerOn();
        }

        public void PowerOff()
        {
            _stateManager.PowerOff();
        }

        #region IMessengerService
        private MessengerStates _messengerState = MessengerStates.Disconnected;

        MessengerStates IMessengerService.State => _messengerState;

        void IMessengerService.Connect()
        {
            if (_messengerState == MessengerStates.Disconnected)
            {
                _messengerState = MessengerStates.Connected;
            }
        }

        void IMessengerService.Disconnect()
        {
            _messengerState = MessengerStates.Disconnected;
        }

        public async Task<IResponse> SendAsync(IRequest request)
        {
            var clock = Stopwatch.StartNew();
            var message = Encoding.UTF8.GetString(request.Data);

            // command interpreter is the heart of the Tello emulator
            var response = await _commandInterpreter.InterpretAsync(message);

            return Response.FromData(request, Encoding.UTF8.GetBytes(response), clock.Elapsed);
        }
        #endregion
    }
}
