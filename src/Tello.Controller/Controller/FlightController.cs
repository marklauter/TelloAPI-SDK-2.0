using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;
using Tello.Scripting;

namespace Tello.Controller
{
    public class FlightController : ITelloController, IStateChangedNotifier, IVideoSampleReadyNotifier, IVideoSampleProvider
    {
        // TimeSpan commandTimeout, string ip = "192.168.10.1", int commandPort = 8889, int statePort = 8890, int videoPort = 11111

        public FlightController(
            IMessengerService messenger,
            IMessageRelayService<IRawDroneState> stateServer,
            IMessageRelayService<IVideoSample> videoServer,
            IVideoSampleProvider videoSampleProvider)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _stateServer = stateServer ?? throw new ArgumentNullException(nameof(stateServer));
            _videoServer = videoServer ?? throw new ArgumentNullException(nameof(videoServer));
            _videoSampleProvider = videoSampleProvider ?? throw new ArgumentNullException(nameof(videoSampleProvider));
            _stateServer.MessageRelayExceptionThrown += StateServerRelayExceptionThrown;
            _stateServer.MessageReceived += StateServerRelayMessageReceived;

            _videoServer.MessageRelayExceptionThrown += _videoServer_RelayExceptionThrown;
            _videoServer.MessageReceived += _videoServer_RelayMessageReceived;
        }

        #region events
        public event EventHandler<VideoSampleReadyArgs> VideoSampleReady;
        public event EventHandler<StateChangedArgs> StateChanged;

        public event EventHandler<QueryResponseReceivedArgs> QueryResponseReceived;
        public event EventHandler<CommandResponseReceivedArgs> CommandResponseReceived;
        public event EventHandler<ExceptionThrownArgs> ExceptionThrown;
        public event EventHandler<CommandExceptionThrownArgs> CommandExceptionThrown;
        #endregion

        #region video
        public enum VideoStates
        {
            Stopped,
            Connecting,
            Streaming,
        }
        public VideoStates VideoState { get; private set; } = VideoStates.Stopped;

        private readonly IMessageRelayService<IVideoSample> _videoServer;
        private readonly IVideoSampleProvider _videoSampleProvider;

        private void _videoServer_RelayMessageReceived(object sender, MessageReceivedArgs<IVideoSample> e)
        {
            VideoSampleReady?.Invoke(this, new VideoSampleReadyArgs(e.Message));
        }

        private void _videoServer_RelayExceptionThrown(object sender, MessageRelayExceptionThrownArgs e)
        {
            ExceptionThrown?.Invoke(this,
                new ExceptionThrownArgs(
                    new TelloControllerException($"VideoReceiver reported an exception of type '{e.Exception.GetType()}' with message '{e.Exception.Message}'", e.Exception)));
        }

        public bool TryGetSample(out IVideoSample sample, TimeSpan timeout)
        {
            return _videoSampleProvider.TryGetSample(out sample, timeout);
        }

        public bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            return _videoSampleProvider.TryGetFrame(out frame, timeout);
        }
        #endregion

        #region flight controller and drone state
        private double _altitudeMslInCm = 243.84; // 8FT MSL @ KTPF (my local FBO)

        public void SetAltimeter(double altitudeMslInCm)
        {
            _altitudeMslInCm = altitudeMslInCm;
        }

        private bool _altZeroed = false;
        private void ZeroAltimeter(double baromerInCm)
        {
            if (_altZeroed)
            {
                return;
            }

            _altZeroed = true;

            Position.ZeroAltimeter(baromerInCm, _altitudeMslInCm);
        }

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
        public ConnectionStates ConnectionState { get; private set; } = ConnectionStates.Disconnected;

        public enum FlightStates
        {
            StandingBy,
            Takingoff,
            InFlight,
            Landing,
            EmergencyStop
        }
        public FlightStates FlightState { get; private set; } = FlightStates.StandingBy;

        private IRawDroneState _droneState = null;
        public ITelloState DroneState => TelloStateFactory.GetState();

        private readonly IMessageRelayService<IRawDroneState> _stateServer;

        private void StateServerRelayMessageReceived(object sender, MessageReceivedArgs<IRawDroneState> e)
        {
            _droneState = e.Message;
            ZeroAltimeter(_droneState.BarometerInCm);
            TelloStateFactory.Update(_droneState);            
            StateChanged?.Invoke(this, new StateChangedArgs(TelloStateFactory.GetState()));
        }

        private void StateServerRelayExceptionThrown(object sender, MessageRelayExceptionThrownArgs e)
        {
            ExceptionThrown?.Invoke(this,
                new ExceptionThrownArgs(
                    new TelloControllerException($"StateReceiver reported an exception of type '{e.Exception.GetType()}' with message '{e.Exception.Message}'", e.Exception)));
        }

        #endregion

        #region command queue

        private readonly IMessengerService _messenger;

        private readonly ConcurrentQueue<Tuple<TelloCommands, string, TimeSpan, object[]>> _messageQueue = new ConcurrentQueue<Tuple<TelloCommands, string, TimeSpan, object[]>>();

        internal bool IsResponseOk(TelloCommands command, string responseMessage)
        {
            if (String.IsNullOrEmpty(responseMessage))
            {
                return false;
            }

            switch (command)
            {
                // ok / error commands
                case TelloCommands.EnterSdkMode:
                case TelloCommands.Takeoff:
                case TelloCommands.Land:
                case TelloCommands.Stop:
                case TelloCommands.StartVideo:
                case TelloCommands.StopVideo:
                case TelloCommands.EmergencyStop:
                case TelloCommands.Up:
                case TelloCommands.Down:
                case TelloCommands.Left:
                case TelloCommands.Right:
                case TelloCommands.Forward:
                case TelloCommands.Back:
                case TelloCommands.ClockwiseTurn:
                case TelloCommands.CounterClockwiseTurn:
                case TelloCommands.Flip:
                case TelloCommands.Go:
                case TelloCommands.Curve:
                case TelloCommands.SetSpeed:
                case TelloCommands.SetRemoteControl:
                case TelloCommands.SetWiFiPassword:
                case TelloCommands.SetStationMode:
                    return responseMessage.ToLowerInvariant() == "ok";

                case TelloCommands.GetSpeed:
                case TelloCommands.GetBattery:
                case TelloCommands.GetTime:
                case TelloCommands.GetWIFISnr:
                case TelloCommands.GetSdkVersion:
                case TelloCommands.GetSerialNumber:
                    return responseMessage.ToLowerInvariant() != "error";

                default:
                    return false;
            }
        }

        private bool IsValueCommand(TelloCommands command)
        {
            switch (command)
            {
                case TelloCommands.GetSpeed:
                case TelloCommands.GetBattery:
                case TelloCommands.GetTime:
                case TelloCommands.GetWIFISnr:
                case TelloCommands.GetSdkVersion:
                case TelloCommands.GetSerialNumber:
                    return true;
                default:
                    return false;
            }
        }

        private void HandleResponse(TelloCommands command, string responseValue, object[] args)
        {
            try
            {
                switch (command)
                {
                    case TelloCommands.EnterSdkMode:
                        ConnectionState = ConnectionStates.CommandLinkEstablished;
                        ProcessMessageQueue();
                        RunKeepAlive();
                        break;
                    case TelloCommands.Takeoff:
                        FlightState = FlightStates.InFlight;
                        break;
                    case TelloCommands.EmergencyStop:
                    case TelloCommands.Land:
                        FlightState = FlightStates.StandingBy;
                        break;
                    case TelloCommands.Stop:
                        // hovers in air - no state change
                        break;
                    case TelloCommands.StartVideo:
                        VideoState = VideoStates.Streaming;
                        break;
                    case TelloCommands.StopVideo:
                        VideoState = VideoStates.Stopped;
                        break;
                    case TelloCommands.GetSpeed:
                        ReportedSpeed = Int32.Parse(responseValue);
                        break;
                    case TelloCommands.GetBattery:
                        ReportedBattery = Int32.Parse(responseValue);
                        break;
                    case TelloCommands.GetTime:
                        ReportedTime = Int32.Parse(responseValue);
                        break;
                    case TelloCommands.GetWIFISnr:
                        ReportedWiFiSnr = responseValue;
                        break;
                    case TelloCommands.GetSdkVersion:
                        ReportedSdkVersion = responseValue;
                        break;
                    case TelloCommands.GetSerialNumber:
                        ReportedSerialNumber = responseValue;
                        break;
                    case TelloCommands.Left:
                    case TelloCommands.Right:
                    case TelloCommands.Forward:
                    case TelloCommands.Back:
                        TelloStateFactory.Move(command, (int)args[0]);
                        break;
                    case TelloCommands.ClockwiseTurn:
                    case TelloCommands.CounterClockwiseTurn:
                        TelloStateFactory.Turn(command, (int)args[0]);
                        break;
                    case TelloCommands.Go:
                        TelloStateFactory.Go((int)args[0], (int)args[1]);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new TelloControllerException($"Handle response failed with message '{ex.Message}'", ex);
            }
        }

        /// <summary>
        /// don't use this directly. call TrySendMessage instead.
        /// sends a message to Tello and awaits a response, checks the response, throws on bad responses, which are caught by queue processor and raised through the FlightControllerExceptionThrown event
        /// </summary>
        /// <param name="commandTuple"></param>
        /// <returns></returns>
        private async Task SendMessage(Tuple<TelloCommands, string, TimeSpan, object[]> commandTuple)
        {
            if (commandTuple == null)
            {
                throw new ArgumentNullException(nameof(commandTuple));
            }

            var command = commandTuple.Item1;
            var message = commandTuple.Item2 ?? throw new ArgumentNullException(nameof(commandTuple.Item2));
            var timeout = commandTuple.Item3;
            var args = commandTuple.Item4;

            var response = default(IResponse);
            var clock = Stopwatch.StartNew();
            try
            {
                Log.WriteLine($"sending '{message}'");

                var request = Request.FromData(Encoding.UTF8.GetBytes(message), timeout);
                response = await _messenger.SendAsync(request);
                clock.Stop();
            }
            catch (Exception ex)
            {
                throw new TelloControllerException($"'{message}' command failed with message '{ex.Message}'", ex);
            }

            if (response.Successful)
            {
                var responseValue = Encoding.UTF8.GetString(response.Data);

                Log.WriteLine($"response received for '{message}' - '{responseValue}' - ok? {response.Successful}");

                if (IsResponseOk(command, responseValue))
                {
                    HandleResponse(command, responseValue, args);

                    if (IsValueCommand(command))
                    {
                        QueryResponseReceived?.Invoke(this, new QueryResponseReceivedArgs(command.ToReponse(), responseValue, clock.Elapsed));
                    }
                    else
                    {
                        CommandResponseReceived?.Invoke(this, new CommandResponseReceivedArgs(command, responseValue, clock.Elapsed));
                    }
                }
                else
                {
                    Log.WriteLine($"'{message}' command received unexpected response - '{responseValue}'");

                    throw new TelloControllerException($"'{message}' command received unexpected response '{responseValue}'");
                }
            }
            else // udp receiver pooped
            {
                Log.WriteLine($"'{message}' command failed with message - '{response.ErrorMessage}'");

                throw new TelloControllerException(
                    $"'{message}' command failed with message '{response.ErrorMessage}'",
                    response.Exception);
            }
        }

        /// <summary>
        /// wraps SendMessage calls in safe try/catch with event handlers
        /// </summary>
        /// <param name="commandTuple"></param>
        /// <returns>true if no exceptions were encountered, else false</returns>
        private async Task<bool> TrySendMessage(Tuple<TelloCommands, string, TimeSpan, object[]> commandTuple)
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
            catch (TelloControllerException ex)
            {
                CommandExceptionThrown?.Invoke(this, new CommandExceptionThrownArgs(command, ex, clock.Elapsed));
            }
            catch (Exception ex)
            {
                CommandExceptionThrown?.Invoke(this,
                    new CommandExceptionThrownArgs(
                        command,
                        new TelloControllerException($"SendMessage failed with message '{ex.Message}'", ex),
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
                        EnqueueCommand(TelloCommands.GetBattery);
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
        private Tuple<TelloCommands, string, TimeSpan, object[]> CreateCommand(TelloCommands command, params object[] args)
        {
            return new Tuple<TelloCommands, string, TimeSpan, object[]>(command, command.ToMessage(args), command.ToTimeout(args), args);
        }

        private void EnqueueCommand(TelloCommands command, params object[] args)
        {
            if (ConnectionState != ConnectionStates.Disconnected)
            {
                try
                {
                    _messageQueue.Enqueue(CreateCommand(command, args));
                }
                catch (Exception ex)
                {
                    throw new TelloControllerException($"Failed to enqueue command '{command}' because {ex.GetType()} - '{ex.Message}'", ex);
                }
            }
        }
        #endregion

        #region command methods

        public void ExecuteScript(TelloScript script)
        {
            var token = script.NextToken();
            while (token != null)
            {
                EnqueueCommand(token.Command, token.Args);
                token = script.NextToken();
            }
        }

        public void ExecuteScript(string json)
        {
            ExecuteScript(TelloScript.FromJson(json));
        }

        /// <summary>
        /// establish a command link with Tello - validates network connection, connects to UDP, sends "command" to Tello
        /// </summary>
        public async void Connect()
        {
            ConnectionState = ConnectionStates.Connecting;
            try
            {
                _stateServer.Start();
                _messenger.Connect();
                ConnectionState = ConnectionStates.Connected;
                if (!await TrySendMessage(CreateCommand(TelloCommands.EnterSdkMode)))
                {
                    ConnectionState = ConnectionStates.Disconnected;
                }
            }
            catch (Exception ex)
            {
                _messenger.Disconnect();
                _stateServer.Stop();
                ConnectionState = ConnectionStates.Disconnected;
                ExceptionThrown?.Invoke(this,
                    new ExceptionThrownArgs(
                        new TelloControllerException($"Connection failed with message '{ex.Message}'", ex)));
            }
        }

        public void Disconnect()
        {
            _messenger.Disconnect();
            _stateServer.Stop();
            ConnectionState = ConnectionStates.Disconnected;
        }

        /// <summary>
        /// auto take off to 20cm
        /// </summary>
        public void TakeOff()
        {
            if (FlightState == FlightStates.StandingBy || FlightState == FlightStates.EmergencyStop)
            {
                FlightState = FlightStates.Takingoff;
                EnqueueCommand(TelloCommands.Takeoff);
            }
        }

        /// <summary>
        /// auto land
        /// </summary>
        public void Land()
        {
            if (FlightState == FlightStates.InFlight || FlightState == FlightStates.Takingoff)
            {
                FlightState = FlightStates.Landing;
                EnqueueCommand(TelloCommands.Land);
            }
        }

        /// <summary>
        /// stops moving and hovers
        /// </summary>
        public async void Stop()
        {
            // stop and emergency can be sent immediately - no need to wait
            await TrySendMessage(CreateCommand(TelloCommands.Land));
        }

        /// <summary>
        /// stops motors immediately
        /// </summary>
        public async void EmergencyStop()
        {
            // stop and emergency can be sent immediately - no need to wait
            FlightState = FlightStates.EmergencyStop;
            await TrySendMessage(CreateCommand(TelloCommands.EmergencyStop));
        }

        /// <summary>
        /// begins H264 encoded video stream to port 11111, subscribe to FrameReady events to take advantage of video
        /// </summary>
        public async void StartVideo()
        {
            if (VideoState == VideoStates.Stopped)
            {
                VideoState = VideoStates.Connecting;

                _videoServer.Start();

                if (!await TrySendMessage(CreateCommand(TelloCommands.StartVideo)))
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
                await TrySendMessage(CreateCommand(TelloCommands.StopVideo));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoUp(int cm)
        {
            EnqueueCommand(TelloCommands.Up, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoDown(int cm)
        {
            EnqueueCommand(TelloCommands.Down, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoLeft(int cm)
        {
            EnqueueCommand(TelloCommands.Left, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoRight(int cm)
        {
            EnqueueCommand(TelloCommands.Right, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoForward(int cm)
        {
            EnqueueCommand(TelloCommands.Forward, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cm">20 to 500</param>
        public void GoBackward(int cm)
        {
            EnqueueCommand(TelloCommands.Back, cm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees">1 to 360</param>
        public void TurnClockwise(int degress)
        {
            EnqueueCommand(TelloCommands.ClockwiseTurn, degress);
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
            EnqueueCommand(TelloCommands.CounterClockwiseTurn, degress);
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
        public void Flip(CardinalDirections direction)
        {
            EnqueueCommand(TelloCommands.Flip, direction.ToChar());
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
            EnqueueCommand(TelloCommands.Go, x, y, z, speed);
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
            EnqueueCommand(TelloCommands.Curve, x1, y1, z1, x2, y2, z2, speed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sides">3 to 15</param>
        /// <param name="length">length of each side. 20 to 500 in cm</param>
        /// <param name="speed">cm/s 10 to 100</param>
        public void FlyPolygon(int sides, int length, int speed, ClockDirections clockDirection, bool land = false)
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

            var turnMethod = default(Action<int>);
            switch (clockDirection)
            {
                case ClockDirections.Clockwise:
                    turnMethod = TurnClockwise;
                    break;
                case ClockDirections.CounterClockwise:
                    turnMethod = TurnCounterClockwise;
                    break;
            }

            var angle = (int)Math.Round(360.0 / sides);
            for (var i = 0; i < sides; ++i)
            {
                GoForward(length);
                turnMethod(angle);
            }

            if (land)
            {
                Land();
            }
        }

        public void SetHeight(int cm)
        {
            if (_droneState != null)
            {
                var newHeight = cm - _droneState.HeightInCm;
                if (newHeight >= 20 && newHeight <= 500)
                {
                    if (newHeight < 0)
                    {
                        GoDown(Math.Abs(newHeight));
                    }
                    else
                    {
                        GoUp(newHeight);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed">cm/s, 10 to 100</param>
        public void SetSpeed(int speed)
        {
            EnqueueCommand(TelloCommands.SetSpeed, speed);
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
            EnqueueCommand(TelloCommands.SetRemoteControl, leftRight, forwardBackward, upDown, yaw);
        }

        public void SetWIFIPassword(string ssid, string password)
        {
            EnqueueCommand(TelloCommands.SetWiFiPassword, ssid, password);
        }

        public void SetStationMode(string ssid, string password)
        {
            EnqueueCommand(TelloCommands.SetStationMode, ssid, password);
        }

        /// <summary>
        /// get speed in cm/s, 10 to 100
        /// </summary>
        public void GetSpeed()
        {
            EnqueueCommand(TelloCommands.GetSpeed);
        }

        /// <summary>
        /// get battery percentage remaining, 0% to 100%
        /// </summary>
        public void GetBattery()
        {
            EnqueueCommand(TelloCommands.GetBattery);
        }

        public void GetTime()
        {
            EnqueueCommand(TelloCommands.GetTime);
        }

        public void GetWIFISNR()
        {
            EnqueueCommand(TelloCommands.GetWIFISnr);
        }

        public void GetSdkVersion()
        {
            EnqueueCommand(TelloCommands.GetSdkVersion);
        }

        public void GetWIFISerialNumber()
        {
            EnqueueCommand(TelloCommands.GetSerialNumber);
        }
        #endregion

    }
}
