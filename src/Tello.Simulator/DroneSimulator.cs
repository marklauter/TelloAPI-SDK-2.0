using Messenger.Simulator;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tello.Simulator.Messaging;
using Tello.State;

namespace Tello.Simulator
{
    public sealed class DroneSimulator
    {
        //todo: 1. execute the appropriate command simulation
        //todo: 2. update state
        //todo: 3. notify state transmitter
        //todo: 4. compose and return the appropriate command response

        private bool _isVideoStreaming = false;
        private bool _isFlying = false;
        private Vector _position = new Vector();
        private ITelloState _state = new TelloState();
        private int _speed = 0;
        private int _height = 0;
        private bool _inCommandMode = false;

        public DroneSimulator()
        {
            MessageHandler = new DroneMessageHandler(DroneSimulator_CommandReceived);
            StateTransmitter = new StateTransmitter();
            VideoTransmitter = new VideoTransmitter();

            VideoThread();
            StateThread();
        }

        public IDroneMessageHandler MessageHandler { get; }
        public IDroneTransmitter StateTransmitter { get; }
        public IDroneTransmitter VideoTransmitter { get; }

        private async void VideoThread()
        {
            await Task.Run(() =>
            {
                var spinWait = new SpinWait();
                while (true)
                {
                    if (_isVideoStreaming)
                    {
                        (VideoTransmitter as VideoTransmitter).AddVideoSegment(Array.Empty<byte>());
                    }
                    spinWait.SpinOnce();
                }
            });
        }

        private async void StateThread()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (_inCommandMode)
                    {
                        (StateTransmitter as StateTransmitter).SetState(_state);
                    }
                    await Task.Delay(12 * 1000);
                }
            });
        }

        private string DroneSimulator_CommandReceived(Command command)
        {
            try
            {
                return Invoke(command);
            }
            catch (Exception ex)
            {
                return $"error {ex.GetType().Name}: {ex.Message}";
            }
        }

        private string Invoke(Command command)
        {
            if (command != Commands.EnterSdkMode && !_inCommandMode)
            {
                throw new TelloException("Call EnterSdkMode first.");
            }

            if (!_isFlying && command.Rule.MustBeInFlight)
            {
                throw new TelloException("Call Takeoff first.");
            }

            switch (command.Rule.Response)
            {
                case Responses.Ok:
                    HandleOk(command);
                    _state = new TelloState(_position);
                    return "ok";
                case Responses.Speed:
                    return _speed.ToString();
                case Responses.Battery:
                    return "99";
                case Responses.Time:
                    return "0";
                case Responses.WIFISnr:
                    return "unk";
                case Responses.SdkVersion:
                    return "Sim V1";
                case Responses.SerialNumber:
                    return "SIM-1234";
                default:
                    throw new NotSupportedException();
            }
        }

        private void HandleOk(Command command)
        {
            switch ((Commands)command)
            {
                case Commands.EnterSdkMode:
                    _inCommandMode = true;
                    break;

                case Commands.Takeoff:
                    _height = 20;
                    _isFlying = true;
                    break;

                case Commands.EmergencyStop:
                case Commands.Land:
                    _height = 0;
                    _isFlying = false;
                    break;

                case Commands.StartVideo:
                    _isVideoStreaming = true;
                    break;
                case Commands.StopVideo:
                    _isVideoStreaming = false;
                    break;

                case Commands.Left:
                    _position = _position.Move(CardinalDirections.Left, (int)command.Arguments[0]);
                    break;
                case Commands.Right:
                    _position = _position.Move(CardinalDirections.Right, (int)command.Arguments[0]);
                    break;
                case Commands.Forward:
                    _position = _position.Move(CardinalDirections.Front, (int)command.Arguments[0]);
                    break;
                case Commands.Back:
                    _position = _position.Move(CardinalDirections.Back, (int)command.Arguments[0]);
                    break;
                case Commands.ClockwiseTurn:
                    _position = _position.Turn(ClockDirections.Clockwise, (int)command.Arguments[0]);
                    break;
                case Commands.CounterClockwiseTurn:
                    _position = _position.Turn(ClockDirections.CounterClockwise, (int)command.Arguments[0]);
                    break;
                case Commands.Go:
                    _position = _position.Go((int)command.Arguments[0], (int)command.Arguments[1]);
                    break;

                case Commands.SetSpeed:
                    _speed = (int)command.Arguments[0];
                    break;

                case Commands.Stop:
                    break;

                case Commands.Up:
                    _height += (int)command.Arguments[0];
                    break;
                case Commands.Down:
                    _height -= (int)command.Arguments[0];
                    if (_height < 0)
                    {
                        _height = 0;
                    }
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
                    throw new NotSupportedException();
            }
        }
    }
}
