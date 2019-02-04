using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Tello.Messaging;


namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    public class TelloEmulator : IMessengerService
    {
        public TelloEmulator()
        {
            _droneState = new DroneState();
            _stateController = new StateManager((DroneState)_droneState);
            _commandInterpreter = new CommandInterpreter(_stateController);

            StateServer = new StateServer(_droneState);
            VideoServer = new VideoServer(30, 0);
        }

        public IRelayService<IDroneState> StateServer { get; }
        public IRelayService<IVideoSample> VideoServer { get; }

        private readonly IDroneState _droneState;
        private readonly StateManager _stateController;
        private readonly CommandInterpreter _commandInterpreter;

        #region IMessenger
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
            var response = await _commandInterpreter.InterpretAsync(message);
            return Response.FromData(request, Encoding.UTF8.GetBytes(response), clock.Elapsed);
        }
        #endregion
    }
}
