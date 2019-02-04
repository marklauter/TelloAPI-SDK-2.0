using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Controller
{
    public class FlightController
    {
        // TimeSpan commandTimeout, string ip = "192.168.10.1", int commandPort = 8889, int statePort = 8890, int videoPort = 11111

        public FlightController(IMessengerService messenger, IRelayService<IDroneState> stateServer, IRelayService<IVideoSample> videoServer)
        {
            _messenger = messenger;
            _stateServer = stateServer;
            _videoServer = videoServer;
        }

        #region events
        public event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
        public event EventHandler<DroneStateReceivedArgs> DroneStateReceived;
        public event EventHandler<FlightControllerValueReceivedArgs> FlightControllerValueReceived;
        public event EventHandler<FlightControllerExceptionThrownArgs> FlightControllerExceptionThrown;
        public event EventHandler<FlightControllerResponseReceivedArgs> FlightControllerResponseReceived;
        public event EventHandler<FlightControllerCommandExceptionThrownArgs> FlightControllerCommandExceptionThrown;        
        #endregion

        #region messenger
        private readonly IMessengerService _messenger;
        #endregion

        #region video
        private readonly IRelayService<IVideoSample> _videoServer;

        private void VideoReceiverNotificationHandler(IRelayService<IVideoSample> receiver, IVideoSample sample)
        {
            VideoSampleReady?.Invoke(this, new VideoSampleReadyArgs(sample));
        }

        private void VideoReceiverErrorHandler(IRelayService<IVideoSample> receiver, Exception ex)
        {
            FlightControllerExceptionThrown?.Invoke(this,
                new FlightControllerExceptionThrownArgs(
                    new FlightControllerException($"VideoReceiver reported an exception of type '{ex.GetType()}' with message '{ex.Message}'", ex)));

        }
        #endregion

        #region flight controller and drone state

        public int ReportedSpeed { get; private set; }
        public int ReportedBattery { get; private set; }
        public int ReportedTime { get; private set; }
        public string ReportedWiFiSnr { get; private set; }
        public string ReportedSdkVersion { get; private set; }
        public string ReportedSerialNumber { get; private set; }

        public enum ConnectionStates
        {
            Connecting,
            Connected,
            CommandLinkEstablished,
            Disconnected
        }
        public ConnectionStates ConnectionState = ConnectionStates.Disconnected;

        public enum FlightStates
        {
            StandingBy,
            Takingoff,
            InFlight,
            Landing,
            EmergencyStop
        }
        public FlightStates FlightState { get; private set; } = FlightStates.StandingBy;

        public enum VideoStates
        {
            Stopped,
            Connecting,
            Streaming,
        }
        public VideoStates VideoState { get; private set; } = VideoStates.Stopped;

        private readonly IRelayService<IDroneState> _stateServer;

        private void StateReceiverNotificationHandler(IRelayService<IDroneState> receiver, IDroneState state)
        {
            DroneStateReceived?.Invoke(this, new DroneStateReceivedArgs(state));
        }

        private void StateReceiverErrorHandler(IRelayService<IDroneState> receiver, Exception ex)
        {
            FlightControllerExceptionThrown?.Invoke(this, 
                new FlightControllerExceptionThrownArgs(
                    new FlightControllerException($"StateReceiver reported an exception of type '{ex.GetType()}' with message '{ex.Message}'", ex)));
        }
        #endregion

        #region command queue

        private readonly ConcurrentQueue<Tuple<Commands, string, TimeSpan>> _messageQueue = new ConcurrentQueue<Tuple<Commands, string, TimeSpan>>();

        private bool IsResponseOk(Commands command, string responseMessage)
        {
            if (String.IsNullOrEmpty(responseMessage))
            {
                return false;
            }

            switch (command)
            {
                // ok / error commands
                case Commands.EnterSdkMode:
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

        private void HandleResponse(Commands command, string responseValue)
        {
            try
            {
                switch (command)
                {
                    case Commands.EnterSdkMode:
                        ConnectionState = ConnectionStates.CommandLinkEstablished;
                        ProcessMessageQueue();
                        RunKeepAlive();
                        break;
                    case Commands.Takeoff:
                        FlightState = FlightStates.InFlight;
                        break;
                    case Commands.EmergencyStop:
                    case Commands.Land:
                        FlightState = FlightStates.StandingBy;
                        break;
                    case Commands.Stop:
                        // hovers in air - no state change
                        break;
                    case Commands.StartVideo:
                        VideoState = VideoStates.Streaming;
                        break;
                    case Commands.StopVideo:
                        VideoState = VideoStates.Stopped;
                        break;
                    case Commands.GetSpeed:
                        ReportedSpeed = Int32.Parse(responseValue);
                        break;
                    case Commands.GetBattery:
                        ReportedBattery = Int32.Parse(responseValue);
                        break;
                    case Commands.GetTime:
                        ReportedTime = Int32.Parse(responseValue);
                        break;
                    case Commands.GetWiFiSnr:
                        ReportedWiFiSnr = responseValue;
                        break;
                    case Commands.GetSdkVersion:
                        ReportedSdkVersion = responseValue;
                        break;
                    case Commands.GetSerialNumber:
                        ReportedSerialNumber = responseValue;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new FlightControllerException($"Handle response failed with message '{ex.Message}'", ex);
            }
        }

        /// <summary>
        /// don't use this directly. call TrySendMessage instead.
        /// sends a message to Tello and awaits a response, checks the response, throws on bad responses, which are caught by queue processor and raised through the FlightControllerExceptionThrown event
        /// </summary>
        /// <param name="commandTuple"></param>
        /// <returns></returns>
        private async Task SendMessage(Tuple<Commands, string, TimeSpan> commandTuple)
        {
            if (commandTuple == null)
            {
                throw new ArgumentNullException(nameof(commandTuple));
            }

            var command = commandTuple.Item1;
            var message = commandTuple.Item2 ?? throw new ArgumentNullException(nameof(commandTuple.Item2));
            var timeout = commandTuple.Item3;

            var response = default(IResponse);
            var clock = Stopwatch.StartNew();
            try
            {
                #region debug output
#if DEBUG
                Debug.WriteLine($"{DateTime.Now.ToString("o")} - sending '{message}'");
#endif
                #endregion
                //todo: get timeouts for various actions based on command and params
                var request = Request.FromData(Encoding.UTF8.GetBytes(message), timeout);
                response = await _messenger.SendAsync(request);
                clock.Stop();
            }
            catch (Exception ex)
            {
                throw new FlightControllerException($"'{message}' command failed with message '{ex.Message}'", ex);
            }

            if (response.Successful)
            {
                var responseValue = Encoding.UTF8.GetString(response.Data);

                #region debug output
#if DEBUG
                Debug.WriteLine($"{DateTime.Now.ToString("o")} - response received for '{message}' - '{responseValue}' - ok? {response.Successful}");
#endif
                #endregion

                if (IsResponseOk(command, responseValue))
                {
                    HandleResponse(command, responseValue);

                    if (IsValueCommand(command))
                    {
                        FlightControllerValueReceived?.Invoke(this, new FlightControllerValueReceivedArgs(command.ToReponse(), responseValue, clock.Elapsed));
                    }
                    else
                    {
                        FlightControllerResponseReceived?.Invoke(this, new FlightControllerResponseReceivedArgs(command, responseValue, clock.Elapsed));
                    }
                }
            }
            else // udp receiver pooped
            {
                #region debug output
#if DEBUG
                Debug.WriteLine($"{DateTime.Now.ToString("o")} - '{message}' command failed with message - '{response.ErrorMessage}'");
#endif
                #endregion

                throw new FlightControllerException(
                    $"'{message}' command failed with message '{response.ErrorMessage}'",
                    response.Exception);
            }
        }

        /// <summary>
        /// wraps SendMessage calls in safe try/catch with event handlers
        /// </summary>
        /// <param name="commandTuple"></param>
        /// <returns>true if no exceptions were encountered, else false</returns>
        private async Task<bool> TrySendMessage(Tuple<Commands, string, TimeSpan> commandTuple)
        {
            if (commandTuple == null)
            {
                throw new ArgumentNullException(nameof(commandTuple));
            }

            var command = commandTuple.Item1;

            var result = false;
            var clock = Stopwatch.StartNew();
            try
            {
                await SendMessage(commandTuple);
                result = true;
            }
            catch (FlightControllerException ex)
            {
                FlightControllerExceptionThrown?.Invoke(this, new FlightControllerCommandExceptionThrownArgs(command, ex, clock.Elapsed));
            }
            catch (Exception ex)
            {
                FlightControllerExceptionThrown?.Invoke(this, 
                    new FlightControllerCommandExceptionThrownArgs(
                        command, 
                        new FlightControllerException($"SendMessage failed with message '{ex.Message}'", ex),
                        clock.Elapsed));
            }
            return result;
        }

        /// <summary>
        /// process items in the queue such that Tello doesn't get overwhelmed with messages. Each message must be accepted, processed and ACK'd by Tello before the next command can be sent. This prevents the "no joystick" error message.
        /// </summary>
        private async void ProcessMessageQueue()
        {
            await Task.Run(async () =>
            {
                var spinWait = new SpinWait();
                while (ConnectionState != ConnectionStates.Disconnected)
                {
                    if (!_messageQueue.IsEmpty)
                    {
                        if (_messageQueue.TryDequeue(out var tuple))
                        {
                            await TrySendMessage(tuple);
                        }
                    }
                    else
                    {
                        spinWait.SpinOnce();
                    }
                }
            });
        }

        /// <summary>
        /// Tello auto lands after 15 seconds without commands as a safety mesasure, so we're going to send a keep alive message every 5 seconds
        /// </summary>
        private async void RunKeepAlive()
        {
            await Task.Run(async () =>
            {
                while (ConnectionState == ConnectionStates.CommandLinkEstablished)
                {
                    await Task.Delay(1000 * 10);
                    if (_messageQueue.IsEmpty)
                    {
                        EnqueueCommand(Commands.GetBattery);
                    }
                }
            });
        }

        /// <summary>
        /// creates a command tuple for processing by the message queue and the SendMessage method
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Tuple<Commands, string, TimeSpan> CreateCommand(Commands command, params object[] args)
        {
            return new Tuple<Commands, string, TimeSpan>(command, command.ToMessage(args), command.ToTimeout(args));
        }

        private void EnqueueCommand(Commands command, params object[] args)
        {
            if (ConnectionState != ConnectionStates.Disconnected)
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

        #region command methods

        /// <summary>
        /// establish a command link with Tello - validates network connection, connects to UDP, sends "command" to Tello
        /// </summary>
        public async void EnterSdkMode()
        {
            ConnectionState = ConnectionStates.Connecting;
            try
            {
                _stateServer.Listen(StateReceiverNotificationHandler, StateReceiverErrorHandler);
                _messenger.Connect();
                ConnectionState = ConnectionStates.Connected;
                if (!await TrySendMessage(CreateCommand(Commands.EnterSdkMode)))
                {
                    ConnectionState = ConnectionStates.Disconnected;
                }
            }
            catch (Exception ex)
            {
                _stateServer.Stop();
                ConnectionState = ConnectionStates.Disconnected;
                FlightControllerExceptionThrown?.Invoke(this, 
                    new FlightControllerExceptionThrownArgs(
                        new FlightControllerException($"Connection failed with message '{ex.Message}'", ex)));
            }
        }

        /// <summary>
        /// auto take off to 20cm
        /// </summary>
        public void TakeOff()
        {
            FlightState = FlightStates.Takingoff;
            EnqueueCommand(Commands.Takeoff);
        }

        /// <summary>
        /// auto land
        /// </summary>
        public void Land()
        {
            if (FlightState == FlightStates.InFlight || FlightState == FlightStates.Takingoff)
            {
                FlightState = FlightStates.Landing;
                EnqueueCommand(Commands.Land);
            }
        }

        /// <summary>
        /// stops moving and hovers
        /// </summary>
        public async void Stop()
        {
            // stop and emergency can be sent immediately - no need to wait
            await TrySendMessage(CreateCommand(Commands.Land));
        }

        /// <summary>
        /// stops motors immediately
        /// </summary>
        public async void EmergencyStop()
        {
            // stop and emergency can be sent immediately - no need to wait
            FlightState = FlightStates.EmergencyStop;
            await TrySendMessage(CreateCommand(Commands.EmergencyStop));
        }

        /// <summary>
        /// begins H264 encoded video stream to port 11111, subscribe to FrameReady events to take advantage of video
        /// </summary>
        public async void StartVideo()
        {
            if (VideoState == VideoStates.Stopped)
            {
                VideoState = VideoStates.Connecting;

                _videoServer.Listen(VideoReceiverNotificationHandler, VideoReceiverErrorHandler);

                if (!await TrySendMessage(CreateCommand(Commands.StartVideo)))
                {
                    _videoServer.Stop();
                    VideoState = VideoStates.Stopped;
                }
            }
        }

        /// <summary>
        /// stops video stream
        /// </summary>
        public async void StopVideo()
        {
            if (VideoState != VideoStates.Stopped)
            {
                _videoServer.Stop();
                await TrySendMessage(CreateCommand(Commands.StopVideo));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoUp(int cm)
        {
            EnqueueCommand(Commands.Up, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoDown(int cm)
        {
            EnqueueCommand(Commands.Down, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoLeft(int cm)
        {
            EnqueueCommand(Commands.Left, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoRight(int cm)
        {
            EnqueueCommand(Commands.Right, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoForward(int cm)
        {
            EnqueueCommand(Commands.Forward, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoBackward(int cm)
        {
            EnqueueCommand(Commands.Back, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees">1 to 360</param>
        public void TurnClockwise(int degress)
        {
            EnqueueCommand(Commands.ClockwiseTurn, degress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees">1 to 360</param>
        public void TurnRight(int degress)
        {
            TurnClockwise(degress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees">1 to 360</param>
        public void TurnCounterClockwise(int degress)
        {
            EnqueueCommand(Commands.CounterClockwiseTurn, degress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees">1 to 360</param>
        public void TurnLeft(int degress)
        {
            TurnCounterClockwise(degress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FlipDirections">FlipDirections.Left, FlipDirections.Right, FlipDirections.Front, FlipDirections.Back</param>
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

        public enum ClockDirections
        {
            Clockwise,
            CounterClockwise
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sides">3 to 15</param>
        /// <param name="length">length of each side. 20 to 500 in cm</param>
        /// <param name="speed">cm/s 10 to 100</param>
        public void FlyPolygon(int sides, int length, int speed, ClockDirections clockDirection)
        {
            if (sides < 3 || sides > 15)
            {
                throw new ArgumentOutOfRangeException($"{nameof(sides)} allowed values: 3 to 15");
            }

            if (length < 20 || length > 500)
            {
                throw new ArgumentOutOfRangeException($"{nameof(length)} allowed values: 20 to 500");
            }

            if (speed < 10 || speed > 100)
            {
                throw new ArgumentOutOfRangeException($"{nameof(speed)} allowed values: 10 to 100");
            }

            if (FlightState == FlightStates.StandingBy)
            {
                TakeOff();
            }

            SetSpeed(speed);

            var angle = (int)Math.Round(360.0 / sides);
            for (var i = 0; i < sides; ++i)
            {
                GoForward(length);
                switch (clockDirection)
                {
                    case ClockDirections.Clockwise:
                        TurnClockwise(angle);
                        break;
                    case ClockDirections.CounterClockwise:
                        TurnCounterClockwise(angle);
                        break;
                }
            }

            Land();
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
