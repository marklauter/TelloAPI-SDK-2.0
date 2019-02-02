using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tello.Controller.Messages;
using Tello.Controller.State;
using Tello.Controller.Video;
using Tello.Udp;

namespace Tello.Controller
{
    public sealed class FlightController
    {
        public FlightController(TimeSpan commandTimeout, string ip = "192.168.10.1", int commandPort = 8889, int statePort = 8890, int videoPort = 11111)
        {
            _transceiver = new Transceiver(ip, commandPort, commandTimeout);
            _transceiver.Connected += _transceiver_Connected;

            _stateServer = new StateServer(statePort);
            _stateServer.DroneStateReceived += _stateServer_DroneStateReceived;

            _frameServer = new FrameServer(32, TimeSpan.FromSeconds(30), videoPort);
            _frameServer.FrameReady += _frameServer_FrameReady;
        }

        #region events
        public event EventHandler<FrameReadyArgs> FrameReady;
        public event EventHandler<DroneStateReceivedArgs> DroneStateReceived;
        public event EventHandler<FlightControllerValueReceivedArgs> FlightControllerValueReceived;
        public event EventHandler<FlightControllerExceptionThrownArgs> FlightControllerExceptionThrown;
        public event EventHandler<FlightControllerResponseReceivedArgs> FlightControllerResponseReceived;
        #endregion

        #region video
        private readonly FrameServer _frameServer;

        private void _frameServer_FrameReady(object sender, FrameReadyArgs e)
        {
            FrameReady?.Invoke(this, e);
        }
        #endregion

        #region drone state
        private readonly StateServer _stateServer;

        private void _stateServer_DroneStateReceived(object sender, DroneStateReceivedArgs e)
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
                    FlightControllerValueReceived?.Invoke(this, new FlightControllerValueReceivedArgs(command.ToReponse(), response.Value));
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

        private Tuple<Commands, string> CreateCommand(Commands command, params object[] args)
        {
            return new Tuple<Commands, string>(command, command.ToMessage(args));
        }

        private void EnqueueCommand(Commands command, params object[] args)
        {
            if (_connectionState != ConnectionStates.Disconnected)
            {
                try
                {
                    _messageQueue.Enqueue(CreateCommand(command, args));
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

        /// <summary>
        /// establish a command link with Tello - validates network connection, connects to UDP, sends "command" to Tello
        /// </summary>
        public void EstablishCommandLink()
        {
            _connectionState = ConnectionStates.Connecting;
            _transceiver.Connect();

            EnqueueCommand(Commands.EstablishLink);
        }

        /// <summary>
        /// auto take off to 20cm
        /// </summary>
        public void TakeOff()
        {
            EnqueueCommand(Commands.Takeoff);
        }

        /// <summary>
        /// auto land
        /// </summary>
        public void Land()
        {
            EnqueueCommand(Commands.Land);
        }

        /// <summary>
        /// stops moving and hovers
        /// </summary>
        public async void Stop()
        {
            // stop and emergency can be sent immediately - no need to wait
            await SendMessage(CreateCommand(Commands.Land));
        }

        /// <summary>
        /// stops motors immediately
        /// </summary>
        public async void EmergencyStop()
        {
            // stop and emergency can be sent immediately - no need to wait
            await SendMessage(CreateCommand(Commands.EmergencyStop));
        }

        /// <summary>
        /// begins H264 encoded video stream to port 11111, subscribe to FrameReady events to take advantage of video
        /// </summary>
        public async void StartVideo()
        {
            await SendMessage(CreateCommand(Commands.StartVideo));
        }

        /// <summary>
        /// stops video stream
        /// </summary>
        public async void StopVideo()
        {
            await SendMessage(CreateCommand(Commands.StopVideo));
        }

        public void GoUp(int cm)
        {
            EnqueueCommand(Commands.Up, cm);
        }

        public void GoDown(int cm)
        {
            EnqueueCommand(Commands.Down, cm);
        }

        public void GoLeft(int cm)
        {
            EnqueueCommand(Commands.Left, cm);
        }

        public void GoRight(int cm)
        {
            EnqueueCommand(Commands.Right, cm);
        }

        public void GoForward(int cm)
        {
            EnqueueCommand(Commands.Forward, cm);
        }

        public void GoBackward(int cm)
        {
            EnqueueCommand(Commands.Back, cm);
        }

        public void TurnClockwise(int degress)
        {
            EnqueueCommand(Commands.ClockwiseTurn, degress);
        }

        public void TurnRight(int degress)
        {
            TurnClockwise(degress);
        }

        public void TurnCounterClockwise(int degress)
        {
            EnqueueCommand(Commands.CounterClockwiseTurn, degress);
        }

        public void TurnLeft(int degress)
        {
            TurnCounterClockwise(degress);
        }

        public void Flip(FlipDirections direction)
        {
            EnqueueCommand(Commands.Flip, direction.ToChar());
        }

        /// <summary>
        /// x, y and z values cannot be set between -20 and 20 simultaneously
        /// </summary>
        /// <param name="x">-500 to 500</param>
        /// <param name="y">-500 to 500</param>
        /// <param name="z">-500 to 500</param>
        /// <param name="speed">cm/s, 10 to 100</param>
        public void Go(int x, int y, int z, int speed)
        {
            EnqueueCommand(Commands.Go, x, y, z, speed);
        }

        /// <summary>
        /// x, y and z values cannot be set between -20 and 20 simultaneously
        /// </summary>
        /// <param name="x1">-500 to 500</param>
        /// <param name="y1">-500 to 500</param>
        /// <param name="z1">-500 to 500</param>
        /// <param name="x2">-500 to 500</param>
        /// <param name="y2">-500 to 500</param>
        /// <param name="z2">-500 to 500</param>
        /// <param name="speed">cm/s, 10 to 60</param>
        public void Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed)
        {
            EnqueueCommand(Commands.Curve, x1, y1, z1, x2, y2, z2, speed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed">cm/s, 10 to 100</param>
        public void SetSpeed(int speed)
        {
            EnqueueCommand(Commands.SetSpeed, speed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftRight">-100 to 100</param>
        /// <param name="forwardBackward">-100 to 100</param>
        /// <param name="upDown">-100 to 100</param>
        /// <param name="yaw">-100 to 100</param>
        public void Set4ChannelRC(int leftRight, int forwardBackward, int upDown, int yaw)
        {
            EnqueueCommand(Commands.SetRemoteControl, leftRight, forwardBackward, upDown, yaw);
        }

        public void SetWIFIPassword(string ssid, string password)
        {
            EnqueueCommand(Commands.SetWiFiPassword, ssid, password);
        }

        public void SetStationMode(string ssid, string password)
        {
            EnqueueCommand(Commands.SetStationMode, ssid, password);
        }

        /// <summary>
        /// get speed in cm/s, 10 to 100
        /// </summary>
        public void GetSpeed()
        {
            EnqueueCommand(Commands.GetSpeed);
        }

        /// <summary>
        /// get battery percentage remaining, 0% to 100%
        /// </summary>
        public void GetBattery()
        {
            EnqueueCommand(Commands.GetBattery);
        }

        public void GetTime()
        {
            EnqueueCommand(Commands.GetTime);
        }

        public void GetWIFISNR()
        {
            EnqueueCommand(Commands.GetWiFiSnr);
        }

        public void GetSdkVersion()
        {
            EnqueueCommand(Commands.GetSdkVersion);
        }

        public void GetWiFiSerialNumber()
        {
            EnqueueCommand(Commands.GetSerialNumber);
        }
        #endregion 
    }
}
