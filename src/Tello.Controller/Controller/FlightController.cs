using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tello.Udp;

namespace Tello.Controller
{
    public sealed class FlightController
    {
        public FlightController(TimeSpan commandTimeout, string ip = "192.168.10.1", int commandPort = 8889, int statePort = 8890, int videoPort = 11111)
        {
            _transceiver = new Transceiver(ip, commandPort, commandTimeout);
            _transceiver.Connected += _transceiver_Connected;

            _stateListener = new StateListener(statePort);
            _stateListener.DroneStateReceived += _stateListener_DroneStateReceived;
        }

        #region events
        public event EventHandler<FlightControllerValueReceivedArgs> FlightControllerValueReceived;
        public event EventHandler<FlightControllerResponseReceivedArgs> FlightControllerResponseReceived;
        public event EventHandler<FlightControllerExceptionThrownArgs> FlightControllerExceptionThrown;
        public event EventHandler<DroneStateReceivedArgs> DroneStateReceived;
        #endregion

        #region drone state
        private readonly StateListener _stateListener;

        private void _stateListener_DroneStateReceived(object sender, DroneStateReceivedArgs e)
        {
            DroneStateReceived?.Invoke(this, e);
        }
        #endregion

        #region command queue

        private readonly ConcurrentQueue<Tuple<Commands, string>> _messageQueue = new ConcurrentQueue<Tuple<Commands, string>>();

        private bool IsResponseOk(Commands command, string responseMessage)
        {
            if (String.IsNullOrEmpty(responseMessage))
            {
                return false;
            }

            switch (command)
            {
                // ok / error commands
                case Commands.EstablishLink:
                case Commands.Takeoff:
                case Commands.Land:
                case Commands.Stop:
                case Commands.StartVideo:
                case Commands.StopVideo:
                case Commands.EmergencyStop:
                case Commands.Up:
                case Commands.Down:
                case Commands.Left:
                case Commands.Right:
                case Commands.Forward:
                case Commands.Back:
                case Commands.ClockwiseTurn:
                case Commands.CounterClockwiseTurn:
                case Commands.Flip:
                case Commands.Go:
                case Commands.Curve:
                case Commands.SetSpeed:
                case Commands.SetRemoteControl:
                case Commands.SetWiFiPassword:
                case Commands.SetStationMode:
                    return responseMessage.ToLowerInvariant() == "ok";

                case Commands.GetSpeed:
                case Commands.GetBattery:
                case Commands.GetTime:
                case Commands.GetWiFiSnr:
                case Commands.GetSdkVersion:
                case Commands.GetSerialNumber:
                    return responseMessage.ToLowerInvariant() != "error";

                default:
                    return false;
            }
        }

        private bool IsValueCommand(Commands command)
        {
            switch (command)
            {
                case Commands.GetSpeed:
                case Commands.GetBattery:
                case Commands.GetTime:
                case Commands.GetWiFiSnr:
                case Commands.GetSdkVersion:
                case Commands.GetSerialNumber:
                    return true;
                default:
                    return false;
            }
        }

        private async Task SendMessage(Tuple<Commands, string> commandTuple)
        {
            if (commandTuple == null)
            {
                throw new ArgumentNullException(nameof(commandTuple));
            }

            var command = commandTuple.Item1;
            var message = commandTuple.Item2;

            Transceiver.Response response = null;
            try
            {
                Debug.WriteLine($"{DateTime.Now.ToString("o")} - sending '{message}'");
                response = await _transceiver.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new FlightControllerException($"'{message}' command failed with message '{ex.Message}'", ex);
            }

            if (response.IsSuccess && IsResponseOk(command, response.Value))
            {
                Debug.WriteLine($"{DateTime.Now.ToString("o")} - response received for '{message}' - '{response.Value}'");

                FlightControllerResponseReceived?.Invoke(this, new FlightControllerResponseReceivedArgs(response.Value));
                if (IsValueCommand(command))
                {
                    FlightControllerValueReceived?.Invoke(this, new FlightControllerValueReceivedArgs(command.ToValueReponseType(), response.Value));
                }
            }
            else
            {
                var err = response.IsSuccess
                    ? response.Value
                    : response.ErrorMessage;

                Debug.WriteLine($"{DateTime.Now.ToString("o")} - '{message}' command failed with message - '{err}'");

                throw new FlightControllerException(
                    $"'{message}' command failed with message '{err}'",
                    response.Exception);
            }
        }

        private async void ProcessMessageQueue()
        {
            await Task.Run(async () =>
            {
                var spinWait = new SpinWait();
                while (_connectionState != ConnectionStates.Disconnected)
                {
                    if (!_messageQueue.IsEmpty)
                    {
                        if (_messageQueue.TryDequeue(out var tuple))
                        {
                            try
                            {
                                await SendMessage(tuple);
                            }
                            catch (FlightControllerException ex)
                            {
                                FlightControllerExceptionThrown?.Invoke(this, new FlightControllerExceptionThrownArgs(ex));
                            }
                        }
                    }
                    else
                    {
                        spinWait.SpinOnce();
                    }
                }
            });
        }

        private void EnqueueCommand(Commands command, params object[] args)
        {
            if (_connectionState != ConnectionStates.Disconnected)
            {
                try
                {
                    _messageQueue.Enqueue(new Tuple<Commands, string>(command, command.ToMessage(args)));
                }
                catch (Exception ex)
                {
                    throw new FlightControllerException($"Failed to enqueue command '{command}'", ex);
                }
            }
        }
        #endregion

        #region transceiver
        private readonly Transceiver _transceiver;

        private enum ConnectionStates
        {
            Connecting,
            Connected,
            Disconnected
        }
        private ConnectionStates _connectionState = ConnectionStates.Disconnected;

        private void _transceiver_Connected(object sender, EventArgs e)
        {
            _connectionState = ConnectionStates.Connected;
            ProcessMessageQueue();
        }
        #endregion

        #region command messages
        public void EstablishLink()
        {
            _connectionState = ConnectionStates.Connecting;
            _transceiver.Connect();

            EnqueueCommand(Commands.EstablishLink);
        }

        public void GetSpeed()
        {
            EnqueueCommand(Commands.GetSpeed);
        }
        #endregion 
    }
}
