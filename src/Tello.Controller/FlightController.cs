using Messenger;
using System;
using System.Threading.Tasks;
using Tello.Events;
using Tello.Messaging;
using Tello.State;

namespace Tello.Controller
{
    public sealed class FlightController : Observer<IResponse<string>>, IFlightController
    {
        private readonly TelloMessenger _messenger;

        public FlightController(ITransceiver transceiver) : base()
        {
            _messenger = new TelloMessenger(transceiver ?? throw new ArgumentNullException(nameof(transceiver)));
            Subscribe(_messenger);
        }

        #region Observer<IResponse<string>> - transceiver reponse handling
        public override void OnError(Exception error)
        {
            try
            {
                ExceptionThrown?.Invoke(this, new ExceptionThrownArgs(new TelloException("FlightController Error", error)));
            }
            catch { }
        }

        private void HandleOk(IResponse<string> response, Command command)
        {
            switch ((Commands)command)
            {
                case Commands.Takeoff:
                    _isFlying = true;
                    break;
                case Commands.Land:
                case Commands.EmergencyStop:
                    _isFlying = false;
                    break;
                case Commands.StartVideo:
                    _isVideoStreaming = true;
                    VideoStreamingStateChanged?.Invoke(this, new VideoStreamingStateChangedArgs(_isVideoStreaming));
                    break;
                case Commands.StopVideo:
                    _isVideoStreaming = false;
                    VideoStreamingStateChanged?.Invoke(this, new VideoStreamingStateChangedArgs(_isVideoStreaming));
                    break;

                case Commands.Left:
                    Position = Position.Move(CardinalDirections.Left, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.Right:
                    Position = Position.Move(CardinalDirections.Right, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.Forward:
                    Position = Position.Move(CardinalDirections.Front, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.Back:
                    Position = Position.Move(CardinalDirections.Back, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.ClockwiseTurn:
                    Position = Position.Turn(ClockDirections.Clockwise, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.CounterClockwiseTurn:
                    Position = Position.Turn(ClockDirections.CounterClockwise, (int)((Command)response.Request.Data).Arguments[0]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;
                case Commands.Go:
                    Position = Position.Go((int)((Command)response.Request.Data).Arguments[0], (int)((Command)response.Request.Data).Arguments[1]);
                    PositionChanged?.Invoke(this, new PositionChangedArgs(Position));
                    break;

                case Commands.SetSpeed:
                    InterogativeState.Speed = (int)((Command)response.Request.Data).Arguments[0];
                    break;

                case Commands.Stop:
                    break;
                case Commands.Up:
                    break;
                case Commands.Down:
                    break;
                case Commands.Curve:
                    break;
                case Commands.Flip:
                    break;

                case Commands.SetRemoteControl:
                    break;
                case Commands.SetWiFiPassword:
                    break;
                case Commands.SetStationMode:
                    break;

                default:
                    break;
            }
        }

        public override void OnNext(IResponse<string> response)
        {
            try
            {
                response = new TelloResponse(response);
                var command = (Command)response.Request.Data;

                if (response.Success)
                {
                    if (response.Message != Responses.Error.ToString().ToLowerInvariant())
                    {
                        switch (command.Rule.Response)
                        {
                            case Responses.Ok:
                                if (response.Message == Responses.Ok.ToString().ToLowerInvariant())
                                {
                                    HandleOk(response, command);
                                }
                                else
                                {
                                    throw new TelloException($"'{command}' expecting response '{Responses.Ok.ToString().ToLowerInvariant()}' returned message '{response.Message}' at {response.Timestamp.ToString("o")} after {response.TimeTaken.TotalMilliseconds}ms");
                                }
                                break;
                            case Responses.Speed:
                                InterogativeState.Speed = Int32.Parse(response.Message);
                                break;
                            case Responses.Battery:
                                InterogativeState.Battery = Int32.Parse(response.Message);
                                break;
                            case Responses.Time:
                                InterogativeState.Time = Int32.Parse(response.Message);
                                break;
                            case Responses.WIFISnr:
                                InterogativeState.WIFISnr = response.Message;
                                break;
                            case Responses.SdkVersion:
                                InterogativeState.SdkVersion = response.Message;
                                break;
                            case Responses.SerialNumber:
                                InterogativeState.SerialNumber = response.Message;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        throw new TelloException($"{command} returned message '{response.Message}' at {response.Timestamp.ToString("o")} after {response.TimeTaken.TotalMilliseconds}ms");
                    }
                }
                else
                {
                    OnError(new TelloException($"{command} returned message '{response.Message}' at {response.Timestamp.ToString("o")} after {response.TimeTaken.TotalMilliseconds}ms", response.Exception));
                }
                ResponseReceived?.Invoke(this, new ResponseReceivedArgs(response as TelloResponse));
            }
            catch (Exception ex)
            {
                OnError(new TelloException(ex.Message, ex));
            }
        }
        #endregion

        #region ITelloController

        public event EventHandler<ResponseReceivedArgs> ResponseReceived;
        public event EventHandler<ExceptionThrownArgs> ExceptionThrown;
        public event EventHandler<ConnectionStateChangedArgs> ConnectionStateChanged;
        public event EventHandler<PositionChangedArgs> PositionChanged;
        public event EventHandler<VideoStreamingStateChangedArgs> VideoStreamingStateChanged;

        public InterogativeState InterogativeState { get; } = new InterogativeState();
        public Vector Position { get; private set; } = new Vector();
        public ITelloState State { get; private set; } = new TelloState();

        public void UpdateState(object _, StateChangedArgs e)
        {
            State = e.State;
        }

        /// <summary>
        /// Tello auto lands after 15 seconds without commands as a safety mesasure, so we're going to send a keep alive message every 5 seconds
        /// </summary>
        private async void RunKeepAlive()
        {
            await Task.Run(async () =>
            {
                while (_isConnected)
                {
                    await Task.Delay(1000 * 10);
                    GetBattery();
                }
            });
        }

        #region controller state
        private bool _isConnected = false;
        private bool _isFlying = false;
        private bool _isVideoStreaming = false;
        private bool CanManeuver => _isConnected && _isFlying;
        private bool CanTakeoff => _isConnected && !_isFlying;
        #endregion

        #region enter/exit sdk mode (connect/disconnect)
        public async Task<bool> Connect()
        {
            if (!_isConnected)
            {
                var response = await _messenger.SendAsync(Commands.EnterSdkMode);
                _isConnected = response != null && response.Success;
                if (_isConnected)
                {
                    RunKeepAlive();
                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedArgs(_isConnected));
                }
            }
            return _isConnected;
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _isConnected = false;
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedArgs(_isConnected));
            }
        }
        #endregion

        #region state interogation
        public async void GetBattery()
        {
            await _messenger.SendAsync(Commands.GetBattery);
        }

        public async void GetSdkVersion()
        {
            await _messenger.SendAsync(Commands.GetSdkVersion);
        }

        public async void GetSpeed()
        {
            await _messenger.SendAsync(Commands.GetSpeed);
        }

        public async void GetTime()
        {
            await _messenger.SendAsync(Commands.GetTime);
        }

        public async void GetSerialNumber()
        {
            await _messenger.SendAsync(Commands.GetSerialNumber);
        }

        public async void GetWIFISNR()
        {
            await _messenger.SendAsync(Commands.GetWIFISnr);
        }
        #endregion

        #region maneuver
        public async void TakeOff()
        {
            if (CanTakeoff)
            {
                await _messenger.SendAsync(Commands.Takeoff);
            }
        }

        public async void Land()
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Land);
            }
        }

        public async Task EmergencyStop()
        {
            await _messenger.SendAsync(Commands.EmergencyStop);
            _isFlying = false;
            Disconnect();
        }

        public async Task Stop()
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Stop);
            }
        }

        public async void Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Curve, x1, y1, z1, x2, y2, z2, speed);
            }
        }

        public async void Flip(CardinalDirections direction)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Flip, (char)(CardinalDirection)direction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sides">3 to 15</param>
        /// <param name="length">length of each side. 20 to 500 in cm</param>
        /// <param name="speed">cm/s 10 to 100</param>
        public void FlyPolygon(int sides, int length, int speed, ClockDirections clockDirection)
        {
            if (!CanManeuver)
            {
                return;
            }

            if (sides < 3 || sides > 15)
            {
                throw new ArgumentOutOfRangeException($"{nameof(sides)} allowed values: 3 to 15");
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
        }

        public async void Go(int x, int y, int z, int speed)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Go, x, y, z, speed);
            }
        }

        public async void GoBackward(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Back, cm);
            }
        }

        public async void GoDown(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Down, cm);
            }
        }

        public async void GoForward(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Forward, cm);
            }
        }

        public async void GoLeft(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Left, cm);
            }
        }

        public async void GoRight(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Right, cm);
            }
        }

        public async void GoUp(int cm)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.Up, cm);
            }
        }

        public async void TurnClockwise(int degress)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.ClockwiseTurn, degress);
            }
        }

        public async void TurnCounterClockwise(int degress)
        {
            if (CanManeuver)
            {
                await _messenger.SendAsync(Commands.CounterClockwiseTurn, degress);
            }
        }

        public void TurnLeft(int degress)
        {
            TurnCounterClockwise(degress);
        }

        public void TurnRight(int degress)
        {
            TurnClockwise(degress);
        }

        public void SetHeight(int cm)
        {
            if (CanManeuver && State != null)
            {
                var delta = cm - State.HeightInCm;
                if (delta >= 20 && delta <= 500)
                {
                    if (delta < 0)
                    {
                        GoDown(Math.Abs(delta));
                    }
                    else
                    {
                        GoUp(delta);
                    }
                }
            }
        }

        #endregion

        #region drone configuration
        public async void Set4ChannelRC(int leftRight, int forwardBackward, int upDown, int yaw)
        {
            if (_isConnected)
            {
                await _messenger.SendAsync(Commands.SetRemoteControl, leftRight, forwardBackward, upDown, yaw);
            }
        }

        public async void SetSpeed(int speed)
        {
            if (_isConnected)
            {
                await _messenger.SendAsync(Commands.SetSpeed, speed);
            }
        }

        public async void SetStationMode(string ssid, string password)
        {
            if (_isConnected)
            {
                await _messenger.SendAsync(Commands.SetStationMode, ssid, password);
            }
        }

        public async void SetWIFIPassword(string ssid, string password)
        {
            if (_isConnected)
            {
                await _messenger.SendAsync(Commands.SetWiFiPassword, ssid, password);
            }
        }
        #endregion

        #region video state
        public async void StartVideo()
        {
            if (_isConnected && !_isVideoStreaming)
            {
                await _messenger.SendAsync(Commands.StartVideo);
            }
        }

        public async void StopVideo()
        {
            if (_isConnected && _isVideoStreaming)
            {
                await _messenger.SendAsync(Commands.StopVideo);
            }
        }
        #endregion

        #endregion
    }
}
