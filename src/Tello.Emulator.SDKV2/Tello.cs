using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Tello.Messaging;


namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    public class Tello : IMessenger, IReceiver<INotification>
    {
        public Tello()
        {
            _stateServer = new StateServer(_droneState);
            _stateController = new StateController((DroneState)_droneState);
            _commandInterpreter = new CommandInterpreter(_stateController);
        }

        private readonly IDroneState _droneState = new DroneState();
        private readonly StateServer _stateServer;
        private readonly StateController _stateController;
        private readonly CommandInterpreter _commandInterpreter;


        #region IMessenger
        private MessengerStates _messengerState = MessengerStates.Disconnected;
        MessengerStates IMessenger.State => _messengerState;

        void IMessenger.Connect()
        {
            if (_messengerState == MessengerStates.Disconnected)
            {
                _messengerState = MessengerStates.Connected;
            }
        }

        void IMessenger.Disconnect()
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

        #region IReceiver<INotification>
        ReceiverStates IReceiver<INotification>.State => _stateServer.State;

        void IReceiver<INotification>.Listen(Action<IReceiver<INotification>, INotification> messageHandler, Action<IReceiver<INotification>, Exception> errorHandler)
        {
            _stateServer.Listen(messageHandler, errorHandler);
        }

        void IReceiver<INotification>.Stop()
        {
            _stateServer.Stop();
        }
        #endregion
    }
}
